using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.Models;
using tuexamapi.Util;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;
        public TuExamContext _context;

        public AccountController(ILogger<AccountController> logger, TuExamContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet]
        [Route("api/login")]
        public object login(string username, string password)
        {
            var user = _context.Users.Where(w => w.UserName == username).FirstOrDefault();
            if (user != null)
                return user;
            return CreatedAtAction(nameof(register), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpPost]
        [Route("api/register")]
        public object register(User model)
        {
            if (model == null)
                return CreatedAtAction(nameof(register), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var user = _context.Users.Where(w => w.UserName == model.UserName).FirstOrDefault();
            if (user != null)
                return CreatedAtAction(nameof(register), new { result = ResultCode.DuplicateData, message = ResultMessage.DuplicateData });

            if (ModelState.IsValid)
            {
                model.Password = DataEncryptor.Encrypt(model.Password);
                model.ConfirmPassword = DataEncryptor.Encrypt(model.ConfirmPassword);
                model.Create_On = DateUtil.Now();
                model.Create_By = model.UserName;
                model.Update_On = DateUtil.Now();
                model.Update_By = model.UserName;

                _context.Users.Add(model);
                _context.SaveChanges();
                return CreatedAtAction(nameof(register), new { result = ResultCode.Success, message = ResultMessage.Success });
            }

            return CreatedAtAction(nameof(register), new { result = ResultCode.InvalidInput, message = ResultMessage.InvalidInput });
        }

        [HttpPost]
        [Route("api/delete")]
        public object delete(User model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName))
                return CreatedAtAction(nameof(register), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var user = _context.Users.Where(w => w.UserName == model.UserName).FirstOrDefault();
            if (user == null)
                return CreatedAtAction(nameof(register), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            _context.Users.Remove(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(register), new { result = ResultCode.Success, message = ResultMessage.Success });
        }
    }
}
