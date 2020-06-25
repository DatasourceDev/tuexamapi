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
    public class SubjectController : ControllerBase
    {

        private readonly ILogger<SubjectController> _logger;
        public TuExamContext _context;

        public SubjectController(ILogger<SubjectController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listsubject")]
        public object listAllsubject(string text_search, string status_search, string group_search)
        {
            var subject = _context.Subjects.Include(i=>i.SubjectGroup).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(status_search))
                subject = subject.Where(w => w.Status == status_search.toStatus());
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if(groupID > 0)
                    subject = subject.Where(w => w.SubjectGroupID == groupID);
            }
            var subjects = new List<Subject>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        subjects.AddRange(subject.Where(w => w.Name.Contains(text)));
                    }
                }
                subjects = subjects.Distinct().ToList();
            }
            else
            {
                subjects = subject.ToList();
            }
                

            return subjects.Select(s => new
            {
                id = s.ID,
                name = s.Name,
                description = s.Description,
                order = s.Order,
                status = s.Status.toStatusName(),
                group = s.SubjectGroup.Name,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.group).ThenBy(o2=>o2.order).ToArray();
        }

        [HttpGet]
        [Route("listActivesubject")]
        public object listActivesubject(string group_search)
        {
            var groupID = NumUtil.ParseInteger(group_search);

            var group = _context.Subjects.Where(w=>w.SubjectGroupID == groupID).Include(i => i.SubjectGroup).Where(w => w.Status == StatusType.Active);
            if (group != null)
                return group.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    order = s.Order,
                    status = s.Status.toStatusName(),
                    group = s.SubjectGroup.Name,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.order).ToArray();
            return CreatedAtAction(nameof(listActivesubject), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getordernext")]
        public object getordernext(int? id)
        {
            var groupcnt = _context.Subjects.Where(w => w.SubjectGroupID == id).Count();
            return groupcnt + 1;
        }

        [HttpGet]
        [Route("getsubject")]
        public object getsubject(int? id)
        {
            var subject = _context.Subjects.Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                name = s.Name,
                description = s.Description,
                order = s.Order,
                status = s.Status,
                groupid = s.SubjectGroupID,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (subject != null)
                return subject;
            return CreatedAtAction(nameof(getsubject), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Subject>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var subject = _context.Subjects.Where(w => w.Name == model.Name & w.SubjectGroupID == model.SubjectGroupID).FirstOrDefault();
            if (subject != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            subject = new Subject();
            subject.Create_On = DateUtil.Now();
            subject.Update_On = DateUtil.Now();
            subject.Create_By = model.Update_By;
            subject.Update_By = model.Update_By;
            subject.Status = model.Status;
            subject.Name = model.Name;
            subject.Description = model.Description;
            subject.SubjectGroupID = model.SubjectGroupID;
            subject.Order = model.Order;

            _context.Subjects.Add(subject);
            _context.SaveChanges();

            foreach (var group in _context.SubjectGroups)
            {
                var order = 1;
                foreach (var s in _context.Subjects.Where(w => w.SubjectGroupID == group.ID).OrderBy(o => o.Order))
                {
                    s.Order = order;
                    order++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<Subject>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var subject = _context.Subjects.Where(w => w.Name == model.Name & w.SubjectGroupID == model.SubjectGroupID & w.ID != model.ID).FirstOrDefault();
            if (subject != null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            subject = _context.Subjects.Where(w => w.ID == model.ID).FirstOrDefault();
            if (subject != null)
            {
                subject.Update_On = DateUtil.Now();
                subject.Update_By = model.Update_By;
                subject.Status = model.Status;
                subject.Name = model.Name;
                subject.Description = model.Description;
                subject.SubjectGroupID = model.SubjectGroupID;
                subject.Order = model.Order;
                _context.SaveChanges();

                foreach(var group in _context.SubjectGroups)
                {
                    var order = 1;
                    foreach(var s in _context.Subjects.Where(w=>w.SubjectGroupID == group.ID).OrderBy(o=>o.Order))
                    {
                        s.Order = order;
                        order++;
                    }
                }
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

            var subject = _context.Subjects.Where(w => w.ID == id).FirstOrDefault();
            if (subject == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var questions = _context.Questions.Where(w => w.SubjectID == subject.ID);
            if (questions.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            var subs = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID);
            if (subs.Count() > 0)
                _context.SubjectSubs.RemoveRange(subs);

            var examsetups = _context.ExamSetups.Where(w => w.SubjectID == subject.ID);
            if (examsetups.Count() > 0)
                _context.ExamSetups.RemoveRange(examsetups);

            var rtsetups = _context.SendResultSetups.Where(w => w.SubjectID == subject.ID);
            if (rtsetups.Count() > 0)
                _context.SendResultSetups.RemoveRange(rtsetups);


            _context.Subjects.Remove(subject);
            _context.SaveChanges();

            foreach (var group in _context.SubjectGroups)
            {
                var order = 1;
                foreach (var s in _context.Subjects.Where(w => w.SubjectGroupID == group.ID).OrderBy(o => o.Order))
                {
                    s.Order = order;
                    order++;
                }
            }
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("moveup")]
        public object moveup(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(moveup), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Subjects.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(moveup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
                      
            var latestindex = this._context.Subjects.Where(w => w.Order < model.Order & w.SubjectGroupID == model.SubjectGroupID).OrderByDescending(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.Subjects.Where(w => w.SubjectGroupID == model.SubjectGroupID).OrderBy(o => o.Order))
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
            return CreatedAtAction(nameof(moveup), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("movedown")]
        public object movedown(int? id, string update_by)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(movedown), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var model = _context.Subjects.Where(w => w.ID == id).FirstOrDefault();
            if (model == null)
                return CreatedAtAction(nameof(movedown), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

          
            var latestindex = this._context.Subjects.Where(w => w.Order > model.Order & w.SubjectGroupID == model.SubjectGroupID).OrderBy(o => o.Order).FirstOrDefault();
            var i = 1;
            foreach (var item in this._context.Subjects.Where(w => w.SubjectGroupID == model.SubjectGroupID).OrderBy(o => o.Order))
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
            return CreatedAtAction(nameof(movedown), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
