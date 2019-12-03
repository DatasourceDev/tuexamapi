using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var dpassword = DataEncryptor.Decrypt(user.Password);
            if (password == dpassword)
            {
                var token = CreateToken(user);
                var staff = _context.Staffs.Where(w => w.UserID == user.ID).Select(s => new
                {
                    username = s.User.UserName,
                    id = s.UserID,
                    staffid = s.ID,
                    firstname = s.FirstName,
                    lastname = s.LastName,
                    profileImg = "",
                }).FirstOrDefault();

                if (staff == null)
                    return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

                return CreatedAtAction(nameof(login), new { result = ResultCode.Success, message = ResultMessage.Success, token = token, user = staff });
            }
            return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("loginstudent")]
        public object loginstudent(string username, string password)
        {
            var user = _context.Users.Where(w => w.UserName == username).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(login), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var dpassword = DataEncryptor.Decrypt(user.Password);
            if (password == dpassword)
            {
                var token = CreateToken(user);
                var student = _context.Students.Where(w => w.UserID == user.ID).Select(s => new
                {
                    username = s.User.UserName,
                    id = s.UserID,
                    studentid = s.ID,
                    studentcode = s.StudentCode,
                    prefix = s.Prefix.toPrefixName(),
                    firstname = s.FirstName,
                    lastname = s.LastName,
                    profileImg = "",
                }).FirstOrDefault();

                if(student == null)
                    return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

                return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.Success, message = ResultMessage.Success, token = token, user = student });
            }
            return CreatedAtAction(nameof(loginstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
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
    }


}
