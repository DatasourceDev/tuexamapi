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
    public class SubjectSubController : ControllerBase
    {

        private readonly ILogger<SubjectSubController> _logger;
        public TuExamContext _context;

        public SubjectSubController(ILogger<SubjectSubController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listsub")]
        public object listAllsub(string text_search, string status_search, string group_search, string subject_search, int pageno = 1)
        {
            var subjectsub = _context.SubjectSubs.Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => 1 == 1);
            if (!string.IsNullOrEmpty(status_search))
                subjectsub = subjectsub.Where(w => w.Status == status_search.toStatus());

            if (!string.IsNullOrEmpty(subject_search))
            {
                var subjectID = NumUtil.ParseInteger(subject_search);
                if (subjectID > 0)
                    subjectsub = subjectsub.Where(w => w.SubjectID == subjectID);
            }
            if (!string.IsNullOrEmpty(group_search))
            {
                var groupID = NumUtil.ParseInteger(group_search);
                if (groupID > 0)
                    subjectsub = subjectsub.Where(w => w.Subject.SubjectGroupID == groupID);
            }
            var subjectsubs = new List<SubjectSub>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        subjectsubs.AddRange(subjectsub.Where(w => w.Name.Contains(text)));
                    }
                }
                subjectsubs = subjectsubs.Distinct().ToList();
            }
            else
            {
                subjectsubs = subjectsub.ToList();
            }


            int skipRows = (pageno - 1) * 25;
            var itemcnt = subjectsubs.Count();
            var pagelen = itemcnt / 25;
            if (itemcnt % 25 > 0)
                pagelen += 1;
            return CreatedAtAction(nameof(listAllsub), new
            {
                data = subjectsubs.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    description = s.Description,
                    status = s.Status.toStatusName(),
                    group = s.Subject.SubjectGroup.Name,
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    index = s.Order,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o => o.subjectindex).ThenBy(o => o.index).Skip(skipRows).Take(25).ToArray(),
                pagelen = pagelen
            }); ;

        }

        [HttpGet]
        [Route("listActivesub")]
        public object listActivesub(string subject_search)
        {
            var subjectID = NumUtil.ParseInteger(subject_search);
            var subjectsub = _context.SubjectSubs.Where(w => w.SubjectID == subjectID).Include(i => i.Subject).Include(i => i.Subject.SubjectGroup).Where(w => w.Status == StatusType.Active);
            if (subjectsub != null)
                return subjectsub.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    status = s.Status.toStatusName(),
                    subject = s.Subject.Name,
                    subjectindex = s.Subject.Order,
                    index = s.Order,
                    group = s.Subject.SubjectGroup.Name,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.group).ThenBy(o => o.subjectindex).ThenBy(o => o.index).ToArray();
            return CreatedAtAction(nameof(listActivesub), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getsub")]
        public object getsub(int? id)
        {
            var sub = _context.SubjectSubs.Include(i => i.Subject).Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                name = s.Name,
                description = s.Description,
                status = s.Status,
                subjectid = s.SubjectID,
                groupid = s.Subject.SubjectGroupID,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (sub != null)
                return sub;
            return CreatedAtAction(nameof(getsub), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<SubjectSub>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var sub = _context.SubjectSubs.Where(w => w.Name == model.Name & w.SubjectID == model.SubjectID).FirstOrDefault();
            if (sub != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            sub = new SubjectSub();
            sub.Create_On = DateUtil.Now();
            sub.Update_On = DateUtil.Now();
            sub.Create_By = model.Update_By;
            sub.Update_By = model.Update_By;
            sub.Status = model.Status;
            sub.Name = model.Name;
            sub.Description = model.Description;
            sub.SubjectID = model.SubjectID;

            _context.SubjectSubs.Add(sub);
            _context.SaveChanges();
            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<SubjectSub>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var sub = _context.SubjectSubs.Where(w => w.Name == model.Name & w.SubjectID == model.SubjectID & w.ID != model.ID).FirstOrDefault();
            if (sub != null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            sub = _context.SubjectSubs.Where(w => w.ID == model.ID).FirstOrDefault();
            if (sub != null)
            {
                sub.Update_On = DateUtil.Now();
                sub.Update_By = model.Update_By;
                sub.Status = model.Status;
                sub.Name = model.Name;
                sub.Description = model.Description;
                sub.SubjectID = model.SubjectID;
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

            var subject = _context.SubjectSubs.Where(w => w.ID == id).FirstOrDefault();
            if (subject == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var questions = _context.Questions.Where(w => w.SubjectSubID == id);
            if (questions.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });

            _context.SubjectSubs.Remove(subject);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
