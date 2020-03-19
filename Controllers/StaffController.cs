using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class StaffController : ControllerBase
    {

        private readonly ILogger<StaffController> _logger;
        public TuExamContext _context;

        public StaffController(ILogger<StaffController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet]
        [Route("liststaff")]
        public object listAllstaff(string text_search, string status_search, string role_search)
        {
            var staff = _context.Staffs.Include(i => i.User).Where(w => 1 == 1);
          

            if (!string.IsNullOrEmpty(status_search))
                staff = staff.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(role_search))
            {
                var role =  NumUtil.ParseInteger(role_search);
                if(role == 0)
                    staff = staff.Where(w => w.isAdmin == true);
                else if (role == 1)
                    staff = staff.Where(w => w.isMasterAdmin == true);
                else if (role == 2)
                    staff = staff.Where(w => w.isQuestionAppr == true);
                else if (role == 3)
                    staff = staff.Where(w => w.isMasterQuestionAppr == true);
                else if (role == 4)
                    staff = staff.Where(w => w.isTestAppr == true);
                else if (role == 5)
                    staff = staff.Where(w => w.isMasterTestAppr == true);
            }

            var staffs = new List<Staff>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        staffs.AddRange(staff.Where(w => w.FirstName.Contains(text)
                            | w.LastName.Contains(text)
                            | w.IDCard.Contains(text)
                            | w.Phone.Contains(text)
                            | w.Phone2.Contains(text)
                            | w.Email.Contains(text)
                            | w.Passport.Contains(text)
                            | w.StaffCode.Contains(text)
                            | (w.FirstName + " " + w.LastName).Contains(text)
                        ));
                    }
                }
                staffs = staffs.Distinct().ToList();
            }
            else
            {
                staffs = staff.ToList();
            }
            return staffs.Select(s => new
            {
                id = s.ID,
                prefix = s.Prefix.toPrefixName(),
                firstname = s.FirstName,
                lastname = s.LastName,
                idcard = s.IDCard,
                phone = s.Phone,
                phone2 = s.Phone2,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                staffcode = s.StaffCode,
                userid = s.UserID,
                username = s.User.UserName,
                opendate = DateUtil.ToDisplayDate(s.OpenDate),
                expirydate = DateUtil.ToDisplayDate(s.ExpiryDate),
                status = s.Status.toStatusName(),
                isadmin = s.isAdmin,
                ismasteradmin = s.isMasterAdmin,
                isquestionappr = s.isQuestionAppr,
                ismasterquestionappr = s.isMasterQuestionAppr,
                istestappr = s.isTestAppr,
                ismastertestappr = s.isMasterTestAppr,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.firstname).ThenBy(o2 => o2.lastname).ToArray();
        }

        [HttpGet]
        [Route("listlog")]
        public object listlog(string staff_search, string from_search, string to_search, int pageno = 1)
        {
            var id = NumUtil.ParseInteger(staff_search);
            var logs = _context.LoginStaffHistorys.Include(i => i.Staff).Where(w => 1 == 1);
            if (id > 0)
                logs = logs.Where(w => w.StaffID == id);

            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search).Value.Date;
                logs = logs.Where(w => w.Update_On.Value.Date >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search).Value.Date;
                logs = logs.Where(w => w.Update_On.Value.Date <= date);
            }

            int skipRows = (pageno - 1) * 25;
            var itemcnt = logs.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;

            return CreatedAtAction(nameof(listlog), new
            {
                data = logs.Select(s => new
                {
                    id = s.ID,
                    prefix = s.Staff.Prefix.toPrefixName(),
                    firstname = s.Staff.FirstName,
                    lastname = s.Staff.LastName,
                    authtype = s.AuthType.toAuthType(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                    date = s.Update_On,
                }).OrderByDescending(o => o.date).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen
            }); ;
        }

        [HttpGet]
        [Route("getstaff")]
        public object getstaff(int? id)
        {
            var group = _context.Staffs.Include(i => i.User).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                username = s.User.UserName,
                prefix = s.Prefix,
                firstname = s.FirstName,
                lastname = s.LastName,
                idcard = s.IDCard,
                phone = s.Phone,
                phone2 = s.Phone2,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                staffcode = s.StaffCode,
                userid = s.UserID,
                opendate = DateUtil.ToDisplayDate(s.OpenDate),
                expirydate = DateUtil.ToDisplayDate(s.ExpiryDate),
                status = s.Status,
                isadmin = s.isAdmin,
                ismasteradmin = s.isMasterAdmin,
                isquestionappr = s.isQuestionAppr,
                ismasterquestionappr = s.isMasterQuestionAppr,
                istestappr = s.isTestAppr,
                ismastertestappr = s.isMasterTestAppr,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (group != null)
                return group;
            return CreatedAtAction(nameof(getstaff), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<UserDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var users = _context.Users.Count() + 1;
            var username = "AD" + users.ToString("0000");
            model.username = username;

            var u = new User();
            u.UserName = model.username;
            u.Password = DataEncryptor.Encrypt(model.username);
            u.ConfirmPassword = DataEncryptor.Encrypt(model.username);
            u.Create_On = DateUtil.Now();
            u.Create_By = model.update_by;
            u.Update_On = DateUtil.Now();
            u.Update_By = model.update_by;
            u.isAdmin = true;

            var staff = new Staff();
            staff.FirstName = model.firstname;
            staff.LastName = model.lastname;
            staff.Prefix = model.prefix.toPrefix();
            staff.Address = model.address;
            staff.Email = model.email;
            staff.Phone = model.phone;
            staff.Phone2 = model.phone2;
            staff.Passport = model.passport;
            staff.IDCard = model.idcard;
            staff.OpenDate = DateUtil.ToDate(model.opendate);
            staff.ExpiryDate = DateUtil.ToDate(model.expirydate);
            staff.Status = model.status.toStatus();
            staff.Create_On = DateUtil.Now();
            staff.Create_By = model.update_by;
            staff.Update_On = DateUtil.Now();
            staff.Update_By = model.update_by;
            staff.isAdmin = model.isadmin.HasValue ? model.isadmin.Value : false;
            staff.isMasterAdmin = model.ismasteradmin.HasValue ? model.ismasteradmin.Value : false;
            staff.isQuestionAppr = model.isquestionappr.HasValue ? model.isquestionappr.Value : false;
            staff.isMasterQuestionAppr = model.ismasterquestionappr.HasValue ? model.ismasterquestionappr.Value : false;
            staff.isTestAppr = model.istestappr.HasValue ? model.istestappr.Value : false;
            staff.isMasterTestAppr = model.ismastertestappr.HasValue ? model.ismastertestappr.Value : false;
            staff.User = u;

            _context.Staffs.Add(staff);
            _context.SaveChanges();

            username = "AD" + u.ID.ToString("0000");
            u.UserName = username;
            u.Password = DataEncryptor.Encrypt(u.UserName);
            _context.SaveChanges();

            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<UserDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var staff = _context.Staffs.Where(w => w.ID == model.id).FirstOrDefault();
            if (staff != null)
            {
                staff.Update_On = DateUtil.Now();
                staff.Update_By = model.update_by;
                staff.Status = model.status.toStatus();
                staff.Prefix = model.prefix.toPrefix();
                staff.FirstName = model.firstname;
                staff.LastName = model.lastname;
                staff.IDCard = model.idcard;
                staff.Phone = model.phone;
                staff.Phone2 = model.phone2;
                staff.Email = model.email;
                staff.OpenDate = DateUtil.ToDate(model.opendate);
                staff.ExpiryDate = DateUtil.ToDate(model.expirydate);
                staff.isAdmin = model.isadmin.HasValue ? model.isadmin.Value : false;
                staff.isMasterAdmin = model.ismasteradmin.HasValue ? model.ismasteradmin.Value : false;
                staff.isQuestionAppr = model.isquestionappr.HasValue ? model.isquestionappr.Value : false;
                staff.isMasterQuestionAppr = model.ismasterquestionappr.HasValue ? model.ismasterquestionappr.Value : false;
                staff.isTestAppr = model.istestappr.HasValue ? model.istestappr.Value : false;
                staff.isMasterTestAppr = model.ismastertestappr.HasValue ? model.ismastertestappr.Value : false;

                _context.SaveChanges();
                return CreatedAtAction(nameof(update), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(update), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }
        [HttpGet]
        [Route("delete")]
        public object delete(int? id)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var staff = _context.Staffs.Where(w => w.ID == id).FirstOrDefault();
            if (staff == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var user = _context.Users.Where(w => w.ID == staff.UserID).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var logs = _context.LoginStaffHistorys.Where(w => w.StaffID == staff.ID);
            if (logs.Count() > 0)
                _context.LoginStaffHistorys.RemoveRange(logs);

            _context.Staffs.Remove(staff);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpPost]
        [Route("resetpassword")]
        public object resetpassword([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<PasswordDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(resetpassword), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var staff = _context.Staffs.Where(w => w.ID == model.id).FirstOrDefault();
            if (staff != null)
            {
                var user = _context.Users.Where(w => w.ID == model.uid).FirstOrDefault();
                if (user != null)
                {
                    user.Password = DataEncryptor.Encrypt(model.password);
                    user.ConfirmPassword = DataEncryptor.Encrypt(model.password);
                    user.Update_By = model.update_by;
                    user.Update_On = DateUtil.Now();
                }
                staff.Update_By = model.update_by;
                staff.Update_On = DateUtil.Now();
                _context.SaveChanges();
                return CreatedAtAction(nameof(resetpassword), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(resetpassword), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }
    }


}
