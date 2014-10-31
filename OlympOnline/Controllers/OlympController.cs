using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OlympOnline.Models;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace OlympOnline.Controllers
{
    public class OlympController : Controller
    {
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication(params string[] errors)
        {
            //if (Util.bUseRedirection && Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 &&
            //    Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
            //    return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            if (errors != null && errors.Length > 0)
            {
                foreach (string er in errors)
                    ModelState.AddModelError("", er);
            }
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            ApplicationModel model = new ApplicationModel();
            
            //string query = "SELECT DISTINCT Id, Name FROM City ORDER BY Name";
            //DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            //model.Cities =
            //    (from DataRow rw in tbl.Rows
            //     select new
            //     {
            //         Value = rw.Field<int>("Id"),
            //         Text = rw.Field<string>("Name")
            //     }).AsEnumerable()
            //    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
            //    .ToList();

            string query = "SELECT DISTINCT SubjectId, Subject FROM extOlympiadInternet WHERE Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND Year=2013 ORDER BY Subject";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                model.Subjects =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("SubjectId"),
                         Text = rw.Field<string>("Subject")
                     }).AsEnumerable()
                     .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                     .ToList();
            }
            catch {  }
            
            return View("NewApplication", model);
        }

        [HttpPost]
        public ActionResult NewApp()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            string sCityId = Request.Form["hCity"];
            string sSubjectId = Request.Form["Subject"];
            string sOlympFormId = Request.Form["hOlympForm"];
            string sSchoolClassInterval = Request.Form["hClass"];
            //string sStageId = Request.Form["Stage"];
            //string sDate = Request.Form["Date"];

            //DateTime dtDate;
            //DateTime.TryParse(sDate, out dtDate);

            int iCityId = Util.ParseSafe(sCityId);
            int iSubjectId = Util.ParseSafe(sSubjectId);
            int iOlympFormId = Util.ParseSafe(sOlympFormId);
            int iSchoolClassInterval = Util.ParseSafe(sSchoolClassInterval);
            //int iStageId = Util.ParseSafe(sStageId);

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT Id FROM Olympiad WHERE SubjectId=@SubjectId AND Year=2013 AND StageId=@StageId AND IsOpen=1 ";// AND CityId=@CityId AND [Date]=@Date";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@StageId", 1);
            
            if (iOlympFormId != 0)
            {
                query += " AND OlympFormId=@OlympFormId";
                dic.Add("@OlympFormId", iOlympFormId);
            }

            if (iSchoolClassInterval > 0)
            {
                query += " AND SchoolClassIntervalId=@SchoolClassIntervalId ";
                dic.Add("@SchoolClassIntervalId", iSchoolClassInterval);
            }

            if (iCityId > 0)
            {
                query += " AND CityId=@CityId ";
                dic.Add("@CityId", iCityId);
            }

            //dic.Add("@Date", dtDate);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count > 1)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Неоднозначный выбор олимпиады (" + tbl.Rows.Count + ")" } });
            if (tbl.Rows.Count == 0)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Не найдена олимпиада" } });

            int OlympiadId = tbl.Rows[0].Field<int>("Id");

            query = "SELECT DateOfClose FROM Olympiad WHERE Id=@Id";
            DateTime DateOfClose = (DateTime?)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", OlympiadId } }) ?? DateTime.Now.AddYears(2);

            if (DateTime.Now > DateOfClose)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на данное направление прекращена " + DateOfClose.ToString("dd.MM.yyyy") } });

            query = "SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND OlympiadId IS NOT NULL";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            var eIds =
                from DataRow rw in tbl.Rows
                select rw.Field<int>("OlympiadId");
            if (eIds.Contains(OlympiadId))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

            //список заявлений на олимпиады есть, теперь проверим, нет ли в списке занятых олимпиад из других городов
            dic.Clear();
            query = "SELECT Id FROM extOlympiad WHERE SubjectId=@SubjectId AND Year=2013 ";
            dic.Add("@SubjectId", iSubjectId);
            if (iOlympFormId != 0)
            {
                query += " AND OlympFormId=@OlympFormId";
                dic.Add("@OlympFormId", iOlympFormId);
            }
            if (iSchoolClassInterval > 0)
            {
                query += " AND SchoolClassIntervalId=@SchoolClassIntervalId ";
                dic.Add("@SchoolClassIntervalId", iSchoolClassInterval);
            }
            if (iCityId > 0)
            {
                query += " AND CityId=@CityId ";
                dic.Add("@CityId", iCityId);
            }
            tbl = Util.AbitDB.GetDataTable(query, dic);
            var AllOlymps = from DataRow rw in tbl.Rows
                            select rw.Field<int>("Id");
            if (AllOlymps.Intersect(eIds).Count() > 0)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Вы уже зарегистрировались на олимпиаду в другом месте!" } });

            //список заявлений на олимпиады есть, теперь проверим, нет ли в списке занятых олимпиад в других формах проведения
            dic.Clear();
            query = "SELECT Id FROM extOlympiad WHERE SubjectId=@SubjectId AND Year=2013 ";
            dic.Add("@SubjectId", iSubjectId);
            if (iSchoolClassInterval > 0)
            {
                query += " AND SchoolClassIntervalId=@SchoolClassIntervalId ";
                dic.Add("@SchoolClassIntervalId", iSchoolClassInterval);
            }
            if (iCityId > 0)
            {
                query += " AND CityId=@CityId ";
                dic.Add("@CityId", iCityId);
            }
            tbl = Util.AbitDB.GetDataTable(query, dic);
            AllOlymps = from DataRow rw in tbl.Rows
                            select rw.Field<int>("Id");
            if (AllOlymps.Intersect(eIds).Count() > 0)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Вы уже зарегистрировались на олимпиаду, проводимую в другой форме!" } });

            Guid appId = Guid.NewGuid();
            query = "INSERT INTO [Application] (Id, PersonId, OlympiadId, Enabled, DateOfStart) " +
                "VALUES (@Id, @PersonId, @OlympiadId, @Enabled, @DateOfStart)";
            Dictionary<string, object> prms = new Dictionary<string, object>()
            {
                { "@Id", appId },
                { "@PersonId", PersonId },
                { "@OlympiadId", OlympiadId },
                { "@Enabled", true },
                { "@DateOfStart", DateTime.Now }
            };

            Util.AbitDB.ExecuteQuery(query, prms);

            query = "SELECT [BlackBoardCode] FROM [extOlympiadInternet] WHERE Id=@Id";
            dic.Clear();
            dic.Add("@Id", OlympiadId);
            string bbcode = Util.AbitDB.GetStringValue(query, dic);

            if (!string.IsNullOrEmpty(bbcode))
            {
                query = "INSERT INTO OlympInBBUser (Id, BBUserId, BBCourseCode, IsImported) VALUES (NEWID(), @UserId, @BBCode, 0)";
                dic.Clear();
                dic.Add("@UserId", PersonId);
                dic.Add("@BBCode", bbcode);
                Util.AbitDB.ExecuteQuery(query, dic);
            }

            //query = "SELECT Person.Surname, Person.Name, Person.SecondName, Entry.LicenseProgramCode, Entry.LicenseProgramName, Entry.ObrazProgramName " +
            //    " FROM [Application] INNER JOIN Person ON Person.Id=[Application].PersonId " +
            //    " INNER JOIN Entry ON Application.EntryId=Entry.Id WHERE Application.Id=@AppId";
            //DataTable Tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
            //var fileInfo =
            //    (from DataRow rw in Tbl.Rows
            //     select new
            //     {
            //         Surname = rw.Field<string>("Surname"),
            //         Name = rw.Field<string>("Name"),
            //         SecondName = rw.Field<string>("SecondName"),
            //         ProfessionCode = rw.Field<string>("LicenseProgramCode"),
            //         Profession = rw.Field<string>("LicenseProgramName"),
            //         ObrazProgram = rw.Field<string>("ObrazProgramName")
            //     }).FirstOrDefault();
            //
            //if (iEntry == 2)
            //{
            //    byte[] pdfData = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
            //    DateTime dateTime = DateTime.Now;

            //    query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileExtention, FileData, FileSize, IsReadOnly, LoadDate, Comment, MimeType) " +
            //        " VALUES (@Id, @PersonId, @FileName, @FileExtention, @FileData, @FileSize, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
            //    prms.Clear();
            //    prms.Add("@Id", Guid.NewGuid());
            //    prms.Add("@PersonId", appId);
            //    prms.Add("@FileName", fileInfo.Surname + " " + fileInfo.Name.FirstOrDefault() + " - Заявление [" + dateTime.ToString("dd.MM.yyyy") + "].pdf");
            //    prms.Add("@FileExtention", ".pdf");
            //    prms.Add("@FileData", pdfData);
            //    prms.Add("@FileSize", pdfData.Length);
            //    prms.Add("@IsReadOnly", true);
            //    prms.Add("@LoadDate", dateTime);
            //    prms.Add("@Comment", "Заявление на направление (" + fileInfo.ProfessionCode + ") " + fileInfo.Profession + ", образовательная программа \""
            //        + fileInfo.ObrazProgram + "\", от " + dateTime.ToShortDateString());
            //    prms.Add("@MimeType", "[Application]/pdf");
            //    Util.AbitDB.ExecuteQuery(query, prms);
            //}
            return RedirectToAction("Index", "Olymp", new RouteValueDictionary() { { "id", appId.ToString("N") } });
        }

        public ActionResult Index(string id)
        {
            if (Util.bUseRedirection && Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return RedirectToAction("LogOn", "Account");

            Guid appId = new Guid();
            if (!Guid.TryParse(id, out appId))
                return RedirectToAction("Main", "Applicant");

            Dictionary<string, object> prms = new Dictionary<string, object>()
            {
                { "@PersonId", personId },
                { "@Id", appId }
            };

            DataTable tbl =
                Util.AbitDB.GetDataTable("SELECT Id, City, Subject, Stage, Enabled, DateOfDisable, DateOfStart, BlackBoardUrl, IsFullTime FROM [extApplication] WHERE PersonId=@PersonId AND Id=@Id", prms);

            if (tbl.Rows.Count == 0)
                return RedirectToAction("Main", "Applicant");

            DataRow rw = tbl.Rows[0];

            ExtApplicationModel model = new ExtApplicationModel()
            {
                Id = rw.Field<Guid>("Id"),
                Enabled = rw.Field<bool?>("Enabled") ?? false,
                DateOfDisable = rw.Field<DateTime?>("DateOfDisable").HasValue ? rw.Field<DateTime?>("DateOfDisable").Value.ToString("dd.MM.yyyy HH:mm:ss") : "",
                Subject = rw.Field<string>("Subject"),
                Stage = rw.Field<string>("Stage"),
                City = rw.Field<string>("City"),
                DateOfApply = rw.Field<DateTime>("DateOfStart"),
                BlackBoardURL = rw.Field<string>("BlackBoardUrl"),
                IsFullTime = rw.Field<bool?>("IsFullTime") ?? false
            };

            //var app = new
            //{
            //    Id = rw.Field<Guid>("Id"),
            //    Profession = rw.Field<string>("LicenseProgramName"),
            //    Priority = rw.Field<int>("Priority"),
            //    ObrazProgram = rw.Field<string>("ObrazProgramName"),
            //    Specialization = rw.Field<string>("ProfileName"),
            //    StudyForm = rw.Field<string>("StudyFormName"),
            //    StudyBasis = rw.Field<string>("StudyBasisName"),
            //    Enabled = rw.Field<bool?>("Enabled"),
            //    DateOfDisable = rw.Field<DateTime?>("DateOfDisable"),
            //    EntryTypeId = rw.Field<int?>("StudyLevelId") == 17 ? 2 : 1,
            //    ComissionAddress = rw.Field<string>("ComAddress"),
            //    ComissionYaCoord = rw.Field<string>("YaMapCoord")
            //};
            //
            //string query = "SELECT DISTINCT Exam FROM AbitMark INNER JOIN Student ON Student.Id = AbitMark.AbiturientId WHERE " +
            //    "Profession=@Profession AND ObrazProgram=@ObrazProgram AND Specialization=@Specialization AND StudyBasisId=@StudyBasisId AND StudyFormId=@StudyFormId";
            //
            //prms.Clear();
            //prms.Add("@Profession", app.Profession);
            //prms.Add("@ObrazProgram", app.ObrazProgram);
            //prms.Add("@Specialization", app.Specialization == null ? "" : app.Specialization);
            //prms.Add("@StudyBasisId", app.StudyBasisId);
            //prms.Add("@StudyFormId", app.StudyFormId);
            //
            //tbl = Util.StudDB.GetDataTable(query, prms);
            //
            //var exams = (from DataRow row in tbl.Rows
            //             select row.Field<string>("Exam")
            //             ).ToList();
            //
            //
            //query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM ApplicationFile WHERE ApplicationId=@AppId";
            //tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
            //var lFiles =
            //    (from DataRow row in tbl.Rows
            //     select new AppendedFile()
            //     {
            //         Id = row.Field<Guid>("Id"),
            //         FileName = row.Field<string>("FileName"),
            //         FileSize = row.Field<int>("FileSize"),
            //         Comment = row.Field<string>("Comment"),
            //         IsShared = false,
            //         IsApproved = row.Field<bool?>("IsApproved").HasValue ?
            //            row.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
            //     }).ToList();
            //
            //query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
            //tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", personId } });
            //var lSharedFiles =
            //    (from DataRow row in tbl.Rows
            //     select new AppendedFile()
            //     {
            //         Id = row.Field<Guid>("Id"),
            //         FileName = row.Field<string>("FileName"),
            //         FileSize = row.Field<int>("FileSize"),
            //         Comment = row.Field<string>("Comment"),
            //         IsShared = true,
            //         IsApproved = row.Field<bool?>("IsApproved").HasValue ?
            //            row.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
            //     }).ToList();
            //
            //var AllFiles = lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();
            //
            //query = "SELECT Id, MailText FROM MotivationMail WHERE ApplicationId=@AppId";
            //tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", app.Id } });
            //
            //string motivMail = "";
            //Guid motivId = Guid.Empty;
            //if (tbl.Rows.Count > 0)
            //{
            //    motivMail = tbl.Rows[0].Field<string>("MailText");
            //    motivId = tbl.Rows[0].Field<Guid>("Id");
            //}

            return View(model);
        }

        public ActionResult GetPrint(string id)
        {
            if (Util.bUseRedirection && Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Authentification Error"), "text/plain");

            Guid appId;
            if (!Guid.TryParse(id, out appId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Ошибка идентификатора заявления"), "text/plain");

            string query = "SELECT COUNT(Id) FROM Application WHERE Id=@Id AND PersonId=@PersonId";
            int cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", appId }, { "@PersonId", personId } });
            if (cnt == 0)
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Access error"), "text/plain");

            byte[] bindata = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
            if (bindata == null)
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Ошибка при создании файла"), "text/plain");
            return new FileContentResult(bindata, "application/pdf") { FileDownloadName = "Application.pdf" };
        }

        #region Ajax
        //public JsonResult GetSubjects(string cityId)
        //{
        //    int iCityId;
        //    if (!int.TryParse(cityId, out iCityId))
        //        return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

        //    string query = "SELECT DISTINCT SubjectId, Subject FROM extOlympiadInternet WHERE Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND CityId=@CityId ORDER BY Subject";
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("@PersonId", PersonId);
        //    dic.Add("@CityId", iCityId);
        //    try
        //    {
        //        DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

        //        var lst = (from DataRow rw in tbl.Rows
        //                   select new { Id = rw["SubjectId"].ToString(), Name = rw["Subject"].ToString() }).ToList();

        //        return Json(new { IsOk = true, List = lst });
        //    }
        //    catch
        //    {
        //        return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
        //    }
        //}

        public JsonResult GetSubjects()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = "SELECT DISTINCT SubjectId, Subject FROM extOlympiadInternet WHERE Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND Year=@Year ORDER BY Subject";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Year", Util.iOlympYear);
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["SubjectId"].ToString(), Name = rw["Subject"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetStages(string cityId, string subjectId)
        {
            int iCityId;
            if (!int.TryParse(cityId, out iCityId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = "SELECT DISTINCT StageId, Stage FROM extOlympiadInternet WHERE Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND CityId=@CityId AND SubjectId=@SubjectId ORDER BY Stage";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@CityId", iCityId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@Year", Util.iOlympYear);
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["StageId"].ToString(), Name = rw["Stage"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetDates(string cityId, string subjectId, string stageId)
        {
            int iCityId;
            if (!int.TryParse(cityId, out iCityId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iStageId;
            if (!int.TryParse(stageId, out iStageId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = "SELECT DISTINCT CONVERT(nvarchar, [Date], 104) AS Name FROM extOlympiadInternet WHERE Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND CityId=@CityId AND SubjectId=@SubjectId AND StageId=@StageId ORDER BY 1";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@CityId", iCityId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@StageId", iStageId);

            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["Name"].ToString(), Name = rw["Name"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetOlympPlaces(string cityId, string subjectId, string stageId, string date)
        {
            int iCityId;
            if (!int.TryParse(cityId, out iCityId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iStageId;
            if (!int.TryParse(stageId, out iStageId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            DateTime dtDate;
            if (!DateTime.TryParse(date, CultureInfo.CreateSpecificCulture("ru-RU"), DateTimeStyles.None, out dtDate))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = @"SELECT OlympPlaceId AS Id, OlympPlace AS Name 
FROM extOlympiadInternet WHERE extOlympiadInternet.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) 
AND CityId=@CityId AND SubjectId=@SubjectId AND StageId=@StageId AND Date=@Date";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@CityId", iCityId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@StageId", iStageId);
            dic.Add("@Date", dtDate.Date);

            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["Id"].ToString(), Name = rw["Name"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetOlympForms(string subjectId)
        {
            //int iCityId;
            //if (!int.TryParse(cityId, out iCityId))
            //    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = @"SELECT DISTINCT OlympFormId AS Id, OlympForm AS Name 
FROM extOlympiadInternet WHERE extOlympiadInternet.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND SubjectId=@SubjectId AND Year=@Year";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@Year", Util.iOlympYear);

            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["Id"].ToString(), Name = rw["Name"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetSchoolClassIntervals(string subjectId, string olympFormId)
        {
            int iOlympFormId;
            int.TryParse(olympFormId, out iOlympFormId);

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = @"SELECT DISTINCT extOlympiadInternet.SchoolClassIntervalId AS Id, extOlympiadInternet.SchoolClassInterval AS Name 
FROM extOlympiadInternet 
INNER JOIN SchoolClassInSchoolClassInterval ON SchoolClassInSchoolClassInterval.SchoolClassIntervalId = extOlympiadInternet.SchoolClassIntervalId 
WHERE extOlympiadInternet.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND SubjectId=@SubjectId AND Year=@Year
AND SchoolClassInSchoolClassInterval.SchoolClassId = (SELECT SchoolClassId FROM Person WHERE Id=@PersonId)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@Year", Util.iOlympYear);
            if (iOlympFormId > 0)
            {
                query += " AND OlympFormId=@OlympFormId ";
                dic.Add("@OlympFormId", iOlympFormId);
            }

            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                if (tbl.Rows.Count > 0)
                {
                    var lst = (from DataRow rw in tbl.Rows
                               select new { Id = rw["Id"].ToString(), Name = rw["Name"].ToString() }).ToList();

                    return Json(new { IsOk = true, List = lst });
                }
                else
                {
                    return Json(new { IsOk = false, ErrorMessage = "Не найдено доступных для Вас групп классов (или Вы уже подавали заявление на данную группу)" });
                }
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetCities(string subjectId, string olympFormId, string schoolClassIntervalId)
        {
            int iOlympFormId;
            int.TryParse(olympFormId, out iOlympFormId);

            int iSchoolClassIntervalId;
            int.TryParse(schoolClassIntervalId, out iSchoolClassIntervalId);

            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = @"SELECT DISTINCT extOlympiad.CityId AS Id, extOlympiad.City AS Name, extOlympiad.CitySortOrder" + 
                " FROM extOlympiad INNER JOIN SchoolClassInSchoolClassInterval ON SchoolClassInSchoolClassInterval.SchoolClassIntervalId = extOlympiad.SchoolClassIntervalId" +
                " WHERE extOlympiad.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND extOlympiad.SubjectId=@SubjectId" +
                " AND SchoolClassInSchoolClassInterval.SchoolClassId = (SELECT SchoolClassId FROM Person WHERE Id=@PersonId)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@Year", Util.iOlympYear);
            if (iOlympFormId > 0)
            {
                query += " AND OlympFormId=@OlympFormId ";
                dic.Add("@OlympFormId", iOlympFormId);
            }
            if (iSchoolClassIntervalId > 0)
            {
                query += " AND extOlympiad.SchoolClassIntervalId=@SchoolClassIntervalId ";
                dic.Add("@SchoolClassIntervalId", iSchoolClassIntervalId);
            }

            try
            {
                string obderby = " ORDER BY extOlympiad.CitySortOrder, extOlympiad.City ";
                DataTable tbl = Util.AbitDB.GetDataTable(query + obderby, dic);

                var lst = (from DataRow rw in tbl.Rows
                           select new { Id = rw["Id"].ToString(), Name = rw["Name"].ToString() }).ToList();

                return Json(new { IsOk = true, List = lst });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }

//        public JsonResult CheckInfo(string cityId, string subjectId, string stageId, string date, string placeId)
//        {
//            int iCityId;
//            if (!int.TryParse(cityId, out iCityId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            int iSubjectId;
//            if (!int.TryParse(subjectId, out iSubjectId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            int iStageId;
//            if (!int.TryParse(stageId, out iStageId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            DateTime dtDate;
//            if (!DateTime.TryParse(date, CultureInfo.CreateSpecificCulture("ru-RU"), DateTimeStyles.None, out dtDate))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            int iOlympPlaceId;
//            if (!int.TryParse(placeId, out iOlympPlaceId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

//            string query = @"SELECT City, Subject, convert(nvarchar, [Date], 104) AS 'Date', Stage, OlympPlace 
//FROM extOlympiad WHERE extOlympiad.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) 
//AND CityId=@CityId AND SubjectId=@SubjectId AND StageId=@StageId AND Date=@Date AND OlympPlaceId=@OlympPlaceId";
//            Dictionary<string, object> dic = new Dictionary<string, object>();
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@CityId", iCityId);
//            dic.Add("@SubjectId", iSubjectId);
//            dic.Add("@StageId", iStageId);
//            dic.Add("@Date", dtDate.Date);
//            dic.Add("@OlympPlaceId", iOlympPlaceId);

//            try
//            {
//                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

//                if (tbl.Rows.Count == 0)
//                    return Json(new { IsOk = false, ErrorMessage = "Олимпиада не найдена или заявление уже подавалось" });
//                if (tbl.Rows.Count == 2)
//                    return Json(new { IsOk = false, ErrorMessage = "Ошибка: найдено более одной олимпиады" });

//                DataRow rw = tbl.Rows[0];

//                var data = new
//                {
//                    City = rw["City"].ToString(),
//                    Subject = rw["Subject"].ToString(),
//                    Date = rw["Date"].ToString(),
//                    Stage = rw["Stage"].ToString(),
//                    OlympPlace = rw["OlympPlace"].ToString()
//                };

//                return Json(new { IsOk = true, Vals = data });
//            }
//            catch
//            {
//                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
//            }
//        }

        public JsonResult CheckInfo(string subjectId, string olympFormId, string schoolClassInterval, string cityId)
        {
            int iSubjectId;
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iOlympFormId;
            int.TryParse(olympFormId, out iOlympFormId);

            int iSchoolClassInterval;
            int.TryParse(schoolClassInterval, out iSchoolClassInterval);

            int iCityId;
            int.TryParse(cityId, out iCityId);

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = @"SELECT extOlympiad.Subject, ISNULL(extOlympiad.SchoolClassInterval, 'нет') AS SchoolClassInterval, ISNULL(extOlympiad.OlympForm, 'нет') AS OlympForm, " +
                " extOlympiad.City, extOlympiad.DateOfStart, extOlympiad.DateOfClose, extOlympiad.IsOpen " +
                " FROM extOlympiad INNER JOIN SchoolClassInSchoolClassInterval ON SchoolClassInSchoolClassInterval.SchoolClassIntervalId = extOlympiad.SchoolClassIntervalId" +
                " WHERE extOlympiad.Id NOT IN (SELECT OlympiadId FROM [Application] WHERE PersonId=@PersonId) AND extOlympiad.SubjectId=@SubjectId AND extOlympiad.Year=@Year " +
                " AND SchoolClassInSchoolClassInterval.SchoolClassId = (SELECT SchoolClassId FROM Person WHERE Id=@PersonId)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@SubjectId", iSubjectId);
            dic.Add("@Year", Util.iOlympYear);

            if (iOlympFormId > 0)
            {
                query += " AND extOlympiad.OlympFormId=@OlympFormId ";
                dic.Add("@OlympFormId", iOlympFormId);
            }
            if (iSchoolClassInterval > 0)
            {
                query += " AND extOlympiad.SchoolClassIntervalId=@SchoolClassIntervalId ";
                dic.Add("@SchoolClassIntervalId", iSchoolClassInterval);
            }

            if (iCityId > 0)
            {
                query += " AND extOlympiad.CityId=@CityId ";
                dic.Add("@CityId", iCityId);
            }

            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                if (tbl.Rows.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = "Олимпиада не найдена или заявление уже подавалось" });
                if (tbl.Rows.Count == 2)
                    return Json(new { IsOk = false, ErrorMessage = "Ошибка: найдено более одной олимпиады" });

                DataRow rw = tbl.Rows[0];



                DateTime dateOfStart = DateTime.Now.AddDays(-1);
                if (!DateTime.TryParse(rw["DateOfStart"].ToString(), out dateOfStart))
                    dateOfStart = DateTime.Now.AddDays(-1);
                if (DateTime.Now < dateOfStart)
                    return Json(new { IsOk = false, ErrorMessage = "Приём заявлений на участие в олимпиаду ещё не начался" });
                
                DateTime dateOfСlose = DateTime.Now.AddDays(1);
                if (!DateTime.TryParse(rw["DateOfClose"].ToString(), out dateOfСlose))
                    dateOfСlose = DateTime.Now.AddDays(1);
                if (DateTime.Now > dateOfСlose)
                    return Json(new { IsOk = false, ErrorMessage = "Приём заявлений на участие в олимпиаду закончился" });

                bool isOpen = (bool?)rw["IsOpen"] ?? true;
                if (!isOpen)
                    return Json(new { IsOk = false, ErrorMessage = "Ошибка: приём заявлений на участие в олимпиаду закрыт" });

                var data = new
                {
                    Subject = rw["Subject"].ToString(),
                    Class = rw["SchoolClassInterval"].ToString(),
                    Form = rw["OlympForm"].ToString(),
                    City = rw["City"].ToString()
                };

                return Json(new { IsOk = true, Vals = data });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }

        public JsonResult GetCityNames(string regionId)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var towns = Util.GetCityListByRegion(sRegionKladrCode);

                return Json(new { IsOk = true, List = towns.Select(x => x.Value).Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetStreetNames(string regionId, string cityName)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var streets = Util.GetStreetListByRegion(sRegionKladrCode, cityName);

                return Json(new { IsOk = true, List = streets.Select(x => x.Value).Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetHouseNames(string regionId, string cityName, string streetName)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var streets = Util.GetHouseListByStreet(sRegionKladrCode, cityName, streetName);

                return Json(new { IsOk = true, List = streets.Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        #endregion
    }
}
