using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

using System.Runtime.InteropServices;

using System.Data;
//using System.Drawing;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.pdf.draw;

public class HTMLWorkerExtended : HTMLWorker
{
   LineSeparator line = new LineSeparator(4f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1);
   public HTMLWorkerExtended(IDocListener document)
      : base(document)
   {

   }
   public override void StartElement(string tag, IDictionary<string, string> str)
   {
      if (tag.Equals("hrline"))
         document.Add(new Chunk(line));
      else
         base.StartElement(tag, str);
   }
}

public class PdfPageEvent : iTextSharp.text.pdf.PdfPageEventHelper
{
   // This is the contentbyte object of the writer
   PdfContentByte cb;

   // we will put the final number of pages in a template
   PdfTemplate template;

   // this is the BaseFont we are going to use for the header / footer
   BaseFont bf = null;

   // this is the font size page number footer
   // Added by sun 27-06-2016
   float fontsizepagenumber = 8;

   // This keeps track of the creation time
   public DateTime PrintTime { get; set; }
   public byte[] Logoleft { get; set; }
   public string Title { get; set; }
   public string Company_Name { get; set; }
   public string HeaderLeft { get; set; }
   public Font HeaderFont { get; set; }
   public Font FooterFont { get; set; }
   public string Footer1 { get; set; }
   public string Footer2 { get; set; }
   public byte[] LogoLeftOnFirstPage { get; set; }
   public string CountryName { get; set; }
   public List<string> Header { get; set; }
   private iTextSharp.text.Font boldFont { get; set; }
   private iTextSharp.text.Font normalFont { get; set; }
   public iTextSharp.text.Image jpg { get; set; }
   public iTextSharp.text.Image jpg2 { get; set; }
    public iTextSharp.text.Image jpg3 { get; set; }

    public Font fontNormal { get; set; }
   public Font fontH1 { get; set; }
   public Font fontH3 { get; set; }

   public string webRootPath { get; set; }
   public string bgimg { get; set; }
   public byte[] bgImgInByte { get; set; }
    public byte[] logoimg { get; set; }
   public byte[] orglogoimg { get; set; }
    public float logox { get; set; }
   public float logoy { get; set; }
   public float logwidht { get; set; }
   public float logoheight { get; set; }

    public override void OnOpenDocument(PdfWriter writer, Document document)
    {
        base.OnOpenDocument(writer, document);
        try
        {
            //var currentdate = StoredProcedure.GetCurrentDate();
            //if (PrintTime == null)
            //{
            //    PrintTime = currentdate;
            //}

            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);


            //BaseFont source_sans_pro = BaseFont.CreateFont(webRootPath + @"\fonts\source_sans_pro\" + "SourceSansPro-Regular.ttf", BaseFont.CP1250, BaseFont.EMBEDDED);
            fontNormal = new Font(bf, (float)9.8, Font.NORMAL, new BaseColor(64, 64, 65));
            fontH1 = new Font(bf, 13, Font.NORMAL, new BaseColor(64, 64, 65));
            fontH3 = new Font(bf, 15, Font.NORMAL, new BaseColor(64, 64, 65));

            var pageSize = document.PageSize;
            //string imageFilePath = webRootPath + @"\assets\certificate\" + bgimg.Replace("-", "_");
            //jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
            if (bgImgInByte != null)
            {
                jpg = iTextSharp.text.Image.GetInstance(bgImgInByte);
                jpg.Alignment = iTextSharp.text.Image.ALIGN_TOP;
                jpg.ScaleToFit(pageSize.Width, pageSize.Height);
                jpg.SetAbsolutePosition(0, 0);
            }

            if (logoimg != null)
            {
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(logoimg);
                jpg2 = image;
                jpg2.Alignment = iTextSharp.text.Image.ALIGN_TOP;
                jpg2.ScaleToFit(60, 60);
                jpg2.SetAbsolutePosition(500, 55);
            }

            //if (orglogoimg != null)
            //{
            //   iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(orglogoimg);
            //   jpg3 = image;
            //   jpg3.Alignment = iTextSharp.text.Image.ALIGN_TOP;
            //   jpg3.ScaleToFit(60, 60);
            //   jpg3.SetAbsolutePosition(700, 55);
            //}

            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }
        catch
        {
        }
    }

   public override void OnStartPage(PdfWriter writer, Document document)
   {
      base.OnStartPage(writer, document);
      var pageSize = document.PageSize;
      Font fontNormal = new Font(bf, 7, Font.NORMAL);
      Font fontH3 = new Font(bf, 13, Font.NORMAL);
      Font fontH1 = new Font(bf, 15, Font.BOLD);

      var table = new PdfPTable(3)
      {
         TotalWidth = pageSize.Width - 80,
         LockedWidth = true
      };
      table.SetWidthPercentage(new float[] { (float)(table.TotalWidth * 0.25), (float)(table.TotalWidth * 0.40), (float)(table.TotalWidth * 0.35) }, pageSize);

      var nested = new PdfPTable(1);

      PdfPCell nesthousing = new PdfPCell(nested);
      nesthousing.Border = 0;
      nesthousing.Padding = 0f;
      nesthousing.PaddingBottom = 20f;
      table.AddCell(nesthousing);

      var tcell = new PdfPCell(new Phrase(Title, fontH1));
      tcell.Border = 0;
      tcell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
      tcell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
      //tcell.PaddingBottom = 20f;
      table.AddCell(tcell);
      document.Add(table);
      if (jpg != null)
      {
         Paragraph paragraph = new Paragraph("");
         document.Add(jpg);
         document.Add(paragraph);
      }
      if (jpg2 != null)
      {
         Paragraph paragraph = new Paragraph("");
         document.Add(jpg2);
         document.Add(paragraph);
      }
        if (jpg3 != null)
        {
            Paragraph paragraph = new Paragraph("");
            document.Add(jpg3);
            document.Add(paragraph);
        }
    }

   // write on end of each page
   public override void OnEndPage(PdfWriter writer, Document document)
   {

   }

   //write on close of document
   public override void OnCloseDocument(PdfWriter writer, Document document)
   {
      base.OnCloseDocument(writer, document);
      //template.BeginText();
      //template.SetFontAndSize(bf, fontsizepagenumber);
      //template.SetTextMatrix(0, 0);
      //template.ShowText("" + (writer.PageNumber));
      //template.EndText();


   }
}