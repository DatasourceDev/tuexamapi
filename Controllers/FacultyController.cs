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
    public class FacultyController : ControllerBase
    {

        private readonly ILogger<FacultyController> _logger;
        public TuExamContext _context;

        public FacultyController(ILogger<FacultyController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }
      

        [HttpGet]
        [Route("listActivefaculty")]
        public object listActivefaculty()
        {
            var facultys = _context.Facultys;
            if (facultys != null)
                return facultys.Select(s => new
                {
                    id = s.ID,
                    name = s.FacultyName,
                }).OrderBy(o => o.name).ToArray();
            return CreatedAtAction(nameof(listActivefaculty), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }
     
    }
}
