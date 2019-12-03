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
    public class QuestionController : ControllerBase
    {

        private readonly ILogger<QuestionController> _logger;
        public TuExamContext _context;

        public QuestionController(ILogger<QuestionController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listquestion")]
        public object listAllquestion(string text_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search, string approve_search, string test_filter, int pageno = 1)
        {
            var questions = _context.Questions.Include(i => i.SubjectSub).Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(text_search))
                questions = questions.Where(w => w.QuestionCode.Contains(text_search)
                | w.QuestionTh.Contains(text_search)
                | w.QuestionEn.Contains(text_search)
                | w.Remark.Contains(text_search)
                );

            if (!string.IsNullOrEmpty(status_search))
                questions = questions.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(sub_search))
            {
                var subID = NumUtil.ParseInteger(sub_search);
                if (subID > 0)
                    questions = questions.Where(w => w.SubjectSubID == subID);
            }
            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    questions = questions.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    questions = questions.Where(w => w.Subject.SubjectGroupID == groupID);
            }
            if (!string.IsNullOrEmpty(level_search))
                questions = questions.Where(w => w.QuestionLevel == level_search.toLevel());

            if (!string.IsNullOrEmpty(course_search))
            {
                var courseID = NumUtil.ParseInteger(course_search);
                if (courseID.ToString().toCourse() == Course.Thai)
                    questions = questions.Where(w => w.CourseTh == true);
                else if (courseID.ToString().toCourse() == Course.English)
                    questions = questions.Where(w => w.CourseEn == true);
            }

            if (!string.IsNullOrEmpty(approve_search))
                questions = questions.Where(w => w.ApprovalStatus == approve_search.toApprovalStatus());

            if (!string.IsNullOrEmpty(test_filter))
            {
                var testID = NumUtil.ParseInteger(test_filter);
                var test = _context.Tests.Where(w => w.ID == testID).FirstOrDefault();
                if(test != null && test.TestQuestionType == TestQuestionType.Custom)
                {
                    var qcustoms = _context.TestQCustoms.Where(w => w.TestID == testID).Select(s=>s.QuestionID);
                    if(qcustoms.Count() > 0)
                    {
                        questions = questions.Where(w => !qcustoms.Contains(w.ID));
                    }
                }
            }
            int skipRows = (pageno - 1) * 25;
            var itemcnt = questions.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAllquestion), new
            {
                data = questions.Select(s => new
                {
                    id = s.ID,
                    questioncode = s.QuestionCode,
                    questiontype = s.QuestionType.toQuestionTypeMin2(),
                    questiontypeid = s.QuestionType,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Index,
                    subjectsub = s.SubjectSub.Name,
                    courseth = s.CourseTh,
                    courseen = s.CourseEn,
                    questionth = s.QuestionTh,
                    questionen = s.QuestionEn,
                    status = s.Status.toStatusName(),
                    approvalstatus = s.ApprovalStatus.toApprovalStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    questionlevel = s.QuestionLevel.toQuestionLevelName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.subjectsub).ThenBy(o4 => o4.id).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen
            }); ;

        }

        [HttpGet]
        [Route("listActivequestion")]
        public object listActivequestion()
        {
            var subjectsub = _context.Questions.Include(i => i.SubjectSub).Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => w.Status == StatusType.Active);
            if (subjectsub != null)
                return subjectsub.Select(s => new
                {
                    id = s.ID,
                    questioncode = s.QuestionCode,
                    questiontype = s.QuestionType.toQuestionType(),
                    questiontypeid = s.QuestionType,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Index,
                    subjectsub = s.SubjectSub.Name,
                    courseth = s.CourseTh,
                    courseen = s.CourseEn,
                    questionth = s.QuestionTh,
                    questionen = s.QuestionEn,
                    status = s.Status.toStatusName(),
                    approvalstatus = s.ApprovalStatus.toApprovalStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.subjectsub).ToArray();
            return CreatedAtAction(nameof(listActivequestion), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getquestion")]
        public object getquestion(int? id)
        {
            var sub = _context.Questions.Include(i => i.Subject).Where(w => w.ID == id).Select(s => new
            {
                id = s.ID,
                questioncode = s.QuestionCode,
                questiontype = s.QuestionType,
                groupid = s.SubjectGroupID,
                subjectid = s.SubjectID,
                subid = s.SubjectSubID,
                courseth = s.CourseTh,
                courseen = s.CourseEn,
                questionth = s.QuestionTh,
                questionen = s.QuestionEn,
                keyword = s.Keyword,
                questionlevel = s.QuestionLevel,
                timelimit = s.TimeLimit,
                timelimittype = s.TimeLimitType,
                status = s.Status,
                approvalstatus = s.ApprovalStatus,
                remark = s.Remark,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (sub != null)
                return sub;
            return CreatedAtAction(nameof(getquestion), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Question>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            model.Create_On = DateUtil.Now();
            model.Update_On = DateUtil.Now();
            model.Create_By = model.Update_By;
            model.Update_By = model.Update_By;

            _context.Questions.Add(model);
            _context.SaveChanges();
            var qcode = "MC" + model.ID.ToString("00000000");
            model.QuestionCode = qcode;
            _context.SaveChanges();

            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success, id = model.ID });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Question>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var question = _context.Questions.Where(w => w.ID == model.ID).FirstOrDefault();
            if (question != null)
            {
                model.Update_On = DateUtil.Now();
                model.QuestionCode = "MC" + question.ID.ToString("00000000");
                _context.Entry(question).CurrentValues.SetValues(model);
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

            var question = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (question == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var tresults = _context.TestResultStudentQAnies.Where(w => w.QuestionID == question.ID);
            if (tresults.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID);
            if (answers.Count() > 0)
                _context.QuestionAnies.RemoveRange(answers);

            var qcustoms = _context.TestQCustoms.Where(w => w.QuestionID == question.ID);
            if (qcustoms.Count() > 0)
                _context.TestQCustoms.RemoveRange(qcustoms);

            _context.Questions.Remove(question);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("getanswernext")]
        public object getanswernext(int? id)
        {
            var answercnt = _context.QuestionAnies.Where(w => w.QuestionID == id).Count();
            return answercnt + 1;
        }

        [HttpGet]
        [Route("listanswer")]
        public object listAllanswer(string id)
        {
            var answers = _context.QuestionAnies.Where(w => 1 == 1);

            var qID = NumUtil.ParseInteger(id);
            if (qID > 0)
                answers = answers.Where(w => w.QuestionID == qID);

            return answers.Select(s => new
            {
                id = s.ID,
                order = s.Order,
                answerth = s.AnswerTh,
                answeren = s.AnswerEn,
                description = s.Description,
                point = s.Point,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.order).ToArray()
           ;

        }
        [HttpGet]
        [Route("getanswer")]
        public object getanswer(int? id)
        {
            var sub = _context.QuestionAnies.Where(w => w.ID == id).Select(s => new
            {
                id = s.ID,
                order = s.Order,
                answesth = s.AnswerTh,
                answesen = s.AnswerEn,
                description = s.Description,
                point = s.Point,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (sub != null)
                return sub;
            return CreatedAtAction(nameof(getanswer), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("answerinsert")]
        public object answerinsert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<QuestionAns>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var answer = new QuestionAns();
            model.Create_On = DateUtil.Now();
            model.Update_On = DateUtil.Now();
            model.Create_By = model.Update_By;
            model.Update_By = model.Update_By;

            _context.QuestionAnies.Add(model);
            _context.SaveChanges();

            var i = 1;
            var answers = _context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID).OrderBy(w => w.Order);
            foreach (var ans in answers)
            {
                ans.Order = i;
                i++;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(answerinsert), new { result = ResultCode.Success, message = ResultMessage.Success, id = model.ID });
        }

        [HttpPost]
        [Route("answerupdate")]
        public object answerupdate([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<QuestionAns>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var answer = _context.QuestionAnies.Where(w => w.ID == model.ID).FirstOrDefault();
            if (answer != null)
            {
                model.Update_On = DateUtil.Now();
                _context.Entry(answer).CurrentValues.SetValues(model);
                _context.SaveChanges();
                var i = 1;
                var answers = _context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID).OrderBy(w => w.Order);
                foreach (var ans in answers)
                {
                    ans.Order = i;
                    i++;
                }
                _context.SaveChanges();
                return CreatedAtAction(nameof(answerupdate), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(answerupdate), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpGet]
        [Route("answerdelete")]
        public object answerdelete(int? id)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var answer = _context.QuestionAnies.Where(w => w.ID == id).FirstOrDefault();
            if (answer == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var qid = answer.QuestionID;
            _context.QuestionAnies.Remove(answer);
            _context.SaveChanges();
            var i = 1;
            var answers = _context.QuestionAnies.Where(w => w.QuestionID == qid).OrderBy(w => w.Order);
            foreach (var ans in answers)
            {
                ans.Order = i;
                i++;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(answerdelete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

    }
}
