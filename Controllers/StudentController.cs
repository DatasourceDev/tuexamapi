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
    public class StudentController : ControllerBase
    {

        private readonly ILogger<StudentController> _logger;
        public TuExamContext _context;

        public StudentController(ILogger<StudentController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet]
        [Route("liststudent")]
        public object listAllstudent(string text_search, string status_search, string course_search, string faculty_search)
        {
            var student = _context.Students.Include(i=>i.User).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(text_search))
                student = student.Where(w => w.FirstName.Contains(text_search) 
                | w.LastName.Contains(text_search) 
                | w.FirstNameEn.Contains(text_search) 
                | w.LastNameEn.Contains(text_search)
                | w.IDCard.Contains(text_search)
                | w.Phone.Contains(text_search)
                | w.Email.Contains(text_search)
                | w.Passport.Contains(text_search)
                | w.StudentCode.Contains(text_search)
                );

            if (!string.IsNullOrEmpty(status_search))
                student = student.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(course_search))
                student = student.Where(w => w.Course == course_search.toCourse());

            if (!string.IsNullOrEmpty(faculty_search))
                student = student.Where(w => w.Faculty == faculty_search);

            return student.Select(s => new
            {
                id = s.ID,
                prefix = s.Prefix.toPrefixName(),
                firstname = s.FirstName,
                lastname = s.LastName,
                idcard = s.IDCard,
                phone = s.Phone,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.Faculty,
                course = s.Course.toCourseName(),
                userid = s.UserID,
                username = s.User.UserName,
                status = s.Status.toStatusName(),
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.studentcode).ThenBy(o1 => o1.firstname).ThenBy(o2=>o2.lastname).ToArray();
        }

        [HttpGet]
        [Route("getstudentbyuid")]
        public object getstudentbyuid(int? id)
        {
            var group = _context.Students.Include(i=>i.User).Where(w => w.UserID == id).Select(s => new
            {
                id = s.ID,
                username = s.User.UserName,
                prefix = s.Prefix,
                firstname = s.FirstName,
                lastname = s.LastName,
                idcard = s.IDCard,
                phone = s.Phone,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.Faculty,
                course = s.Course,
                userid = s.UserID,
                status = s.Status,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (group != null)
                return group;
            return CreatedAtAction(nameof(getstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getstudent")]
        public object getstudent(int? id)
        {
            var group = _context.Students.Include(i => i.User).Where(w => w.ID == id).Select(s => new
            {
                id = s.ID,
                username = s.User.UserName,
                prefix = s.Prefix,
                firstname = s.FirstName,
                lastname = s.LastName,
                idcard = s.IDCard,
                phone = s.Phone,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.Faculty,
                course = s.Course,
                userid = s.UserID,
                status = s.Status,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (group != null)
                return group;
            return CreatedAtAction(nameof(getstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<UserDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var users = _context.Users.Count()+1;
            model.username = model.idcard;

            var u = new User();
            u.UserName = model.username;
            u.Password = DataEncryptor.Encrypt(model.idcard);
            u.ConfirmPassword = DataEncryptor.Encrypt(model.idcard);
            u.Create_On = DateUtil.Now();
            u.Create_By = model.updateby;
            u.Update_On = DateUtil.Now();
            u.Update_By = model.updateby;

            var student = new Student();
            student.FirstName = model.firstname;
            student.LastName = model.lastname;
            student.FirstNameEn = model.firstnameen;
            student.LastNameEn = model.lastnameen;
            student.Prefix = model.prefix.toPrefix();
            student.Address = model.address;
            student.Email = model.email;
            student.Phone = model.phone;
            student.Passport = model.passport;
            student.IDCard = model.idcard;
            student.StudentCode = model.studentcode;
            student.Faculty = model.faculty;
            student.Course = model.course.toCourse();
            student.Status = model.status.toStatus();
            student.Create_On = DateUtil.Now();
            student.Create_By = model.updateby;
            student.Update_On = DateUtil.Now();
            student.Update_By = model.updateby;
            student.User = u;

            _context.Students.Add(student);
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

            var student = _context.Students.Where(w => w.ID == model.id).FirstOrDefault();
            if (student != null)
            {
                student.Update_On = DateUtil.Now();
                student.Update_By = model.updateby;
                student.Status = model.status.toStatus();
                student.Prefix = model.prefix.toPrefix();
                student.Address = model.address;
                student.FirstName = model.firstname;
                student.LastName = model.lastname;
                student.FirstNameEn = model.firstnameen;
                student.LastNameEn = model.lastnameen;
                student.IDCard = model.idcard;
                student.StudentCode = model.studentcode;
                student.Phone = model.phone;
                student.Email = model.email;
                student.Faculty = model.faculty;
                student.Course = model.course.toCourse();
                _context.SaveChanges();
                return CreatedAtAction(nameof(update), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(update), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpPost]
        [Route("delete")]
        public object delete([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<UserDTO>(json.GetRawText());
            if (model == null || string.IsNullOrEmpty(model.username))
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var user = _context.Users.Where(w => w.UserName == model.username).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var students = _context.Students.Where(w => w.UserID == user.ID);
            if (students.Count() > 0)
            {
                foreach(var student in students)
                {
                    var regs = _context.ExamRegisters.Where(w => w.StudentID == student.ID);
                    if (regs.Count() > 0)
                        _context.ExamRegisters.RemoveRange(regs);
                }
                _context.Students.RemoveRange(students);

            }

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

            var student = _context.Students.Where(w => w.ID == model.id).FirstOrDefault();
            if (student != null)
            {
                var user = _context.Users.Where(w => w.ID == model.uid).FirstOrDefault();
                if(user != null)
                {
                    user.Password = DataEncryptor.Encrypt(model.password);
                    user.ConfirmPassword = DataEncryptor.Encrypt(model.password);
                    user.Update_By = model.update_by;
                    user.Update_On = DateUtil.Now();
                }
                student.Update_By = model.update_by;
                student.Update_On = DateUtil.Now();
                _context.SaveChanges();
                return CreatedAtAction(nameof(resetpassword), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(resetpassword), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }


    }


}
