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
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;
        public TuExamContext _context;

        public TestController(ILogger<TestController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listtest")]
        public object listAlltest(string text_search, string status_search, string group_search, string subject_search, int pageno = 1)
        {
            var test = _context.Tests.Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(status_search))
                test = test.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    test = test.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    test = test.Where(w => w.Subject.SubjectGroupID == groupID);
            }

            var tests = new List<Test>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        tests.AddRange(test.Where(w => w.Name.Contains(text)));
                    }
                }
                tests = tests.Distinct().ToList();
            }
            else
            {
                tests = test.ToList();
            }

            int skipRows = (pageno - 1) * 25;
            var itemcnt = tests.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAlltest), new
            {
                data = tests.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    status = s.Status.toStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    testcode = s.TestCode,
                    approvalstatus = s.ApprovalStatus,
                    approvalstatusname = s.ApprovalStatus.toTestApprovalStatusName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.name).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;

        }

        [HttpGet]
        [Route("listActivetest")]
        public object listActivetest(string group_search, string subject_search)
        {
            var test = _context.Tests.Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => w.Status == StatusType.Active & w.ApprovalStatus == TestApprovalType.Approved);
            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    test = test.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    test = test.Where(w => w.Subject.SubjectGroupID == groupID);
            }
            if (test != null)
                return test.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    status = s.Status.toStatusName(),
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    group = s.Subject.SubjectGroup.Name,
                    testcode = s.TestCode,
                    approvalstatus = s.ApprovalStatus,
                    approvalstatusname = s.ApprovalStatus.toTestApprovalStatusName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.name).ToArray();
            return CreatedAtAction(nameof(listActivetest), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }


        [HttpGet]
        [Route("gettest")]
        public object gettest(int? id)
        {
            var test = _context.Tests.Include(i => i.SubjectGroup).Include(i => i.Subject).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                name = s.Name,
                status = s.Status,
                statusname = s.Status.toStatusName(),
                subjectid = s.SubjectID,
                subject = s.Subject.Name,
                groupid = s.Subject.SubjectGroupID,
                group = s.SubjectGroup.Name,
                testcode = s.TestCode,
                description = s.Description,
                timelimit = s.TimeLimit,
                timelimittype = s.TimeLimitType,
                timelimittypename = s.TimeLimitType.toTimeType(),
                testdoexamtype = s.TestDoExamType,
                testdoexamtypename = s.TestDoExamType.toTestDoExamType(),
                course = s.Course,
                coursename = s.Course.toCourseName(),
                showresult = s.ShowResult,
                showresultname = s.ShowResult.toShowResult(),
                approvalstatus = s.ApprovalStatus,
                approvalstatusname = s.ApprovalStatus.toTestApprovalStatusName(),
                testquestiontype = s.TestQuestionType,
                testquestiontypename = s.TestQuestionType.toTestQuestionType(),
                testcustomordertype = s.TestCustomOrderType,
                testcustomordertypename = s.TestCustomOrderType.toTestCustomOrderType(),
                remark =s.Remark,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (test != null)
                return test;
            return CreatedAtAction(nameof(gettest), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Test>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            model.Create_On = DateUtil.Now();
            model.Update_On = DateUtil.Now();
            model.Create_By = model.Update_By;
            model.Update_By = model.Update_By;

            _context.Tests.Add(model);
            _context.SaveChanges();

            var code = "T" + model.ID.ToString("00000000");
            model.TestCode = code;
            _context.SaveChanges();

            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success, id = model.ID, groupid = model.SubjectGroupID, subjectid = model.SubjectID });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Test>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var test = _context.Tests.Where(w => w.Name == model.Name & w.SubjectID == model.SubjectID & w.ID != model.ID).FirstOrDefault();
            if (test != null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            test = _context.Tests.Where(w => w.ID == model.ID).FirstOrDefault();
            if (test != null)
            {
                model.Update_On = DateUtil.Now();
                model.TestCode = "T" + test.ID.ToString("00000000");
                _context.Entry(test).CurrentValues.SetValues(model);
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

            var test = _context.Tests.Where(w => w.ID == id).FirstOrDefault();
            if (test == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var qcustoms = _context.TestQCustoms.Where(w => w.TestID == id);
            if (qcustoms.Count() > 0)
                _context.TestQCustoms.RemoveRange(qcustoms);

            var qrandoms = _context.TestQRandoms.Where(w => w.TestID == id);
            if (qrandoms.Count() > 0)
                _context.TestQRandoms.RemoveRange(qrandoms);

            var exams = _context.Exams.Where(w => w.TestID == id);
            if (exams.Count() > 0)
            {
                foreach (var exam in exams)
                {
                    var registers = _context.ExamRegisters.Where(w => w.ExamID == exam.ID);
                    if (registers.Count() > 0)
                        _context.ExamRegisters.RemoveRange(registers);

                    var tresults = _context.TestResults.Where(w => w.ExamID == exam.ID);
                    if (tresults.Count() > 0)
                        return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

                    //foreach (var tresult in tresults)
                    //{
                    //    var tstudents = _context.TestResultStudents.Where(w => w.TestResultID == tresult.ID);
                    //    foreach (var tstudent in tstudents)
                    //    {
                    //        var tqans = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID);
                    //        if (tqans.Count() > 0)
                    //            _context.TestResultStudentQAnies.RemoveRange(tqans);
                    //    }
                    //    if (tstudents.Count() > 0)
                    //        _context.TestResultStudents.RemoveRange(tstudents);
                    //}
                    //if (tresults.Count() > 0)
                    //    _context.TestResults.RemoveRange(tresults);
                }
                _context.Exams.RemoveRange(exams);
            }

            var tstudents2 = _context.TestResultStudents.Where(w => w.TestID == id);
            if (tstudents2.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });
            //foreach (var tstudent in tstudents2)
            //{
            //    var tqans = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstudent.ID);
            //    if (tqans.Count() > 0)
            //        _context.TestResultStudentQAnies.RemoveRange(tqans);
            //}
            //if (tstudents2.Count() > 0) { }
            //    _context.TestResultStudents.RemoveRange(tstudents2);

            var approvals = _context.TestApprovals.Where(w => w.TestID == id);
            foreach (var appr in approvals)
            {
                var apprstaffs = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == appr.ID);
                if (apprstaffs.Count() > 0)
                    _context.TestApprovalStaffs.RemoveRange(apprstaffs);

                _context.TestApprovals.Remove(appr);
            }

            _context.Tests.Remove(test);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("listqrandom")]
        public object listAllqrandom(string id)
        {
            var qrandom = _context.TestQRandoms.Include(i => i.SubjectSub).Where(w => 1 == 1);

            var tID = NumUtil.ParseInteger(id);
            if (tID > 0)
                qrandom = qrandom.Where(w => w.TestID == tID);

            return qrandom.Select(s => new
            {
                id = s.ID,
                questiontype = s.QuestionType.toQuestionTypeMin(),
                questiontypeid = s.QuestionType,
                testid = s.TestID,
                subid = s.SubjectSubID,
                sub = s.SubjectSub.Name,
                veryeasy = s.VeryEasy,
                easy = s.Easy,
                mid = s.Mid,
                hard = s.Hard,
                veryhard = s.VeryHard,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.questiontypeid).ThenBy(o => o.sub).ToArray()
           ;

        }

        [HttpGet]
        [Route("getqrandom")]
        public object getqrandom(int? id)
        {
            var qrandom = _context.TestQRandoms.Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                questiontype = s.QuestionType,
                testid = s.TestID,
                subid = s.SubjectSubID,
                veryeasy = s.VeryEasy,
                easy = s.Easy,
                mid = s.Mid,
                hard = s.Hard,
                veryhard = s.VeryHard,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (qrandom != null)
                return qrandom;
            return CreatedAtAction(nameof(getqrandom), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("qrandominsert")]
        public object qrandominsert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<TestQRandom>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            model.Create_On = DateUtil.Now();
            model.Update_On = DateUtil.Now();
            model.Create_By = model.Update_By;
            model.Update_By = model.Update_By;

            _context.TestQRandoms.Add(model);
            _context.SaveChanges();

            int cnt = 0;
            var qrandoms = _context.TestQRandoms.Where(w => w.TestID == model.TestID);
            foreach (var ran in qrandoms)
            {
                cnt += ran.VeryEasy.HasValue ? ran.VeryEasy.Value : 0;
                cnt += ran.Easy.HasValue ? ran.Easy.Value : 0;
                cnt += ran.Mid.HasValue ? ran.Mid.Value : 0;
                cnt += ran.Hard.HasValue ? ran.Hard.Value : 0;
                cnt += ran.VeryHard.HasValue ? ran.VeryHard.Value : 0;
            }

            var test = _context.Tests.Where(w => w.ID == model.TestID).FirstOrDefault();
            if (test != null)
                test.QuestionCnt = cnt;
            return CreatedAtAction(nameof(qrandominsert), new { result = ResultCode.Success, message = ResultMessage.Success, id = model.ID });
        }

        [HttpPost]
        [Route("qrandomupdate")]
        public object qrandomupdate([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<TestQRandom>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var answer = _context.TestQRandoms.Where(w => w.ID == model.ID).FirstOrDefault();
            if (answer != null)
            {
                model.Update_On = DateUtil.Now();
                _context.Entry(answer).CurrentValues.SetValues(model);
                _context.SaveChanges();

                int cnt = 0;
                var qrandoms = _context.TestQRandoms.Where(w => w.TestID == model.TestID);
                foreach (var ran in qrandoms)
                {
                    cnt += ran.VeryEasy.HasValue ? ran.VeryEasy.Value : 0;
                    cnt += ran.Easy.HasValue ? ran.Easy.Value : 0;
                    cnt += ran.Mid.HasValue ? ran.Mid.Value : 0;
                    cnt += ran.Hard.HasValue ? ran.Hard.Value : 0;
                    cnt += ran.VeryHard.HasValue ? ran.VeryHard.Value : 0;
                }

                var test = _context.Tests.Where(w => w.ID == model.TestID).FirstOrDefault();
                if (test != null)
                    test.QuestionCnt = cnt;

                return CreatedAtAction(nameof(qrandomupdate), new { result = ResultCode.Success, message = ResultMessage.Success });
            }


            return CreatedAtAction(nameof(qrandomupdate), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpGet]
        [Route("qrandomdelete")]
        public object qrandomdelete(int? id)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var qrandom = _context.TestQRandoms.Where(w => w.ID == id).FirstOrDefault();
            if (qrandom == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var tid = qrandom.TestID;
            _context.TestQRandoms.Remove(qrandom);
            _context.SaveChanges();

            int cnt = 0;
            var qrandoms = _context.TestQRandoms.Where(w => w.TestID == qrandom.TestID);
            foreach (var ran in qrandoms)
            {
                cnt += ran.VeryEasy.HasValue ? ran.VeryEasy.Value : 0;
                cnt += ran.Easy.HasValue ? ran.Easy.Value : 0;
                cnt += ran.Mid.HasValue ? ran.Mid.Value : 0;
                cnt += ran.Hard.HasValue ? ran.Hard.Value : 0;
                cnt += ran.VeryHard.HasValue ? ran.VeryHard.Value : 0;
            }

            var test = _context.Tests.Where(w => w.ID == qrandom.TestID).FirstOrDefault();
            if (test != null)
                test.QuestionCnt = cnt;
            return CreatedAtAction(nameof(qrandomdelete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("listqcustom")]
        public object listAllqcustom(string id)
        {
            var qcustom = _context.TestQCustoms.Include(i => i.Question).Where(w => 1 == 1);

            var tID = NumUtil.ParseInteger(id);
            if (tID > 0)
                qcustom = qcustom.Where(w => w.TestID == tID);

            var questioncnt = qcustom.Where(w => w.Question.QuestionType != QuestionType.ReadingText && w.Question.QuestionType != QuestionType.MultipleMatching).Count();          
            questioncnt += _context.Questions.Where(w => w.QuestionParentID.HasValue && qcustom.Select(s => s.Question.ID).Contains(w.QuestionParentID.Value)).Count();
            return CreatedAtAction(nameof(listAlltest), new
            {
                data = qcustom.Select(s => new
                {
                    id = s.ID,
                    testid = s.TestID,
                    order = s.Order,
                    questionid = s.QuestionID,
                    questionth = s.Question.QuestionTh,
                    questionen = s.Question.QuestionEn,
                    questionlevel = s.Question.QuestionLevel.toQuestionLevelName(),
                    questiontype = s.Question.QuestionType.toQuestionTypeMin2(),
                    childcnt = _context.Questions.Where(w => w.QuestionParentID == s.QuestionID).Count(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.order).ToArray(),              
                questioncnt = questioncnt,
            }); 
        }


        [HttpGet]
        [Route("chooseqcustom")]
        public object chooseqcustom(string choose, string tid, string update_by)
        {
            if (choose == null)
                return CreatedAtAction(nameof(chooseqcustom), new { result = ResultCode.Success, message = ResultMessage.Success });

            var chs = choose.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var qid = NumUtil.ParseInteger(ch);
                    var tqcustom = new TestQCustom();
                    tqcustom.QuestionID = qid;
                    tqcustom.TestID = NumUtil.ParseInteger(tid);
                    tqcustom.Create_On = DateUtil.Now();
                    tqcustom.Update_On = DateUtil.Now();
                    tqcustom.Update_By = update_by;
                    tqcustom.Create_By = update_by;
                    _context.TestQCustoms.Add(tqcustom);
                }
            }
            _context.SaveChanges();
            var testID = NumUtil.ParseInteger(tid);
            var i = 1;
            var questions = _context.TestQCustoms.Where(w => w.TestID == testID).OrderBy(w => w.QuestionID);
            foreach (var q in questions)
            {
                q.Update_On = DateUtil.Now();
                q.Update_By = update_by;
                q.Order = i;
                i++;
            }
            var test = _context.Tests.Where(w => w.ID == testID).FirstOrDefault();
            if (test != null)
            {
                test.Update_On = DateUtil.Now();
                test.Update_By = update_by;
                test.QuestionCnt = i - 1;
            }

            _context.SaveChanges();
            return CreatedAtAction(nameof(chooseqcustom), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("qcustomdelete")]
        public object qcustomdelete(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(qcustomdelete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var qcustom = _context.TestQCustoms.Where(w => w.ID == id).FirstOrDefault();
            if (qcustom == null)
                return CreatedAtAction(nameof(qcustomdelete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var testID = qcustom.TestID;

            _context.TestQCustoms.Remove(qcustom);
            _context.SaveChanges();
            var i = 1;
            var questions = _context.TestQCustoms.Where(w => w.TestID == testID).OrderBy(w => w.ID);
            foreach (var q in questions)
            {
                q.Update_On = DateUtil.Now();
                q.Update_By = update_by;
                q.Order = i;
                i++;
            }
            var test = _context.Tests.Where(w => w.ID == testID).FirstOrDefault();
            if (test != null)
            {
                test.Update_On = DateUtil.Now();
                test.Update_By = update_by;
                test.QuestionCnt = i - 1;
            }

            _context.SaveChanges();
            return CreatedAtAction(nameof(qcustomdelete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("qcustommoveup")]
        public object qcustommoveup(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(qcustommoveup), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestQCustoms.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(qcustommoveup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var test = _context.Tests.Where(w => w.ID == model.TestID).FirstOrDefault();
            if (test != null)
            {
                test.Update_On = DateUtil.Now();
                test.Update_By = update_by;
            }
            var latestindex = this._context.TestQCustoms.Where(w => w.Order < model.Order & w.TestID == model.TestID).OrderByDescending(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.TestQCustoms.Where(w => w.TestID == model.TestID).OrderBy(o => o.Order))
            {
                item.Update_On = DateUtil.Now();
                item.Update_By = update_by;
                if (latestindex != null && latestindex.ID == item.ID)
                {
                    latestindex.Order = i + 1;
                }
                else if (latestindex != null && model.ID == item.ID)
                {
                    item.Order = i;
                    i += 2;
                }
                else
                {
                    item.Order = i;
                    i++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(qcustommoveup), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("qcustommovedown")]
        public object qcustommovedown(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(qcustommovedown), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestQCustoms.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(qcustommovedown), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var test = _context.Tests.Where(w => w.ID == model.TestID).FirstOrDefault();
            if (test != null)
            {
                test.Update_On = DateUtil.Now();
                test.Update_By = update_by;
            }

            var latestindex = this._context.TestQCustoms.Where(w => w.Order > model.Order & w.TestID == model.TestID).OrderBy(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.TestQCustoms.Where(w => w.TestID == model.TestID).OrderBy(o => o.Order))
            {
                item.Update_On = DateUtil.Now();
                item.Update_By = update_by;
                if (latestindex != null && latestindex.ID == item.ID)
                {
                    latestindex.Order = i;
                    i += 2;
                }
                else if (latestindex != null && model.ID == item.ID)
                {
                    item.Order = i + 1;
                }
                else
                {
                    item.Order = i;
                    i++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(qcustommovedown), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listtestapprove")]
        public object listtestapprove(string appr_search, int pageno = 1)
        {
            var appr = appr_search.toTestApprovalStatus();
            var tests = _context.Tests.Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => w.ApprovalStatus == appr);
            var approvals = _context.TestApprovals.Where(w => tests.Select(s => s.ID).Contains(w.TestID));

            int skipRows = (pageno - 1) * 10;
            var itemcnt = tests.Count();
            var pagelen = itemcnt / 10;
            if (itemcnt % 10 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listtestapprove), new
            {
                data = tests.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    status = s.Status.toStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    testcode = s.TestCode,
                    approvalstatus = s.ApprovalStatus,
                    approvalstatusname = s.ApprovalStatus.toTestApprovalStatusName(),
                    apprid = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.TestID == s.ID).FirstOrDefault().ID : 0,
                    approvalcnt = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.TestID == s.ID).FirstOrDefault().ApprovalCnt : 0,
                    approvedcnt = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.TestID == s.ID).FirstOrDefault().ApprovedCnt : 0,
                    rejectedcnt = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.TestID == s.ID).FirstOrDefault().RejectedCnt : 0,
                    startfrom = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? DateUtil.ToDisplayDate(approvals.Where(w => w.TestID == s.ID).FirstOrDefault().StartFrom) : null,
                    endfrom = approvals.Where(w => w.TestID == s.ID).FirstOrDefault() != null ? DateUtil.ToDisplayDate(approvals.Where(w => w.TestID == s.ID).FirstOrDefault().EndFrom) : null,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.name).Skip(skipRows).Take(10).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;

        }

        [HttpGet]
        [Route("listapprstaff")]
        public object listapprstaff(int? id)
        {
            var testappr = _context.TestApprovals.Where(w => w.TestID == id).OrderByDescending(o => o.ID).FirstOrDefault();
            var staffs = _context.TestApprovalStaffs.Include(i => i.Staff).Where(w => w.TestApprovalID == testappr.ID);

            if (staffs != null)
                return staffs.Select(s => new
                {
                    id = s.ID,
                    staffid = s.StaffID,
                    firstname = s.Staff.FirstName,
                    lastname = s.Staff.LastName,
                    remark = s.Remark,
                    approvalstatusname = s.TestApprovalType.toTestApprovalStatusName(),
                    approvalstatus = s.TestApprovalType,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).ToArray();
            return CreatedAtAction(nameof(listapprstaff), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }


        [HttpGet]
        [Route("approveconfirm")]
        public object approveconfirm(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Tests.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.ApprovalStatus = TestApprovalType.Pending;

            var staffs = _context.Staffs.Where(w => w.isTestAppr);
            var testappr = new TestApproval();
            testappr.TestID = model.ID;
            testappr.ApprovalCnt = staffs.Count();
            testappr.ApprovedCnt = 0;
            testappr.StartFrom = DateUtil.Now();
            testappr.EndFrom = DateUtil.Now().AddMonths(1);
            testappr.Create_On = DateUtil.Now();
            testappr.Update_On = DateUtil.Now();
            testappr.Create_By = update_by;
            testappr.Update_By = update_by;

            foreach (var staff in staffs)
            {
                var appstaff = new TestApprovalStaff();
                appstaff.StaffID = staff.ID;
                appstaff.TestApprovalType = TestApprovalType.Pending;
                appstaff.TestApproval = testappr;
                appstaff.Create_On = DateUtil.Now();
                appstaff.Update_On = DateUtil.Now();
                appstaff.Create_By = update_by;
                appstaff.Update_By = update_by;
                _context.TestApprovalStaffs.Add(appstaff);
            }
            _context.TestApprovals.Add(testappr);
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("approved")]
        public object approved(int? id, int? staffid, string update_by, string remark)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            var model = _context.TestApprovals.Include(i => i.Test).Where(w => w.TestID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var appstaff = _context.TestApprovalStaffs.Where(w => w.StaffID == staffid & w.TestApprovalID == model.ID).FirstOrDefault();
            if (appstaff == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            appstaff.TestApprovalType = TestApprovalType.Approved;
            appstaff.Remark = remark;
            appstaff.Update_On = DateUtil.Now();
            appstaff.Update_By = update_by;
            _context.SaveChanges();

            var approvedcnt = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == model.ID & w.TestApprovalType == TestApprovalType.Approved).Count();
            var rejectedcnt = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == model.ID & w.TestApprovalType == TestApprovalType.Reject).Count();
            model.ApprovedCnt = approvedcnt;
            model.RejectedCnt = rejectedcnt;
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            if (model.ApprovedCnt >= (decimal)model.ApprovalCnt / 2M)
            {
                model.Test.Update_On = DateUtil.Now();
                model.Test.Update_By = update_by;
                model.Test.ApprovalStatus = TestApprovalType.Approved;
                model.ApprovalStatus = TestApprovalType.Approved;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("rejected")]
        public object rejected(int? id, int? staffid, string update_by, string remark)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestApprovals.Include(i => i.Test).Where(w => w.TestID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var appstaff = _context.TestApprovalStaffs.Where(w => w.StaffID == staffid & w.TestApprovalID == model.ID).FirstOrDefault();
            if (appstaff == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            appstaff.TestApprovalType = TestApprovalType.Reject;
            appstaff.Remark = remark;
            appstaff.Update_On = DateUtil.Now();
            appstaff.Update_By = update_by;
            _context.SaveChanges();

            var approvedcnt = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == model.ID & w.TestApprovalType == TestApprovalType.Approved).Count();
            var rejectedcnt = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == model.ID & w.TestApprovalType == TestApprovalType.Reject).Count();
            model.ApprovedCnt = approvedcnt;
            model.RejectedCnt = rejectedcnt;
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            if (model.RejectedCnt >= (decimal)model.RejectedCnt / 2M)
            {
                model.Test.Update_On = DateUtil.Now();
                model.Test.Update_By = update_by;
                model.Test.ApprovalStatus = TestApprovalType.Reject;
                model.ApprovalStatus = TestApprovalType.Reject;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("approvedmaster")]
        public object approvedmaster(int? id, int? staffid, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestApprovals.Include(i => i.Test).Where(w => w.TestID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Test.Update_On = DateUtil.Now();
            model.Test.Update_By = update_by;
            model.Test.ApprovalStatus = TestApprovalType.Approved;
            model.Test.Remark = "อนุมัติโดยผู้คัดเลือกพิเศษ";
            model.ApprovalStatus = TestApprovalType.Approved;

            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("rejectedmaster")]
        public object rejectedmaster(int? id, int? staffid, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestApprovals.Include(i => i.Test).Where(w => w.TestID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Test.Update_On = DateUtil.Now();
            model.Test.Update_By = update_by;
            model.Test.ApprovalStatus = TestApprovalType.Reject;
            model.Test.Remark = "ไม่อนุมัติโดยผู้คัดเลือกพิเศษ";
            model.ApprovalStatus = TestApprovalType.Reject;
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("draftmaster")]
        public object draftmaster(int? id, int? staffid, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.TestApprovals.Include(i => i.Test).Where(w => w.TestID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Test.Update_On = DateUtil.Now();
            model.Test.Update_By = update_by;
            model.Test.ApprovalStatus = TestApprovalType.Draft;
            model.Test.Remark = "";

            var appstaff = _context.TestApprovalStaffs.Where(w => w.TestApprovalID == model.ID);
            if (appstaff.Count() > 0)
                _context.TestApprovalStaffs.RemoveRange(appstaff);
            _context.TestApprovals.Remove(model);

            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("changestatus")]
        public object changestatus(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(changestatus), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Tests.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(changestatus), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            if (model.Status == StatusType.Active)
                model.Status = StatusType.InActive;
            else if (model.Status == StatusType.InActive)
                model.Status = StatusType.Active;

            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            _context.SaveChanges();
            return CreatedAtAction(nameof(changestatus), new { result = ResultCode.Success, message = ResultMessage.Success, status = model.Status, statusname = model.Status.toStatusName() });
        }


    }
}
