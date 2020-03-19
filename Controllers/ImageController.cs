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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        public SystemConf _conf;
        private readonly ILogger<ImageController> _logger;
        public TuExamContext _context;

        public ImageController(ILogger<ImageController> logger, TuExamContext context, IOptions<SystemConf> conf)
        {
            this._logger = logger;
            this._context = context;
            this._conf = conf.Value;

        }

        [HttpGet]
        [Route("listimage")]
        public object listimage()
        {
            var images = _context.ImageFiles.OrderByDescending(o=>o.Update_On);
            var fileurl = this._conf.HostUrl + "\\images\\";
            fileurl = fileurl.Replace("\\", "/");

            return images.Select(s => new
            {
                id = s.ID,
                url = fileurl + s.ID + ".png",
                create_on = DateUtil.ToDisplayDateTime(s.Create_On),
                create_by = s.Create_By,
                update_on = DateUtil.ToDisplayDateTime(s.Update_On),
                update_by = s.Update_By,
            }).ToArray();
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadimage")]
        public object uploadimage([FromBody] JsonElement json)
        {
            var model = JsonConvert.DeserializeObject<ImageDTO>(json.GetRawText());
            if (!string.IsNullOrEmpty(model.image))
            {
                var imgfile = new ImageFile();
                imgfile.Create_On = DateUtil.Now();
                imgfile.Update_On = DateUtil.Now();
                imgfile.Create_By = model.update_by;
                imgfile.Update_By = model.update_by;
                _context.ImageFiles.Add(imgfile);
                _context.SaveChanges();

                var str = model.image.Replace("data:image/jpeg;base64,", "");
                str = str.Replace("data:image/png;base64,", "");
                byte[] bytes = Convert.FromBase64String(str);


                var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\" + imgfile.ID + ".png";


                var filename = filePath;
                filePath = filePath.Replace("\\", "/");

                Image img;
                var memory = new MemoryStream();
                using (var ms = new MemoryStream(bytes))
                {
                    img = Image.FromStream(ms);
                    img.Save(filePath, ImageFormat.Png);
                }
                return CreatedAtAction(nameof(uploadimage), new { result = ResultCode.Success, message = ResultMessage.Success, id= imgfile.ID });
            }

            return CreatedAtAction(nameof(uploadimage), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("delete")]
        public object delete(int? id)
        {
            if (!id.HasValue)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.InputHasNotFound, message = ResultMessage.InputHasNotFound });

            var img = _context.ImageFiles.Where(w => w.ID == id).FirstOrDefault();
            if (img == null)
                return CreatedAtAction(nameof(delete), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });

            _context.ImageFiles.Remove(img);
            _context.SaveChanges();
            return CreatedAtAction(nameof(delete), new { result = ResultCode.Success, message = ResultMessage.Success });
        }


    }
}
