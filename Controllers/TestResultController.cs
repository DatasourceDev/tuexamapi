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
using System.IO;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestResultController : ControllerBase
    {
        public SystemConf _conf;
        private readonly ILogger<TestResultController> _logger;
        public TuExamContext _context;

        public TestResultController(ILogger<TestResultController> logger, TuExamContext context, IOptions<SystemConf> conf)
        {
            this._logger = logger;
            this._context = context;
            this._conf = conf.Value;
        }


        [HttpGet]
        [Route("inittestresultstudent")]
        public object inittestresultstudent(string student_search, string date_search, string update_by)
        {
            var studentid = NumUtil.ParseInteger(student_search);
            if(studentid <= 0 )
                return CreatedAtAction(nameof(start), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var examids = _context.ExamRegisters.Where(w => w.StudentID == studentid).Select(s => s.ExamID);

            var date = DateUtil.ToDate(date_search);
            var exams = _context.Exams
                .Include(i => i.Test)
                .Include(i => i.Subject)
                .Include(i => i.Subject.SubjectGroup)
                .Where(w => examids.Contains(w.ID) & w.ExamDate.Value.Date == date.Value.Date);

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
                    testresult.Update_By = update_by;
                    testresult.Create_By = update_by;
                    _context.TestResults.Add(testresult);
                }
            }
            _context.SaveChanges();

            var tstudents = new List<TestResultStudent>();
            foreach (var ex in exams)
            {
                var testresult = _context.TestResults.Where(w => w.ExamID == ex.ID).FirstOrDefault();
                var tstudent = _context.TestResultStudents.Include(i => i.Test).Where(w => w.ExamID == ex.ID && w.StudentID == studentid).FirstOrDefault();
                var register = _context.ExamRegisters.Where(w => w.StudentID == studentid & w.ExamID == ex.ID).FirstOrDefault();
                testresult.ProveStatus = ProveStatus.Pending;
                testresult.Update_On = DateUtil.Now();
                testresult.Update_By = update_by;
                if (tstudent == null)
                {
                    tstudent = new TestResultStudent();
                    tstudent.ExamID = ex.ID;
                    tstudent.Exam = ex;
                    tstudent.ProveStatus = ProveStatus.Pending;
                    tstudent.ExamingStatus = ExamingStatus.None;
                    tstudent.ExamRegisterType = register.ExamRegisterType;
                    tstudent.StudentID = studentid;
                    tstudent.Create_On = DateUtil.Now();
                    tstudent.Create_By = update_by;
                    tstudent.Update_On = DateUtil.Now();
                    tstudent.Update_By = update_by;
                    tstudent.TestResultID = testresult.ID;
                    if (ex.Test == null)
                    {
                        var test = _context.Tests.Where(w => w.SubjectID == ex.SubjectID && w.Status == StatusType.Active).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
                        if (test != null)
                        {
                            tstudent.Test = test;
                            tstudent.TestID = test.ID;
                            _context.TestResultStudents.Add(tstudent);
                            tstudents.Add(tstudent);
                        }
                    }
                    else
                    {
                        tstudent.Test = ex.Test;
                        tstudent.TestID = ex.TestID.Value;
                        _context.TestResultStudents.Add(tstudent);
                        tstudents.Add(tstudent);
                    }
                }
                else
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
                        var qrandoms = _context.TestQRandoms.Include(i => i.Test).Where(w => w.TestID == tstudent.TestID);
                        var ranresults = new List<TestResultStudentQAns>();
                        var levels = new List<QuestionLevel>();
                        levels.Add(QuestionLevel.VeryEasy);
                        levels.Add(QuestionLevel.Easy);
                        levels.Add(QuestionLevel.Mid);
                        levels.Add(QuestionLevel.Hard);
                        levels.Add(QuestionLevel.VeryHard);
                        levels = levels.OrderBy(r => Guid.NewGuid()).ToList();
                        foreach (var ran in qrandoms)
                        {
                            foreach (var level in levels)
                            {
                                var number = 0;
                                if (level == QuestionLevel.VeryEasy)
                                    number = ran.VeryEasy.HasValue ? ran.VeryEasy.Value : 0;
                                else if (level == QuestionLevel.Easy)
                                    number = ran.Easy.HasValue ? ran.Easy.Value : 0;
                                else if (level == QuestionLevel.Mid)
                                    number = ran.Mid.HasValue ? ran.Mid.Value : 0;
                                else if (level == QuestionLevel.Hard)
                                    number = ran.Hard.HasValue ? ran.Hard.Value : 0;
                                else if (level == QuestionLevel.VeryHard)
                                    number = ran.VeryHard.HasValue ? ran.VeryHard.Value : 0;

                                if (number > 0)
                                {
                                    var ranresult = getrandomquestion(tstudent.ID, ran, level, number, update_by);
                                    if (ranresult.Count > 0)
                                        ranresults.AddRange(ranresult);
                                }
                            }
                        }
                        foreach (var tsq in ranresults)
                        {
                            tsq.Index = i; i++;
                            _context.TestResultStudentQAnies.Add(tsq);
                        }
                    }
                    else
                    {
                        var qcustoms = _context.TestQCustoms.Include(i => i.Question).Where(w => w.TestID == tstudent.TestID).OrderBy(o => o.Order);
                        if (tstudent.Test.TestCustomOrderType == TestCustomOrderType.Random)
                            qcustoms = qcustoms.OrderBy(r => Guid.NewGuid());

                        foreach (var custom in qcustoms)
                        {
                            if (custom.Question.QuestionType == QuestionType.ReadingText | custom.Question.QuestionType == QuestionType.MultipleMatching)
                            {
                                var children = _context.Questions.Where(w => w.QuestionParentID == custom.QuestionID).OrderBy(o => o.ChildOrder);
                                foreach (var child in children)
                                {
                                    var tsq = new TestResultStudentQAns();
                                    tsq.QuestionID = child.ID;
                                    tsq.TestResultStudentID = tstudent.ID;
                                    tsq.Create_On = DateUtil.Now();
                                    tsq.Update_On = DateUtil.Now();
                                    tsq.Create_By = update_by;
                                    tsq.Update_By = update_by;
                                    tsq.Index = i;
                                    i++;
                                    _context.TestResultStudentQAnies.Add(tsq);
                                }
                            }
                            else
                            {
                                var tsq = new TestResultStudentQAns();
                                tsq.QuestionID = custom.QuestionID;
                                tsq.TestResultStudentID = tstudent.ID;
                                tsq.Create_On = DateUtil.Now();
                                tsq.Update_On = DateUtil.Now();
                                tsq.Create_By = update_by;
                                tsq.Update_By = update_by;
                                tsq.Index = i;
                                i++;
                                _context.TestResultStudentQAnies.Add(tsq);
                            }

                        }
                    }
                    tstudent.Test.QuestionCnt = i - 1;
                    tstudent.QuestionCnt = i - 1;
                }
                else
                {
                    var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID).Count();
                    tstudent.Test.QuestionCnt = qcnt;
                    tstudent.QuestionCnt = qcnt;
                }
            }
            _context.SaveChanges();

            foreach (var ex in exams)
            {
                var exam = _context.Exams.Where(w => w.ID == ex.ID).FirstOrDefault();
                if (exam != null)
                {
                    exam.ExamRegisterCnt = _context.TestResultStudents.Where(w => w.ExamID == ex.ID).Count();
                    exam.Update_On = DateUtil.Now();
                    exam.Update_By = update_by;
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
                questioncnt = s.QuestionCnt,
                groupname = s.Exam.Subject.SubjectGroup.Name,
                group = s.Exam.Subject.SubjectGroup.ID,
                doexamorder = s.Exam.Subject.SubjectGroup.DoExamOrder,
                subject = s.Exam.Subject.Name,
                subjectorder = s.Exam.Subject.Order,
                examingstatus = s.ExamingStatus,
                allowexam = !s.Exam.Subject.SubjectGroup.DoExamOrder,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.subjectorder).ToArray();
        }



        private List<TestResultStudentQAns> getrandomquestion(int tstudentid, TestQRandom ran, QuestionLevel level, int number, string update_by)
        {
            var rancnt = 0;
            var ranresult = new List<TestResultStudentQAns>();

            for (var i = 0; i < 10; i++)
            {
                rancnt = 0;
                ranresult = new List<TestResultStudentQAns>();
                var qrans = _context.Questions
                    .Where(w => w.SubjectGroupID == ran.Test.SubjectGroupID
                    & w.SubjectID == ran.Test.SubjectID
                    & w.QuestionType == ran.QuestionType
                    & w.QuestionLevel == level && !w.QuestionParentID.HasValue)
                    .OrderBy(r => Guid.NewGuid());

                if (qrans.Count() == 0)
                    break;

                foreach (var q in qrans)
                {
                    if (rancnt >= number)
                        break;

                    if (q.QuestionType == QuestionType.ReadingText | q.QuestionType == QuestionType.MultipleMatching)
                    {
                        var children = _context.Questions.Where(w => w.QuestionParentID == q.ID);
                        if (children.Count() + rancnt > number)
                            break;
                        foreach (var child in children)
                        {
                            var tsq = new TestResultStudentQAns();
                            tsq.QuestionID = child.ID;
                            tsq.TestResultStudentID = tstudentid;
                            tsq.Create_On = DateUtil.Now();
                            tsq.Create_By = update_by;
                            tsq.Update_On = DateUtil.Now();
                            tsq.Update_By = update_by;
                            ranresult.Add(tsq);
                        }
                        rancnt += children.Count();
                    }
                    else
                    {
                        var tsq = new TestResultStudentQAns();
                        tsq.QuestionID = q.ID;
                        tsq.TestResultStudentID = tstudentid;
                        tsq.Create_On = DateUtil.Now();
                        tsq.Create_By = update_by;
                        tsq.Update_On = DateUtil.Now();
                        tsq.Update_By = update_by;
                        ranresult.Add(tsq);
                        rancnt++;
                    }

                }

                if (rancnt == number)
                    break;
            }
            return ranresult;
        }

        [HttpGet]
        [Route("start")]
        public object start(int? id, string update_by)
        {
            var model = _context.TestResultStudents.Include(i => i.Test).Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(start), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var examing = _context.TestResultStudents.Where(w => w.ID != id & w.StudentID == model.StudentID & w.ExamingStatus == ExamingStatus.Examing).FirstOrDefault();
            if (examing != null)
                return CreatedAtAction(nameof(start), new { result = ResultCode.DuplicateData, message = "ไม่สามารถเริ่มแบบทดสอบนี้ได้ เนื่องจากผู้เข้าสอบอยู่ระหว่างทำแบบทดสอบอื่น" });

            if (model.ExamingStatus == ExamingStatus.None)
            {
                model.Start_On = DateUtil.Now();
                model.ExamingStatus = ExamingStatus.Examing;
                if (model.Test != null)
                    model.Expriry_On = model.Start_On.Value.AddSeconds(FuncUtil.GetMaxTimeLimit(model.Test.TimeLimitType, model.Test.TimeLimit));
            }
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            var exam = _context.Exams.Where(w => w.ID == model.ExamID).FirstOrDefault();
            if (exam != null)
            {
                exam.ExamRegisterCnt = _context.TestResultStudents.Where(w => w.ExamID == model.ExamID).Count();
                exam.Update_On = DateUtil.Now();
                exam.Update_By = update_by;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(start), new { result = ResultCode.Success, message = ResultMessage.Success, index = 1 });
        }

        [HttpGet]
        [Route("con")]
        public object con(int? id, int? ix, string update_by)
        {
            var model = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.Index == ix).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(inittestresultstudent), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var question = _context.Questions.Where(w => w.ID == model.QuestionID).FirstOrDefault();
            if (question == null)
                return CreatedAtAction(nameof(inittestresultstudent), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });


            var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id).Count();
            var answeredindex = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.Answered == true).Select(s => s.Index);
            var provedindex = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == id & w.ProveStatus == ProveStatus.Proved).Select(s => s.Index);

            var answeredcnt = answeredindex.Count();
            var attanswers = new List<object>();
            var mmanswers = new List<QuestionAns>();
            var mcanswers = new List<QuestionAns>();
            if (question.QuestionType == QuestionType.MultipleChoice)
                mcanswers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID).ToList();
            else if (question.QuestionType == QuestionType.MultipleMatchingSub)
                mmanswers = _context.QuestionAnies.Where(w => w.QuestionID == question.QuestionParentID).ToList();
            else if (question.QuestionType == QuestionType.Attitude)
            {
                var attsetup = _context.AttitudeSetups.Where(w => w.AttitudeAnsType == question.AttitudeAnsType & w.AttitudeAnsSubType == question.AttitudeAnsSubType).FirstOrDefault();
                if (attsetup != null)
                {
                    if (question.AttitudeAnsType == AttitudeAnsType.Type2 | question.AttitudeAnsType == AttitudeAnsType.Type3 | question.AttitudeAnsType == AttitudeAnsType.Type4 | question.AttitudeAnsType == AttitudeAnsType.Type5 | question.AttitudeAnsType == AttitudeAnsType.Type6 | question.AttitudeAnsType == AttitudeAnsType.Type7)
                    {
                        attanswers.Add(new { id = 1, answerth = attsetup.Text1, answeren = attsetup.TextEn1 });
                        attanswers.Add(new { id = 2, answerth = attsetup.Text2, answeren = attsetup.TextEn2 });
                    }
                    if (question.AttitudeAnsType == AttitudeAnsType.Type3 | question.AttitudeAnsType == AttitudeAnsType.Type4 | question.AttitudeAnsType == AttitudeAnsType.Type5 | question.AttitudeAnsType == AttitudeAnsType.Type6 | question.AttitudeAnsType == AttitudeAnsType.Type7)
                        attanswers.Add(new { id = 3, answerth = attsetup.Text3, answeren = attsetup.TextEn3 });
                    if (question.AttitudeAnsType == AttitudeAnsType.Type4 | question.AttitudeAnsType == AttitudeAnsType.Type5 | question.AttitudeAnsType == AttitudeAnsType.Type6 | question.AttitudeAnsType == AttitudeAnsType.Type7)
                        attanswers.Add(new { id = 4, answerth = attsetup.Text4, answeren = attsetup.TextEn4 });
                    if (question.AttitudeAnsType == AttitudeAnsType.Type5 | question.AttitudeAnsType == AttitudeAnsType.Type6 | question.AttitudeAnsType == AttitudeAnsType.Type7)
                        attanswers.Add(new { id = 5, answerth = attsetup.Text5, answeren = attsetup.TextEn5 });
                    if (question.AttitudeAnsType == AttitudeAnsType.Type6 | question.AttitudeAnsType == AttitudeAnsType.Type7)
                        attanswers.Add(new { id = 6, answerth = attsetup.Text6, answeren = attsetup.TextEn6 });
                    if (question.AttitudeAnsType == AttitudeAnsType.Type7)
                        attanswers.Add(new { id = 7, answerth = attsetup.Text7, answeren = attsetup.TextEn7 });
                }
            }

            var pquestionth = "";
            var pquestionen = "";
            var pid = "";
            var pfileurl = "";
            var pfiletype = "";
            if (question.QuestionParentID.HasValue && question.QuestionParentID.Value > 0)
            {
                var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                if (parent != null)
                {
                    pid = parent.ID.ToString();
                    pquestionth = parent.QuestionTh;
                    pquestionen = parent.QuestionEn;
                    pfileurl = parent.FileUrl;
                    pfiletype = parent.FileType;
                }
            }

            return CreatedAtAction(nameof(inittestresultstudent), new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                tresultstudentid = model.ID,
                index = ix,
                pid = pid,
                pquestionth = pquestionth,
                pquestionen = pquestionen,
                questionth = question.QuestionTh,
                questionen = question.QuestionEn,
                questiontype = question.QuestionType,
                fileurl = question.FileUrl,
                filetype = question.FileType,
                pfileurl = pfileurl,
                pfiletype = pfiletype,
                questioncnt = qcnt,
                answeredcnt = answeredcnt,
                answeredindex = answeredindex,
                provedindex= provedindex,
                answerid = model.QuestionAnsID,
                attanswerid = model.QuestionAnsAttitudeID,
                tfanswer = model.TFAns,
                textanswer = model.TextAnswer,
                filenameanswer = model.FileName,
                fileurlanswer = model.FileUrl,
                point = model.Point,
                provestatus = model.ProveStatus.toProveStatusName(),
                mcanswers = mcanswers.OrderBy(o => o.Order).Select(s => new
                {
                    id = s.ID,
                    answerth = s.AnswerTh,
                    answeren = s.AnswerEn
                }).ToArray(),
                mmanswers = mmanswers.OrderBy(o => o.Order).Select(s => new
                {
                    id = s.ID,
                    choice = s.Choice,
                    answerth = s.AnswerTh,
                    answeren = s.AnswerEn
                }).ToArray(),
                attanswers = attanswers.ToArray(),
            });
        }

        [HttpGet]
        [Route("answer")]
        public object answer(int? id, int? qtype, int? answerid, string update_by, string textanswer)
        {
            var model = _context.TestResultStudentQAnies.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(answer), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var questiontype = qtype.HasValue ? qtype.Value.ToString().toQuestionType() : QuestionType.MultipleChoice;
            if (questiontype == QuestionType.MultipleChoice)
                model.QuestionAnsID = answerid;
            else if (questiontype == QuestionType.TrueFalse)
            {
                if (answerid == 1)
                    model.TFAns = true;
                else if (answerid == 0)
                    model.TFAns = false;
            }
            if (questiontype == QuestionType.MultipleMatching | questiontype == QuestionType.MultipleMatchingSub)
                model.QuestionAnsID = answerid;
            else if (questiontype == QuestionType.ShortAnswer | questiontype == QuestionType.Essay)
                model.TextAnswer = textanswer;
            else if (questiontype == QuestionType.Attitude)
                model.QuestionAnsAttitudeID = answerid;

            if (questiontype == QuestionType.ShortAnswer | questiontype == QuestionType.Essay)
            {
                if (!string.IsNullOrEmpty(model.TextAnswer))
                    model.Answered = true;
                else
                    model.Answered = false;
            }
            else
                model.Answered = true;

            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            _context.SaveChanges();
            return CreatedAtAction(nameof(answer), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("pointanswer")]
        public object pointanswer(int? id, int? qtype, string update_by, string pointanswer)
        {
            var model = _context.TestResultStudentQAnies.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(answer), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var questiontype = qtype.HasValue ? qtype.Value.ToString().toQuestionType() : QuestionType.MultipleChoice;
            if (questiontype == QuestionType.ShortAnswer | questiontype == QuestionType.Essay | questiontype == QuestionType.Assignment)
            {
                if (!string.IsNullOrEmpty(pointanswer))
                {

                    model.Point = NumUtil.ParseDecimal(pointanswer);
                    model.Update_On = DateUtil.Now();
                    model.Update_By = update_by;
                    _context.SaveChanges();
                }
            }
            decimal totalpoint = prove(model.TestResultStudentID, update_by);
            return CreatedAtAction(nameof(answer), new { result = ResultCode.Success, message = ResultMessage.Success, totalpoint = totalpoint });
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("fileupload")]
        [Obsolete]
        public object fileupload([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportAnswerDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var id = NumUtil.ParseInteger(model.tresultstudentid);
                var tresultstudent = _context.TestResultStudentQAnies.Where(w => w.ID == id).FirstOrDefault();
                if (tresultstudent != null)
                {
                    var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\resultanwers\\" + tresultstudent.ID + "\\";
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    var filename = filePath + model.fileupload.filename;

                    var file = Convert.FromBase64String(model.fileupload.value);
                    using (MemoryStream ms = new MemoryStream(file))
                    {
                        FileStream filestream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                        ms.WriteTo(filestream);
                        filestream.Close();
                        ms.Close();
                    }

                    var fileurl = filename;
                    var host =
                    fileurl = fileurl.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", this._conf.HostUrl);
                    fileurl = fileurl.Replace("\\", "/");
                    tresultstudent.FileName = model.fileupload.filename;
                    tresultstudent.FileUrl = fileurl;
                    tresultstudent.FileType = model.fileupload.filetype;
                    tresultstudent.Update_By = model.update_by;
                    tresultstudent.Update_On = DateUtil.Now();
                    tresultstudent.Answered = true;
                    _context.SaveChanges();
                    return CreatedAtAction(nameof(fileupload), new
                    {
                        result = ResultCode.Success,
                        message = ResultMessage.Success,
                        filetype = tresultstudent.FileType,
                        fileurl = tresultstudent.FileUrl,
                        filename = tresultstudent.FileName,
                    });
                }

            }
            return CreatedAtAction(nameof(fileupload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }
        [HttpGet]
        [Route("timestamp")]
        public object timestamp(int? id, string update_by)
        {
            var model = _context.TestResultStudents.Include(i => i.Test).Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(timestamp), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            model.TimeRemaining = FuncUtil.GetTimeRemaining(model.Test.TimeLimitType, model.Test.TimeLimit, model.Start_On);
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            _context.SaveChanges();
            return CreatedAtAction(nameof(timestamp), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listtestresult")]
        public object listtestresult(string text_search, string status_search, string group_search, string subject_search, string from_search, string to_search, int pageno = 1)
        {
            var tresult = _context.TestResults.Include(i => i.Exam.Test).Include(i => i.Exam.Subject).Include(i => i.Exam.Subject.SubjectGroup).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(status_search))
                tresult = tresult.Where(w => w.ProveStatus == status_search.toProveStatus());

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    tresult = tresult.Where(w => w.Exam.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    tresult = tresult.Where(w => w.Exam.Subject.SubjectGroupID == groupID);
            }
            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                tresult = tresult.Where(w => w.Exam.ExamDate >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                tresult = tresult.Where(w => w.Exam.ExamDate <= date);
            }

            var tresults = new List<TestResult>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();

                        var registerIds = _context.TestResultStudents.Where(w => w.Student.FirstName.Contains(text)
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

                        tresults.AddRange(tresult.Where(w => w.Exam.Test.Name.Contains(text)
                            | w.Exam.ExamCode.Contains(text)
                            | registerIds.Contains(w.ExamID)
                            ));
                    }
                }
                tresults = tresults.Distinct().ToList();
            }
            else
            {
                tresults = tresult.ToList();
            }
            int skipRows = (pageno - 1) * 25;
            var itemcnt = tresults.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listtestresult), new
            {
                data = tresults.Select(s => new
                {
                    id = s.ID,
                    //status = s.Status.toStatusName(),
                    test = s.Exam.ExamTestType == ExamTestType.Random ? _context.Tests.Where(w => w.SubjectID == s.Exam.SubjectID & w.Status == StatusType.Active).Count() == 1 ? _context.Tests.Where(w => w.SubjectID == s.Exam.SubjectID & w.Status == StatusType.Active).FirstOrDefault().Name : "สุ่ม" : s.Exam.Test.Name,
                    group = s.Exam.Subject.SubjectGroup.Name,
                    subject = s.Exam.Subject.Name,
                    subjectorder = s.Exam.Subject.Order,
                    examdate = DateUtil.ToDisplayDate(s.Exam.ExamDate),
                    date = s.Exam.ExamDate,
                    examperiod = s.Exam.ExamPeriod.toExamPeriodName(),
                    examperiodid = s.Exam.ExamPeriod,
                    registercnt = s.Exam.RegisterCnt,
                    examregistercnt = s.Exam.ExamRegisterCnt,
                    provedcnt = s.ProvedCnt,
                    unprovedcnt = s.Exam.ExamRegisterCnt - s.ProvedCnt,
                    provestatus = s.ProveStatus,
                    provestatusname = s.ProveStatus.toProveStatusName(),
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
                examid = tresult.ExamID,
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
        public object listtestresultstudent(int? id, string text_search, string status_search)
        {
            var tstudent = _context.TestResultStudents
                .Include(i => i.TestResult)
                .Include(i => i.Student)
                .Include(i => i.Test).Where(w => w.ExamID == id);

            if (!string.IsNullOrEmpty(status_search))
                tstudent = tstudent.Where(w => w.ProveStatus == status_search.toProveStatus());


            var tstudents = new List<TestResultStudent>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        tstudents.AddRange(tstudent.Where(w => w.Exam.Test.Name.Contains(text)
                            | w.Student.FirstName.Contains(text)
                            | w.Exam.ExamCode.Contains(text)
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
                        ));
                    }
                }
                tstudents = tstudents.Distinct().ToList();
            }
            else
            {
                tstudents = tstudent.ToList();
            }

            return tstudents.Select(s => new
            {
                id = s.ID,
                studentid = s.StudentID,
                studentcode = s.Student.StudentCode,
                prefix = s.Student.Prefix.toPrefixName(),
                firstname = s.Student.FirstName,
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
                sendbypost = s.SendByPost,
                other = s.Other,
                email = s.Email,
                address = s.Address,
                phone = s.Student.Phone,
                provestatus = s.ProveStatus,
                provestatusname = s.ProveStatus.toProveStatusName(),
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.studentcode).ToArray();
        }

        [HttpGet]
        [Route("listregistered")]
        public object listregistered(int? id)
        {
            var teststudent = _context.TestResultStudents.Where(w => w.ExamID == id).Select(s => s.StudentID);
            var exam = _context.ExamRegisters.Include(i => i.Exam).Where(w => w.ExamID == id & !teststudent.Contains(w.StudentID));
            return exam.Select(s => new
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
                examregistertype = s.ExamRegisterType.toExamRegisterType(),
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

            if (tresult.Test != null)
            {
                tresult.TimeRemaining = FuncUtil.GetTimeRemaining(tresult.Test.TimeLimitType, tresult.Test.TimeLimit, tresult.Start_On);
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
                firstname = tresult.Student.FirstName,
                lastname = tresult.Student.LastName,
                firstnameen = tresult.Student.FirstNameEn,
                lastnameen = tresult.Student.LastNameEn,
                address = tresult.Student.Address,
                email = tresult.Student.Email,
                group = tresult.Exam.SubjectGroup.Name,
                groupid = tresult.Exam.SubjectGroupID,
                subject = tresult.Exam.Subject.Name,
                subjectid = tresult.Exam.SubjectID,
                questioncnt = qcnt,
                answeredcnt = answeredcnt,
                starton = DateUtil.ToDisplayDateTime(tresult.Start_On),
                endon = DateUtil.ToDisplayDateTime(tresult.End_On),
                correctcnt = tresult.CorrectCnt,
                wrongcnt = tresult.WrongCnt,
                point = tresult.Point,
                examingstatus = tresult.ExamingStatus,
                timeremaining = tresult.TimeRemaining,
                sendbyemail = tresult.SendByEmail,
                sendbypost = tresult.SendByPost,
                other = tresult.Other,
                showresult = tresult.Test.ShowResult,
            }); ;
        }

        [HttpGet]
        [Route("testresultstudentisexsit")]
        public object testresultstudentisexsit(int? examid, int? studentid)
        {
            var tresult = _context.TestResultStudents.Where(w => w.ExamID == examid & w.StudentID == studentid).FirstOrDefault();

            if (tresult == null)
                return CreatedAtAction(nameof(testresultstudentisexsit), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            return CreatedAtAction(nameof(testresultstudentisexsit), new { result = ResultCode.Success, message = ResultMessage.Success });
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

            if (tresultstudent.ExamingStatus == ExamingStatus.Examing)
            {
                tresultstudent.ExamingStatus = ExamingStatus.Done;
                tresultstudent.End_On = DateUtil.Now();
            }
            else if (tresultstudent.ExamingStatus == ExamingStatus.None)
            {
                tresultstudent.ExamingStatus = ExamingStatus.Absent;
            }

            tresultstudent.Email = model.Email;
            tresultstudent.Address = model.Address;
            tresultstudent.Update_By = model.Update_By;
            tresultstudent.Update_On = DateUtil.Now();
            tresultstudent.SendByEmail = model.SendByEmail;
            tresultstudent.SendByPost = model.SendByPost;
            tresultstudent.Other = model.Other;
            _context.SaveChanges();
            prove(model.ID, model.Update_By);
            return CreatedAtAction(nameof(gettestresult), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        private decimal prove(int? tresultstudentid, string update_by)
        {
            var tresultstudent = _context.TestResultStudents.Where(w => w.ID == tresultstudentid).FirstOrDefault();
            if (tresultstudent == null)
                return 0;

            tresultstudent.Point = 0;
            tresultstudent.ProveStatus = ProveStatus.Pending;
            tresultstudent.WrongCnt = 0;
            tresultstudent.CorrectCnt = 0;

            /*calculate point and prove per question*/
            var tanswers = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tresultstudentid);
            foreach (var tanswer in tanswers)
            {
                var question = _context.Questions.Where(w => w.ID == tanswer.QuestionID).FirstOrDefault();
                if (question != null)
                {
                    if (question.QuestionType == QuestionType.MultipleChoice)
                    {
                        if (tanswer.QuestionAnsID.HasValue)
                        {
                            var answer = _context.QuestionAnies.Where(w => w.ID == tanswer.QuestionAnsID).FirstOrDefault();
                            if (answer != null)
                            {
                                tresultstudent.Point += answer.Point;
                                tanswer.Point = answer.Point;
                                if (answer.Point > 0)
                                    tresultstudent.CorrectCnt++;
                            }
                        }
                        tanswer.ProveStatus = ProveStatus.Proved;
                    }
                    else if (question.QuestionType == QuestionType.TrueFalse)
                    {
                        if (tanswer.TFAns.HasValue)
                        {
                            decimal tfpoint = 0;
                            if (tanswer.TFAns.Value == true)
                                tfpoint = question.TPoint.HasValue ? question.TPoint.Value : 0;
                            else if (tanswer.TFAns.Value == false)
                                tfpoint = question.FPoint.HasValue ? question.FPoint.Value : 0;

                            tresultstudent.Point += tfpoint;
                            tanswer.Point = tfpoint;
                            if (tfpoint > 0)
                                tresultstudent.CorrectCnt++;
                        }
                        tanswer.ProveStatus = ProveStatus.Proved;
                    }
                    else if (question.QuestionType == QuestionType.MultipleMatching | question.QuestionType == QuestionType.MultipleMatchingSub)
                    {
                        var answer = _context.QuestionAnies.Where(w => w.ID == tanswer.QuestionAnsID).FirstOrDefault();
                        if (answer != null)
                        {
                            if (question.Choice == answer.Choice)
                            {
                                tresultstudent.CorrectCnt++;
                                if (question.Point.HasValue)
                                {
                                    tresultstudent.Point += question.Point.Value;
                                    tanswer.Point = question.Point.Value;
                                }
                            }
                        }
                        tanswer.ProveStatus = ProveStatus.Proved;
                    }
                    else if (question.QuestionType == QuestionType.ShortAnswer | question.QuestionType == QuestionType.Essay | question.QuestionType == QuestionType.Assignment)
                    {
                        if (tanswer.Point.HasValue)
                        {
                            tresultstudent.Point += tanswer.Point.Value;
                            if (tanswer.Point.Value > 0)
                                tresultstudent.CorrectCnt++;
                            tanswer.ProveStatus = ProveStatus.Proved;
                        }
                    }
                    else if (question.QuestionType == QuestionType.Attitude)
                    {
                        if (tanswer.QuestionAnsAttitudeID.HasValue)
                        {
                            var attsetup = _context.AttitudeSetups.Where(w => w.AttitudeAnsType == question.AttitudeAnsType & w.AttitudeAnsSubType == question.AttitudeAnsSubType).FirstOrDefault();
                            if (attsetup != null)
                            {
                                if (tanswer.QuestionAnsAttitudeID.HasValue)
                                {
                                    decimal attpoint = 0;
                                    if (tanswer.QuestionAnsAttitudeID == 1)
                                        attpoint = attsetup.Point1.HasValue ? attsetup.Point1.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 2)
                                        attpoint = attsetup.Point2.HasValue ? attsetup.Point2.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 3)
                                        attpoint = attsetup.Point3.HasValue ? attsetup.Point3.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 4)
                                        attpoint = attsetup.Point4.HasValue ? attsetup.Point4.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 5)
                                        attpoint = attsetup.Point5.HasValue ? attsetup.Point5.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 6)
                                        attpoint = attsetup.Point6.HasValue ? attsetup.Point6.Value : 0;
                                    else if (tanswer.QuestionAnsAttitudeID == 7)
                                        attpoint = attsetup.Point7.HasValue ? attsetup.Point7.Value : 0;

                                    tresultstudent.Point += attpoint;
                                    tanswer.Point = attpoint;
                                    if (attpoint > 0)
                                        tresultstudent.CorrectCnt++;
                                }
                            }
                        }
                        tanswer.ProveStatus = ProveStatus.Proved;
                    }
                }
            }
            _context.SaveChanges();

            /*calculate point and prove per student test*/
            var unprove = _context.TestResultStudentQAnies.Where(w => w.ProveStatus == ProveStatus.Pending & w.TestResultStudentID == tresultstudentid).Count();
            if (unprove > 0)
                tresultstudent.ProveStatus = ProveStatus.Pending;
            else
                tresultstudent.ProveStatus = ProveStatus.Proved;

            tresultstudent.WrongCnt = tresultstudent.QuestionCnt - tresultstudent.CorrectCnt;
            _context.SaveChanges();

            /*calculate point and prove per exam*/
            var students = _context.TestResultStudents.Where(w => w.TestResultID == tresultstudent.TestResultID);
            var provecnt = students.Where(w => w.ProveStatus == ProveStatus.Proved).Count();
            var studentcnt = students.Count();
            var tresult = _context.TestResults.Where(w => w.ID == tresultstudent.TestResultID).FirstOrDefault();
            if (tresult != null)
            {
                tresult.ProvedCnt = provecnt;
                if (studentcnt == provecnt)
                {
                    tresult.ProveStatus = ProveStatus.Proved;
                    tresult.ProvedCnt = provecnt;
                    tresult.Update_By = update_by;
                    tresult.Update_On = DateUtil.Now();
                }
            }
            _context.SaveChanges();
            return tresultstudent.Point;
        }

        [HttpGet]
        [Route("listrepair")]
        public object listrepair(int? id)
        {
            var curdate = DateUtil.Now();
            var teststudent = _context.TestResultStudents
                .Include(i => i.Exam)
                .Where(w =>
                (w.ExamingStatus == ExamingStatus.None & w.Exam.ExamDate.Value.Date < curdate.Date) ||
                (w.ExamingStatus == ExamingStatus.Examing & w.Expriry_On.Value <= curdate));

            if (id.HasValue)
                teststudent = teststudent.Where(w => w.ExamID == id);
            return teststudent.Select(s => new
            {
                id = s.ID,
                email = s.Student.Email,
                address = s.Student.Address,
                phone = s.Student.Phone,
            }).ToArray();
        }
    }
}
