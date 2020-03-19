using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using tuexamapi.DAL;
using tuexamapi.DTO;
using tuexamapi.Models;
using tuexamapi.Util;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;
        public TuExamContext _context;
        public JwtAuthentication _jwt;

        public AccountController(ILogger<AccountController> logger, TuExamContext context, IOptions<JwtAuthentication> jwt)
        {
            this._logger = logger;
            this._context = context;
            this._jwt = jwt.Value;
        }

        [HttpGet]
        [Route("login")]
        public object login(string username, string password)
        {
            var user = _context.Users.Where(w => w.UserName == username).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(login), new { result = ResultCode.WrongAccountorPassword, message = ResultMessage.WrongAccountorPassword });

            var dpassword = DataEncryptor.Decrypt(user.Password);
            if (password == dpassword)
            {
                var token = CreateToken(user);
                var staff = _context.Staffs.Where(w => w.UserID == user.ID);

                if (staff.FirstOrDefault() == null)
                    return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

                if (staff.FirstOrDefault().Status == StatusType.InActive)
                    return CreatedAtAction(nameof(login), new { result = ResultCode.InactiveAccount, message = ResultMessage.InactiveAccount });

                var s = staff.Select(s => new
                {
                    username = s.User.UserName,
                    id = s.UserID,
                    staffid = s.ID,
                    firstname = s.FirstName,
                    lastname = s.LastName,
                    profileImg = "",
                    isAdmin = s.isAdmin,
                    isMasterAdmin = s.isMasterAdmin,
                    isQuestionAppr = s.isQuestionAppr,
                    isMasterQuestionAppr = s.isMasterQuestionAppr,
                    isTestAppr = s.isTestAppr,
                    isMasterTestAppr = s.isMasterTestAppr,
                }).FirstOrDefault();

                if (s == null)
                    return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

                var log = new LoginStaffHistory();
                log.StaffID = s.staffid;
                log.UserID = s.id;
                log.AuthType = AuthType.Login;
                log.Create_On = DateUtil.Now();
                log.Create_By = s.username;
                log.Update_On = DateUtil.Now();
                log.Update_By = s.username;
                _context.LoginStaffHistorys.Add(log);


                _context.SaveChanges();
                return CreatedAtAction(nameof(login), new { result = ResultCode.Success, message = ResultMessage.Success, token = token, user = s });
            }
            return CreatedAtAction(nameof(login), new { result = ResultCode.WrongAccountorPassword, message = ResultMessage.WrongAccountorPassword });
        }

        [HttpGet]
        [Route("loginstudent")]
        public object loginstudent(string username, string password)
        {
            var user = _context.Users.Where(w => w.UserName == username).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(login), new { result = ResultCode.WrongAccountorPassword, message = ResultMessage.WrongAccountorPassword });

            var dpassword = DataEncryptor.Decrypt(user.Password);
            if (password == dpassword)
            {
                var token = CreateToken(user);
                var student = _context.Students.Where(w => w.UserID == user.ID & w.Status == StatusType.Active).Select(s => new
                {
                    username = s.User.UserName,
                    id = s.UserID,
                    studentid = s.ID,
                    studentcode = s.StudentCode,
                    course = s.Course,
                    prefix = s.Prefix.toPrefixName(),
                    firstname = s.FirstName,
                    lastname = s.LastName,
                    idcard = s.IDCard,
                    profileImg = "",
                }).FirstOrDefault();

                if (student == null)
                    return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });


                var log = new LoginStudentHistory();
                log.StudentID = student.studentid;
                log.UserID = student.id;
                log.AuthType = AuthType.Login;
                log.Create_On = DateUtil.Now();
                log.Create_By = student.username;
                log.Update_On = DateUtil.Now();
                log.Update_By = student.username;
                _context.LoginStudentHistorys.Add(log);

                var tokens = _context.LoginTokens.Where(w => w.StudentID == student.studentid);
                if (tokens.Count() > 0)
                    _context.LoginTokens.RemoveRange(tokens);

                var tok = new LoginToken();
                tok.StudentID = student.studentid;
                tok.UserID = student.id;
                tok.Token = token;
                tok.Create_On = DateUtil.Now();
                tok.Create_By = student.username;
                tok.Update_On = DateUtil.Now();
                tok.Update_By = student.username;
                tok.ExpiryDate = DateUtil.Now().AddHours(8);
                _context.LoginTokens.Add(tok);
                _context.SaveChanges();

                return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.Success, message = ResultMessage.Success, token = token, user = student });
            }
            return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.WrongAccountorPassword, message = ResultMessage.WrongAccountorPassword });
        }

        public string CreateToken(User loginUser)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, loginUser.UserName),
                //new Claim(JwtRegisteredClaimNames.Typ, loginUser.UserLevelId),
                //new Claim(JwtRegisteredClaimNames.Sid, Version),
                new Claim(JwtRegisteredClaimNames.NameId,loginUser.ID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecurityKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var iisUser = _jwt.ValidIssuer;
            var audience = _jwt.ValidAudience;

            var token = new JwtSecurityToken(iisUser,
                audience,
                //claims,
                expires: DateTime.Now.AddDays(60),
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpGet]
        [Route("logout")]
        public object logout(int id)
        {
            var staff = _context.Staffs.Include(i => i.User).Where(w => w.ID == id).FirstOrDefault();
            if (staff == null)
                return CreatedAtAction(nameof(logout), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var log = new LoginStaffHistory();
            log.StaffID = staff.ID;
            log.UserID = staff.UserID;
            log.AuthType = AuthType.Logout;
            log.Create_On = DateUtil.Now();
            log.Create_By = staff.User.UserName;
            log.Update_On = DateUtil.Now();
            log.Update_By = staff.User.UserName;
            _context.LoginStaffHistorys.Add(log);
            _context.SaveChanges();

            return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("logoutstudent")]
        public object logoutstudent(int id)
        {
            var student = _context.Students.Include(i => i.User).Where(w => w.ID == id).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(logoutstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var tokens = _context.LoginTokens.Where(w => w.StudentID == id);
            if (tokens.Count() > 0)
                _context.LoginTokens.RemoveRange(tokens);

            var log = new LoginStudentHistory();
            log.StudentID = student.ID;
            log.UserID = student.UserID;
            log.AuthType = AuthType.Logout;
            log.Create_On = DateUtil.Now();
            log.Create_By = student.User.UserName;
            log.Update_On = DateUtil.Now();
            log.Update_By = student.User.UserName;
            _context.LoginStudentHistorys.Add(log);
            _context.SaveChanges();

            return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("tokenisexist")]
        public object tokenisexist(string token, int id)
        {
            var tok = _context.LoginTokens.Where(w => w.Token == token & w.StudentID == id & w.ExpiryDate.Value.Date >= DateUtil.Now()).FirstOrDefault();
            if (tok == null)
                return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }


}
