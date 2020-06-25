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

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectGroupController : ControllerBase
    {

        private readonly ILogger<SubjectGroupController> _logger;
        public TuExamContext _context;

        public SubjectGroupController(ILogger<SubjectGroupController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listgroup")]
        public object listAllgroup(string text_search, string status_search)
        {
            var group = _context.SubjectGroups.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(status_search))
                group = group.Where(w => w.Status == status_search.toStatus());

            var groups = new List<SubjectGroup>();
            if (!string.IsNullOrEmpty(text_search))
            {
                var text_splits = text_search.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var text_split in text_splits)
                {
                    if (!string.IsNullOrEmpty(text_split))
                    {
                        var text = text_split.Trim();
                        groups.AddRange(group.Where(w => w.Name.Contains(text)));
                    }
                }
                groups = groups.Distinct().ToList();
            }
            else
            {
                groups = group.ToList();
            }

            return groups.Select(s => new
            {
                id = s.ID,
                name = s.Name,
                status = s.Status.toStatusName(),
                doexamorder = s.DoExamOrder,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.name).ToArray();
        }

        [HttpGet]
        [Route("listActivegroup")]
        public object listActivegroup(bool nogreats)
        {
            var group = _context.SubjectGroups.Where(w => w.Status == StatusType.Active);
            if(nogreats == true)
            {
                group = group.Where(w => w.Name != "GREATS");
            }
            if (group != null)
                return group.Select(s => new
                {
                    id = s.ID,
                    name = s.Name,
                    status = s.Status.toStatusName(),
                    doexamorder = s.DoExamOrder,
                    create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                    create_by = s.Create_By,
                    update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                    update_by = s.Update_By,
                }).OrderBy(o => o.name).ToArray();
            return CreatedAtAction(nameof(listActivegroup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("getgroup")]
        public object getgroup(int? id)
        {
            var group = _context.SubjectGroups.Where(w => w.ID == id).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                name = s.Name,
                status = s.Status,
                color1 = s.Color1,
                color2 = s.Color2,
                color3 = s.Color3,
                doexamorder = s.DoExamOrder,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (group != null)
                return group;
            return CreatedAtAction(nameof(getgroup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("insert")]
        public object insert([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<SubjectGroup>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var group = _context.SubjectGroups.Where(w => w.Name == model.Name).FirstOrDefault();
            if (group != null)
                return CreatedAtAction(nameof(insert), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            group = new SubjectGroup();
            group.Color1 = model.Color1;
            group.Color2 = model.Color2;
            group.Color3 = model.Color3;
            group.Create_On = DateUtil.Now();
            group.Update_On = DateUtil.Now();
            group.Create_By = model.Update_By;
            group.Update_By = model.Update_By;
            group.Status = model.Status;
            group.Name = model.Name;
            group.DoExamOrder = model.DoExamOrder;

            _context.SubjectGroups.Add(group);
            _context.SaveChanges();
            return CreatedAtAction(nameof(insert), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpPost]
        [Route("update")]
        public object update([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<SubjectGroup>(json.GetRawText());
            if (model == null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var group = _context.SubjectGroups.Where(w => w.Name == model.Name & w.ID != model.ID).FirstOrDefault();
            if (group != null)
                return CreatedAtAction(nameof(update), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            group = _context.SubjectGroups.Where(w => w.ID == model.ID).FirstOrDefault();
            if (group != null)
            {
                group.Update_On = DateUtil.Now();
                group.Update_By = model.Update_By;
                group.Status = model.Status;
                group.Name = model.Name;
                group.Color1 = model.Color1;
                group.Color2 = model.Color2;
                group.Color3 = model.Color3;
                group.DoExamOrder = model.DoExamOrder;

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

            var group = _context.SubjectGroups.Where(w => w.ID == id).FirstOrDefault();
            if (group == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            var questions = _context.Questions.Where(w => w.SubjectGroupID == id);
            if (questions.Count() > 0)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataInUse, message = ResultMessage.DataInUse });


            var subjects = _context.Subjects.Where(w => w.SubjectGroupID == id);
            foreach(var subject in subjects)
            {
                var subs = _context.SubjectSubs.Where(w => w.SubjectID == subject.ID);
                if (subs.Count() > 0)
                    _context.SubjectSubs.RemoveRange(subs);

                var examsetups = _context.ExamSetups.Where(w => w.SubjectID == subject.ID);
                if (examsetups.Count() > 0)
                    _context.ExamSetups.RemoveRange(examsetups);
            }

            var gexamsetups = _context.ExamSetups.Where(w => w.SubjectGroupID == id);
            if (gexamsetups.Count() > 0)
                _context.ExamSetups.RemoveRange(gexamsetups);

            if (subjects.Count() > 0)
                _context.Subjects.RemoveRange(subjects);


            _context.SubjectGroups.Remove(group);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
