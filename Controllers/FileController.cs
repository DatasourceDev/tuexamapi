using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tuexamapi.DAL;
using tuexamapi.Models;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using tuexamapi.Util;
using OfficeOpenXml;
using static tuexamapi.Controllers.ReportController;
using System.Text.Json;
using Newtonsoft.Json;
using tuexamapi.DTO;
using Microsoft.Extensions.Options;

namespace tuexamapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<FileController> _logger;
        public TuExamContext _context;
        public SystemConf _conf;

        public FileController(ILogger<FileController> logger, TuExamContext context, IHostingEnvironment hostingEnvironment, IOptions<SystemConf> conf)
        {
            this._logger = logger;
            this._context = context;
            this._hostingEnvironment = hostingEnvironment;
            this._conf = conf.Value;

        }

        [HttpGet]
        [Route("getfile")]
        public async Task<object> getfile(int? questionid)
        {
            var question = _context.Questions.Include(i => i.Subject).Where(w => w.ID == questionid).FirstOrDefault();
            if (question != null)
            {
                //var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\questions\\" + question.ID + "\\";
                var filename = question.FileName;

                if (System.IO.File.Exists(filename))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var mimeType = "audio/mp3";
                    return File(memory, mimeType, Path.GetFileName(filename));
                }
            }
            return CreatedAtAction(nameof(getfile), new { result = ResultCode.DataHasNotFound, message = ResultMessage.DataHasNotFound });
        }

        [HttpGet]
        [Route("examstudentlistpdf")]
        public void examstudentlistpdf(int id)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;

            this.HttpContext.Response.ContentType = "application/pdf";

            var pdfDoc = new iTextSharp.text.Document(PageSize.A4, 0f, 0f, 40f, 40f);
            var htmlparser = new HTMLWorker(pdfDoc);
            var writer = PdfWriter.GetInstance(pdfDoc, this.HttpContext.Response.Body);

            PdfPageEvent pageevent = new PdfPageEvent();
            pageevent.PrintTime = DateTime.Now;
            pageevent.webRootPath = webRootPath;

            writer.PageEvent = pageevent;
            var pageSize = pdfDoc.PageSize;
            var THSarabunNew = BaseFont.CreateFont(webRootPath + @"\fonts\THSarabunNew.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(THSarabunNew, 14);
            pdfDoc.Open();

            PdfPTable table = new PdfPTable(5);
            var totalwidth = pdfDoc.PageSize.Width - 80f;
            table.SetWidthPercentage(new float[] {
                (float)(0.2 * totalwidth) ,
                (float)(0.4 * totalwidth) ,
                (float)(0.2 * totalwidth) ,
                (float)(0.05 * totalwidth) ,
                (float)(0.15 * totalwidth)
            }, pdfDoc.PageSize);
            var cell = new PdfPCell(new Phrase(12, "รหัสนักศึกษา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ชื่อ-นามสกุล", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วันเวลาที่ลงทะเบียน", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "วิชา", font));
            cell.Padding = 5;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(12, "ลายเซ็น", font));
            cell.Padding = 5;
            table.AddCell(cell);

            foreach (var student in _context.ExamRegisters.Include(i => i.Student).Include(i => i.Exam.Subject).Where(w => w.ExamID == id).OrderBy(o => o.Student.StudentCode))
            {
                cell = new PdfPCell(new Phrase(12, student.Student.StudentCode, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, student.Student.Prefix.toPrefixName() + " " + student.Student.FirstName + " " + student.Student.LastName, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, DateUtil.ToDisplayDateTime(student.Create_On), font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, student.Exam.Subject.Name, font));
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(12, "", font));
                cell.Padding = 5;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            pdfDoc.Close();


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
