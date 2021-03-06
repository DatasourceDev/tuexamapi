﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.DTO;
using tuexamapi.Models;
using tuexamapi.Util;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System.Net.Mail;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        public SystemConf _conf;

        private readonly ILogger<QuestionController> _logger;
        public TuExamContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public QuestionController(ILogger<QuestionController> logger, TuExamContext context, IHostingEnvironment hostingEnvironment, IOptions<SystemConf> conf)
        {
            this._logger = logger;
            this._context = context;
            this._hostingEnvironment = hostingEnvironment;
            this._conf = conf.Value;

        }

        [HttpGet]
        [Route("listquestion")]
        public object listAllquestion(string text_search, string from_search, string to_search, string qtype_search, string status_search, string group_search, string subject_search, string sub_search, string level_search, string course_search, string approve_search, string test_filter, int pageno = 1)
        {
            var question = _context.Questions
                .Include(i => i.SubjectSub)
                .Include(i => i.Subject)
                .Include(i => i.Subject.SubjectGroup)
                .Where(w => !w.QuestionParentID.HasValue);

            if (!string.IsNullOrEmpty(qtype_search))
                question = question.Where(w => w.QuestionType == qtype_search.toQuestionType());

            if (!string.IsNullOrEmpty(status_search))
                question = question.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(sub_search))
            {
                var subID = NumUtil.ParseInteger(sub_search);
                if (subID > 0)
                    question = question.Where(w => w.SubjectSubID == subID);
            }
            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    question = question.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    question = question.Where(w => w.Subject.SubjectGroupID == groupID);
            }
            if (!string.IsNullOrEmpty(level_search))
                question = question.Where(w => w.QuestionLevel == level_search.toLevel());

            if (!string.IsNullOrEmpty(course_search))
            {
                var courseID = NumUtil.ParseInteger(course_search);
                if (courseID.ToString().toCourse() == Course.Thai)
                    question = question.Where(w => w.CourseTh == true);
                else if (courseID.ToString().toCourse() == Course.English)
                    question = question.Where(w => w.CourseEn == true);
            }
            if (!string.IsNullOrEmpty(from_search))
            {
                var date = DateUtil.ToDate(from_search);
                question = question.Where(w => w.Create_On.Value.Date >= date);
            }
            if (!string.IsNullOrEmpty(to_search))
            {
                var date = DateUtil.ToDate(to_search);
                question = question.Where(w => w.Create_On.Value.Date <= date);
            }
            if (!string.IsNullOrEmpty(approve_search))
                question = question.Where(w => w.ApprovalStatus == approve_search.toApprovalStatus());

            if (!string.IsNullOrEmpty(test_filter))
            {
                var testID = NumUtil.ParseInteger(test_filter);
                var test = _context.Tests.Where(w => w.ID == testID).FirstOrDefault();
                if (test != null && test.TestQuestionType == TestQuestionType.Custom)
                {
                    var qcustoms = _context.TestQCustoms.Where(w => w.TestID == testID).Select(s => s.QuestionID);
                    if (qcustoms.Count() > 0)
                    {
                        question = question.Where(w => !qcustoms.Contains(w.ID));
                    }
                }
            }

            var questions = new List<Question>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        questions.AddRange(question.Where(w => w.QuestionCode.Contains(text)
                            | w.QuestionTh.Contains(text)
                            | w.QuestionEn.Contains(text)
                            | w.Remark.Contains(text)
                            | w.Keyword.Contains(text)
                            | w.QuestionCode.Contains(text)
                            ));
                    }
                }
                questions = questions.Distinct().ToList();
            }
            else
            {
                questions = question.ToList();
            }

            int skipRows = (pageno - 1) * 100;
            var itemcnt = questions.Count();
            var pagelen = itemcnt / 100;
            if (itemcnt % 100 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAllquestion), new
            {
                data = questions.Select(s => new
                {
                    id = s.ID,
                    questionno = s.No,
                    questioncode = s.QuestionCode,
                    questiontype = s.QuestionType.toQuestionTypeMin2(),
                    questiontypeid = s.QuestionType,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    subjectsub = s.SubjectSub.Name,
                    courseth = s.CourseTh,
                    courseen = s.CourseEn,
                    questionth = s.QuestionTh,
                    questionen = s.QuestionEn,
                    status = s.Status.toStatusName(),
                    approvalstatus = s.ApprovalStatus,
                    approvalstatusname = s.ApprovalStatus.toApprovalStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    questionlevel = s.QuestionLevel.toQuestionLevelName(),
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.id).Skip(skipRows).Take(100).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;

        }

        [HttpGet]
        [Route("getquestion")]
        public object getquestion(int? id)
        {
            var question = _context.Questions.Include(i => i.SubjectGroup).Include(i => i.SubjectSub).Include(i => i.Subject).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                questioncode = s.QuestionCode,
                questiontype = s.QuestionType,
                groupid = s.SubjectGroupID,
                group = s.SubjectGroup.Name,
                subjectid = s.SubjectID,
                subject = s.Subject.Name,
                subid = s.SubjectSubID,
                sub = s.SubjectSub.Name,
                courseth = s.CourseTh,
                courseen = s.CourseEn,
                questionth = s.QuestionTh,
                questionen = s.QuestionEn,
                keyword = s.Keyword,
                questionlevel = s.QuestionLevel,
                questionlevelname = s.QuestionLevel.toQuestionLevelName(),
                timelimit = s.TimeLimit,
                timelimittype = s.TimeLimitType,
                timelimittypename = s.TimeLimitType.toTimeType(),
                status = s.Status,
                statusname = s.Status.toStatusName(),
                approvalstatus = s.ApprovalStatus,
                approvalstatusname = s.ApprovalStatus.toQuestionApprovalStatusName(),
                remark = s.Remark,
                attitudeanstype = s.AttitudeAnsType,
                attitudeanstypename = s.AttitudeAnsType.HasValue ? s.AttitudeAnsType.Value.toAttitudeAnsTypeName() : "",
                attitudeanssubtype = s.AttitudeAnsSubType,
                attitudeanssubtypename = s.AttitudeAnsSubType.HasValue ? s.AttitudeAnsSubType.Value.toAttitudeAnsSubType() : "",
                fileurl = s.FileUrl,
                filetype = s.FileType,
                point = s.Point,
                point1 = s.Point1,
                point2 = s.Point2,
                point3 = s.Point3,
                point4 = s.Point4,
                point5 = s.Point5,
                point6 = s.Point6,
                point7 = s.Point7,
                tpoint = s.TPoint,
                fpoint = s.FPoint,
                order = s.ChildOrder,
                choice = s.Choice,
                answertype = s.AnswerType,
                anssub1 = s.AnswerSubjectSub1,
                anssub2 = s.AnswerSubjectSub2,
                anssub3 = s.AnswerSubjectSub3,
                anssub4 = s.AnswerSubjectSub4,
                anssub5 = s.AnswerSubjectSub5,
                anssub6 = s.AnswerSubjectSub6,
                anssub7 = s.AnswerSubjectSub7,

                anssubname1 = s.AnswerSubjectSub1.HasValue ? _context.SubjectSubs.Where(w=>w.ID == s.AnswerSubjectSub1).Select(s=>s.Name).FirstOrDefault() : "",
                anssubname2 = s.AnswerSubjectSub2.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub2).Select(s => s.Name).FirstOrDefault() : "",
                anssubname3 = s.AnswerSubjectSub3.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub3).Select(s => s.Name).FirstOrDefault() : "",
                anssubname4 = s.AnswerSubjectSub4.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub4).Select(s => s.Name).FirstOrDefault() : "",
                anssubname5 = s.AnswerSubjectSub5.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub5).Select(s => s.Name).FirstOrDefault() : "",
                anssubname6 = s.AnswerSubjectSub6.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub6).Select(s => s.Name).FirstOrDefault() : "",
                anssubname7 = s.AnswerSubjectSub7.HasValue ? _context.SubjectSubs.Where(w => w.ID == s.AnswerSubjectSub7).Select(s => s.Name).FirstOrDefault() : "",
                randomchoice = s.RandomChoice,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (question != null)
                return question;
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
            if (model.QuestionParentID == 0)
                model.QuestionParentID = null;

            _context.Questions.Add(model);
            _context.SaveChanges();
            var qcode = "";
            if (model.QuestionParentID.HasValue && model.QuestionParentID.Value > 0)
            {
                var question = _context.Questions.Where(w => w.ID == model.QuestionParentID).FirstOrDefault();
                if (question != null)
                {
                    if (question.QuestionType == QuestionType.ReadingText)
                    {
                        qcode = "R" + model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
                        var childCnt = _context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder).Count();
                        model.ChildOrder = childCnt;
                    }
                    else
                    {
                        qcode = model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
                        var childCnt = _context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder).Count();
                        model.ChildOrder = childCnt;
                    }
                }

            }
            else
                qcode = model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
            model.QuestionCode = qcode;
            _context.SaveChanges();

            if (model.QuestionParentID.HasValue && model.QuestionParentID.Value > 0)
            {
                var i = 1;
                var questions = _context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder).ThenBy(o => o.ID);
                foreach (var q in questions)
                {
                    q.ChildOrder = i;
                    q.Update_On = DateUtil.Now();
                    q.Update_By = model.Update_By;
                    i++;
                }
                _context.SaveChanges();
            }
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
                model.Create_By = question.Create_By;
                model.Create_On = question.Create_On;
                model.Update_On = DateUtil.Now();
                var qcode = "";
                if (model.QuestionParentID.HasValue && model.QuestionParentID.Value > 0)
                {
                    if (question.QuestionType == QuestionType.ReadingText)
                        qcode = "R" + model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
                    else
                        qcode = model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
                }
                else
                    qcode = model.QuestionType.toQuestionTypeMin2() + model.ID.ToString("00000000");
                model.QuestionCode = qcode;

                if (model.QuestionType == QuestionType.ReadingText | model.QuestionType == QuestionType.MultipleMatching)
                {
                    var questions = _context.Questions.Where(w => w.QuestionParentID == model.ID);
                    foreach (var q in questions)
                    {
                        q.SubjectGroupID = model.SubjectGroupID;
                        q.SubjectID = model.SubjectID;
                        q.SubjectSubID = model.SubjectSubID;
                        q.CourseTh = model.CourseTh;
                        q.CourseEn = model.CourseEn;
                        q.Keyword = model.Keyword;
                        q.QuestionLevel = model.QuestionLevel;
                        q.TimeLimit = model.TimeLimit;
                        q.TimeLimitType = model.TimeLimitType;
                        q.ApprovalStatus = model.ApprovalStatus;
                        q.Update_On = DateUtil.Now();
                        q.Update_By = model.Update_By;
                    }
                }
                if (model.QuestionParentID == 0)
                    model.QuestionParentID = null;

                _context.Entry(question).CurrentValues.SetValues(model);
                _context.SaveChanges();

                if (model.QuestionParentID.HasValue && model.QuestionParentID.Value > 0)
                {
                    var i = 1;
                    var questions = _context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder).ThenBy(o => o.ID);
                    foreach (var q in questions)
                    {
                        q.ChildOrder = i;
                        q.Update_On = DateUtil.Now();
                        q.Update_By = model.Update_By;
                        i++;
                    }
                    _context.SaveChanges();
                }

                return CreatedAtAction(nameof(update), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(update), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpGet]
        [Route("delete")]
        public object delete(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var question = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (question == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var tresults = _context.TestResultStudentQAnies.Where(w => w.QuestionID == question.ID);
            if (tresults.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            var children = _context.Questions.Where(w => w.QuestionParentID == question.ID);
            foreach (var child in children)
            {
                var ctresults = _context.TestResultStudentQAnies.Where(w => w.QuestionID == child.ID);
                if (ctresults.Count() > 0)
                    return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

                var canswers = _context.QuestionAnies.Where(w => w.QuestionID == child.ID);
                if (canswers.Count() > 0)
                    _context.QuestionAnies.RemoveRange(canswers);

                var cqcustoms = _context.TestQCustoms.Where(w => w.QuestionID == child.ID);
                if (cqcustoms.Count() > 0)
                    _context.TestQCustoms.RemoveRange(cqcustoms);

                _context.Questions.Remove(child);
            }

            var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID);
            if (answers.Count() > 0)
                _context.QuestionAnies.RemoveRange(answers);

            var qcustoms = _context.TestQCustoms.Where(w => w.QuestionID == question.ID);
            if (qcustoms.Count() > 0)
                _context.TestQCustoms.RemoveRange(qcustoms);
            _context.Questions.Remove(question);
            _context.SaveChanges();
            if (question.QuestionParentID.HasValue)
            {
                var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                if (parent != null)
                {
                    parent.Update_On = DateUtil.Now();
                    parent.Update_By = update_by;
                }
                var i = 1;
                var questions = _context.Questions.Where(w => w.QuestionParentID == question.QuestionParentID).OrderBy(o => o.ChildOrder).ThenBy(o => o.ID);
                foreach (var q in questions)
                {
                    q.ChildOrder = i;
                    q.Update_On = DateUtil.Now();
                    q.Update_By = update_by;
                    i++;
                }
                _context.SaveChanges();
            }
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("deleteall")]
        public object deleteall(string choose, string update_by)
        {
            var chs = choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var id = NumUtil.ParseInteger(ch);
                    var question = _context.Questions.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
                    if (question != null)
                    {
                        if (question.ApprovalStatus != QuestionApprovalType.Draft)
                            continue;

                        var tresults = _context.TestResultStudentQAnies.Where(w => w.QuestionID == question.ID);
                        if (tresults.Count() > 0)
                            return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

                        var children = _context.Questions.Where(w => w.QuestionParentID == question.ID);
                        foreach (var child in children)
                        {
                            var ctresults = _context.TestResultStudentQAnies.Where(w => w.QuestionID == child.ID);
                            if (ctresults.Count() > 0)
                                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

                            var canswers = _context.QuestionAnies.Where(w => w.QuestionID == child.ID);
                            if (canswers.Count() > 0)
                                _context.QuestionAnies.RemoveRange(canswers);

                            var cqcustoms = _context.TestQCustoms.Where(w => w.QuestionID == child.ID);
                            if (cqcustoms.Count() > 0)
                                _context.TestQCustoms.RemoveRange(cqcustoms);

                            _context.Questions.Remove(child);
                        }

                        var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID);
                        if (answers.Count() > 0)
                            _context.QuestionAnies.RemoveRange(answers);

                        var qcustoms = _context.TestQCustoms.Where(w => w.QuestionID == question.ID);
                        if (qcustoms.Count() > 0)
                            _context.TestQCustoms.RemoveRange(qcustoms);

                        var qapprs = _context.QuestionApprovals.Where(w => w.QuestionID == question.ID);
                        foreach (var qappr in qapprs)
                        {
                            var staffs = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == qappr.ID);
                            if (staffs.Count() > 0)
                                _context.QuestionApprovalStaffs.RemoveRange(staffs);

                            _context.QuestionApprovals.Remove(qappr);
                        }

                        _context.Questions.Remove(question);
                        _context.SaveChanges();
                        if (question.QuestionParentID.HasValue)
                        {
                            var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                            if (parent != null)
                            {
                                parent.Update_On = DateUtil.Now();
                                parent.Update_By = update_by;
                            }
                            var i = 1;
                            var questions = _context.Questions.Where(w => w.QuestionParentID == question.QuestionParentID).OrderBy(o => o.ChildOrder).ThenBy(o => o.ID);
                            foreach (var q in questions)
                            {
                                q.ChildOrder = i;
                                q.Update_On = DateUtil.Now();
                                q.Update_By = update_by;
                                i++;
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approvedmasterall), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("moveup")]
        public object moveup(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(moveup), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(moveup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var latestindex = this._context.Questions.Where(w => w.ChildOrder < model.ChildOrder & w.QuestionParentID == model.QuestionParentID).OrderByDescending(o => o.ChildOrder).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder))
            {
                item.Update_By = update_by;
                item.Update_On = DateUtil.Now();
                if (latestindex != null && latestindex.ID == item.ID)
                {
                    latestindex.ChildOrder = i + 1;
                }
                else if (latestindex != null && model.ID == item.ID)
                {
                    item.ChildOrder = i;
                    i += 2;
                }
                else
                {
                    item.ChildOrder = i;
                    i++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(moveup), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("movedown")]
        public object movedown(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(movedown), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(movedown), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });


            var latestindex = this._context.Questions.Where(w => w.ChildOrder > model.ChildOrder & w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.Questions.Where(w => w.QuestionParentID == model.QuestionParentID).OrderBy(o => o.ChildOrder))
            {
                item.Update_By = update_by;
                item.Update_On = DateUtil.Now();
                if (latestindex != null && latestindex.ID == item.ID)
                {
                    latestindex.ChildOrder = i;
                    i += 2;
                }
                else if (latestindex != null && model.ID == item.ID)
                {
                    item.ChildOrder = i + 1;
                }
                else
                {
                    item.ChildOrder = i;
                    i++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(movedown), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("fileupload")]
        [Obsolete]
        public object fileupload([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportQuestionDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var qid = NumUtil.ParseInteger(model.questionid);
                var question = _context.Questions.Where(w => w.ID == qid).FirstOrDefault();
                if (question != null)
                {
                    var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\questions\\" + question.ID + "\\";
                    //var webRootPath = _hostingEnvironment.WebRootPath + "\\questions\\" + question.ID + "\\" + model.fileupload.filename;
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
                    question.FileName = model.fileupload.filename;
                    question.FileUrl = fileurl;
                    question.FileType = model.fileupload.filetype;
                    question.Update_By = model.update_by;
                    question.Update_On = DateUtil.Now();
                    _context.SaveChanges();
                    return CreatedAtAction(nameof(fileupload), new
                    {
                        result = ResultCode.Success,
                        message = ResultMessage.Success,
                        filetype = question.FileType,
                        fileurl = question.FileUrl,
                        filename = question.FileName,
                    });
                }

            }
            return CreatedAtAction(nameof(fileupload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }

        [HttpGet]
        [Route("filedelete")]
        public object filedelete(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(filedelete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var question = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (question == null)
                return CreatedAtAction(nameof(filedelete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\questions\\" + question.ID + "\\";
            if (Directory.Exists(filePath))
            {
                string[] filePaths = Directory.GetFiles(filePath);
                foreach (var file in filePaths)
                {
                    System.IO.File.Delete(file);
                }
            }
            question.FileName = "";
            question.FileUrl = "";
            question.FileType = "";
            question.Update_By = update_by;
            question.Update_On = DateUtil.Now();
            _context.SaveChanges();
            return CreatedAtAction(nameof(filedelete), new { result = ResultCode.Success, message = ResultMessage.Success });
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
                choice = s.Choice,
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
            var answer = _context.QuestionAnies.Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                order = s.Order,
                choice = s.Choice,
                answesth = s.AnswerTh,
                answesen = s.AnswerEn,
                description = s.Description,
                point = s.Point,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (answer != null)
                return answer;
            return CreatedAtAction(nameof(getanswer), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("answerinsert")]
        public object answerinsert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<QuestionAns>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var dups = _context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID);
            if (!string.IsNullOrEmpty(model.Choice))
            {
                dups = dups.Where(w => w.Choice == model.Choice);
                if (dups.FirstOrDefault() != null)
                    return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });
            }

            var answer = new QuestionAns();
            model.Create_On = DateUtil.Now();
            model.Update_On = DateUtil.Now();
            model.Create_By = model.Update_By;

            var question = _context.Questions.Where(w => w.ID == model.QuestionID).FirstOrDefault();
            if (question != null)
            {
                question.Update_By = model.Update_By;
                question.Update_On = DateUtil.Now();
            }

            _context.QuestionAnies.Add(model);
            _context.SaveChanges();

            var i = 1;
            var answers = _context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID).OrderBy(w => w.Order);
            foreach (var ans in answers)
            {
                ans.Order = i;
                ans.Update_By = model.Update_By;
                ans.Update_On = DateUtil.Now();
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

            var dups = _context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID);
            if (!string.IsNullOrEmpty(model.Choice))
            {
                dups = dups.Where(w => w.Choice == model.Choice & w.ID != model.ID);
                if (dups.FirstOrDefault() != null)
                    return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });
            }

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
                    ans.Update_By = model.Update_By;
                    ans.Update_On = DateUtil.Now();
                    i++;
                }
                _context.SaveChanges();
                return CreatedAtAction(nameof(answerupdate), new { result = ResultCode.Success, message = ResultMessage.Success });
            }
            return CreatedAtAction(nameof(answerupdate), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpGet]
        [Route("answerdelete")]
        public object answerdelete(int? id, string update_by)
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
                ans.Update_By = update_by;
                ans.Update_On = DateUtil.Now();
                i++;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(answerdelete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("answermoveup")]
        public object answermoveup(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(answermoveup), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.QuestionAnies.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(answermoveup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var question = _context.Questions.Where(w => w.ID == model.QuestionID).FirstOrDefault();
            if (question != null)
            {
                question.Update_By = update_by;
                question.Update_On = DateUtil.Now();
                if (question.QuestionParentID.HasValue)
                {
                    var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.Update_By = update_by;
                        parent.Update_On = DateUtil.Now();
                    }
                }
            }

            var latestindex = this._context.QuestionAnies.Where(w => w.Order < model.Order & w.QuestionID == model.QuestionID).OrderByDescending(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID).OrderBy(o => o.Order))
            {
                item.Update_By = update_by;
                item.Update_On = DateUtil.Now();
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
            return CreatedAtAction(nameof(answermoveup), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("answermovedown")]
        public object answermovedown(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(answermovedown), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.QuestionAnies.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(answermovedown), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var question = _context.Questions.Where(w => w.ID == model.QuestionID).FirstOrDefault();
            if (question != null)
            {
                question.Update_By = update_by;
                question.Update_On = DateUtil.Now();
                if (question.QuestionParentID.HasValue)
                {
                    var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.Update_By = update_by;
                        parent.Update_On = DateUtil.Now();
                    }
                }
            }
            var latestindex = this._context.QuestionAnies.Where(w => w.Order > model.Order & w.QuestionID == model.QuestionID).OrderBy(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.QuestionAnies.Where(w => w.QuestionID == model.QuestionID).OrderBy(o => o.Order))
            {
                item.Update_By = update_by;
                item.Update_On = DateUtil.Now();
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
            return CreatedAtAction(nameof(answermovedown), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listchildquestion")]
        public object listchildquestion(string pid)
        {
            var qid = NumUtil.ParseInteger(pid);
            var questions = _context.Questions
                .Include(i => i.SubjectSub)
                .Include(i => i.Subject)
                .Include(i => i.Subject.SubjectGroup)
                .Where(w => w.QuestionParentID == qid)
                .OrderBy(o => o.ChildOrder);

            return questions.Select(s => new
            {
                id = s.ID,
                order = s.ChildOrder,
                questioncode = s.QuestionCode,
                questiontype = s.QuestionType.toQuestionTypeMin2(),
                questiontypeid = s.QuestionType,
                subject = s.Subject.Name,
                subjectindex = s.Subject.Order,
                subjectsub = s.SubjectSub.Name,
                courseth = s.CourseTh,
                courseen = s.CourseEn,
                questionth = s.QuestionTh,
                questionen = s.QuestionEn,
                status = s.Status.toStatusName(),
                approvalstatus = s.ApprovalStatus.toApprovalStatusName(),
                group = s.Subject.SubjectGroup.Name,
                questionlevel = s.QuestionLevel.toQuestionLevelName(),
                point = s.Point,
                choice = s.Choice,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.order).ToArray()
           ;
        }

        public class question_import
        {
            public int tempid { get; set; }
            public int id { get; set; }
            public string no { get; set; }
            public string subjectname { get; set; }
            public string subjectsub { get; set; }
            public string questiontype { get; set; }
            public string questionth { get; set; }
            public string attanstype { get; set; }
            public string subattanstype { get; set; }
            public string point1 { get; set; }
            public string point2 { get; set; }
            public string point3 { get; set; }
            public string point4 { get; set; }
            public string point5 { get; set; }
            public string point6 { get; set; }
            public string point7 { get; set; }

            public decimal? maxpoint { get; set; }
            public string tpoint { get; set; }
            public string fpoint { get; set; }
            public string choice { get; set; }

            public string answersub1 { get; set; }

            public string answersub2 { get; set; }

            public string answersub3 { get; set; }

            public string answersub4 { get; set; }

            public string answersub5 { get; set; }

            public string answersub6 { get; set; }

            public string answersub7 { get; set; }

            public AnswerType anstype { get; set; }

            public string update_by { get; set; }

            public List<answer_import> answers { get; set; }
            public int? parentid { get; set; }

        }
        public class answer_import
        {
            public string answerth { get; set; }
            public string point { get; set; }
            public string choice { get; set; }

        }
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public object upload([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportExamRegisterDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var file = Convert.FromBase64String(model.fileupload.value);
                if (model.fileupload.filename.Contains(".doc"))
                    return uploadword(file, model.update_by);
                else if (model.fileupload.filename.Contains(".xls"))
                    return uploadexcel(file, model.update_by);
                else
                    return uploadtext(file, model.update_by);
            }
            return CreatedAtAction(nameof(upload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }
        public object uploadtext(byte[] file, string update_by)
        {
            var questions = new List<question_import>();
            using (MemoryStream ms = new MemoryStream(file))
            {
                StreamReader reader = new StreamReader(ms);
                var alltext = reader.ReadToEnd();
                string[] lines = alltext.Split("\r\n");

                var id = 0;
                var first = true;
                var qstart = false;
                int? parentid = null;
                foreach (var line in lines)
                {
                    var question = new question_import();
                    var answer = new answer_import();
                    question.answers = new List<answer_import>();
                    question.update_by = update_by;
                    var colcnt = 0;

                    var full = line;
                    if (full.ToString().Contains("[MM]") | full.ToString().Contains("[MS]") | full.ToString().Contains("[RSA]") | full.ToString().Contains("[RAS]") | full.ToString().Contains("[RES]") | full.ToString().Contains("[RTF]") | full.ToString().Contains("[RAT]") | full.ToString().Contains("[RMC]") | full.ToString().Contains("[MC]") | full.ToString().Contains("[AT]") | full.ToString().Contains("[MM]") | full.ToString().Contains("[RT]") | full.ToString().Contains("[TF]") | full.ToString().Contains("[ES]") | full.ToString().Contains("[AS]"))
                        first = true;
                    if (full.ToString().Contains("[MC]") | full.ToString().Contains("[AT]") | full.ToString().Contains("[MM]") | full.ToString().Contains("[RT]") | full.ToString().Contains("[TF]") | full.ToString().Contains("[ES]") | full.ToString().Contains("[AS]"))
                        parentid = null;

                    var splits = full.ToString().Split("]", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var t in splits)
                    {
                        if (!string.IsNullOrEmpty(t))
                        {
                            var text = t;
                            if (t.Contains("["))
                                text += "]";

                            if (text.Contains("[") & text.Contains("]"))
                            {
                                if (first == true)
                                {
                                    qstart = true;
                                    if (colcnt == 0)
                                    {
                                        question.subjectname = text.Replace("[", "");
                                        question.subjectname = question.subjectname.Replace("]", "");
                                    }
                                    else if (colcnt == 1)
                                    {
                                        question.questiontype = text.Replace("[", "");
                                        question.questiontype = question.questiontype.Replace("]", "");
                                    }
                                    else if (colcnt == 2)
                                    {
                                        if (full.ToString().Contains("[MS]"))
                                        {
                                            question.choice = text.Replace("[", "");
                                            question.choice = question.choice.Replace("]", "");
                                        }
                                        else
                                        {
                                            question.attanstype = text.Replace("[", "");
                                            question.attanstype = question.attanstype.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 3)
                                    {
                                        question.subattanstype = text.Replace("[", "");
                                        question.subattanstype = question.subattanstype.Replace("]", "");
                                    }
                                    if (parentid.HasValue)
                                        question.parentid = parentid;
                                }
                                else
                                {
                                    qstart = false;
                                    if (colcnt == 0)
                                    {

                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            answer.point = text.Replace("[", "");
                                            answer.point = answer.point.Replace("]", "");
                                            questions[questions.Count - 1].point1 = text.Replace("[", "");
                                            questions[questions.Count - 1].point1 = questions[questions.Count - 1].point1.Replace("]", "");
                                        }
                                        else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.TrueFalse | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                                        {
                                            answer.point = text.Replace("[", "");
                                            answer.point = answer.point.Replace("]", "");
                                            questions[questions.Count - 1].tpoint = text.Replace("[", "");
                                            questions[questions.Count - 1].tpoint = questions[questions.Count - 1].tpoint.Replace("]", "");
                                        }
                                        else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                                        {
                                            answer.choice = text.Replace("[", "");
                                            answer.choice = answer.choice.Replace("]", "");
                                        }
                                        else
                                        {
                                            answer.point = text.Replace("[", "");
                                            answer.point = answer.point.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 1)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point2 = text.Replace("[", "");
                                            questions[questions.Count - 1].point2 = questions[questions.Count - 1].point2.Replace("]", "");
                                        }
                                        else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.TrueFalse | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                                        {
                                            questions[questions.Count - 1].fpoint = text.Replace("[", "");
                                            questions[questions.Count - 1].fpoint = questions[questions.Count - 1].fpoint.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 2)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point3 = text.Replace("[", "");
                                            questions[questions.Count - 1].point3 = questions[questions.Count - 1].point3.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 3)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point4 = text.Replace("[", "");
                                            questions[questions.Count - 1].point4 = questions[questions.Count - 1].point4.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 4)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point5 = text.Replace("[", "");
                                            questions[questions.Count - 1].point5 = questions[questions.Count - 1].point5.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 5)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point6 = text.Replace("[", "");
                                            questions[questions.Count - 1].point6 = questions[questions.Count - 1].point6.Replace("]", "");
                                        }
                                    }
                                    else if (colcnt == 6)
                                    {
                                        if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                        {
                                            questions[questions.Count - 1].point7 = text.Replace("[", "");
                                            questions[questions.Count - 1].point7 = questions[questions.Count - 1].point7.Replace("]", "");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (first == true)
                                    question.questionth += text.Trim();
                                else if (qstart == true)
                                {
                                    questions[questions.Count - 1].questionth += "<br/>";
                                    questions[questions.Count - 1].questionth += text.Trim();
                                }
                                else
                                {
                                    answer.answerth += text.Trim();
                                }
                            }
                            colcnt++;
                        }
                    }

                    if (first == true)
                    {
                        if (question.questiontype.toQuestionType() == QuestionType.ReadingText)
                            parentid = id;
                        question.tempid = id;
                        questions.Add(question);
                        question = new question_import();
                        question.answers = new List<answer_import>();
                        answer = new answer_import();
                        first = false;
                        id++;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(answer.answerth))
                        {
                            if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                            {
                                var q = questions.Where(w => w.tempid == parentid).FirstOrDefault();
                                if (q != null)
                                    q.answers.Add(answer);
                            }
                            else
                            {
                                questions[questions.Count - 1].answers.Add(answer);
                            }
                        }

                        answer = new answer_import();
                    }
                }
                return savequestion(questions);
            }
        }
        public object uploadword(byte[] file, string update_by)
        {
            var questions = new List<question_import>();
            using (MemoryStream ms = new MemoryStream(file))
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(ms, false))
                {
                    var id = 0;
                    var first = true;
                    var qstart = false;
                    int? parentid = null;
                    var parts = wordDocument.MainDocumentPart.Document.Descendants().FirstOrDefault();
                    if (parts != null)
                    {
                        foreach (var line in parts.ChildElements)
                        {
                            var question = new question_import();
                            var answer = new answer_import();
                            question.answers = new List<answer_import>();
                            question.update_by = update_by;
                            var colcnt = 0;
                            if (line is Paragraph)
                            {
                                var full = new StringBuilder();
                                foreach (var text in line.Descendants<Text>())
                                {
                                    full.Append(text.InnerText);
                                }

                                if (full.ToString().Contains("[MM]") | full.ToString().Contains("[MS]") | full.ToString().Contains("[RSA]") | full.ToString().Contains("[RAS]") | full.ToString().Contains("[RES]") | full.ToString().Contains("[RTF]") | full.ToString().Contains("[RAT]") | full.ToString().Contains("[RMC]") | full.ToString().Contains("[MC]") | full.ToString().Contains("[AT]") | full.ToString().Contains("[MM]") | full.ToString().Contains("[RT]") | full.ToString().Contains("[TF]") | full.ToString().Contains("[ES]") | full.ToString().Contains("[AS]"))
                                    first = true;
                                if (full.ToString().Contains("[MC]") | full.ToString().Contains("[AT]") | full.ToString().Contains("[MM]") | full.ToString().Contains("[RT]") | full.ToString().Contains("[TF]") | full.ToString().Contains("[ES]") | full.ToString().Contains("[AS]"))
                                    parentid = null;

                                var splits = full.ToString().Split("]", StringSplitOptions.RemoveEmptyEntries);

                                foreach (var t in splits)
                                {
                                    if (!string.IsNullOrEmpty(t))
                                    {
                                        var text = t;
                                        if (t.Contains("["))
                                            text += "]";

                                        if (text.Contains("[") & text.Contains("]"))
                                        {
                                            if (first == true)
                                            {
                                                qstart = true;
                                                if (colcnt == 0)
                                                {
                                                    question.subjectname = text.Replace("[", "");
                                                    question.subjectname = question.subjectname.Replace("]", "");
                                                }
                                                else if (colcnt == 1)
                                                {
                                                    question.questiontype = text.Replace("[", "");
                                                    question.questiontype = question.questiontype.Replace("]", "");
                                                }
                                                else if (colcnt == 2)
                                                {
                                                    if (full.ToString().Contains("[MS]"))
                                                    {
                                                        question.choice = text.Replace("[", "");
                                                        question.choice = question.choice.Replace("]", "");
                                                    }
                                                    else
                                                    {
                                                        question.attanstype = text.Replace("[", "");
                                                        question.attanstype = question.attanstype.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 3)
                                                {
                                                    question.subattanstype = text.Replace("[", "");
                                                    question.subattanstype = question.subattanstype.Replace("]", "");
                                                }
                                                if (parentid.HasValue)
                                                    question.parentid = parentid;
                                            }
                                            else
                                            {
                                                qstart = false;
                                                if (colcnt == 0)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        answer.point = text.Replace("[", "");
                                                        answer.point = answer.point.Replace("]", "");
                                                        questions[questions.Count - 1].point1 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point1 = questions[questions.Count - 1].point1.Replace("]", "");

                                                    }
                                                    else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.TrueFalse | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                                                    {
                                                        answer.point = text.Replace("[", "");
                                                        answer.point = answer.point.Replace("]", "");
                                                        questions[questions.Count - 1].tpoint = text.Replace("[", "");
                                                        questions[questions.Count - 1].tpoint = questions[questions.Count - 1].tpoint.Replace("]", "");
                                                    }
                                                    else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                                                    {
                                                        answer.choice = text.Replace("[", "");
                                                        answer.choice = answer.choice.Replace("]", "");
                                                    }
                                                    else
                                                    {
                                                        answer.point = text.Replace("[", "");
                                                        answer.point = answer.point.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 1)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point2 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point2 = questions[questions.Count - 1].point2.Replace("]", "");
                                                    }
                                                    else if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.TrueFalse | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                                                    {
                                                        questions[questions.Count - 1].fpoint = text.Replace("[", "");
                                                        questions[questions.Count - 1].fpoint = questions[questions.Count - 1].fpoint.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 2)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point3 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point3 = questions[questions.Count - 1].point3.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 3)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point4 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point4 = questions[questions.Count - 1].point4.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 4)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point5 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point5 = questions[questions.Count - 1].point5.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 5)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point6 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point6 = questions[questions.Count - 1].point6.Replace("]", "");
                                                    }
                                                }
                                                else if (colcnt == 6)
                                                {
                                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.Attitude | questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                                                    {
                                                        questions[questions.Count - 1].point7 = text.Replace("[", "");
                                                        questions[questions.Count - 1].point7 = questions[questions.Count - 1].point7.Replace("]", "");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (first == true)
                                                question.questionth += text.Trim();
                                            else if (qstart == true)
                                            {
                                                questions[questions.Count - 1].questionth += "<br/>";
                                                questions[questions.Count - 1].questionth += text.Trim();
                                            }
                                            else
                                            {
                                                answer.answerth += text.Trim();
                                            }
                                        }
                                        colcnt++;
                                    }
                                }
                            }
                            if (first == true)
                            {
                                if (question.questiontype.toQuestionType() == QuestionType.ReadingText | question.questiontype.toQuestionType() == QuestionType.MultipleMatching)
                                    parentid = id;
                                question.tempid = id;
                                questions.Add(question);
                                question = new question_import();
                                question.answers = new List<answer_import>();
                                answer = new answer_import();
                                first = false;
                                id++;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(answer.answerth))
                                {
                                    if (questions[questions.Count - 1].questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                                    {
                                        var q = questions.Where(w => w.tempid == parentid).FirstOrDefault();
                                        if (q != null)
                                            q.answers.Add(answer);
                                    }
                                    else
                                    {
                                        questions[questions.Count - 1].answers.Add(answer);
                                    }
                                }
                                answer = new answer_import();

                            }
                        }

                        return savequestion(questions);
                    }
                }
            }
            return CreatedAtAction(nameof(uploadword), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }
        public object uploadexcel(byte[] file, string update_by)
        {
            var questions = new List<question_import>();
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
                        int totalCols = worksheet.Dimension.End.Column - 2;

                        var id = 0;
                        int? parentid = null;
                        var question = new question_import();

                        for (int i = 2; i <= totalRows; i++)
                        {
                            var j = 1;
                            var subjectname = worksheet.Cells[i, j].Text; j++;
                            var questiontype = worksheet.Cells[i, j].Text; j++;
                            if (!string.IsNullOrEmpty(questiontype))
                            {
                                question = new question_import();
                                question.answers = new List<answer_import>();
                                question.update_by = update_by;
                                question.subjectname = subjectname;
                                question.questiontype = questiontype;
                                question.no = worksheet.Cells[i, j].Text; j++;
                                question.questionth = worksheet.Cells[i, j].Text; j++;
                                if (question.questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                                {
                                    question.choice = worksheet.Cells[i, j].Text; j++;
                                }
                                else
                                {
                                    question.attanstype = worksheet.Cells[i, j].Text; j++;
                                }
                                question.subattanstype = worksheet.Cells[i, j].Text; j++;
                                question.point1 = worksheet.Cells[i, j].Text; j++;
                                question.point2 = worksheet.Cells[i, j].Text; j++;
                                question.point3 = worksheet.Cells[i, j].Text; j++;
                                question.point4 = worksheet.Cells[i, j].Text; j++;
                                question.point5 = worksheet.Cells[i, j].Text; j++;
                                question.point6 = worksheet.Cells[i, j].Text; j++;
                                question.point7 = worksheet.Cells[i, j].Text; j++;

                                if (!string.IsNullOrEmpty(question.questionth))
                                {
                                    if (question.questionth.Contains("\n"))
                                    {
                                        question.questionth = question.questionth.Replace("\n", "<br/>");
                                    }
                                }
                                if (question.questiontype.toQuestionType() == QuestionType.Attitude | question.questiontype.toQuestionType() == QuestionType.MultipleChoice | question.questiontype.toQuestionType() == QuestionType.TrueFalse | question.questiontype.toQuestionType() == QuestionType.ShortAnswer | question.questiontype.toQuestionType() == QuestionType.Essay | question.questiontype.toQuestionType() == QuestionType.Assignment)
                                {
                                    question.parentid = null;
                                }
                                else if (question.questiontype.toQuestionType() == QuestionType.MultipleMatchingSub | question.questiontype.toQuestionType() == QuestionType.ReadingShortAnswer | question.questiontype.toQuestionType() == QuestionType.ReadingMultipleChoice | question.questiontype.toQuestionType() == QuestionType.ReadingAttitude | question.questiontype.toQuestionType() == QuestionType.ReadingTrueFalse | question.questiontype.toQuestionType() == QuestionType.ReadingMultipleMatching | question.questiontype.toQuestionType() == QuestionType.ReadingEssay | question.questiontype.toQuestionType() == QuestionType.ReadingAssignment)
                                {
                                    if (parentid.HasValue)
                                        question.parentid = parentid;
                                }
                                else if (question.questiontype.toQuestionType() == QuestionType.ReadingText | question.questiontype.toQuestionType() == QuestionType.MultipleMatching)
                                {
                                    parentid = id;
                                }

                                if (question.questiontype.toQuestionType() == QuestionType.TrueFalse | question.questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                                {
                                    question.tpoint = question.point1;
                                    question.fpoint = question.point2;
                                }

                                question.tempid = id;
                                questions.Add(question);
                                id++;
                            }
                            else
                            {
                                var answer = new answer_import();
                                if (questions[questions.Count - 1].questiontype == "MS")
                                {
                                    answer.choice = worksheet.Cells[i, j].Text; j++;
                                }
                                else
                                {
                                    answer.point = worksheet.Cells[i, j].Text; j++;
                                }
                                answer.answerth = worksheet.Cells[i, j].Text; j++;
                                if (!string.IsNullOrEmpty(answer.answerth))
                                {
                                    if (questions[questions.Count - 1].questiontype == "MS")
                                    {
                                        var q = questions.Where(w => w.tempid == parentid).FirstOrDefault();
                                        if (q != null)
                                            q.answers.Add(answer);
                                    }
                                    else
                                    {
                                        questions[questions.Count - 1].answers.Add(answer);
                                    }
                                }
                            }
                        }
                        return savequestion(questions);

                    }
                }
            }
        }

        private object savequestion(List<question_import> questions)
        {
            var childorder = 1;
            foreach (var q in questions)
            {
                var question = new Question();
                question.Create_On = DateUtil.Now();
                question.Update_On = DateUtil.Now();
                question.Create_By = q.update_by;
                question.Update_By = q.update_by;
                question.ApprovalStatus = QuestionApprovalType.Draft;
                question.CourseTh = true;
                question.CourseEn = true;
                question.QuestionLevel = QuestionLevel.Mid;
                question.QuestionTh = q.questionth;
                question.QuestionType = q.questiontype.toQuestionType();
                question.Status = StatusType.Active;
                question.Remark = q.no;
                question.Choice = q.choice;
                question.AnswerType = q.anstype;
                question.MaxPoint = q.maxpoint;
                question.No = q.no;
                if (question.QuestionTh.Contains("[") & question.QuestionTh.Contains("]"))
                {
                    var index1 = question.QuestionTh.IndexOf("[");
                    var index2 = question.QuestionTh.IndexOf("]");
                    var imagename = question.QuestionTh.Substring(index1 + 1, index2 - (index1 + 1));
                    var fileurl = this._conf.HostUrl + "\\images\\" + imagename + ".jpg";
                    fileurl = "<img src='" + fileurl.Replace("\\", "/") + "' />";
                    question.QuestionTh = question.QuestionTh.Replace("[" + imagename + "]", fileurl);
                }
                question.QuestionEn = question.QuestionTh;


                var subject = _context.Subjects.Include(i => i.SubjectGroup).Where(w => w.Name == q.subjectname).FirstOrDefault();
                if (subject == null)
                    return CreatedAtAction(nameof(upload), new { result = ResultCode.DataHasNotFound, message = "ไม่พบข้อมูลวิชา " + q.subjectname });

                if (!string.IsNullOrEmpty(q.subjectsub))
                {
                    var sub = _context.SubjectSubs.Where(w => w.Name == q.subjectsub).FirstOrDefault();
                    if (sub == null)
                        return CreatedAtAction(nameof(upload), new { result = ResultCode.DataHasNotFound, message = "ไม่พบข้อมูลวิชาย่อย" });
                    question.SubjectSubID = sub.ID;
                }
                else
                {
                    var sub = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID).FirstOrDefault();
                    if (sub == null)
                        return CreatedAtAction(nameof(upload), new { result = ResultCode.DataHasNotFound, message = "ไม่พบข้อมูลวิชาย่อย" });
                    question.SubjectSubID = sub.ID;
                }

                question.SubjectGroupID = subject.SubjectGroupID;
                question.SubjectID = subject.ID;
                //.Remark = no;
                if (q.questiontype.toQuestionType() == QuestionType.Attitude | q.questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                {
                    question.Point1 = NumUtil.ParseDecimal(q.point1);
                    question.Point2 = NumUtil.ParseDecimal(q.point2);
                    question.Point3 = NumUtil.ParseDecimal(q.point3);
                    question.Point4 = NumUtil.ParseDecimal(q.point4);
                    question.Point5 = NumUtil.ParseDecimal(q.point5);
                    question.Point6 = NumUtil.ParseDecimal(q.point6);
                    question.Point7 = NumUtil.ParseDecimal(q.point7);

                    if (!string.IsNullOrEmpty(q.answersub1))
                    {
                        var sub = _context.SubjectSubs.Where(w => w.Name == q.answersub1).FirstOrDefault();
                        if (sub != null)
                            question.AnswerSubjectSub1 = sub.ID;
                    }
                    if (!string.IsNullOrEmpty(q.answersub2))
                    {
                        var sub = _context.SubjectSubs.Where(w => w.Name == q.answersub2).FirstOrDefault();
                        if (sub != null)
                            question.AnswerSubjectSub2 = sub.ID;
                    }
                    if (!string.IsNullOrEmpty(q.answersub3))
                    {
                        var sub = _context.SubjectSubs.Where(w => w.Name == q.answersub3).FirstOrDefault();
                        if (sub != null)
                            question.AnswerSubjectSub3 = sub.ID;
                    }
                    if (!string.IsNullOrEmpty(q.answersub4))
                    {
                        var sub = _context.SubjectSubs.Where(w => w.Name == q.answersub4).FirstOrDefault();
                        if (sub != null)
                            question.AnswerSubjectSub4 = sub.ID;
                    }
                    question.AttitudeAnsType = q.attanstype.toAttitudeAnsType();
                    question.AttitudeAnsSubType = q.subattanstype.toAttitudeAnsSubType();
                }

                if (q.questiontype.toQuestionType() == QuestionType.MultipleChoice | q.questiontype.toQuestionType() == QuestionType.ReadingMultipleChoice)
                {
                    var order = 1;
                    foreach (var ans in q.answers)
                    {
                        var answer = new QuestionAns();
                        answer.Question = question;
                        answer.AnswerTh = ans.answerth;
                        answer.Point = NumUtil.ParseDecimal(ans.point);
                        answer.Order = order;
                        answer.Create_On = DateUtil.Now();
                        answer.Update_On = DateUtil.Now();
                        answer.Create_By = q.update_by;
                        answer.Update_By = q.update_by;

                        if (answer.AnswerTh.Contains("[") & answer.AnswerTh.Contains("]"))
                        {
                            var index1 = answer.AnswerTh.IndexOf("[");
                            var index2 = answer.AnswerTh.IndexOf("]");
                            var imagename = answer.AnswerTh.Substring(index1 + 1, index2 - (index1 + 1));
                            var fileurl = this._conf.HostUrl + "\\images\\" + imagename + ".jpg";
                            fileurl = "<img src='" + fileurl.Replace("\\", "/") + "' />";
                            answer.AnswerTh = answer.AnswerTh.Replace("[" + imagename + "]", fileurl);
                        }
                        answer.AnswerEn = answer.AnswerTh;
                        _context.QuestionAnies.Add(answer);
                        order++;
                    }
                }

                if (q.questiontype.toQuestionType() == QuestionType.TrueFalse | q.questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                {
                    question.TPoint = NumUtil.ParseDecimal(q.tpoint);
                    question.FPoint = NumUtil.ParseDecimal(q.fpoint);
                }

                if (q.questiontype.toQuestionType() == QuestionType.ReadingText)
                {
                    childorder = 1;
                }

                if (q.questiontype.toQuestionType() == QuestionType.ReadingAttitude | q.questiontype.toQuestionType() == QuestionType.ReadingMultipleChoice | q.questiontype.toQuestionType() == QuestionType.ReadingTrueFalse | q.questiontype.toQuestionType() == QuestionType.ReadingShortAnswer | q.questiontype.toQuestionType() == QuestionType.ReadingEssay | q.questiontype.toQuestionType() == QuestionType.ReadingAssignment)
                {
                    var parent = questions.Where(w => w.tempid == q.parentid).FirstOrDefault();
                    if (parent != null)
                        question.QuestionParentID = parent.id;

                    if (q.questiontype.toQuestionType() == QuestionType.ReadingMultipleChoice)
                        question.QuestionType = QuestionType.MultipleChoice;
                    else if (q.questiontype.toQuestionType() == QuestionType.ReadingTrueFalse)
                        question.QuestionType = QuestionType.TrueFalse;
                    else if (q.questiontype.toQuestionType() == QuestionType.ReadingShortAnswer)
                        question.QuestionType = QuestionType.ShortAnswer;
                    else if (q.questiontype.toQuestionType() == QuestionType.ReadingEssay)
                        question.QuestionType = QuestionType.Essay;
                    else if (q.questiontype.toQuestionType() == QuestionType.ReadingAssignment)
                        question.QuestionType = QuestionType.Assignment;
                    else if (q.questiontype.toQuestionType() == QuestionType.ReadingAttitude)
                        question.QuestionType = QuestionType.Attitude;

                    question.ChildOrder = childorder;
                    childorder++;
                }

                if (q.questiontype.toQuestionType() == QuestionType.MultipleMatching)
                {
                    childorder = 1;
                    var order = 1;
                    foreach (var ans in q.answers)
                    {
                        var answer = new QuestionAns();
                        answer.Question = question;
                        answer.AnswerTh = ans.answerth;
                        answer.AnswerEn = answer.AnswerTh;
                        answer.Choice = ans.choice;
                        answer.Order = order;
                        answer.Create_On = DateUtil.Now();
                        answer.Update_On = DateUtil.Now();
                        answer.Create_By = q.update_by;
                        answer.Update_By = q.update_by;
                        _context.QuestionAnies.Add(answer);
                        order++;
                    }

                }
                if (q.questiontype.toQuestionType() == QuestionType.MultipleMatchingSub)
                {
                    var parent = questions.Where(w => w.tempid == q.parentid).FirstOrDefault();
                    if (parent != null)
                        question.QuestionParentID = parent.id;

                    question.Point = 1;
                    question.Choice = q.choice;
                    question.ChildOrder = childorder;
                    childorder++;
                }

                _context.Questions.Add(question);
                _context.SaveChanges();
                question.QuestionCode = question.QuestionType.toQuestionTypeMin2() + question.ID.ToString("00000000");
                _context.SaveChanges();
                q.id = question.ID;
            }
            return CreatedAtAction(nameof(uploadword), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("listquestionapprove")]
        public object listquestionapprove(string appr_search, int pageno = 1)
        {
            var appr = appr_search.toQuestionApprovalStatus();
            var questions = _context.Questions.Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Include(i => i.SubjectSub).Where(w => w.ApprovalStatus == appr);
            var approvals = _context.QuestionApprovals.Where(w => questions.Select(s => s.ID).Contains(w.QuestionID));

            int skipRows = (pageno - 1) * 10;
            var itemcnt = questions.Count();
            var pagelen = itemcnt / 10;
            if (itemcnt % 10 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listquestionapprove), new
            {
                data = questions.Select(s => new
                {
                    id = s.ID,
                    //name = s.Name,
                    status = s.Status.toStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    subject = s.Subject.Name,
                    sub = s.SubjectSub.Name,
                    subjectindex = s.Subject.Order,
                    questioncode = s.QuestionCode,
                    approvalstatus = s.ApprovalStatus,
                    approvalstatusname = s.ApprovalStatus.toQuestionApprovalStatusName(),
                    apprid = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().ID : 0,
                    approvalcnt = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().ApprovalCnt : 0,
                    approvedcnt = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().ApprovedCnt : 0,
                    rejectedcnt = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().RejectedCnt : 0,
                    startfrom = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? DateUtil.ToDisplayDate(approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().StartFrom) : null,
                    endfrom = approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault() != null ? DateUtil.ToDisplayDate(approvals.Where(w => w.QuestionID == s.ID).FirstOrDefault().EndFrom) : null,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.subjectindex).ThenBy(o3 => o3.id).Skip(skipRows).Take(10).ToArray(),
                pagelen = pagelen,
                itemcnt = itemcnt,
            }); ;

        }

        [HttpGet]
        [Route("listapprstaff")]
        public object listapprstaff(int? id)
        {
            var questionappr = _context.QuestionApprovals.Where(w => w.QuestionID == id).OrderByDescending(o => o.ID).FirstOrDefault();
            if(questionappr != null)
            {
                var staffs = _context.QuestionApprovalStaffs.Include(i => i.Staff).Where(w => w.QuestionApprovalID == questionappr.ID);
                if (staffs != null && staffs.Count() > 0)
                    return staffs.Select(s => new
                    {
                        id = s.ID,
                        staffid = s.StaffID,
                        firstname = s.Staff.FirstName,
                        lastname = s.Staff.LastName,
                        remark = s.Remark,
                        approvalstatusname = s.QuestionApprovalType.toQuestionApprovalStatusName(),
                        approvalstatus = s.QuestionApprovalType,
                        create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                        create_by = s.Create_By,
                        update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                        update_by = s.Update_By,
                    }).ToArray();
            }
            return CreatedAtAction(nameof(listapprstaff), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }


        [HttpGet]
        [Route("approveconfirm")]
        public object approveconfirm(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.ApprovalStatus = QuestionApprovalType.Pending;

            var staffs = _context.Staffs.Where(w => w.isQuestionAppr);
            var appr = new QuestionApproval();
            appr.QuestionID = model.ID;
            appr.ApprovalCnt = staffs.Count();
            appr.ApprovedCnt = 0;
            appr.StartFrom = DateUtil.Now();
            appr.EndFrom = DateUtil.Now().AddMonths(1);
            appr.Create_On = DateUtil.Now();
            appr.Update_On = DateUtil.Now();
            appr.Create_By = update_by;
            appr.Update_By = update_by;

            foreach (var staff in staffs)
            {
                var appstaff = new QuestionApprovalStaff();
                appstaff.StaffID = staff.ID;
                appstaff.QuestionApprovalType = QuestionApprovalType.Pending;
                appstaff.QuestionApproval = appr;
                appstaff.Create_On = DateUtil.Now();
                appstaff.Update_On = DateUtil.Now();
                appstaff.Create_By = update_by;
                appstaff.Update_By = update_by;
                _context.QuestionApprovalStaffs.Add(appstaff);
            }
            _context.QuestionApprovals.Add(appr);
            _context.SaveChanges();

            //sendNotificationEmail("voranun.datasource@gmail.com","Test","Test");
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("approveconfirmall")]
        public object approveconfirmall(string choose, string update_by)
        {
            var chs = choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var id = NumUtil.ParseInteger(ch);
                    var model = _context.Questions.Where(w => w.ID == id).FirstOrDefault();
                    if (model == null)
                        return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

                    if (model.ApprovalStatus != QuestionApprovalType.Draft)
                        continue;

                    model.ApprovalStatus = QuestionApprovalType.Pending;

                    var staffs = _context.Staffs.Where(w => w.isQuestionAppr);
                    var appr = new QuestionApproval();
                    appr.QuestionID = model.ID;
                    appr.ApprovalCnt = staffs.Count();
                    appr.ApprovedCnt = 0;
                    appr.StartFrom = DateUtil.Now();
                    appr.EndFrom = DateUtil.Now().AddMonths(1);
                    appr.Create_On = DateUtil.Now();
                    appr.Update_On = DateUtil.Now();
                    appr.Create_By = update_by;
                    appr.Update_By = update_by;

                    foreach (var staff in staffs)
                    {
                        var appstaff = new QuestionApprovalStaff();
                        appstaff.StaffID = staff.ID;
                        appstaff.QuestionApprovalType = QuestionApprovalType.Pending;
                        appstaff.QuestionApproval = appr;
                        appstaff.Create_On = DateUtil.Now();
                        appstaff.Update_On = DateUtil.Now();
                        appstaff.Create_By = update_by;
                        appstaff.Update_By = update_by;
                        _context.QuestionApprovalStaffs.Add(appstaff);
                    }
                    _context.QuestionApprovals.Add(appr);

                }
            }

            _context.SaveChanges();

            //sendNotificationEmail("voranun.datasource@gmail.com","Test","Test");
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("approved")]
        public object approved(int? id, int? staffid, string update_by, string remark)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });


            var model = _context.QuestionApprovals.Include(i => i.Question).Where(w => w.QuestionID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var appstaff = _context.QuestionApprovalStaffs.Where(w => w.StaffID == staffid & w.QuestionApprovalID == model.ID).FirstOrDefault();
            if (appstaff == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            appstaff.QuestionApprovalType = QuestionApprovalType.Approved;
            appstaff.Remark = remark;
            appstaff.Update_On = DateUtil.Now();
            appstaff.Update_By = update_by;
            _context.SaveChanges();

            var approvedcnt = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID & w.QuestionApprovalType == QuestionApprovalType.Approved).Count();
            var rejectedcnt = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID & w.QuestionApprovalType == QuestionApprovalType.Reject).Count();
            model.ApprovedCnt = approvedcnt;
            model.RejectedCnt = rejectedcnt;
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            if (model.ApprovedCnt >= (decimal)model.ApprovalCnt / 2M)
            {
                model.Question.Update_On = DateUtil.Now();
                model.Question.Update_By = update_by;
                model.Question.ApprovalStatus = QuestionApprovalType.Approved;
                model.ApprovalStatus = QuestionApprovalType.Approved;
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

            var model = _context.QuestionApprovals.Include(i => i.Question).Where(w => w.QuestionID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var appstaff = _context.QuestionApprovalStaffs.Where(w => w.StaffID == staffid & w.QuestionApprovalID == model.ID).FirstOrDefault();
            if (appstaff == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            appstaff.QuestionApprovalType = QuestionApprovalType.Reject;
            appstaff.Remark = remark;
            appstaff.Update_On = DateUtil.Now();
            appstaff.Update_By = update_by;
            _context.SaveChanges();

            var approvedcnt = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID & w.QuestionApprovalType == QuestionApprovalType.Approved).Count();
            var rejectedcnt = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID & w.QuestionApprovalType == QuestionApprovalType.Reject).Count();
            model.ApprovedCnt = approvedcnt;
            model.RejectedCnt = rejectedcnt;
            model.Update_On = DateUtil.Now();
            model.Update_By = update_by;

            if (model.RejectedCnt >= (decimal)model.RejectedCnt / 2M)
            {
                model.Question.Update_On = DateUtil.Now();
                model.Question.Update_By = update_by;
                model.Question.ApprovalStatus = QuestionApprovalType.Reject;
                model.ApprovalStatus = QuestionApprovalType.Reject;
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


        [HttpGet]
        [Route("approvedmaster")]
        public object approvedmaster(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.QuestionApprovals.Include(i => i.Question).Where(w => w.QuestionID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Question.Update_On = DateUtil.Now();
            model.Question.Update_By = update_by;
            model.Question.ApprovalStatus = QuestionApprovalType.Approved;
            model.Question.Remark = "อนุมัติโดยผู้กลั่นกรองพิเศษ";
            model.ApprovalStatus = QuestionApprovalType.Approved;

            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("approvedmasterall")]
        public object approvedmasterall(string choose, string update_by)
        {
            var chs = choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var id = NumUtil.ParseInteger(ch);
                    var model = _context.Questions.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
                    if (model != null)
                    {

                        model.Update_On = DateUtil.Now();
                        model.Update_By = update_by;
                        model.ApprovalStatus = QuestionApprovalType.Approved;
                        model.Remark = "อนุมัติโดยผู้กลั่นกรองพิเศษ";
                    }
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approvedmasterall), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("rejectedmaster")]
        public object rejectedmaster(int? id, int? staffid, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.QuestionApprovals.Include(i => i.Question).Where(w => w.QuestionID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Question.Update_On = DateUtil.Now();
            model.Question.Update_By = update_by;
            model.Question.ApprovalStatus = QuestionApprovalType.Reject;
            model.Question.Remark = "ไม่อนุมัติโดยผู้กลั่นกรองพิเศษ";
            model.ApprovalStatus = QuestionApprovalType.Reject;
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("rejectedmasterall")]
        public object rejectedmasterall(string choose, string update_by)
        {
            var chs = choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var id = NumUtil.ParseInteger(ch);
                    var model = _context.Questions.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
                    if (model != null)
                    {
                        if (model.ApprovalStatus == QuestionApprovalType.Pending)
                        {
                            model.Update_On = DateUtil.Now();
                            model.Update_By = update_by;
                            model.ApprovalStatus = QuestionApprovalType.Reject;
                            model.Remark = "ไม่อนุมัติโดยผู้กลั่นกรองพิเศษ";
                        }
                    }
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(rejectedmasterall), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("draftmaster")]
        public object draftmaster(int? id, int? staffid, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.QuestionApprovals.Include(i => i.Question).Where(w => w.QuestionID == id).OrderByDescending(i => i.ID).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(approved), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            model.Question.Update_On = DateUtil.Now();
            model.Question.Update_By = update_by;
            model.Question.ApprovalStatus = QuestionApprovalType.Draft;
            model.Question.Remark = "";

            var appstaff = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID);
            if (appstaff.Count() > 0)
                _context.QuestionApprovalStaffs.RemoveRange(appstaff);
            _context.QuestionApprovals.Remove(model);

            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("draftmasterall")]
        public object draftmasterall(string choose, string update_by)
        {
            var chs = choose.Split(";");
            foreach (var ch in chs)
            {
                if (!string.IsNullOrEmpty(ch))
                {
                    var id = NumUtil.ParseInteger(ch);
                    var model = _context.Questions.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
                    if (model != null)
                    {
                        model.Update_On = DateUtil.Now();
                        model.Update_By = update_by;
                        model.ApprovalStatus = QuestionApprovalType.Draft;
                        model.Remark = "";
                    }

                    var appstaff = _context.QuestionApprovalStaffs.Where(w => w.QuestionApprovalID == model.ID);
                    if (appstaff.Count() > 0)
                        _context.QuestionApprovalStaffs.RemoveRange(appstaff);

                    var app = _context.QuestionApprovals.Where(w => w.ID == model.ID).FirstOrDefault();
                    if (app != null)
                        _context.QuestionApprovals.Remove(app);

                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(approveconfirm), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("changestatus")]
        public object changestatus(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(changestatus), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Questions.Where(w => w.ID == id).OrderByDescending(i => i.ID).FirstOrDefault();
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

        public string sendNotificationEmail(string to, string header, string message)
        {
            var msg = new System.Text.StringBuilder();
            try
            {
                var SMTP_SERVER = _conf.SMTP_SERVER;
                var SMTP_PORT = _conf.SMTP_PORT;
                var SMTP_USERNAME = _conf.SMTP_USERNAME;
                var SMTP_PASSWORD = _conf.SMTP_PASSWORD;
                var SMTP_FROM = _conf.SMTP_FROM;
                bool STMP_SSL = _conf.STMP_SSL;

                SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
                System.Net.NetworkCredential cred = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                smtpClient.UseDefaultCredentials = true;
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = STMP_SSL;

                var mail = new MailMessage(SMTP_FROM, to, header, message);
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                smtpClient.Credentials = cred;
                smtpClient.Send(mail);

                return msg.ToString();
            }
            catch (Exception ex)
            {
                msg.AppendLine(" EXCEPTION: " + ex.Message);
            }
            return msg.ToString();
        }

        #region temp


        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadgtemp")]
        public object uploadgtemp([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportExamRegisterDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var file = Convert.FromBase64String(model.fileupload.value);
                var questions = new List<question_import>();
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
                            int totalCols = worksheet.Dimension.End.Column;
                            var subjectname = worksheet.Cells[1, 1].Text;
                            for (int i = 1; i <= totalCols; i++)
                            {
                                var j = 2;
                                var subname = worksheet.Cells[j, i].Text; j++;
                                var questionno = worksheet.Cells[j, i].Text; j++;
                                var row1 = worksheet.Cells[j, i].Text; j++;
                                var row2 = worksheet.Cells[j, i].Text; j++;
                                var row3 = worksheet.Cells[j, i].Text; j++;
                                var row4 = worksheet.Cells[j, i].Text; j++;
                                var question = _context.Questions.Where(w => w.No == questionno & w.Subject.Name == subjectname).FirstOrDefault();
                                if (question == null)
                                    continue;

                                var sub = _context.SubjectSubs.Where(w => w.Name == subname).FirstOrDefault();
                                if (sub != null)
                                {
                                    question.SubjectSubID = sub.ID;

                                    var parent = _context.Questions.Where(w => w.ID == question.QuestionParentID).FirstOrDefault();
                                    if (parent != null)
                                        parent.SubjectSubID = sub.ID;
                                }


                                question.Update_By = model.update_by;
                                if (question.QuestionType == QuestionType.Attitude | question.QuestionType == QuestionType.ReadingAttitude)
                                {
                                    if (!string.IsNullOrEmpty(row4))
                                        question.AttitudeAnsType = AttitudeAnsType.Type4;
                                    else if (!string.IsNullOrEmpty(row3))
                                        question.AttitudeAnsType = AttitudeAnsType.Type3;
                                    else
                                        question.AttitudeAnsType = AttitudeAnsType.Type2;
                                    question.AttitudeAnsSubType = AttitudeAnsSubType.Sub1;
                                }

                                var no = NumUtil.ParseInteger(questionno);

                                var subo = _context.SubjectSubs.Where(w => w.Name == "O").FirstOrDefault();
                                if (question.SubjectSubID == subo.ID)
                                {
                                    question.AnswerType = AnswerType.SubjectSub;
                                    if (!string.IsNullOrEmpty(row1))
                                    {
                                        var sub1 = _context.SubjectSubs.Where(w => w.Name == row1).FirstOrDefault();
                                        if (sub1 != null)
                                            question.AnswerSubjectSub1 = sub1.ID;
                                    }
                                    if (!string.IsNullOrEmpty(row2))
                                    {
                                        var sub1 = _context.SubjectSubs.Where(w => w.Name == row2).FirstOrDefault();
                                        if (sub1 != null)
                                            question.AnswerSubjectSub2 = sub1.ID;
                                    }
                                    if (!string.IsNullOrEmpty(row3))
                                    {
                                        var sub1 = _context.SubjectSubs.Where(w => w.Name == row3).FirstOrDefault();
                                        if (sub1 != null)
                                            question.AnswerSubjectSub3 = sub1.ID;
                                    }
                                    if (!string.IsNullOrEmpty(row4))
                                    {
                                        var sub1 = _context.SubjectSubs.Where(w => w.Name == row4).FirstOrDefault();
                                        if (sub1 != null)
                                            question.AnswerSubjectSub4 = sub1.ID;
                                    }
                                }
                                else
                                {
                                    if (question.QuestionType == QuestionType.Attitude | question.QuestionType == QuestionType.ReadingAttitude)
                                    {
                                        question.AnswerType = AnswerType.Point;
                                        question.Point1 = NumUtil.ParseDecimal(row1);
                                        question.Point2 = NumUtil.ParseDecimal(row2);
                                        question.Point3 = NumUtil.ParseDecimal(row3);
                                        question.Point4 = NumUtil.ParseDecimal(row4);

                                        question.MaxPoint = NumUtil.ParseDecimal(row1);
                                        if (question.MaxPoint < NumUtil.ParseDecimal(row2))
                                            question.MaxPoint = NumUtil.ParseDecimal(row2);
                                        if (question.MaxPoint < NumUtil.ParseDecimal(row3))
                                            question.MaxPoint = NumUtil.ParseDecimal(row3);
                                        if (question.MaxPoint < NumUtil.ParseDecimal(row4))
                                            question.MaxPoint = NumUtil.ParseDecimal(row4);
                                    }
                                    else if (question.QuestionType == QuestionType.MultipleChoice | question.QuestionType == QuestionType.ReadingMultipleChoice)
                                    {
                                        var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID).OrderBy(o => o.Order);
                                        var aindex = 1;
                                        foreach (var answer in answers)
                                        {
                                            if (aindex == 1)
                                                answer.Point = NumUtil.ParseDecimal(row1);
                                            else if (aindex == 2)
                                                answer.Point = NumUtil.ParseDecimal(row2);
                                            else if (aindex == 3)
                                                answer.Point = NumUtil.ParseDecimal(row3);
                                            else if (aindex == 4)
                                                answer.Point = NumUtil.ParseDecimal(row4);
                                            aindex++;
                                        }
                                    }
                                }
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }


            return CreatedAtAction(nameof(upload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadganswertemp")]
        public object uploadganswertemp([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImportExamRegisterDTO>(json.GetRawText());
            if (model != null && model.fileupload != null)
            {
                var file = Convert.FromBase64String(model.fileupload.value);
                var questions = new List<question_import>();
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
                            var subi = _context.SubjectSubs.Where(w => w.Name == "I").FirstOrDefault();
                            var subs = _context.SubjectSubs.Where(w => w.Name == "S").FirstOrDefault();

                            var worksheet = package.Workbook.Worksheets.First();
                            int totalRows = worksheet.Dimension.End.Row;
                            int totalCols = worksheet.Dimension.End.Column;
                            var subjectname = worksheet.Cells[1, 1].Text;
                            var subject = _context.Subjects.Where(w => w.Name == subjectname).FirstOrDefault();
                            var questionnolist = new List<int>();

                            for (int col = 2; col <= totalCols; col++)
                            {
                                var no = worksheet.Cells[2, col].Text;
                                questionnolist.Add(NumUtil.ParseInteger(no));
                            }

                            for (int row = 3; row <= totalRows; row++)
                            {                                
                                var studentcode = worksheet.Cells[row, 1].Text;
                                var student = _context.Students.Where(w => w.StudentCode == studentcode).FirstOrDefault();
                                if (student == null)
                                {
                                    var u = new User();
                                    u.UserName = studentcode;
                                    u.Password = DataEncryptor.Encrypt(studentcode);
                                    u.ConfirmPassword = DataEncryptor.Encrypt(studentcode);
                                    u.Create_On = DateUtil.Now();
                                    u.Create_By = model.update_by;
                                    u.Update_On = DateUtil.Now();
                                    u.Update_By = model.update_by;

                                    student = new Student();
                                    student.FirstName = studentcode;
                                    student.LastName = studentcode;
                                    student.FirstNameEn = studentcode;
                                    student.LastNameEn = studentcode;
                                    student.Prefix = Prefix.Mr;
                                    student.IDCard = studentcode;
                                    student.StudentCode = studentcode;
                                    student.Course = Course.Thai;
                                    student.Status = StatusType.Active;
                                    student.Create_On = DateUtil.Now();
                                    student.Create_By = model.update_by;
                                    student.Update_On = DateUtil.Now();
                                    student.Update_By = model.update_by;
                                    student.User = u;

                                    _context.Students.Add(student);
                                    _context.SaveChanges();
                                }
                                var exam = _context.Exams.Where(w => w.Subject.Name == subjectname).OrderByDescending(o => o.ID).FirstOrDefault();
                                if (exam != null)
                                {
                                    var exreg = _context.ExamRegisters.Where(w => w.ExamID == exam.ID & w.StudentID == student.ID).FirstOrDefault();
                                    if (exreg == null)
                                    {
                                        var reg = new ExamRegister();
                                        reg.StudentID = student.ID;
                                        reg.ExamID = NumUtil.ParseInteger(exam.ID);
                                        reg.ExamRegisterType = ExamRegisterType.Advance;
                                        reg.Create_On = DateUtil.Now();
                                        reg.Update_On = DateUtil.Now();
                                        reg.Create_By = model.update_by;
                                        reg.Update_By = model.update_by;
                                        _context.ExamRegisters.Add(reg);
                                        _context.SaveChanges();
                                    }
                                }

                                var testresult = _context.TestResults.Where(w => w.ExamID == exam.ID).FirstOrDefault();
                                if (testresult == null)
                                {
                                    testresult = new TestResult();
                                    testresult.ExamID = exam.ID;
                                    testresult.ProvedCnt = 0;
                                    testresult.Update_On = DateUtil.Now();
                                    testresult.Create_On = DateUtil.Now();
                                    testresult.Update_By = model.update_by;
                                    testresult.Create_By = model.update_by;
                                    _context.TestResults.Add(testresult);
                                }
                                _context.SaveChanges();

                                var tstudents = new List<TestResultStudent>();

                                var tstudent = _context.TestResultStudents.Include(i => i.Test).Where(w => w.ExamID == exam.ID && w.StudentID == student.ID).FirstOrDefault();
                                var register = _context.ExamRegisters.Where(w => w.StudentID == student.ID & w.ExamID == exam.ID).FirstOrDefault();
                                testresult.ProveStatus = ProveStatus.Pending;
                                testresult.Update_On = DateUtil.Now();
                                testresult.Update_By = model.update_by;
                                if (tstudent == null)
                                {
                                    tstudent = new TestResultStudent();
                                    tstudent.ExamID = exam.ID;
                                    tstudent.Exam = exam;
                                    tstudent.ProveStatus = ProveStatus.Proved;
                                    tstudent.ExamingStatus = ExamingStatus.Done;
                                    tstudent.ExamRegisterType = register.ExamRegisterType;
                                    tstudent.StudentID = student.ID;
                                    tstudent.Create_On = DateUtil.Now();
                                    tstudent.Create_By = model.update_by;
                                    tstudent.Update_On = DateUtil.Now();
                                    tstudent.Update_By = model.update_by;
                                    tstudent.TestResultID = testresult.ID;
                                    if (exam.Test == null)
                                    {
                                        var test = _context.Tests.Where(w => w.SubjectID == exam.SubjectID && w.Status == StatusType.Active).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
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
                                        tstudent.Test = exam.Test;
                                        tstudent.TestID = exam.TestID.Value;
                                        _context.TestResultStudents.Add(tstudent);
                                        tstudents.Add(tstudent);
                                    }
                                }
                                else
                                    tstudents.Add(tstudent);

                                _context.SaveChanges();

                                foreach (var tstud in tstudents)
                                {
                                    var i = 1;
                                    var tsqs = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstud.ID).FirstOrDefault();
                                    if (tsqs == null)
                                    {
                                        var qcustoms = _context.TestQCustoms.Include(i => i.Question).Where(w => w.TestID == tstud.TestID).OrderBy(o => o.Order);

                                        foreach (var custom in qcustoms)
                                        {
                                            if (custom.Question.QuestionType == QuestionType.ReadingText | custom.Question.QuestionType == QuestionType.MultipleMatching)
                                            {
                                                var children = _context.Questions.Where(w => w.QuestionParentID == custom.QuestionID).OrderBy(o => o.ChildOrder);
                                                foreach (var child in children)
                                                {
                                                    var tsq = new TestResultStudentQAns();
                                                    tsq.QuestionID = child.ID;
                                                    tsq.TestResultStudentID = tstud.ID;
                                                    tsq.Create_On = DateUtil.Now();
                                                    tsq.Update_On = DateUtil.Now();
                                                    tsq.Create_By = model.update_by;
                                                    tsq.Update_By = model.update_by;
                                                    tsq.Index = i;
                                                    i++;
                                                    _context.TestResultStudentQAnies.Add(tsq);
                                                }
                                            }
                                            else
                                            {
                                                var tsq = new TestResultStudentQAns();
                                                tsq.QuestionID = custom.QuestionID;
                                                tsq.TestResultStudentID = tstud.ID;
                                                tsq.Create_On = DateUtil.Now();
                                                tsq.Update_On = DateUtil.Now();
                                                tsq.Create_By = model.update_by;
                                                tsq.Update_By = model.update_by;
                                                tsq.Index = i;
                                                i++;
                                                _context.TestResultStudentQAnies.Add(tsq);
                                            }

                                        }

                                        tstud.Test.QuestionCnt = i - 1;
                                        tstud.QuestionCnt = i - 1;
                                    }
                                    else
                                    {
                                        var qcnt = _context.TestResultStudentQAnies.Where(w => w.TestResultStudentID == tstud.ID).Count();
                                        tstud.Test.QuestionCnt = qcnt;
                                        tstud.QuestionCnt = qcnt;
                                    }
                                }
                                _context.SaveChanges();


                                exam.ExamRegisterCnt = _context.TestResultStudents.Where(w => w.ExamID == exam.ID).Count();
                                exam.Update_On = DateUtil.Now();
                                exam.Update_By = model.update_by;

                                _context.SaveChanges();

                                var noindex = 0;
                                for (int col = 2; col <= totalCols; col++)
                                {
                                    var no = questionnolist[noindex];

                                    var tanswers = _context.TestResultStudentQAnies.Include(i=>i.Question).Where(w => w.Index == no & w.TestResultStudentID == tstudent.ID).FirstOrDefault();
                                    if(tanswers != null)
                                    {
                                        var question = tanswers.Question;
                                        if (question != null)
                                        {                                           
                                            var aindex = 1;
                                            if (question.QuestionType == QuestionType.MultipleChoice)
                                            {
                                                var choose = worksheet.Cells[row, col].Text;
                                                var choose1 = NumUtil.ParseInteger(choose);
                                                var answers = _context.QuestionAnies.Where(w => w.QuestionID == question.ID).OrderBy(o => o.Order);
                                                foreach (var answer in answers)
                                                {
                                                    if (choose1 == aindex)
                                                    {
                                                        tanswers.QuestionAnsAttitudeID = choose1;
                                                        tanswers.QuestionAnsID = answer.ID;
                                                    }
                                                    aindex++;
                                                }
                                            }
                                            else if (question.QuestionType == QuestionType.Attitude)
                                            {
                                                var choose = worksheet.Cells[row, col].Text;
                                                tanswers.Answered = true;

                                                if (question.AnswerType == AnswerType.SubjectSub)
                                                {
                                                    if (choose == "R1")
                                                    {
                                                        tanswers.SubjectSubID = subi.ID;
                                                        tanswers.QuestionAnsAttitudeID = 1;
                                                    }
                                                    else if (choose == "R2") {
                                                        tanswers.SubjectSubID = subs.ID;
                                                        tanswers.QuestionAnsAttitudeID = 2;
                                                    }
                                                    
                                                }
                                                else
                                                {
                                                    var choose1 = NumUtil.ParseInteger(choose);
                                                    tanswers.QuestionAnsAttitudeID = choose1;
                                                }
                                            }
                                        }
                                    }
                                   
                                    noindex++;
                                }


                                var tresultstudent = _context.TestResultStudents.Where(w => w.ID == tstudent.ID).FirstOrDefault();
                                if (tresultstudent != null)
                                {
                                    if (tresultstudent.ExamingStatus == ExamingStatus.Examing)
                                    {
                                        tresultstudent.ExamingStatus = ExamingStatus.Done;
                                        tresultstudent.End_On = DateUtil.Now();
                                    }
                                    else if (tresultstudent.ExamingStatus == ExamingStatus.None)
                                    {
                                        tresultstudent.ExamingStatus = ExamingStatus.Absent;
                                    }

                                    tresultstudent.Update_By = model.update_by;
                                    tresultstudent.Update_On = DateUtil.Now();
                                    _context.SaveChanges();
                                    prove(tstudent.ID, model.update_by);
                                }
                            }                           
                            _context.SaveChanges();
                            return CreatedAtAction(nameof(upload), new { result = ResultCode.Success, message = ResultMessage.Success, tsresultid = 0 });

                        }
                    }
                }
            }


            return CreatedAtAction(nameof(upload), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

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
                            if (tanswer.Question.AnswerType == AnswerType.SubjectSub)
                            {
                                if (!tanswer.SubjectSubID.HasValue)
                                {
                                    if (tanswer.QuestionAnsAttitudeID == 1)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub1;
                                    else if (tanswer.QuestionAnsAttitudeID == 2)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub2;
                                    else if (tanswer.QuestionAnsAttitudeID == 3)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub3;
                                    else if (tanswer.QuestionAnsAttitudeID == 4)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub4;
                                    else if (tanswer.QuestionAnsAttitudeID == 5)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub5;
                                    else if (tanswer.QuestionAnsAttitudeID == 6)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub6;
                                    else if (tanswer.QuestionAnsAttitudeID == 7)
                                        tanswer.SubjectSubID = tanswer.Question.AnswerSubjectSub7;
                                }
                                tresultstudent.CorrectCnt++;
                            }
                            else
                            {
                                decimal attpoint = 0;
                                if (tanswer.QuestionAnsAttitudeID == 1)
                                    attpoint = question.Point1.HasValue ? question.Point1.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 2)
                                    attpoint = question.Point2.HasValue ? question.Point2.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 3)
                                    attpoint = question.Point3.HasValue ? question.Point3.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 4)
                                    attpoint = question.Point4.HasValue ? question.Point4.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 5)
                                    attpoint = question.Point5.HasValue ? question.Point5.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 6)
                                    attpoint = question.Point6.HasValue ? question.Point6.Value : 0;
                                else if (tanswer.QuestionAnsAttitudeID == 7)
                                    attpoint = question.Point7.HasValue ? question.Point7.Value : 0;

                                tresultstudent.Point += attpoint;
                                tanswer.Point = attpoint;
                                if (attpoint > 0)
                                    tresultstudent.CorrectCnt++;
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

        #endregion

    }

}
