using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.Models;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        public TuExamContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet]
        public IEnumerable<object> Get()
        {
           // var sub = new SubjectGroup();
           // sub.Name = "Test";
           // sub.Status = StatusType.Active;
           // _context.SubjectGroups.Add(sub);
           // _context.SaveChanges();

           //var list = _context.SubjectGroups;

            return Summaries.ToArray();
        }
    }
}
