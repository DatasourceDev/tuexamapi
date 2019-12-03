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

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestResultController : ControllerBase
    {

        private readonly ILogger<TestResultController> _logger;
        public TuExamContext _context;

        public TestResultController(ILogger<TestResultController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }

       


        [HttpGet]
        [Route("inittestresultstudent")]
        public object inittestresultstudent(string student_search, string date_search)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            var examids = _context.ExamRegisters.Where(w => w.StudentID == studentid).Select(s => s.ExamID);

            var date = DateUtil.ToDate(date_search);
            var exams = _context.Exams
                .Include(i => i.Test)
                .Include(i => i.Subject)
                .Include(i => i.Subject.SubjectGroup)
                .Where(w => examids.Contains(w.ID) & w.ExamDate == date);

            foreach (var ex in exams)
            {
                var testresult = _context.TestResults.Where(w => w.ExamID == ex.ID).FirstOrDefault();
                if (testresult == null)
                {
                    testresult = new TestResult();
                    testresult.ExamID = ex.ID;
                    testresult.ProvedCnt = 0;
                    testresult.Update_On = DateUtil.Now();
                    testresult.Create_On = DateUtil.Now();
                    _context.TestResults.Add(testresult);
                }
            }
            _context.SaveChanges();

            var tstudents = new List<TestResultStudent>();
            foreach (var ex in exams)
            {
                var testresult = _context.TestResults.Where(w => w.ExamID == ex.ID).FirstOrDefault();
                var tstudent = _context.TestResultStudents.Include(i => i.Test).Where(w => w.ExamID == ex.ID && w.StudentID == studentid).FirstOrDefault();
                if (tstudent == null)
                {
                    tstudent = new TestResultStudent();
                    tstudent.ExamID = ex.ID;
                    tstudent.Exam = ex;
                    tstudent.ProveStatus = ProveStatus.Pending;
                    tstudent.ExamingStatus = ExamingStatus.None;
                    tstudent.StudentID = studentid;
                    tstudent.Update_On = DateUtil.Now();
                    tstudent.Create_On = DateUtil.Now();
                    tstudent.TestResultID = testresult.ID;
                    if (ex.Test == null)
                    {
                        var test = _context.Tests.Where(w => w.SubjectID == ex.SubjectID && w.Status == StatusType.Active).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
                        if (test != null)
                        {
                            tstudent.Test = test;
                            tstudent.TestID = test.ID;
                        }
                    }
                    else
                    {
                        tstudent.Test = ex.Test;
                        tstudent.TestID = ex.TestID.Value;
                    }
                    _context.TestResultStudents.Add(tstudent);
                }
                tstudents.Add(tstudent);
            }
            _context.SaveChanges();

            foreach (var tstudent in tstudents)
            {
                var i = 1;
                var tsqs = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID).FirstOrDefault();
                if (tsqs == null)
                {
                    if (tstudent.Test.TestQuestionType == TestQuestionType.Random)
                    {
                        var qrandoms = _context.TestQRandoms.Where(w => w.TestID == tstudent.TestID);
                        var ranresult = new List<TestResultStudentQAns>();
                        foreach (var ran in qrandoms)
                        {
                            if (ran.VeryEasy.HasValue && ran.VeryEasy.Value > 0)
                            {
                                var qrans = _context.Questions.Where(w => w.QuestionType == ran.QuestionType & w.QuestionLevel == QuestionLevel.VeryEasy).OrderBy(r => Guid.NewGuid()).Take(ran.VeryEasy.Value);
                                foreach (var q in qrans)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = q.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();
                                    ranresult.Add(tsq);
                                }
                            }
                            if (ran.Easy.HasValue && ran.Easy.Value > 0)
                            {
                                var qrans = _context.Questions.Where(w => w.QuestionType == ran.QuestionType & w.QuestionLevel == QuestionLevel.Easy).OrderBy(r => Guid.NewGuid()).Take(ran.Easy.Value);
                                foreach (var q in qrans)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = q.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();
                                    ranresult.Add(tsq);
                                }
                            }
                            if (ran.Mid.HasValue && ran.Mid.Value > 0)
                            {
                                var qrans = _context.Questions.Where(w => w.QuestionType == ran.QuestionType & w.QuestionLevel == QuestionLevel.Mid).OrderBy(r => Guid.NewGuid()).Take(ran.Mid.Value);
                                foreach (var q in qrans)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = q.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();
                                    ranresult.Add(tsq);
                                }
                            }
                            if (ran.Hard.HasValue && ran.Hard.Value > 0)
                            {
                                var qrans = _context.Questions.Where(w => w.QuestionType == ran.QuestionType & w.QuestionLevel == QuestionLevel.Hard).OrderBy(r => Guid.NewGuid()).Take(ran.Hard.Value);
                                foreach (var q in qrans)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = q.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();
                                    ranresult.Add(tsq);
                                }
                            }
                            if (ran.VeryHard.HasValue && ran.VeryHard.Value > 0)
                            {
                                var qrans = _context.Questions.Where(w => w.QuestionType == ran.QuestionType & w.QuestionLevel == QuestionLevel.VeryHard).OrderBy(r => Guid.NewGuid()).Take(ran.VeryHard.Value);
                                foreach (var q in qrans)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = q.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();

                                    ranresult.Add(tsq);
                                }
                            }
                        }
                        foreach (var tsq in ranresult.OrderBy(r => Guid.NewGuid()))
                        {
                            tsq.Index = i; i++;
                            _context.TestResultStudentQAnies.Add(tsq);
                        }
                    }
                    else
                    {
                        var qcustoms = _context.TestQCustoms.Where(w => w.TestID == tstudent.TestID);
                        if (tstudent.Test.TestCustomOrderType == TestCustomOrderType.Random)
                        {
                            qcustoms = qcustoms.OrderBy(r => Guid.NewGuid());
                        }
                        foreach (var custom in qcustoms)
                        {
                            var tsq = new TestResultStudentQAns();
                            tsq.QuestionID = custom.QuestionID;
                            tsq.TestResultStudentID = tstudent.ID;
                            tsq.Create_On = DateUtil.Now();
                            tsq.Update_On = DateUtil.Now();
                            tsq.Index = i; i++;
                            _context.TestResultStudentQAnies.Add(tsq);
                        }
                    }
                    tstudent.Test.QuestionCnt = i - 1;
                }
                else
                {
                    var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID).Count();
                    tstudent.Test.QuestionCnt = qcnt;
                }
            }
            _context.SaveChanges();

            return tstudents.Select(s => new
            {
                id = s.ID,
                testid = s.TestID,
                test = s.Test.Name,
                timelimit = s.Test.TimeLimit,
                timelimittype = s.Test.TimeLimitType.toTimeType(),
                questioncnt = s.Test.QuestionCnt,
                group = s.Exam.Subject.SubjectGroup.Name,
                subject = s.Exam.Subject.Name,
                subjectorder = s.Exam.Subject.Index,
                examingstatus = s.ExamingStatus,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.subjectorder).ToArray();
        }


        [HttpGet]
        [Route("start")]
        public object start(int? id)
        {
            var model = _context.TestResultStudents.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(start), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            if (model.ExamingStatus == ExamingStatus.None)
            {
                model.Start_On = DateUtil.Now();
                model.ExamingStatus = ExamingStatus.Examing;
            }
            model.Update_On = DateUtil.Now();

            var exam = _context.Exams.Where(w => w.ID == model.ExamID).FirstOrDefault();
            if(exam != null)
                exam.ExamRegisterCnt = _context.TestResultStudents.Where(w => w.ExamID == model.ExamID).Count();
                
            _context.SaveChanges();
            return CreatedAtAction(nameof(start), new { result = ResultCode.Success, message = ResultMessage.Success, index = 1 });
        }

        [HttpGet]
        [Route("con")]
        public object con(int? id, int? ix)
        {
            var model = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.Index == ix).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(inittestresultstudent), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var question = _context.Questions.Where(w => w.ID == model.QuestionID).FirstOrDefault();
            if (question == null)
                return CreatedAtAction(nameof(inittestresultstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });


            var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id).Count();
            var answeredcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.Answered == true).Count();
            var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID);

            decimal point = 0;
            if (model.QuestionAnsID.HasValue)
            {
                var answer = _context.QuestionAnies.Where(w => w.ID == model.QuestionAnsID).FirstOrDefault();
                if(answer != null)
                {
                    point = answer.Point;
                }
            }
            return CreatedAtAction(nameof(inittestresultstudent), new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                tresultstudentid = model.ID,
                index = ix,
                questionth = question.QuestionTh,
                questionen = question.QuestionEn,
                questiontype = question.QuestionType,
                questioncnt = qcnt,
                point = point,
                answeredcnt = answeredcnt,
                answerid = model.QuestionAnsID,
                answers = answers.OrderBy(o => o.Order).Select(s => new
                {
                    id = s.ID,
                    answerth = s.AnswerTh,
                    answeren = s.AnswerEn
                }).ToArray(),
            });
        }

        [HttpGet]
        [Route("answer")]
        public object answer(int? id, int? answerid)
        {
            var model = _context.TestResultStudentQAnies.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(answer), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            model.QuestionAnsID = answerid;
            model.Answered = true;
            _context.SaveChanges();
            return CreatedAtAction(nameof(answer), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("timestamp")]
        public object timestamp(int? id)
        {
            var model = _context.TestResultStudents.Include(i=>i.Test).Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(timestamp), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            model.TimeRemaining = 0;
            var maxs = 0;
            if (model.Test.TimeLimitType == TimeType.Hour)
                maxs = model.Test.TimeLimit * 60 * 60;
            else if (model.Test.TimeLimitType == TimeType.Minute)
                maxs = model.Test.TimeLimit * 60 ;
            else if(model.Test.TimeLimitType == TimeType.Second)
                maxs = model.Test.TimeLimit;

            if (model.Start_On.HasValue)
            {
                var seconds = 0;
                var diff = (DateUtil.Now() - model.Start_On).Value;
                if (diff.Hours > 0)
                    seconds += diff.Hours * 60 * 60;
                if (diff.Minutes > 0)
                    seconds += diff.Minutes * 60 ;
                if (diff.Seconds > 0)
                    seconds += diff.Seconds;
                model.TimeRemaining = maxs - seconds;
                if (maxs - seconds < 0)
                    model.TimeRemaining = 0;
                model.Update_On = DateUtil.Now();
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(timestamp), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listtestresult")]
        public object listtestresult(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, int pageno = 1)
        {
            var exam = _context.TestResults.Include(i => i.Exam.Subject).Include(i => i.Exam.Subject.SubjectGroup).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(text_search))
            {
                var exams = _context.TestResultStudents.Where(w => w.Student.FirstName.Contains(text_search)
               | w.Student.LastName.Contains(text_search)
               | w.Student.FirstNameEn.Contains(text_search)
               | w.Student.LastNameEn.Contains(text_search)
               | w.Student.IDCard.Contains(text_search)
               | w.Student.Phone.Contains(text_search)
               | w.Student.Email.Contains(text_search)
               | w.Student.Passport.Contains(text_search)
               | w.Student.StudentCode.Contains(text_search)
                ).Select(s => s.ExamID);

                exam = exam.Where(w => w.Exam.Test.Name.Contains(text_search)
                | w.Exam.ExamCode.Contains(text_search)
                | exams.Contains(w.ID)
                );
            }


            if (!string.IsNullOrEmpty(status_search))
                exam = exam.Where(w => w.ProveStatus == status_search.toProveStatus());

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    exam = exam.Where(w => w.Exam.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    exam = exam.Where(w => w.Exam.Subject.SubjectGroupID == groupID);
            }
            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                exam = exam.Where(w => w.Exam.ExamDate >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                exam = exam.Where(w => w.Exam.ExamDate <= date);
            }
            int skipRows = (pageno - 1) * 25;
            var itemcnt = exam.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listtestresult), new
            {
                data = exam.Select(s => new
                {
                    id = s.ID,
                    //status = s.Status.toStatusName(),
                    group = s.Exam.Subject.SubjectGroup.Name,
                    subject = s.Exam.Subject.Name,
                    subjectorder = s.Exam.Subject.Index,
                    examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                    date = s.Exam.ExamDate,
                    examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                    examperiodid = s.Exam.ExamPeriod,
                    registercnt = s.Exam.RegisterCnt,
                    examregistercnt = s.Exam.ExamRegisterCnt,
                    provedcnt = s.ProvedCnt,
                    unprovedcnt = s.Exam.ExamRegisterCnt - s.ProvedCnt,
                    provestatus =s.ProveStatus,
                    provestatusname =s.ProveStatus.toProveStatusName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderByDescending(o => o.date).ThenBy(o => o.examperiodid).ThenBy(o => o.group).ThenBy(o => o.subjectorder).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen
            }); ;

        }


        [HttpGet]
        [Route("gettestresult")]
        public object gettestresult(int? id)
        {
            var tresult = _context.TestResults
                .Include(i => i.Exam)
                .Include(i => i.Exam.SubjectGroup)
                .Include(i => i.Exam.Subject)
                .Where(w => w.ID == id).FirstOrDefault();

            if (tresult == null)
                return CreatedAtAction(nameof(gettestresult), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            return CreatedAtAction(nameof(gettestresult), new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                group = tresult.Exam.SubjectGroup.Name,
                subject = tresult.Exam.Subject.Name,
                examid=tresult.ExamID,
                examdate = DateUtil.ToDisplayDate(tresult.Exam.ExamDate),
                examperiod = tresult.Exam.ExamPeriod.toExamPeriodName(),
                registercnt = tresult.Exam.RegisterCnt,
                examregistercnt = tresult.Exam.ExamRegisterCnt,
                provedcnt = tresult.ProvedCnt,
                provestatus = tresult.ProveStatus.toProveStatusName(),
                unprovedcnt = tresult.Exam.ExamRegisterCnt - tresult.ProvedCnt,
            }); ;
        }

        [HttpGet]
        [Route("listtestresultstudent")]
        public object listtestresultstudent(int? id,string text_search, string status_search)
        {
            var tstudents = _context.TestResultStudents
                .Include(i => i.TestResult)
                .Include(i => i.Student)
                .Include(i => i.Test).Where(w => w.ExamID == id);

            if (!string.IsNullOrEmpty(text_search))
            {              

                tstudents = tstudents.Where(w => w.Exam.Test.Name.Contains(text_search)
                | w.Student.FirstName.Contains(text_search)
                | w.Exam.ExamCode.Contains(text_search)
                | w.Student.LastName.Contains(text_search)
                | w.Student.FirstNameEn.Contains(text_search)
                | w.Student.LastNameEn.Contains(text_search)
                | w.Student.IDCard.Contains(text_search)
                | w.Student.Phone.Contains(text_search)
                | w.Student.Email.Contains(text_search)
                | w.Student.Passport.Contains(text_search)
                | w.Student.StudentCode.Contains(text_search)
                );
            }

            if (!string.IsNullOrEmpty(status_search))
                tstudents = tstudents.Where(w => w.ProveStatus == status_search.toProveStatus());

            return tstudents.Select(s => new
            {
                id = s.ID,
                studentid = s.StudentID,
                studentcode = s.Student.StudentCode,
                prefix = s.Student.Prefix.toPrefixName(),
                firstname =s.Student.FirstName,
                firstnameen = s.Student.FirstNameEn,
                lastname = s.Student.LastName,
                lastnameen = s.Student.LastNameEn,
                starton = DateUtil.ToDisplayDateTime(s.Start_On),
                endon = DateUtil.ToDisplayDateTime(s.End_On),
                testid = s.TestID,
                test = s.Test.Name,
                questioncnt = s.QuestionCnt,
                answeredcnt = s.AnsweredCnt,
                correctcnt = s.CorrectCnt,
                wrongcnt = s.WrongCnt,
                point = s.Point,
                examingstatus = s.ExamingStatus.toExamingStatus(),
                sendbyemail = s.SendByEmail,
                sendbypost= s.SendByPost,
                other= s.Other,
                email = s.Email,
                address = s.Address,
                phone= s.Student.Phone,
                provestatus = s.ProveStatus,
                provestatusname = s.ProveStatus.toProveStatusName(),
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.studentcode).ToArray();
        }

        [HttpGet]
        [Route("gettestresultstudent")]
        public object gettestresultstudent(int? id)
        {
            var tresult = _context.TestResultStudents
                .Include(i => i.Test)
                .Include(i => i.Student)
                .Include(i => i.Exam)
                .Include(i => i.Exam.SubjectGroup)
                .Include(i => i.Exam.Subject)
                .Where(w => w.ID == id).FirstOrDefault();

            if (tresult == null)
                return CreatedAtAction(nameof(gettestresultstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id).Count();
            var answeredcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.Answered == true).Count();

            var maxs = 0;
            tresult.TimeRemaining = 0;
            if (tresult.Test != null)
            {
                if (tresult.Test.TimeLimitType == TimeType.Hour)
                    maxs = tresult.Test.TimeLimit * 60 * 60;
                else if (tresult.Test.TimeLimitType == TimeType.Minute)
                    maxs = tresult.Test.TimeLimit * 60;
                else if (tresult.Test.TimeLimitType == TimeType.Second)
                    maxs = tresult.Test.TimeLimit;
            }
            if (tresult.Start_On.HasValue)
            {
                var seconds = 0;
                var diff = (DateUtil.Now() - tresult.Start_On).Value;
                if (diff.Hours > 0)
                    seconds += diff.Hours * 60 * 60;
                if (diff.Minutes > 0)
                    seconds += diff.Minutes * 60;
                if (diff.Seconds > 0)
                    seconds += diff.Seconds;
                tresult.TimeRemaining = maxs - seconds;
                if (maxs - seconds < 0)
                    tresult.TimeRemaining = 0;
            }
            tresult.QuestionCnt = qcnt;
            tresult.AnsweredCnt = answeredcnt;
            _context.SaveChanges();
            return CreatedAtAction(nameof(gettestresultstudent), new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                tid = tresult.TestResultID,
                prefix = tresult.Student.Prefix.toPrefixName(),
                firstname= tresult.Student.FirstName,
                lastname= tresult.Student.LastName,
                firstnameen = tresult.Student.FirstNameEn,
                lastnameen = tresult.Student.LastNameEn,
                group = tresult.Exam.SubjectGroup.Name,
                subject = tresult.Exam.Subject.Name,
                questioncnt = qcnt,
                answeredcnt = answeredcnt,
                starton = DateUtil.ToDisplayDateTime( tresult.Start_On),
                endon = DateUtil.ToDisplayDateTime(tresult.End_On),
                correctcnt = tresult.CorrectCnt,
                wrongcnt= tresult.WrongCnt,
                point = tresult.Point,
                examingstatus = tresult.ExamingStatus,
                timeremaining = tresult.TimeRemaining,
            });
        }

        [HttpPost]
        [Route("sendresult")]
        public object sendresult([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<TestSendResultDTO>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(sendresult), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            var tresultstudent = _context.TestResultStudents.Where(w => w.ID == model.ID).FirstOrDefault();
            if (tresultstudent == null)
                return CreatedAtAction(nameof(start), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            tresultstudent.End_On = DateUtil.Now();
            tresultstudent.ExamingStatus = ExamingStatus.Done;
            tresultstudent.Email = model.Email;
            tresultstudent.Address = model.Address;
            tresultstudent.Update_By = model.Update_By;
            tresultstudent.Update_On = DateUtil.Now();
            tresultstudent.SendByEmail = model.SendByEmail;
            tresultstudent.SendByPost = model.SendByPost;
            tresultstudent.Other = model.Other;
            tresultstudent.Point = 0;
            tresultstudent.ProveStatus = ProveStatus.Pending;
            tresultstudent.WrongCnt = 0;
            tresultstudent.CorrectCnt =0 ;
            var proved = 0;
            var tanswers = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == model.ID);
            foreach(var tanswer in tanswers)
            {
                var haspoint = false;
                var question = _context.Questions.Where(w => w.ID == tanswer.QuestionID).FirstOrDefault();
                if(question != null)
                {
                    if(question.QuestionType == QuestionType.MultipleChoice)
                    {
                        if (tanswer.QuestionAnsID.HasValue)
                        {
                            var answer = _context.QuestionAnies.Where(w => w.ID == tanswer.QuestionAnsID).FirstOrDefault();
                            if (answer != null)
                            {
                                tresultstudent.Point += answer.Point;
                                if (tresultstudent.Point > 0)
                                {
                                    haspoint = true;
                                    tresultstudent.CorrectCnt += 1;
                                }
                            }
                        }
                        proved++;
                    }
                }
                if (!haspoint)
                    tresultstudent.WrongCnt += 1;
            }
            if (tresultstudent.QuestionCnt == proved)
                tresultstudent.ProveStatus = ProveStatus.Proved;
            _context.SaveChanges();

            var students = _context.TestResultStudents.Where(w => w.TestResultID == tresultstudent.TestResultID);
            var provecnt = students.Where(w => w.ProveStatus == ProveStatus.Proved).Count();
            var studentcnt = students.Count();
            if (studentcnt == provecnt)
            {
                var tresult = _context.TestResults.Where(w => w.ID == tresultstudent.TestResultID).FirstOrDefault();
                if (tresult != null)
                    tresult.ProveStatus = ProveStatus.Proved;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(gettestresult), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
