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
    public class SendResultSetupController : ControllerBase
    {

        private readonly ILogger<SendResultSetupController> _logger;
        public TuExamContext _context;

        public SendResultSetupController(ILogger<SendResultSetupController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listsendresultsetup")]
        public object listsendresultsetup()
        {
            var subjectuse = new List<int>();
            foreach (var group in _context.SubjectGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ID))
            {
                foreach (var subject in _context.Subjects.Where(w => w.Status == StatusType.Active & w.SubjectGroupID == group.ID).OrderBy(o => o.Order))
                {
                    subjectuse.Add(subject.ID);

                    var setup = _context.SendResultSetups.Where(w => w.SubjectGroupID == group.ID & w.SubjectID == subject.ID).FirstOrDefault();
                    if (setup == null)
                    {
                        setup = new SendResultSetup();
                        setup.SubjectGroupID = group.ID;
                        setup.SubjectID = subject.ID;
                        setup.Other = true;
                        setup.SendByEmail = true;
                        setup.SendByPost = true;
                        setup.Create_On = DateUtil.Now();
                        setup.Update_On = DateUtil.Now();
                        _context.SendResultSetups.Add(setup);
                    }


                }
            }
            _context.SaveChanges();

            var unuse = _context.SendResultSetups.Where(w => !subjectuse.Contains(w.SubjectID));
            if (unuse.Count() > 0)
                _context.SendResultSetups.RemoveRange(unuse);
            _context.SaveChanges();

            return _context.SendResultSetups.Select(s => new
            {
                id = s.ID,
                group = s.SubjectGroup.Name,
                groupid = s.SubjectGroupID,
                subject = s.Subject.Name,
                order =s.Subject.Order,
                subjectid = s.SubjectID,
                other = s.Other,
                sendbyemail = s.SendByEmail,
                sendbypost = s.SendByPost,
                description = s.Description,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.group).ThenBy(o => o.order).ToArray();
        }

        [HttpGet]
        [Route("getsendresultsetup")]
        public object getsendresultsetup(string group_searh, string subject_search)
        {
            var groupid = NumUtil.ParseInteger(group_searh);
            var subjectid = NumUtil.ParseInteger(subject_search);
            var setup = _context.SendResultSetups.Where(w => w.SubjectGroupID == groupid & w.SubjectID == subjectid).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                group = s.SubjectGroup.Name,
                groupid = s.SubjectGroupID,
                subject = s.Subject.Name,
                subjectid = s.SubjectID,
                other = s.Other,
                sendbyemail = s.SendByEmail,
                sendbypost = s.SendByPost,
                description = s.Description,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,

            }).FirstOrDefault();

            if (setup != null)
                return setup;
            return CreatedAtAction(nameof(getsendresultsetup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("modify")]
        public object modify([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<SendResultSetup[]>(json.GetRawText());
            if (model != null)
            {
                foreach (var item in model)
                {
                    var setup = _context.SendResultSetups.Where(w => w.ID == item.ID).FirstOrDefault();
                    if(setup != null)
                    {
                        setup.Other = item.Other;
                        setup.SendByEmail = item.SendByEmail;
                        setup.SendByPost = item.SendByPost;
                        setup.Description = item.Description;
                        setup.Update_On = DateUtil.Now();
                        setup.Update_By = item.Update_By;
                    }
                }
                _context.SaveChanges();
            }
            return CreatedAtAction(nameof(modify), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });

        }

    }
}
