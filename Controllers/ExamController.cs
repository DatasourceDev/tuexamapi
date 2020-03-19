using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.DTO;
using tuexamapi.Models;
using tuexamapi.Util;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExamController : ControllerBase
    {

        private readonly ILogger<ExamController> _logger;
        public TuExamContext _context;
        public IWebHostEnvironment _env;

        public ExamController(ILogger<ExamController> logger, TuExamContext context, IWebHostEnvironment env)
        {
            this._logger = logger;
            this._context = context;
            this._env = env;
        }

        [HttpGet]
        [Route("listexam")]
        public object listAllexam(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, int? move_from, int pageno = 1)
        {
            /*repair test student exam*/
            var curdate = DateUtil.Now().Date;
            var rtstudents = _context.TestResultStudents.Include(i=>i.Test).Where(w => w.ExamingStatus == ExamingStatus.Examing);
            foreach (var rtstudent in rtstudents)
            {
                if (rtstudent.Test != null)
                {
                    var remain = FuncUtil.GetTimeRemaining(rtstudent.Test.TimeLimitType, rtstudent.Test.TimeLimit, rtstudent.Start_On);
                    if (remain <= 0)
                    {
                        rtstudent.ExamingStatus = ExamingStatus.Done;
                        rtstudent.Other = true;
                        rtstudent.Update_On = curdate;
                        rtstudent.End_On = rtstudent.Start_On.Value.AddSeconds(FuncUtil.GetMaxTimeLimit(rtstudent.Test.TimeLimitType, rtstudent.Test.TimeLimit));
                    }
                }
            }
            _context.SaveChanges();

            var exam = _context.Exams.Include(i => i.Test).Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => 1 == 1);

            if(move_from.HasValue)
                exam = exam.Where(w => w.ID != move_from & w.ExamDate.Value >= curdate.Date);

            if (!string.IsNullOrEmpty(status_search))
                exam = exam.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    exam = exam.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    exam = exam.Where(w => w.Subject.SubjectGroupID == groupID);
            }
            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                exam = exam.Where(w => w.ExamDate.Value.Date >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                exam = exam.Where(w => w.ExamDate.Value.Date <= date);
            }

            var exams = new List<Exam>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();

                        var registerIDs = _context.ExamRegisters.Where(w => w.Student.FirstName.Contains(text)
                           | w.Student.LastName.Contains(text)
                           | w.Student.FirstNameEn.Contains(text)
                           | w.Student.LastNameEn.Contains(text)
                           | w.Student.IDCard.Contains(text)
                           | w.Student.Phone.Contains(text)
                           | w.Student.Email.Contains(text)
                           | w.Student.Passport.Contains(text)
                           | w.Student.StudentCode.Contains(text)
                           | (w.Student.FirstName + " " + w.Student.LastName).Contains(text)
                           | (w.Student.FirstNameEn + " " + w.Student.LastNameEn).Contains(text)
                        ).Select(s => s.ExamID);

                        exams.AddRange(exam.Where(w => w.Test.Name.Contains(text)
                            | w.ExamCode.Contains(text)
                            | registerIDs.Contains(w.ID)
                            ));
                    }
                }
                exams = exams.Distinct().ToList();
            }
            else
            {
                exams = exam.ToList();
            }
           
            int skipRows = (pageno - 1) * 25;
            var itemcnt = exams.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAllexam), new
            {
                data = exams.Select(s => new
                {
                    id = s.ID,
                    test = s.ExamTestType == ExamTestType.Random ? _context.Tests.Where(w=>w.SubjectID == s.SubjectID & w.Status == StatusType.Active).Count() == 1 ? _context.Tests.Where(w => w.SubjectID == s.SubjectID & w.Status == StatusType.Active).FirstOrDefault().Name :  "สุ่ม" : s.Test.Name,
                    status = s.Status.toStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    subject = s.Subject.Name,
                    subjectorder = s.Subject.Order,
                    examcode = s.ExamCode,
                    examdate = DateUtil.ToDisplayDate(s.ExamDate),
                    date = s.ExamDate,
                    examperiod = s.ExamPeriod.toExamPeriodName(),
                    examperiodid = s.ExamPeriod,
                    examtesttype = s.ExamTestType.toExamTestType(),
                    registercnt = s.RegisterCnt,
                    examregistercnt = s.ExamRegisterCnt,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderByDescending(o => o.date).ThenBy(o => o.examperiodid).ThenBy(o => o.group).ThenBy(o => o.subjectorder).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;

        }


        [HttpGet]
        [Route("getexam")]
        public object getexam(int? id)
        {
            var exam = _context.Exams.Include(i => i.Test).Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                testid = s.TestID,
                test = s.Test.Name,
                status = s.Status,
                groupid = s.Subject.SubjectGroupID,
                group = s.SubjectGroup.Name,
                subjectid = s.SubjectID,
                subject = s.Subject.Name,
                examcode = s.ExamCode,
                examdate = DateUtil.ToDisplayDate(s.ExamDate),
                examperiod = s.ExamPeriod,
                examperiodname = s.ExamPeriod.toExamPeriodName(),
                examtesttype = s.ExamTestType,
                examtesttypename = s.ExamTestType.toExamTestType(),
                registercnt = s.RegisterCnt,
                examregistercnt = s.ExamRegisterCnt,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,

            }).FirstOrDefault();

            if (exam != null)
                return exam;
            return CreatedAtAction(nameof(getexam), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ExamDTO>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var test = _context.Tests.Where(w => w.SubjectGroupID == model.SubjectGroupID && w.SubjectID == model.SubjectID & w.Status == StatusType.Active).FirstOrDefault();
            if (test == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DataHasNotFound, message = "ไม่พบข้อมูล แบบทดสอบ ที่สามารถนำมาใช้สอบได้" });

            //var edate = DateUtil.ToDate(model.ExamDate);
            //var dup = _context.Exams
            //    .Where(w => w.SubjectGroupID == model.SubjectGroupID
            //    & w.SubjectID == model.SubjectID
            //    & w.ExamPeriod == model.ExamPeriod 
            //    & w.ExamDate.Value.Date == edate.Value.Date).FirstOrDefault();
            //if(dup != null)
            //    return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "รอบสอบซ้ำ" });

            var exam = new Exam();
            exam.Create_On = DateUtil.Now();
            exam.Update_On = DateUtil.Now();
            exam.Create_By = model.Update_By;
            exam.Update_By = model.Update_By;
            exam.ExamDate = DateUtil.ToDate(model.ExamDate);
            exam.ExamPeriod = model.ExamPeriod;
            exam.ExamTestType = model.ExamTestType;
            exam.SubjectGroupID = model.SubjectGroupID;
            exam.SubjectID = model.SubjectID;
            exam.TestID = model.TestID;
            _context.Exams.Add(exam);
            _context.SaveChanges();

            var code = "EX" + model.ID.ToString("00000000");
            model.ExamCode = code;
            _context.SaveChanges();

            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success, id = model.ID, groupid = model.SubjectGroupID, subjectid = model.SubjectID });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ExamDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var test = _context.Tests.Where(w => w.SubjectGroupID == model.SubjectGroupID && w.SubjectID == model.SubjectID & w.Status == StatusType.Active).FirstOrDefault();
            if (test == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DataHasNotFound, message = "ไม่พบข้อมูล แบบทดสอบ ที่สามารถนำมาใช้สอบได้" });

            //var edate = DateUtil.ToDate(model.ExamDate);
            //var dup = _context.Exams
            //    .Where(w => w.SubjectGroupID == model.SubjectGroupID
            //    & w.SubjectID == model.SubjectID
            //    & w.ExamPeriod == model.ExamPeriod 
            //    & w.ExamDate.Value.Date == edate.Value.Date
            //    & w.ID != model.ID).FirstOrDefault();
            //if (dup != null)
            //    return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = "รอบสอบซ้ำ" });

            var exam = _context.Exams.Where(w => w.ID == model.ID).FirstOrDefault();
            if (exam != null)
            {
                exam.Update_On = DateUtil.Now();
                exam.Update_By = model.Update_By;
                exam.ExamDate = DateUtil.ToDate(model.ExamDate);
                exam.ExamPeriod = model.ExamPeriod;
                exam.ExamTestType = model.ExamTestType;
                exam.SubjectGroupID = model.SubjectGroupID;
                exam.SubjectID = model.SubjectID;
                exam.TestID = model.TestID;
                exam.ExamCode = "EX" + exam.ID.ToString("00000000");
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

            var exam = _context.Exams.Where(w => w.ID == id).FirstOrDefault();
            if (exam == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var regs = _context.ExamRegisters.Where(w => w.ExamID == id);
            if (regs.Count() > 0)
                _context.ExamRegisters.RemoveRange(regs);

            var tresults = _context.TestResults.Where(w => w.ExamID == exam.ID);
            if (!_env.IsDevelopment())
            {
                if (tresults.Count() > 0)
                    return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            }

            foreach (var tresult in tresults)
            {
                var tstudents = _context.TestResultStudents.Where(w => w.TestResultID == tresult.ID);
                foreach (var tstudent in tstudents)
                {
                    var tqans = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID);
                    if (tqans.Count() > 0)
                        _context.TestResultStudentQAnies.RemoveRange(tqans);
                }
                if (tstudents.Count() > 0)
                    _context.TestResultStudents.RemoveRange(tstudents);
            }
            if (tresults.Count() > 0)
                _context.TestResults.RemoveRange(tresults);

            _context.Exams.Remove(exam);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listAllregistered")]
        public object listAllregistered(string exam_search, int pageno = 1)
        {
            var examid = NumUtil.ParseInteger(exam_search);
            var exam = _context.ExamRegisters.Include(i => i.Exam).Where(w => w.ExamID == examid);

            int skipRows = (pageno - 1) * 25;
            var itemcnt = exam.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAllexam), new
            {
                data = exam.Select(s => new
                {
                    id = s.ID,
                    prefix = s.Student.Prefix.toPrefixName(),
                    firstname = s.Student.FirstName,
                    lastname = s.Student.LastName,
                    firstnameen = s.Student.FirstNameEn,
                    lastnameen = s.Student.LastNameEn,
                    studentcode = s.Student.StudentCode,
                    course = s.Student.Course.toCourseName(),
                    faculty = s.Student.Faculty,
                    email = s.Student.Email,
                    phone = s.Student.Phone,
                    idcard = s.Student.IDCard,
                    examregistertype =s.ExamRegisterType.toExamRegisterType(),
                    inexam = _context.TestResultStudents.Where(w=>w.ExamID == examid & w.StudentID == s.StudentID).FirstOrDefault() != null ? true : false,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.studentcode).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }) ;

        }


        [HttpGet]
        [Route("getregistered")]
        public object getregistered(int? id)
        {
            var exam = _context.ExamRegisters.Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                examid = s.ExamID,
                studentid = s.StudentID,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (exam != null)
                return exam;
            return CreatedAtAction(nameof(getregistered), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
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
                                var txt = worksheet.Cells[i, 1].Text.ToString();
                                if (!string.IsNullOrEmpty(txt))
                                {
                                    var student = _context.Students.Where(w => w.StudentCode == txt).FirstOrDefault();
                                    if (student != null)
                                    {
                                        var examidint = NumUtil.ParseInteger(model.examid);
                                        var reged = _context.ExamRegisters.Where(w => w.StudentID == student.ID & w.ExamID == examidint).FirstOrDefault();
                                        if (reged == null)
                                        {
                                            var register = new ExamRegister();
                                            register.StudentID = student.ID;
                                            register.ExamID = examidint;
                                            register.ExamRegisterType = ExamRegisterType.Advance;
                                            register.Create_On = DateUtil.Now();
                                            register.Update_On = DateUtil.Now();
                                            register.Create_By = model.update_by;
                                            register.Update_By = model.update_by;
                                            _context.ExamRegisters.Add(register);
                                        }

                                    }
                                }
                            }
                            _context.SaveChanges();
                            var examid = NumUtil.ParseInteger(model.examid);
                            var exam = _context.Exams.Where(w => w.ID == examid).FirstOrDefault();
                            if (exam != null)
                            {
                                exam.Update_On = DateUtil.Now();
                                exam.Update_By = model.update_by;
                                exam.RegisterCnt = _context.ExamRegisters.Where(w => w.ExamID == examid).Count();
                                _context.SaveChanges();
                                return CreatedAtAction(nameof(upload), new { result = ResultCode.Success, message = ResultMessage.Success, registercnt = exam.RegisterCnt });
                            }
                        }
                    }
                }
            }
            return CreatedAtAction(nameof(upload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadwalkin")]
        public object uploadwalkin([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ChooseDTO>(json.GetRawText());
            
            var examidint = NumUtil.ParseInteger(model.examid);

            var chs = model.choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var studentid = NumUtil.ParseInteger(ch);

                    var reged = _context.ExamRegisters.Where(w => w.StudentID == studentid & w.ExamID == examidint).FirstOrDefault();
                    if (reged == null)
                    {
                        var register = new ExamRegister();
                        register.StudentID = studentid;
                        register.ExamID = NumUtil.ParseInteger(model.examid);
                        register.ExamRegisterType = ExamRegisterType.WalkIn;
                        register.Create_On = DateUtil.Now();
                        register.Update_On = DateUtil.Now();
                        register.Create_By = model.update_by;
                        register.Update_By = model.update_by;
                        _context.ExamRegisters.Add(register);
                    }
                }
            }
            _context.SaveChanges();
            var exam = _context.Exams.Where(w => w.ID == examidint).FirstOrDefault();
            if (exam != null)
            {
                exam.RegisterCnt = _context.ExamRegisters.Where(w => w.ExamID == examidint).Count();
                exam.Update_On = DateUtil.Now();
                exam.Update_By = model.update_by;
                _context.SaveChanges();
                return CreatedAtAction(nameof(upload), new { result = ResultCode.Success, message = ResultMessage.Success, registercnt = exam.RegisterCnt });
            }
            return CreatedAtAction(nameof(uploadwalkin), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("registermove")]
        public object registermove(int? id, int new_exam, string update_by)
        {
            var register = _context.ExamRegisters.Where(w => w.ID == id).FirstOrDefault();
            if(register == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var dup = _context.ExamRegisters.Where(w => w.StudentID == register.StudentID && w.ExamID == new_exam).FirstOrDefault();
            if(dup != null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            var old_exam = register.ExamID;
            register.ExamID = new_exam;
            register.Update_On = DateUtil.Now();
            register.Update_By = update_by;
            _context.SaveChanges();

            var exam = _context.Exams.Where(w => w.ID == old_exam).FirstOrDefault();
            if (exam != null)
            {
                exam.RegisterCnt = _context.ExamRegisters.Where(w => w.ExamID == old_exam).Count() ;
                exam.Update_On = DateUtil.Now();
                exam.Update_By = update_by;
            }

            var exam2 = _context.Exams.Where(w => w.ID == new_exam).FirstOrDefault();
            if (exam2 != null)
            {
                exam2.RegisterCnt = _context.ExamRegisters.Where(w => w.ExamID == new_exam).Count();
                exam2.Update_On = DateUtil.Now();
                exam2.Update_By = update_by;
            }

            _context.SaveChanges();
            return CreatedAtAction(nameof(uploadwalkin), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("registerdelete")]
        public object registerdelete(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var reg = _context.ExamRegisters.Where(w => w.ID == id).FirstOrDefault();
            if (reg == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var result = _context.TestResultStudents.Where(w => w.ExamID == reg.ExamID & w.StudentID == reg.StudentID).FirstOrDefault();
            if (result != null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            var examid = reg.ExamID;
            _context.ExamRegisters.Remove(reg);
            _context.SaveChanges();

            var exam = _context.Exams.Where(w => w.ID == examid).FirstOrDefault();
            if (exam != null)
            {
                exam.RegisterCnt = _context.ExamRegisters.Where(w => w.ExamID == examid).Count();
                exam.Update_On = DateUtil.Now();
                exam.Update_By = update_by;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listcalendarexam")]
        public object listcalendarexam()
        {
            var exams = _context.Exams.Include(i => i.Subject.SubjectGroup).Where(w => 1 == 1);
            //var colours = new string[] { "#8dc63f", "#ffba00" ,"#40bbea", "#2c2e2f" };

            var objects = exams.Select(s => new
            {
                group = s.SubjectGroup.Name,
                color = (s.ExamPeriod == ExamPeriod.Morning ? s.SubjectGroup.Color1 : (s.ExamPeriod == ExamPeriod.Afternoon ? s.SubjectGroup.Color2 : (s.ExamPeriod == ExamPeriod.Evening ? s.SubjectGroup.Color3 : "#2c2e2f"))),
                date = DateUtil.ToInternalDate(s.ExamDate),
                examperiodName = s.ExamPeriod.toExamPeriodName(),
                examperiod = s.ExamPeriod,
                registeredcnt = _context.ExamRegisters.Where(w => w.Exam.ExamDate.Value.Date == s.ExamDate.Value.Date & w.Exam.ExamPeriod == s.ExamPeriod).Count(),
            }).Distinct();
           
            return objects.ToArray();
        }

       
    }
}