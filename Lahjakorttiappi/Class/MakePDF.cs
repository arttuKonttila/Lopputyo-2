﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;*/
using System.IO;
using System.Data;
using System.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


namespace Lahjakorttiappi.Class

{
    public class MakePDF
    {
        //public Class.Asiakastiedot tiedot = new Class.Asiakastiedot();
        public Paaikkuna info = new Paaikkuna();
        public const String pdfDest = "/data/lahjakortit/lahjakortti.pdf";
        public const String logo = "data/image/logo.jpg";
        pdfInfoclass giftcardInfo = new pdfInfoclass();
        LahjakorttiTiedot parent = new LahjakorttiTiedot();
        

        public void Main(LahjakorttiTiedot kutsuvaLomake)
        {
            //parent = kutsuvaLomake;
            FileInfo file = new FileInfo(pdfDest);
            file.Directory.Create();
            new MakePDF().CreatePdf(pdfDest);
        }

       
        public virtual void CreatePdf (String pdfDest)
        {
            string pdfDestination = System.IO.Path.Combine(Environment.CurrentDirectory, "data/lahjakorttit/lahjakortti.pdf");
          
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "data/image/logo.jpg");
           
            string companyData = System.IO.Path.Combine(Environment.CurrentDirectory, "data/contact.xml");
            string customer = giftcardInfo.Firstname + " " + giftcardInfo.Lastname;
            string service =  "Teillä on hieronta lahjakortti:  "  + giftcardInfo.Duration +" minuutin hierontaa "+ giftcardInfo.Amount + " kertaa";
            string giftCardExDate = "Lahjakortti on voimassa " + giftcardInfo.ExDate + " saakka";
            string company = "", cmAddress = "", cmEmail = "", cmPhone = "", cmPostNum = "", cmPostState = "", cmWeb = "";
            DataSet read = new DataSet();
            read.ReadXml(companyData);

            foreach (DataRow dr in read.Tables[0].Rows)
            {
                company = dr["CompanyName"].ToString().Trim();
                cmAddress = dr  ["Address"].ToString().Trim();
                cmPostNum = dr["PostalNumber"].ToString().Trim();
                cmPostState = dr["PostalState"].ToString().Trim();
                cmPhone = dr["Phone"].ToString().Trim();
                cmEmail = dr["Email"].ToString().Trim();
                cmWeb = dr["WebSite"].ToString().Trim();
            }
            PdfDocument pdfTiedosto = new PdfDocument();
            PdfPage sivu = pdfTiedosto.Pages.Add();
            pdfTiedosto.PageSettings.Margins.All = 50;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
            sivu.Graphics.DrawString("Lahjakortti", font, PdfBrushes.Black,new PointF(100,10));
            PdfGraphics logoPiirto = sivu.Graphics;
            PdfBitmap logo = new PdfBitmap(path);
            logoPiirto.DrawImage(logo, 40, 20);
            sivu.Graphics.DrawString(customer, font, PdfBrushes.AliceBlue,new PointF(300,200));
            //sivu.Graphics.DrawString(service, font, PdfBrushes.Black,new PointF(400,200));
            //sivu.Graphics.DrawString(date, font, PdfBrushes.Black, new PointF(500, 200));
            sivu.Graphics.DrawString(company, font, PdfBrushes.Black, new PointF(600,200));
            pdfTiedosto.Save(pdfDestination);
            /*PdfDocument pdf = new PdfDocument(new PdfWriter(pdfDestination));

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            Document dokumentti = new Document(pdf, PageSize.A4.Rotate());

            //dokumentti.Add(background);
            Paragraph h1 = new Paragraph("LAHJAKORTTI").SetBold().SetFont(font).SetFontSize(35);
            Paragraph hello = new Paragraph("Hei " + customer).SetFont(font).SetFontSize(20);
            Paragraph body = new Paragraph("Sinulla on lahjakortti" + service).SetFont(font).SetFontSize(18);
            Paragraph valid = new Paragraph(date).SetFont(font).SetFontSize(16);
            Paragraph companyInfo = new Paragraph(company + "/n" + cmAddress + "/n" + cmPostNum + " " + cmPostState + "/n" + cmPhone + "/n" + cmEmail + "/n" + cmWeb).SetFont(font).SetFontSize(12);
            // Tryed to get the paragraph to be accessible.
            //h1.getAccessibilityProperties().setRole(StandardRoles.H1);
            dokumentti.Add(h1);
            dokumentti.Add(logo);
            dokumentti.Add(h1);
            dokumentti.Add(hello);
            dokumentti.Add(body);
            dokumentti.Add(valid);
            dokumentti.Add(companyInfo);
            dokumentti.Close();*/
        }
       
    }


    
}
