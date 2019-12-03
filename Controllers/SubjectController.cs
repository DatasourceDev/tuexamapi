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
            if (!string.IsNullOrEmpty(text_search))
                subject = subject.Where(w => w.Name.Contains(text_search));
            if (!string.IsNullOrEmpty(status_search))
                subject = subject.Where(w => w.Status == status_search.toStatus());
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if(groupID > 0)
                    subject = subject.Where(w => w.SubjectGroupID == groupID);
            }
            return subject.Select(s => new
            {
                id = s.ID,
                name = s.Name,
                index = s.Index,
                status = s.Status.toStatusName(),
                group = s.SubjectGroup.Name,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.group).ThenBy(o2=>o2.index).ToArray();
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
                    index = s.Index,
                    status = s.Status.toStatusName(),
                    group = s.SubjectGroup.Name,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o2 => o2.index).ToArray();
            return CreatedAtAction(nameof(listActivesubject), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getsubject")]
        public object getsubject(int? id)
        {
            var group = _context.Subjects.Where(w => w.ID == id).Select(s => new
            {
                id = s.ID,
                name = s.Name,
                index = s.Index,
                status = s.Status,
                groupid = s.SubjectGroupID,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (group != null)
                return group;
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
            subject.SubjectGroupID = model.SubjectGroupID;
            subject.Index = model.Index;

            _context.Subjects.Add(subject);
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
                subject.SubjectGroupID = model.SubjectGroupID;
                subject.Index = model.Index;
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


            

            _context.Subjects.Remove(subject);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
