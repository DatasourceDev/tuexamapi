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
using System.IO;
using OfficeOpenXml;

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
        public object liststudent(string text_search, string status_search, string course_search, int? faculty_search, int pageno = 1)
        {
            var student = _context.Students.Include(i => i.Faculty).Include(i => i.User).Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(status_search))
                student = student.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(course_search))
                student = student.Where(w => w.Course == course_search.toCourse());

            if (faculty_search.HasValue)
                student = student.Where(w => w.FacultyID == faculty_search);

            var students = new List<Student>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        students.AddRange(student.Where(w => w.FirstName.Contains(text)
                           | w.LastName.Contains(text)
                           | w.FirstNameEn.Contains(text)
                           | w.LastNameEn.Contains(text)
                           | w.IDCard.Contains(text)
                           | w.Phone.Contains(text)
                           | w.Email.Contains(text)
                           | w.Passport.Contains(text)
                           | w.StudentCode.Contains(text)
                           | (w.FirstName + " " + w.LastName).Contains(text)
                           | (w.FirstNameEn + " " + w.LastNameEn).Contains(text)
                        ));
                    }
                }
                students = students.Distinct().ToList();
            }
            else
            {
                students = student.ToList();
            }

            int skipRows = (pageno - 1) * 100;
            var itemcnt = students.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;

            return CreatedAtAction(nameof(listAllstudent), new
            {
                data = students.Select(s => new
                {
                    id = s.ID,
                    prefix = s.Prefix.toPrefixName(),
                    firstname = s.FirstName,
                    lastname = s.LastName,
                    firstnameen = s.FirstNameEn,
                    lastnameen = s.LastNameEn,
                    idcard = s.IDCard,
                    phone = s.Phone,
                    email = s.Email,
                    address = s.Address,
                    passport = s.Passport,
                    studentcode = s.StudentCode,
                    faculty = s.Faculty != null ? s.Faculty.FacultyName : "",
                    course = s.Course.toCourseName(),
                    userid = s.UserID,
                    username = s.User.UserName,
                    status = s.Status.toStatusName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.studentcode).ThenBy(o1 => o1.firstname).ThenBy(o2 => o2.lastname).Skip(skipRows).Take(100).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,

            });
        }

        [HttpGet]
        [Route("listAllstudent")]
        public object listAllstudent(string text_search, string status_search, string course_search, int? faculty_search)
        {
            var student = _context.Students.Include(i => i.Faculty).Include(i => i.User).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(status_search))
                student = student.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(course_search))
                student = student.Where(w => w.Course == course_search.toCourse());

            if (faculty_search.HasValue)
                student = student.Where(w => w.FacultyID == faculty_search);

            var students = new List<Student>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        students.AddRange(student.Where(w => w.FirstName.Contains(text)
                           | w.LastName.Contains(text)
                           | w.FirstNameEn.Contains(text)
                           | w.LastNameEn.Contains(text)
                           | w.IDCard.Contains(text)
                           | w.Phone.Contains(text)
                           | w.Email.Contains(text)
                           | w.Passport.Contains(text)
                           | w.StudentCode.Contains(text)
                           | (w.FirstName + " " + w.LastName).Contains(text)
                           | (w.FirstNameEn + " " + w.LastNameEn).Contains(text)
                        ));
                    }
                }
                students = students.Distinct().ToList();
            }
            else
            {
                students = student.ToList();
            }

            return students.Select(s => new
            {
                id = s.ID,
                prefix = s.Prefix.toPrefixName(),
                firstname = s.FirstName,
                lastname = s.LastName,
                firstnameen = s.FirstNameEn,
                lastnameen = s.LastNameEn,
                idcard = s.IDCard,
                phone = s.Phone,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.Faculty != null ? s.Faculty.FacultyName : "",
                course = s.Course.toCourseName(),
                userid = s.UserID,
                username = s.User.UserName,
                status = s.Status.toStatusName(),
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.studentcode).ThenBy(o1 => o1.firstname).ThenBy(o2 => o2.lastname).ToArray();
        }

        [HttpGet]
        [Route("listlog")]
        public object listlog(string student_search, string from_search, string to_search, int pageno = 1)
        {
            var id = NumUtil.ParseInteger(student_search);
            var logs = _context.LoginStudentHistorys.Include(i => i.Student).Where(w => 1 == 1);
            if (id > 0)
                logs = logs.Where(w => w.StudentID == id);

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
                    prefix = s.Student.Prefix.toPrefixName(),
                    firstname = s.Student.FirstName,
                    lastname = s.Student.LastName,
                    firstnameen = s.Student.FirstNameEn,
                    lastnameen = s.Student.LastNameEn,
                    authtype = s.AuthType.toAuthType(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                    date = s.Update_On,
                }).OrderByDescending(o => o.date).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;
        }

        [HttpGet]
        [Route("getstudentbyuid")]
        public object getstudentbyuid(int? id)
        {
            var student = _context.Students.Include(i => i.User).Where(w => w.UserID == id).Select(s => new
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
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.FacultyID,
                course = s.Course,
                userid = s.UserID,
                status = s.Status,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (student != null)
                return student;
            return CreatedAtAction(nameof(getstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getstudent")]
        public object getstudent(int? id)
        {
            var student = _context.Students.Include(i => i.User).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                username = s.User.UserName,
                prefix = s.Prefix,
                firstname = s.FirstName,
                lastname = s.LastName,
                firstnameen = s.FirstNameEn,
                lastnameen = s.LastNameEn,
                idcard = s.IDCard,
                phone = s.Phone,
                email = s.Email,
                address = s.Address,
                passport = s.Passport,
                studentcode = s.StudentCode,
                faculty = s.FacultyID,
                course = s.Course,
                userid = s.UserID,
                status = s.Status,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (student != null)
                return student;
            return CreatedAtAction(nameof(getstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<UserDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var dupid = _context.Students.Where(w => w.IDCard == model.idcard).FirstOrDefault();
            if (dupid != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "เลขประจำตัวประชาชนซ้ำ" });

            if (!string.IsNullOrEmpty(model.studentcode))
            {
                var dupscode = _context.Students.Where(w => w.StudentCode == model.studentcode).FirstOrDefault();
                if (dupscode != null)
                    return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "รหัสนักศึกษาซ้ำ" });

            }

            var users = _context.Users.Count() + 1;
            model.username = model.idcard;

            var u = new User();
            u.UserName = model.username;
            u.Password = DataEncryptor.Encrypt(model.idcard);
            u.ConfirmPassword = DataEncryptor.Encrypt(model.idcard);
            u.Create_On = DateUtil.Now();
            u.Create_By = model.update_by;
            u.Update_On = DateUtil.Now();
            u.Update_By = model.update_by;

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
            student.FacultyID = model.faculty;
            student.Course = model.course.toCourse();
            student.Status = model.status.toStatus();
            student.Create_On = DateUtil.Now();
            student.Create_By = model.update_by;
            student.Update_On = DateUtil.Now();
            student.Update_By = model.update_by;
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

            var dupid = _context.Students.Where(w => w.IDCard == model.idcard & w.ID != model.id).FirstOrDefault();
            if (dupid != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "เลขประจำตัวประชาชนซ้ำ" });

            var dupscode = _context.Students.Where(w => w.StudentCode == model.studentcode & w.ID != model.id).FirstOrDefault();
            if (dupscode != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "รหัสนักศึกษาซ้ำ" });

            var student = _context.Students.Where(w => w.ID == model.id).FirstOrDefault();
            if (student != null)
            {
                if (student.IDCard != model.idcard)
                {
                    var user = _context.Users.Where(w => w.ID == student.UserID).FirstOrDefault();
                    if (user != null)
                    {
                        user.UserName = model.idcard;
                        user.Password = DataEncryptor.Encrypt(model.idcard);
                        user.ConfirmPassword = DataEncryptor.Encrypt(model.idcard);
                        user.Update_On = DateUtil.Now();
                        user.Update_By = model.update_by;
                    }
                }
                student.Update_On = DateUtil.Now();
                student.Update_By = model.update_by;
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
                student.FacultyID = model.faculty;
                student.Course = model.course.toCourse();
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

            var student = _context.Students.Where(w => w.ID == id).FirstOrDefault();
            if (student == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var user = _context.Users.Where(w => w.ID == student.UserID).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var tresult = _context.TestResultStudents.Where(w => w.StudentID == id);
            if (tresult.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            var regs = _context.ExamRegisters.Where(w => w.StudentID == student.ID);
            if (regs.Count() > 0)
                _context.ExamRegisters.RemoveRange(regs);

            var logs = _context.LoginStudentHistorys.Where(w => w.StudentID == student.ID);
            if (logs.Count() > 0)
                _context.LoginStudentHistorys.RemoveRange(logs);

            _context.Students.Remove(student);
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
                if (user != null)
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

       
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public object upload([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportExamRegisterDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var file = Convert.FromBase64String(model.fileupload.value);
                using (MemoryStream ms = new MemoryStream(file))
                {
                    using (ExcelPackage package = new ExcelPackage(ms))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            return CreatedAtAction(nameof(upload), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });
                        }
                        else
                        {
                            var worksheet = package.Workbook.Worksheets.First();
                            int totalRows = worksheet.Dimension.End.Row;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                var j = 1;
                                var studentcode = worksheet.Cells[i, j].Text; j++;
                                var prefix = worksheet.Cells[i, j].Text; j++;
                                var firstname = worksheet.Cells[i, j].Text; j++;
                                var lastname = worksheet.Cells[i, j].Text; j++;
                                var phone = worksheet.Cells[i, j].Text; j++;
                                var email = worksheet.Cells[i, j].Text; j++;
                                var year = worksheet.Cells[i, j].Text; j++;
                                var faculty = worksheet.Cells[i, j].Text; j++;

                                var student = _context.Students.Where(w => w.StudentCode == studentcode).FirstOrDefault();
                                if (student == null)
                                {
                                    student = new Student();
                                    student.Course = Course.Thai;
                                    student.Email = email;
                                    var fac = _context.Facultys.Where(w => w.FacultyName == faculty).FirstOrDefault();
                                    if (fac != null)
                                        student.FacultyID = fac.ID;
                                    student.FirstName = firstname;
                                    student.LastName = lastname;
                                    student.Phone = phone;
                                    student.Prefix = prefix.toPrefix();
                                    student.StudentCode = studentcode;
                                    student.Status = StatusType.Active;
                                    student.Update_On = DateUtil.Now();
                                    student.Create_On = DateUtil.Now();

                                    var user = new User();
                                    user.Password = DataEncryptor.Encrypt(studentcode);
                                    user.UserName = studentcode;
                                    user.Update_On = DateUtil.Now();
                                    user.Create_On = DateUtil.Now();

                                    student.User = user;
                                    _context.Students.Add(student);
                                }
                                else
                                {
                                    student.Course = Course.Thai;
                                    student.Email = email;
                                    var fac = _context.Facultys.Where(w => w.FacultyName == faculty).FirstOrDefault();
                                    if (fac != null)
                                        student.FacultyID = fac.ID;
                                    student.FirstName = firstname;
                                    student.LastName = lastname;
                                    student.Phone = phone;
                                    student.Prefix = prefix.toPrefix();
                                    student.StudentCode = studentcode;
                                    student.Update_On = DateUtil.Now();
                                }
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }
            return CreatedAtAction(nameof(upload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }
    }


}
