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
    public class AttitudeSetupController : ControllerBase
    {

        private readonly ILogger<AttitudeSetupController> _logger;
        public TuExamContext _context;

        public AttitudeSetupController(ILogger<AttitudeSetupController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
        [HttpGet]
        [Route("getattitudesetup")]
        public object getattitudesetup(string type_search, string subtype_search)
        {
            var type = type_search.toAttitudeAnsType();
            var stype = subtype_search.toAttitudeAnsSubType();
            var attitude = _context.AttitudeSetups.Where(w => w.AttitudeAnsType == type && w.AttitudeAnsSubType == stype).Select(s => new
            {
                result = ResultCode.Success,
                message = ResultMessage.Success,
                id = s.ID,
                typeName = s.AttitudeAnsType.toAttitudeAnsTypeName(),
                subtypeName = s.AttitudeAnsSubType.toAttitudeAnsSubType(),
                text1 = s.Text1,
                text2 = s.Text2,
                text3 = s.Text3,
                text4 = s.Text4,
                text5 = s.Text5,
                text6 = s.Text6,
                text7 = s.Text7,
                texten1 = s.TextEn1,
                texten2 = s.TextEn2,
                texten3 = s.TextEn3,
                texten4 = s.TextEn4,
                texten5 = s.TextEn5,
                texten6 = s.TextEn6,
                texten7 = s.TextEn7,
                description = s.Description,
                point1 = s.Point1,
                point2 = s.Point2,
                point3 = s.Point3,
                point4 = s.Point4,
                point5 = s.Point5,
                point6= s.Point6,
                point7 = s.Point7,
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).FirstOrDefault();

            if (attitude != null)
                return attitude;
            return CreatedAtAction(nameof(getattitudesetup), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }
             

        [HttpPost]
        [Route("modify")]
        public object modify([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<AttitudeSetup>(json.GetRawText());

            if (model == null)
                return CreatedAtAction(nameof(modify), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var attitude = _context.AttitudeSetups.Where(w=>w.AttitudeAnsType == model.AttitudeAnsType & w.AttitudeAnsSubType == model.AttitudeAnsSubType).FirstOrDefault();
            if (attitude == null)
            {
                model.Create_On = DateUtil.Now();
                model.Update_On = DateUtil.Now();
                model.Create_By = model.Update_By;
                _context.AttitudeSetups.Add(model);
                _context.SaveChanges();
            }
            else
            {
                model.Update_On = DateUtil.Now();
                model.ID = attitude.ID;
                _context.Entry(attitude).CurrentValues.SetValues(model);
                _context.SaveChanges();
            }
            
            return CreatedAtAction(nameof(modify), new { result = ResultCode.Success, message = ResultMessage.Success });
        }

    }
}
