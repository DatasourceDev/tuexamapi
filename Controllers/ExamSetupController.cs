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
    public class ExamSetupController : ControllerBase
    {

        private readonly ILogger<ExamSetupController> _logger;
        public TuExamContext _context;

        public ExamSetupController(ILogger<ExamSetupController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("listexamsetup")]
        public object listexamsetup()
        {
            var examperiods = new List<ExamPeriod>();
            examperiods.Add(ExamPeriod.Morning);
            examperiods.Add(ExamPeriod.Afternoon);
            examperiods.Add(ExamPeriod.Evening);
            var subjectuse = new List<int>();
            foreach (var group in _context.SubjectGroups.Where(w => w.Status == StatusType.Active).OrderBy(o => o.ID))
            {
                foreach (var subject in _context.Subjects.Where(w => w.Status == StatusType.Active & w.SubjectGroupID == group.ID).OrderBy(o => o.Order))
                {
                    subjectuse.Add(subject.ID);
                    foreach (var period in examperiods)
                    {
                        var setup = _context.ExamSetups.Where(w => w.SubjectGroupID == group.ID & w.SubjectID == subject.ID & w.ExamPeriod == period).FirstOrDefault();
                        if (setup == null)
                        {
                            setup = new ExamSetup();
                            setup.ExamPeriod = period;
                            setup.SubjectGroupID = group.ID;
                            setup.SubjectID = subject.ID;
                            setup.choosed = false;
                            setup.Create_On = DateUtil.Now();
                            setup.Update_On = DateUtil.Now();
                            _context.ExamSetups.Add(setup);
                        }
                    }

                }
            }
            _context.SaveChanges();

            var unuse = _context.ExamSetups.Where(w => !subjectuse.Contains(w.SubjectID));
            if (unuse.Count() > 0)
                _context.ExamSetups.RemoveRange(unuse);
            _context.SaveChanges();

            return _context.ExamSetups.Select(s => new
            {
                id = s.ID,
                examperiod = s.ExamPeriod,
                examperiodname = s.ExamPeriod.toExamPeriodName(),
                choosed = s.choosed,
                group = s.SubjectGroup.Name,
                subject = s.Subject.Name,
                subjectorder = s.Subject.Order,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).OrderBy(o => o.examperiod).ThenBy(o => o.group).ThenBy(o => o.subjectorder).ToArray();
        }


        [HttpGet]
        [Route("dailyexamsetup")]
        public object dailyexamsetup(string date, string update_by)
        {
            var curdate = DateUtil.Now();
            if (!string.IsNullOrEmpty(date))
            {
                var d = DateUtil.ToDate(date);
                if(d.HasValue)
                    curdate = d.Value;
            }
            var setups = _context.ExamSetups.Where(w => w.choosed == true & w.SubjectGroup.Status == StatusType.Active & w.Subject.Status == StatusType.Active);
            foreach(var setup in setups)
            {
                var exam = _context.Exams
                    .Where(w => w.SubjectGroupID == setup.SubjectGroupID 
                    & w.SubjectID == setup.SubjectID 
                    & w.ExamPeriod == setup.ExamPeriod
                    & w.ExamDate.Value.Date == curdate.Date)
                    .FirstOrDefault();
                if(exam == null)
                {
                    var test = _context.Tests.Where(w => w.SubjectGroupID == setup.SubjectGroupID && w.SubjectID == setup.SubjectID & w.Status == StatusType.Active & w.ApprovalStatus == TestApprovalType.Approved).FirstOrDefault();
                    if (test == null)
                        continue;
                    exam = new Exam();
                    exam.Create_On = DateUtil.Now();
                    exam.Update_On = DateUtil.Now();
                    exam.Update_By = update_by;
                    exam.Create_By = update_by;
                    exam.ExamDate = curdate;
                    exam.ExamPeriod = setup.ExamPeriod;
                    exam.ExamTestType = ExamTestType.Random;
                    exam.SubjectGroupID = setup.SubjectGroupID;
                    exam.SubjectID = setup.SubjectID;
                    //exam.TestID = model.TestID;
                    _context.Exams.Add(exam);
                }
            }
            _context.SaveChanges();

            return CreatedAtAction(nameof(dailyexamsetup), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

        [HttpGet]
        [Route("choose")]
        public object choose(string choose, string update_by)
        {
            var chs = new List<string>();
            if (choose != null)
            {
                chs = choose.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            foreach (var setup in _context.ExamSetups)
            {
                setup.Update_On = DateUtil.Now();
                setup.Update_By = update_by;
                if (chs.Contains(setup.ID.ToString()))
                    setup.choosed = true;
                else
                    setup.choosed = false;
            }           
            _context.SaveChanges();
            return CreatedAtAction(nameof(choose), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

    }
}
