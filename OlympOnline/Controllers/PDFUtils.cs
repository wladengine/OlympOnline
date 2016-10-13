using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;

using iTextSharp.text;
using iTextSharp.text.pdf;
using OlympOnline.Models;

namespace OlympOnline.Controllers

{
    public static class PDFUtils
    {
        /// <summary>
        /// PDF Мотивационное письмо
        /// </summary>
        /// <param name="mailId"></param>
        /// <param name="fontspath"></param>
        /// <returns></returns>
        public static byte[] GetMotivateMail(string mailId, string fontspath)
        {
            Guid MailId;
            if (!Guid.TryParse(mailId, out MailId))
                return Encoding.Unicode.GetBytes("");

            MemoryStream ms = new MemoryStream();
                
            string query = "SELECT Surname, Name, SecondName, Phone, Mobiles, [User].Email, MotivationMail.MailText, Entry.ObrazProgramName FROM Person " +
                "INNER JOIN [Application] ON [Application].PersonId=Person.Id " +
                "INNER JOIN Entry ON [Application].EntryId=Entry.Id " +
                "INNER JOIN MotivationMail ON MotivationMail.ApplicationId=[Application].Id " +
                "INNER JOIN [User] ON [User].Id=Person.Id WHERE MotivationMail.Id=@MailId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@MailId", MailId } });
                
            if (tbl.Rows.Count == 0)
                return new byte[1] { 0x00 };

            string FIO = tbl.Rows[0].Field<string>("Surname") + " " + tbl.Rows[0].Field<string>("Name") + " " + tbl.Rows[0].Field<string>("SecondName");
            string phone = tbl.Rows[0].Field<string>("Email") + "\n" + tbl.Rows[0].Field<string>("Phone") + "\n" + tbl.Rows[0].Field<string>("Mobiles");
            string program = tbl.Rows[0].Field<string>("ObrazProgramName");
            string text = tbl.Rows[0].Field<string>("MailText");

            FIO = FIO.Trim();

            using (Document doc = new Document())
            {
                BaseFont baseFont = BaseFont.CreateFont(fontspath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font12 = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font font16 = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font font16U = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.UNDERLINE);
                iTextSharp.text.Font font16B = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();


                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 40f, 25f, 35f });

                //table.SetWidthPercentage(new float[] { 10f, 40f, 15f, 15f, 20f }, doc.PageSize);

                PdfPCell cell = new PdfPCell(new Phrase("Санкт-Петербургский Государственный Университет", font16));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(""));
                cell.Border = 0;
                table.AddCell(cell);

                Phrase ph = new Phrase();
                ph.Add(new Chunk("(ФИО) ", font12));
                ph.Add(new Chunk(FIO, font16U));
                cell = new PdfPCell(ph);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Приемная комиссия", font16));
                cell.HorizontalAlignment = Element.ALIGN_BOTTOM & Element.ALIGN_LEFT;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(""));
                cell.Border = 0;
                table.AddCell(cell);

                ph = new Phrase();
                ph.Add(new Chunk("e-mail, тел. ", font12));
                ph.Add(new Chunk(phone, font16U));
                cell = new PdfPCell(ph);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                table.AddCell(cell);

                doc.Add(table);

                Paragraph p = new Paragraph("Мотивационное письмо \n к заявлению на участие в конкурсе \n по магистерской программе:\n" + program, font16B);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);
                string[] paragraphs = text.Split('\n');
                foreach (string par in paragraphs)
                {
                    p = new Paragraph(par, font12);
                    p.FirstLineIndent = 30;
                    p.Alignment = Element.ALIGN_JUSTIFIED;
                    doc.Add(p);
                }

                p = new Paragraph("\nДата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), font16);
                doc.Add(p);

                p = new Paragraph("\nПодпись: ", font16);
                doc.Add(p);

                doc.Close();
            }

            return ms.ToArray();
        }

        /// <summary>
        /// PDF Список файлов
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="fontPath"></param>
        /// <returns></returns>
        public static byte[] GetFilesList(Guid personId, Guid applicationId, string fontPath)
        {
            MemoryStream ms = new MemoryStream();

            string query = "SELECT Surname, Name, SecondName FROM Person WHERE Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", personId } });

            if (tbl.Rows.Count == 0)
                return new byte[1] { 0x00 };

            var person =
                (from DataRow rw in tbl.Rows
                    select new {
                        Surname = rw.Field<string>("Surname"),
                        Name = rw.Field<string>("Name"),
                        SecondName = rw.Field<string>("SecondName") 
                    }).FirstOrDefault();

            string FIO = person.Surname + " " + person.Name + " " + person.SecondName;
            FIO = FIO.Trim();

            using (Document doc = new Document())
            {
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font12 = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font font16 = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.NORMAL);

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();

                Paragraph p = new Paragraph("ПРИЕМНАЯ КОМИССИЯ СПБГУ", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                p = new Paragraph("Опись \n поданных документов", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                p = new Paragraph(FIO + "\n\n", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 40f, 15f, 15f, 20f });
                //table.SetWidthPercentage(new float[] { 10f, 40f, 15f, 15f, 20f }, doc.PageSize);

                PdfPCell cell = new PdfPCell(new Phrase("№ п/п", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Наименование документа (имя файла)", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Копия / подлинник", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Дата подачи (загрузки)", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Подпись сотрудника ПК, принявшего документ при личной подаче", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                int cnt = 0;
                query = "SELECT FileName, Comment, LoadDate FROM PersonFile WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", personId } });
                var PersonFile =
                    from DataRow rw in tbl.Rows
                    select new
                    {
                        Comment = rw.Field<string>("Comment"),
                        FileName = rw.Field<string>("FileName"),
                        LoadDate = rw.Field<DateTime?>("LoadDate").HasValue ? rw.Field<DateTime>("LoadDate").ToShortDateString() : "нет" 
                    };
                query = "SELECT FileName, Comment, LoadDate FROM ApplicationFile WHERE ApplicationId=@AppId";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", applicationId } });
                var ApplicationFile =
                    from DataRow rw in tbl.Rows
                    select new
                    {
                        Comment = rw.Field<string>("Comment"),
                        FileName = rw.Field<string>("FileName"),
                        LoadDate = rw.Field<DateTime?>("LoadDate").HasValue ? rw.Field<DateTime>("LoadDate").ToShortDateString() : "нет"
                    };

                var AllFiles = ApplicationFile.Union(PersonFile);

                foreach (var file in AllFiles)
                {
                    ++cnt;
                    cell = new PdfPCell(new Phrase(cnt.ToString(), font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    table.AddCell(new Phrase(string.Format("{0} ({1})", file.Comment, file.FileName), font12));

                    cell = new PdfPCell(new Phrase("Копия", font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(file.LoadDate, font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    table.AddCell("");
                }

                for (int j = 0; j < 3; j++)
                {
                    ++cnt;
                    cell = new PdfPCell(new Phrase(cnt.ToString(), font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    for (int z = 0; z < 4; z++)
                    {
                        cell = new PdfPCell(new Phrase("", font12));
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);


                p = new Paragraph("Создано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), font12);
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);

                doc.Close();
            }

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF(Guid appId, string dirPath)
        {
            MemoryStream ms = new MemoryStream();

            string query = @"SELECT 
Surname, Name, SecondName, BirthDate, SchoolName, SchoolNum, 
SchoolClass, Code, extPerson.City, Street, House, Korpus, Flat, IsCountryside,
[Email], Phone, extPerson.Barcode, extApplication.City AS OlympCity, 
extApplication.Subject, extApplication.Date AS OlympDate,
extApplication.Stage, 
PassportSeries, PassportNumber, PassportDate, PassportTypeId, PassportAuthor,
ParentName, ParentAdress,
IsSirota, IsDisabled, 
Teacher
FROM extApplication 
INNER JOIN extPerson ON extPerson.Id = extApplication.PersonId 
LEFT JOIN Teachers ON extApplication.PersonId = Teachers.PersonId and extApplication.OlympiadId  = Teachers.OlympiadId
WHERE extApplication.Id=@Id";

            Dictionary<string, object> sl = new Dictionary<string, object>();
            sl.Add("@Id", appId);
            DataTable tbl = Util.AbitDB.GetDataTable(query, sl);
            if (tbl.Rows.Count == 0)
                return null;

            DataRow rw = tbl.Rows[0];

            var person = new
            {
                Surname = rw["Surname"].ToString(),
                Name = rw["Name"].ToString(),
                SecondName = rw["SecondName"].ToString(),
                BirthDate = rw.Field<DateTime>("BirthDate"),
                SchoolName = rw["SchoolName"].ToString(),
                SchoolNum = rw["SchoolNum"].ToString(),
                SchoolClass = rw["SchoolClass"].ToString(),
                Code = rw["Code"].ToString(),
                City = rw["City"].ToString(),
                Street = rw["Street"].ToString(),
                House = rw["House"].ToString(),
                Korpus = rw["Korpus"].ToString(),
                Flat = rw["Flat"].ToString(),
                Barcode = rw["Barcode"].ToString(),
                OlympCity = rw["OlympCity"].ToString(),
                Subject = rw["Subject"].ToString(),
                OlympDate = rw.Field<DateTime?>("OlympDate").HasValue ? rw.Field<DateTime>("OlympDate").ToShortDateString() : "",
                Stage = rw["Stage"].ToString(),
                Phone = rw["Phone"].ToString(),
                Email = rw["Email"].ToString(),
                PassportSeries = rw["PassportSeries"].ToString(),
                PassportNumber = rw["PassportNumber"].ToString(),
                PassportDate = rw.Field<DateTime>("PassportDate"),
                IsPassport = rw.Field<int>("PassportTypeId") != 4,
                PassportAuthor = rw.Field<string>("PassportAuthor"),
                IsCountryside = rw.Field<bool>("IsCountryside"),
                TeacherName = rw.Field<string>("Teacher"),
                ParentName = rw.Field<string>("ParentName"),
                ParentAdress = rw.Field<string>("ParentAdress"),
                IsSirota = rw.Field<bool>("IsSirota"),
                IsDisabled = rw.Field<bool>("IsDisabled")
            };

            string dotName = "Application_2016.pdf";

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "",
PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING |
PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;

            Barcode128 barcode = new Barcode128();
            barcode.Code = person.Barcode;

            PdfContentByte cb = pdfStm.GetOverContent(1);

            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            img.SetAbsolutePosition(485, 785);
            cb.AddImage(img);

            string Surname = person.Surname.ToUpper();//GetDoubleSpacedString(person.Surname.ToUpper());//person.Surname.ToUpper(); 
            string Name = person.Name.ToUpper();//; GetDoubleSpacedString(person.Name.ToUpper())
            string SecondName = person.SecondName.ToUpper(); //person.SecondName.ToUpper();//GetDoubleSpacedString(person.SecondName.ToUpper());

            for (int i = 0; i < Surname.Length; i++)
                acrFlds.SetField("Surname" + i.ToString(), Surname[i].ToString());
            for (int i = 0; i < Name.Length; i++)
                acrFlds.SetField("Name" + i.ToString() , Name[i].ToString());
            
            for (int i = 0; i < SecondName.Length; i++)
                acrFlds.SetField("SecondName" + i.ToString() , SecondName[i].ToString());

            string BirthDate_Day = person.BirthDate.Day.ToString("D2");// GetSpacedString(person.BirthDate.Day.ToString("D2"));
            string BirthDate_Year = person.BirthDate.Year.ToString();// GetSpacedString(person.BirthDate.Year.ToString());
            string BirthDate_Month = person.BirthDate.Month.ToString("D2");// GetSpacedString(person.BirthDate.Month.ToString("D2"));

            for (int i = 0; i < BirthDate_Day.Length; i++)
                acrFlds.SetField("BirthDate_Day" + i.ToString(), BirthDate_Day[i].ToString());
            for (int i = 0; i < BirthDate_Year.Length; i++)
                acrFlds.SetField("BirthDate_Year"+i.ToString(), BirthDate_Year[i].ToString());
            for (int i = 0; i < BirthDate_Month.Length; i++)
                acrFlds.SetField("BirthDate_Month" + i.ToString(), BirthDate_Month[i].ToString());

            string address = person.Code + (string.IsNullOrEmpty(person.Code) ? "" : " ") + person.City + " " + person.Street.Replace("улица", "ул.").Replace("проспект", "пр.")
                + " " + person.House + " " + person.Korpus + (string.IsNullOrEmpty(person.Korpus) ? "" : " кв. ") + person.Flat.Replace("квартира", "").Replace("кв", "");
            string[] splitStr = GetSplittedStrings(address, 75, 60, 2);
            for (int i = 1; i <= 2; i++)
                acrFlds.SetField("Address" + i, splitStr[i - 1]);

            acrFlds.SetField("ParentName", person.ParentName);
            splitStr = GetSplittedStrings(person.ParentAdress, 50, 70, 2);
            for (int i = 1; i <= 2; i++)
                acrFlds.SetField("ParentAdress" + i, splitStr[i - 1]);

            acrFlds.SetField("PersonName", Surname + " " + Name + " " + SecondName);
            splitStr = GetSplittedStrings(address, 50, 70, 2);
            for (int i = 1; i <= 2; i++)
                acrFlds.SetField("PersonAdress" + i, splitStr[i - 1]);

            acrFlds.SetField("SchoolName", person.SchoolName);
            acrFlds.SetField("SchoolNum", person.SchoolNum);
            acrFlds.SetField("SchoolClass", person.SchoolClass); 

            acrFlds.SetField("OlympSubject", person.OlympDate + " " + person.Subject);
            acrFlds.SetField("OlympCity", person.OlympCity);

            acrFlds.SetField("OlympSubject_1", person.Subject); // родительный падеж не помешал бы

            for (int i = 0; i < person.Phone.Length; i++)
                acrFlds.SetField("Phone" + i.ToString(), person.Phone[i].ToString());
            for (int i = 0; i < person.Email.Length; i++)
                 acrFlds.SetField("Email"+i.ToString(), person.Email[i].ToString());
            for (int i = 0; i < person.TeacherName.Length; i++)
                acrFlds.SetField("TeacherName" + i.ToString(), person.TeacherName[i].ToString().ToUpper());

            if (person.IsCountryside)
                acrFlds.SetField("chbIsCountryside", "1");

            if (person.IsSirota)
                acrFlds.SetField("chbIsSirota", "1");

            if (person.IsDisabled)
                acrFlds.SetField("chbIsDisabled", "1");

            if (person.IsPassport)
                acrFlds.SetField("chbIsPassport", "1");
            else
                acrFlds.SetField("chbIsSvidetelstvo", "1");

            for (int i = 0; i < person.PassportSeries.Length; i++)
                acrFlds.SetField("PassportSeria" + i.ToString(), person.PassportSeries[i].ToString());
            for (int i = 0; i < person.PassportNumber.Length; i++)
                acrFlds.SetField("PassportNumber" + i.ToString(), person.PassportNumber[i].ToString());
            acrFlds.SetField("PassportAuthor", "                   " + person.PassportAuthor + ", " + person.PassportDate.ToShortDateString());

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static string GetSpacedString(string source)
        {
            string s = "";
            foreach (char c in source.ToArray())
            {
                s += c.ToString() + " ";
            }
            return s.Trim();
        }

        public static string GetDoubleSpacedString(string source)
        {
            string s = "";
            int cntr = 0;
            foreach (char c in source.ToArray())
            {
                cntr++;
                if (cntr % 3 == 0)
                {
                    s += c.ToString() + " ";
                    cntr = 0;
                }
                else
                    s += c.ToString() + "  ";
            }
            return s.Trim();
        }
    

    public static string[] GetSplittedStrings(string sourceStr, int firstStrLen, int strLen, int numOfStrings)
        {
            sourceStr = sourceStr ?? "";
            string[] retStr = new string[numOfStrings];
            int index = 0, startindex = 0;
            for (int i = 0; i < numOfStrings; i++)
            {
                if (sourceStr.Length > startindex && startindex >= 0)
                {
                    int rowLength = firstStrLen;//длина первой строки
                    if (i > 1) //длина остальных строк одинакова
                        rowLength = strLen;
                    index = startindex + rowLength;
                    if (index < sourceStr.Length)
                    {
                        index = sourceStr.IndexOf(" ", index);
                        string val = index > 0 ? sourceStr.Substring(startindex, index - startindex) : sourceStr.Substring(startindex);
                        retStr[i] = val;
                    }
                    else
                        retStr[i] = sourceStr.Substring(startindex);
                }
                startindex = index;
            }

            return retStr;
        }
    }
}