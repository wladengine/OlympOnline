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
    public class ApplicantController : Controller
    {
        //
        // GET: /Applicant/
        public ActionResult Index(string step)
        {
            if (Util.bUseRedirection && Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            int stage = 0;
            if (!int.TryParse(step, out stage))
                stage = 1;

            string query = "SELECT Id, Surname, Name, SecondName, BirthPlace, BirthDate, Sex, NationalityId, PassportTypeId, PassportSeries, PassportNumber, PassportAuthor, " +
                      "PassportDate, PassportCode, Phone, Mobiles, CountryId, RegionId, Code, City, Street, House, Korpus, Flat, " +
                      "HighEducationInfo, SchoolExitYear, " + 
                      "SchoolTypeId, SchoolName, RegistrationStage, SchoolCountryId, SchoolRegionId, SchoolClassId, SchoolCity, SchoolNum, IsCountryside, IsDisabled, IsSirota FROM Person WHERE Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", PersonId } });
            if (tbl.Rows.Count == 0)
                stage = 1;
            else if (tbl.Rows[0].Field<int?>("RegistrationStage").HasValue && tbl.Rows[0].Field<int>("RegistrationStage") < stage)
                stage = tbl.Rows[0].Field<int>("RegistrationStage");

            PersonalOffice model = new PersonalOffice() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
            if (model.Stage == 1)
            {
                model.PersonInfo = new InfoPerson();
                if (tbl.Rows.Count != 0)
                {
                    model.PersonInfo.Surname = Server.HtmlDecode(tbl.Rows[0].Field<string>("Surname"));
                    model.PersonInfo.Name = Server.HtmlDecode(tbl.Rows[0].Field<string>("Name"));
                    model.PersonInfo.SecondName = Server.HtmlDecode(tbl.Rows[0].Field<string>("SecondName"));
                    model.PersonInfo.Sex = tbl.Rows[0].Field<bool?>("Sex").HasValue ? (tbl.Rows[0].Field<bool?>("Sex").Value ? "M" : "F") : "M";
                    model.PersonInfo.Nationality = tbl.Rows[0].Field<int>("NationalityId").ToString();
                    model.PersonInfo.BirthPlace = Server.HtmlDecode(tbl.Rows[0].Field<string>("BirthPlace"));
                    model.PersonInfo.BirthDate = tbl.Rows[0].Field<DateTime?>("BirthDate").HasValue ? tbl.Rows[0].Field<DateTime>("BirthDate").ToString("dd.MM.yyyy") : "";
                }

                model.PersonInfo.NationalityList = Util.GetNationalityList();
                model.PersonInfo.SexList = Util.GetSexList(model.Lang);
            }
            else if (model.Stage == 2)
            {
                model.PassportInfo = new PassportPerson();
                model.PassportInfo.PassportTypeList = Util.GetPassportTypeList();

                if (tbl.Rows.Count != 0)
                {
                    model.PassportInfo.PassportType = tbl.Rows[0].Field<int?>("PassportTypeId").HasValue ? tbl.Rows[0].Field<int>("PassportTypeId").ToString() : "1";
                    model.PassportInfo.PassportSeries = Server.HtmlDecode(tbl.Rows[0].Field<string>("PassportSeries"));
                    model.PassportInfo.PassportNumber = Server.HtmlDecode(tbl.Rows[0].Field<string>("PassportNumber"));
                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(tbl.Rows[0].Field<string>("PassportAuthor"));
                    model.PassportInfo.PassportDate = tbl.Rows[0].Field<DateTime?>("PassportDate").HasValue ?
                        tbl.Rows[0].Field<DateTime>("PassportDate").ToString("dd.MM.yyyy") : "";
                    model.PassportInfo.PassportCode = Server.HtmlDecode(tbl.Rows[0].Field<string>("PassportCode"));
                }
            }
            else if (model.Stage == 3)
            {
                model.ContactsInfo = new ContactsPerson();
                if (tbl.Rows.Count != 0)
                {
                    model.ContactsInfo.MainPhone = Server.HtmlDecode(tbl.Rows[0].Field<string>("Phone"));
                    model.ContactsInfo.SecondPhone = Server.HtmlDecode(tbl.Rows[0].Field<string>("Mobiles"));
                    model.ContactsInfo.CountryId = tbl.Rows[0].Field<int?>("CountryId").ToString();
                    model.ContactsInfo.RegionId = tbl.Rows[0].Field<int?>("RegionId").ToString();

                    model.ContactsInfo.Code = Server.HtmlDecode(tbl.Rows[0].Field<string>("Code"));
                    model.ContactsInfo.City = Server.HtmlDecode(tbl.Rows[0].Field<string>("City"));
                    model.ContactsInfo.Street = Server.HtmlDecode(tbl.Rows[0].Field<string>("Street"));
                    model.ContactsInfo.House = Server.HtmlDecode(tbl.Rows[0].Field<string>("House"));
                    model.ContactsInfo.Korpus = Server.HtmlDecode(tbl.Rows[0].Field<string>("Korpus"));
                    model.ContactsInfo.Flat = Server.HtmlDecode(tbl.Rows[0].Field<string>("Flat"));
                    model.ContactsInfo.IsCountryside = tbl.Rows[0].Field<bool>("IsCountryside");
                }

                model.ContactsInfo.CountryList = Util.GetCountryList();
                model.ContactsInfo.RegionList = Util.GetRegionList();
            }
            else if (model.Stage == 4)
            {
                model.EducationInfo = new EducationPerson();

                model.EducationInfo.SchoolTypeList = Util.GetSchoolTypeList();
                model.EducationInfo.CountryList = Util.GetCountryList();
                model.EducationInfo.RegionList = Util.GetRegionList();
                model.EducationInfo.SchoolClassList = Util.GetSchoolClassList();

                model.EducationInfo.SchoolName = Server.HtmlDecode(tbl.Rows[0].Field<string>("SchoolName"));
                model.EducationInfo.SchoolNumber = Server.HtmlDecode(tbl.Rows[0].Field<string>("SchoolNum"));

                model.EducationInfo.SchoolCity = Server.HtmlDecode(tbl.Rows[0].Field<string>("SchoolCity"));
                model.EducationInfo.SchoolNumber = Server.HtmlDecode(tbl.Rows[0].Field<string>("SchoolNum"));

                model.EducationInfo.SchoolTypeId = (tbl.Rows[0].Field<int?>("SchoolTypeId") ?? 1).ToString();
                model.EducationInfo.CountryEducId = (tbl.Rows[0].Field<int?>("SchoolCountryId") ?? 1).ToString();
                model.EducationInfo.RegionEducId = (tbl.Rows[0].Field<int?>("SchoolRegionId") ?? 1).ToString();
                model.EducationInfo.SchoolClassId = (tbl.Rows[0].Field<int?>("SchoolClassId") ?? 1).ToString();

                model.EducationInfo.HighEducationInfo = Server.HtmlDecode(tbl.Rows[0].Field<string>("HighEducationInfo"));
                model.EducationInfo.SchoolExitYear = (tbl.Rows[0].Field<int?>("SchoolExitYear")).ToString();
            }
            else if (model.Stage == 5)
            {
                model.AddInfo = new AdditionalInfoPerson();

                model.AddInfo.IsDisabled = tbl.Rows[0].Field<bool>("IsDisabled");
                model.AddInfo.IsSirota = tbl.Rows[0].Field<bool>("IsSirota");
                
                model.AddInfo.OlympVserosSubjects = Util.GetOlympVserosSubjects();
                model.AddInfo.OtherOlympSubjects = Util.GetOtherOlympSubjects();
                model.AddInfo.OlympStatuses = Util.GetOtherOlympStatus();
                model.AddInfo.OlympVserosStages = Util.GetOtherOlympStage();
                
                model.AddInfo.VserossOlympBase = Util.GetVserossOlympBase(PersonId);
                model.AddInfo.OtherOlympBase = Util.GetOtherOlympBase(PersonId);
            }
            else if (model.Stage == 6)
            {
                if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                    return RedirectToAction("LogOn", "Account");

                if (tbl.Rows[0].Field<int>("RegistrationStage") < 7)
                {
                    model.ParentInfo = new ParentsPersonInfo()
                    {
                        FZ_152Agree = false,
                        ParentAddress = "",
                        ParentName = ""
                    };
                }
                else
                {
                    string quer = "SELECT ParentName, ParentAdress FROM Person WHERE Id=@Id";
                    DataTable tbAdd = Util.AbitDB.GetDataTable(quer, new Dictionary<string, object>() { { "@Id", PersonId } });
                    model.ParentInfo = new ParentsPersonInfo()
                    {
                        FZ_152Agree = false,
                        ParentName =  tbAdd.Rows[0].Field<string>("ParentName") ?? "",
                        ParentAddress = tbAdd.Rows[0].Field<string>("ParentAdress") ?? ""
                    };
                }
            }
            return View("PersonalOffice", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult NextStep(PersonalOffice model)
        {
            Guid UserId;
            if (!Util.CheckAuthCookies(Request.Cookies, out UserId))
                return RedirectToAction("LogOn", "Account");

            string stage = Util.AbitDB.GetStringValue("SELECT RegistrationStage FROM Person WHERE Id=@Id", new Dictionary<string, object>() { { "Id", UserId } });
            int iRegStage = 1;
            if (!int.TryParse(stage, out iRegStage))
                iRegStage = 1;
            bool isNewPerson = false;
            if (string.IsNullOrEmpty(stage))
                isNewPerson = true;

            if (Util.CheckPersonReadOnlyStatus(UserId))
            {
                if (++(model.Stage) <= 6)
                    return RedirectToAction("Index", "Applicant", new RouteValueDictionary() { { "step", model.Stage } });
                else
                    return RedirectToAction("Main", "Applicant");
            }

            if (model.Stage == 1)
            {
                string query;
                if (isNewPerson)
                    query = "INSERT INTO Person (Id, Surname, Name, SecondName, BirthDate, BirthPlace, NationalityId, Sex, RegistrationStage) " +
                        " VALUES (@Id, @Surname, @Name, @SecondName, @BirthDate, @BirthPlace, @NationalityId, @Sex, @RegistrationStage)";
                else
                    query = "UPDATE PERSON SET Surname=@Surname, Name=@Name, SecondName=@SecondName, BirthDate=@BirthDate, BirthPlace=@BirthPlace, " +
                        " NationalityId=@NationalityId, Sex=@Sex, RegistrationStage=@RegistrationStage WHERE Id=@Id";

                DateTime bdate;
                if (!DateTime.TryParse(model.PersonInfo.BirthDate, CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat, DateTimeStyles.None, out bdate))
                    bdate = DateTime.Now.Date;
                bdate = bdate.Date;

                if (bdate.Year < (DateTime.Now.Year - 80) || bdate.Date >= DateTime.Now.Date)
                {
                    model.PersonInfo.NationalityList = Util.GetNationalityList();
                    model.PersonInfo.SexList = Util.GetSexList("ru");

                    ModelState.AddModelError("", "Дата рождения некорректна");
                    return View("PersonalOffice", model);
                }

                int NationalityId = 1;
                if (!int.TryParse(model.PersonInfo.Nationality, out NationalityId))
                    NationalityId = 1;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Id", UserId);
                dic.AddItem("@Surname", model.PersonInfo.Surname);
                dic.AddItem("@Name", model.PersonInfo.Name);
                dic.AddItem("@SecondName", model.PersonInfo.SecondName);
                dic.AddItem("@BirthDate", bdate);
                dic.AddItem("@BirthPlace", model.PersonInfo.BirthPlace);
                dic.AddItem("@NationalityId", NationalityId);
                dic.AddItem("@Sex", model.PersonInfo.Sex == "M" ? true : false);
                dic.AddItem("@RegistrationStage", iRegStage < 2 ? 2 : iRegStage);

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            else if (model.Stage == 2)
            {
                int iPassportType = 1;
                if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
                    iPassportType = 1;

                DateTime dtPassportDate;
                try
                {
                    dtPassportDate = Convert.ToDateTime(model.PassportInfo.PassportDate, System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                }
                catch { dtPassportDate = DateTime.Now; }

                if (dtPassportDate.Year < (DateTime.Now.Year - 40))
                {
                    model.PassportInfo.PassportTypeList = Util.GetPassportTypeList();

                    ModelState.AddModelError("", "Дата паспорта некорректна");
                    return View("PersonalOffice", model);
                }

                string query = "UPDATE PERSON SET PassportTypeId=@PassportTypeId, PassportSeries=@PassportSeries, PassportNumber=@PassportNumber, PassportAuthor=@PassportAuthor, " +
                    " PassportDate=@PassportDate, PassportCode=@PassportCode, RegistrationStage=@RegistrationStage WHERE Id=@Id";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Id", UserId);
                dic.AddItem("@PassportTypeId", iPassportType);
                dic.AddItem("@PassportSeries", model.PassportInfo.PassportSeries);
                dic.AddItem("@PassportNumber", model.PassportInfo.PassportNumber);
                dic.AddItem("@PassportAuthor", model.PassportInfo.PassportAuthor);
                dic.AddItem("@PassportDate", dtPassportDate);
                dic.AddItem("@PassportCode", model.PassportInfo.PassportCode);
                dic.AddItem("@RegistrationStage", iRegStage < 3 ? 3 : iRegStage);

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            else if (model.Stage == 3)
            {
                int countryId = 0;
                if (!int.TryParse(model.ContactsInfo.CountryId, out countryId))
                    countryId = 1;//Russia
                int regionId = 0;
                if (countryId != 1)//иностранцы
                    regionId = 84;//Не РФ
                else//парсим российские регионы
                {
                    if (!int.TryParse(model.ContactsInfo.RegionId, out regionId))
                        regionId = 84;//Не РФ
                }

                string sCity = model.ContactsInfo.City;
                string sStreet = model.ContactsInfo.Street;
                string sHouse = model.ContactsInfo.House;
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(model.ContactsInfo.RegionId);

                string query = "UPDATE Person SET Phone=@Phone, Mobiles=@Mobiles, CountryId=@CountryId, RegionId=@RegionId, Code=@Code, City=@City, " +
                    " Street=@Street, House=@House, Korpus=@Korpus, Flat=@Flat, RegistrationStage=@RegistrationStage, KladrCode=@KladrCode, IsCountryside=@IsCountryside WHERE Id=@Id";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Id", UserId);
                dic.AddItem("@Phone", model.ContactsInfo.MainPhone);
                dic.AddItem("@Mobiles", model.ContactsInfo.SecondPhone);
                dic.AddItem("@CountryId", countryId);
                dic.AddItem("@RegionId", regionId);
                dic.AddItem("@Code", model.ContactsInfo.Code);
                dic.AddItem("@City", sCity);
                dic.AddItem("@Street", sStreet);
                dic.AddItem("@House", sHouse);
                dic.AddItem("@Korpus", model.ContactsInfo.Korpus);
                dic.AddItem("@KladrCode", Util.GetKladrCodeByAddress(sRegionKladrCode, sCity, sStreet, sHouse));
                dic.AddItem("@Flat", model.ContactsInfo.Flat);
                dic.AddItem("@IsCountryside", model.ContactsInfo.IsCountryside);
                dic.AddItem("@RegistrationStage", iRegStage < 4 ? 4 : iRegStage);

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            else if (model.Stage == 4)//образование
            {
                int schoolTypeId;
                if (!int.TryParse(model.EducationInfo.SchoolTypeId, out schoolTypeId))
                    schoolTypeId = 1;
                int iCountryEducId;
                if (!int.TryParse(model.EducationInfo.CountryEducId, out iCountryEducId))
                    iCountryEducId = 1;
                int iRegionEducId;
                if (!int.TryParse(model.EducationInfo.RegionEducId, out iRegionEducId))
                    iRegionEducId = 1;
                int iSchoolClassId;
                if (!int.TryParse(model.EducationInfo.SchoolClassId, out iSchoolClassId))
                    iSchoolClassId = 1;
                 
                int schoolExitYear;
                int.TryParse(model.EducationInfo.SchoolExitYear, out schoolExitYear);

                string qAdditionalString = string.Empty;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Id", UserId);
                dic.AddItem("@SchoolTypeId", schoolTypeId);
                dic.AddItem("@SchoolName", model.EducationInfo.SchoolName);
                dic.AddItem("@SchoolNum", model.EducationInfo.SchoolNumber);
                dic.AddItem("@CountryEducId", iCountryEducId);
                dic.AddItem("@SchoolCity", model.EducationInfo.SchoolCity);
                dic.AddItem("@SchoolRegionId", iRegionEducId);
                dic.AddItem("@SchoolClassId", iSchoolClassId);
                dic.AddItem("@HighEducationInfo", model.EducationInfo.HighEducationInfo);
                
                if (schoolExitYear != 0)
                {
                    dic.AddItem("@SchoolExitYear", schoolExitYear);
                }
                else
                    dic.AddItem("@SchoolExitYear", DBNull.Value);

                if (iRegStage < 5)
                {
                    qAdditionalString += " RegistrationStage=@RegistrationStage, ";
                    dic.Add("@RegistrationStage", 5);
                }

                string query = string.Format("UPDATE Person SET SchoolTypeId=@SchoolTypeId, SchoolName=@SchoolName, SchoolNum=@SchoolNum, SchoolCountryId=@CountryEducId, " +
                    "HighEducationInfo = @HighEducationInfo, SchoolExitYear = @SchoolExitYear," +
                    "SchoolRegionId=@SchoolRegionId, SchoolClassId=@SchoolClassId, {0} SchoolCity=@SchoolCity WHERE Id=@Id", qAdditionalString);

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            else if (model.Stage == 5)
            {
                Util.AbitDB.ExecuteQuery("UPDATE Person SET IsSirota=@IsSirota, IsDisabled=@IsDisabled, RegistrationStage=@RegistrationStage WHERE Id=@Id",
                        new Dictionary<string, object>() { { "@IsSirota", model.AddInfo.IsSirota }, { "@IsDisabled", model.AddInfo.IsDisabled }, { "@RegistrationStage", iRegStage < 6 ? 6 : iRegStage }, { "@Id", UserId } });
            }
            else if (model.Stage == 6)
            {
                if (!model.ParentInfo.FZ_152Agree)
                {
                    ModelState.AddModelError("AddInfo_FZ_152Agree", "Вы должны принять условия");
                    return View("PersonalOffice", model);
                }

                string query = string.Format("UPDATE Person SET ParentName=@ParentName, ParentAdress=@ParentAdress, RegistrationStage=@RegistrationStage WHERE Id=@Id");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", UserId);
                dic.AddItem("@ParentName", model.ParentInfo.ParentName);
                dic.AddItem("@ParentAdress", model.ParentInfo.ParentAddress);
                dic.Add("@RegistrationStage", 100);
                //if (iRegStage <= 6)
                //    dic.Add("@RegistrationStage", 100);
                Util.AbitDB.ExecuteQuery(query, dic);

            }
            if (model.Stage < 6)
            {
                model.Stage++;
                if (model.Stage == 2)
                    model.Stage++;
                return RedirectToAction("Index", "Applicant", new RouteValueDictionary() { { "step", model.Stage } });
            }
            else
                return RedirectToAction("Main", "Applicant");
        }

        public ActionResult Main()
        {
            if (Util.bUseRedirection && Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            //Validation
            Guid PersonID;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
                return RedirectToAction("LogOn", "Account");

            SimplePerson model = new SimplePerson();
            model.Applications = new List<SimpleApplication>();
            model.Files = new List<AppendedFile>();

            string query = "SELECT Surname, Name, SecondName, RegistrationStage FROM PERSON WHERE Id=@Id";
            Dictionary<string, object> dic = new Dictionary<string, object>() { { "@Id", PersonID } };
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count != 1)
                return RedirectToAction("Index");

            int? regStage = tbl.Rows[0].Field<int?>("RegistrationStage");
            if (!regStage.HasValue)
                return RedirectToAction("Index");
            if (regStage.HasValue && regStage.Value < 100)
                return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage.Value.ToString() } });

            model.Name = tbl.Rows[0].Field<string>("Name");
            model.Surname = tbl.Rows[0].Field<string>("Surname");
            model.SecondName = tbl.Rows[0].Field<string>("SecondName");

            query = "SELECT Id, City, Subject, Stage, convert(nvarchar, [Date], 104) AS 'Date', Enabled FROM [extApplication] WHERE PersonId=@PersonId";
            dic.Clear();
            dic.Add("@PersonId", PersonID);
            tbl = Util.AbitDB.GetDataTable(query, dic);
            foreach (DataRow rw in tbl.Rows)
            {
                model.Applications.Add(new SimpleApplication()
                {
                    Id = rw.Field<Guid>("Id"),
                    City = rw.Field<string>("City"),
                    Stage = rw.Field<string>("Stage"),
                    Subject = rw.Field<string>("Subject"),
                    Date = rw.Field<string>("Date"),
                    Enabled = rw.Field<bool?>("Enabled") ?? true
                });
            }

            model.Messages = Util.GetNewPersonalMessages(PersonID);
            if (model.Applications.Count == 0)
            {
                model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, 
                    Text = string.Format("Для подачи заявления нажмите кнопку <a href=\"{0}/Olymp/NewApplication\">\"Подать новое заявление\"</a>", Util.ServerAddress) });
            }

            return View("Main", model);
        }

        [HttpPost]
        public ActionResult ChangePriority(MotivateMailModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            int prior = 0;
            string[] allKeys = Request.Form.AllKeys;
            foreach (string key in allKeys)
            {
                Guid appId;
                if (!Guid.TryParse(key, out appId))
                    continue;

                string query = "UPDATE [Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Priority", ++prior);
                dic.AddItem("@Id", appId);
                dic.AddItem("@PersonId", PersonId);

                try
                {
                    Util.AbitDB.ExecuteQuery(query, dic);
                }
                catch { }
            }
            return RedirectToAction("PriorityChanger");
        }

        //[Authorize]
        //public ActionResult AddFiles()
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return RedirectToAction("LogOn", "Account");

        //    string query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
        //    DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

        //    List<AppendedFile> lst =
        //        (from DataRow rw in tbl.Rows
        //         select new AppendedFile() { Id = rw.Field<Guid>("Id"), FileName = rw.Field<string>("FileName"), FileSize = rw.Field<int>("FileSize"), Comment = rw.Field<string>("Comment") })
        //        .ToList();

        //    AppendFilesModel model = new AppendFilesModel() { Files = lst };
        //    return View(model);
        //}

        //public ActionResult AddSharedFiles()
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return RedirectToAction("LogOn", "Account");

        //    Util.SetThreadCultureByCookies(Request.Cookies);

        //    string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
        //    DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

        //    List<AppendedFile> lst =
        //        (from DataRow rw in tbl.Rows
        //         select new AppendedFile()
        //         {
        //             Id = rw.Field<Guid>("Id"),
        //             FileName = rw.Field<string>("FileName"),
        //             FileSize = rw.Field<int>("FileSize"),
        //             Comment = rw.Field<string>("Comment"),
        //             IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
        //                rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
        //         }).ToList();

        //    AppendFilesModel model = new AppendFilesModel() { Files = lst };
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult AddSharedFile()
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return Json("Ошибка авторизации");

        //    if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
        //        return Json("Файл не приложен или пуст");

        //    string fileName = Request.Files["File"].FileName;
        //    int lastSlashPos = 0;
        //    lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
        //    if (lastSlashPos > 0)
        //        fileName = fileName.Substring(lastSlashPos);
        //    string fileComment = Request.Form["Comment"];
        //    int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
        //    byte[] fileData = new byte[fileSize];
        //    //читаем данные из ПОСТа
        //    Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
        //    string fileext = "";
        //    try
        //    {
        //        fileext = fileName.Substring(fileName.LastIndexOf('.'));
        //    }
        //    catch
        //    {
        //        fileext = "";
        //    }

        //    try
        //    {
        //        string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, LoadDate, Comment, MimeType) " +
        //            " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @LoadDate, @Comment, @MimeType)";
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("@Id", Guid.NewGuid());
        //        dic.Add("@PersonId", PersonId);
        //        dic.Add("@FileName", fileName);
        //        dic.Add("@FileData", fileData);
        //        dic.Add("@FileSize", fileSize);
        //        dic.Add("@FileExtention", fileext);
        //        dic.Add("@LoadDate", DateTime.Now);
        //        dic.Add("@Comment", fileComment);
        //        dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

        //        Util.AbitDB.ExecuteQuery(query, dic);
        //    }
        //    catch
        //    {
        //        return Json("Ошибка при записи файла");
        //    }

        //    return RedirectToAction("AddSharedFiles");
        //}

        //[HttpPost]
        //public ActionResult AddFile()
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return Json("Ошибка авторизации");

        //    if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
        //        return Json("Файл не приложен или пуст");

        //    string fileName = Request.Files["File"].FileName;
        //    int lastSlashPos = 0;
        //    lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
        //    if (lastSlashPos > 0)
        //        fileName = fileName.Substring(lastSlashPos);
        //    string fileComment = Request.Form["Comment"];
        //    int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
        //    byte[] fileData = new byte[fileSize];
        //    //читаем данные из ПОСТа
        //    Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
        //    string fileext = "";
        //    try
        //    {
        //        fileext = fileName.Substring(fileName.LastIndexOf('.'));
        //    }
        //    catch
        //    {
        //        fileext = "";
        //    }

        //    try
        //    {
        //        string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType) " +
        //            " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("@Id", Guid.NewGuid());
        //        dic.Add("@PersonId", PersonId);
        //        dic.Add("@FileName", fileName);
        //        dic.Add("@FileData", fileData);
        //        dic.Add("@FileSize", fileSize);
        //        dic.Add("@FileExtention", fileext);
        //        dic.Add("@IsReadOnly", false);
        //        dic.Add("@LoadDate", DateTime.Now);
        //        dic.Add("@Comment", fileComment);
        //        dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

        //        Util.AbitDB.ExecuteQuery(query, dic);
        //    }
        //    catch
        //    {
        //        return Json("Ошибка при записи файла");
        //    }

        //    return RedirectToAction("AddFiles");
        //}

        //public ActionResult GetFile(string id)
        //{
        //    Guid FileId = new Guid();
        //    if (!Guid.TryParse(id, out FileId))
        //        return Content("Некорректный идентификатор файла");

        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return Content("Authorization required");

        //    DataTable tbl = Util.AbitDB.GetDataTable("SELECT FileName, FileData, MimeType, FileExtention FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id",
        //        new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", FileId } });

        //    if (tbl.Rows.Count == 0)
        //        return Content("Файл не найден");

        //    string fileName = tbl.Rows[0].Field<string>("FileName");
        //    string contentType = tbl.Rows[0].Field<string>("MimeType");
        //    byte[] content = tbl.Rows[0].Field<byte[]>("FileData");
        //    string ext = tbl.Rows[0].Field<string>("FileExtention");


        //    if (string.IsNullOrEmpty(contentType))
        //    {
        //        if (string.IsNullOrEmpty(ext))
        //            contentType = "application/octet-stream";
        //        else
        //            contentType = Util.GetMimeFromExtention(ext);
        //    }
        //    bool openMenu = true;
        //    if (ext.IndexOf("jpg", StringComparison.OrdinalIgnoreCase) != -1)
        //        openMenu = false;
        //    if (ext.IndexOf("jpeg", StringComparison.OrdinalIgnoreCase) != -1)
        //        openMenu = false;
        //    if (ext.IndexOf("gif", StringComparison.OrdinalIgnoreCase) != -1)
        //        openMenu = false;
        //    if (ext.IndexOf("png", StringComparison.OrdinalIgnoreCase) != -1)
        //        openMenu = false;

        //    //var file = Util.ABDB.PersonFile.Where(x => x.PersonId == PersonId && x.Id == FileId).
        //    //    Select(x => new { RealName = x.FileName, x.FileData }).FirstOrDefault();

        //    try
        //    {
        //        if (openMenu)
        //            return File(content, contentType, fileName);
        //        else
        //            return File(content, contentType);
        //    }
        //    catch
        //    {
        //        return Content("Ошибка при чтении файла");
        //    }
        //}

        //public ActionResult GetMotivationMailPDF(string id)
        //{
        //    Guid personId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out personId))
        //        return Content("Authorization required");
        //    string fontspath = Server.MapPath("~/Templates/times.ttf");
        //    return File(PDFUtils.GetMotivateMail(id, fontspath), "application/pdf", "MotivateEdit.pdf");
        //}

        //public ActionResult FilesList(string id)
        //{
        //    Guid PersonId;
        //    Guid ApplicationId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return Content(Resources.ServerMessages.AuthorizationRequired);
        //    if (!Guid.TryParse(id, out ApplicationId))
        //        return Content(Resources.ServerMessages.IncorrectGUID);
        //    string fontspath = Server.MapPath("~/Templates/times.ttf");
        //    return File(PDFUtils.GetFilesList(PersonId, ApplicationId, fontspath), "application/pdf", "FilesList.pdf");
        //}
        
        #region Ajax

        //[OutputCache(NoStore = true, Duration = 0)]
        //public ActionResult GetFacs(string studyform, string studybasis, string entry)
        //{
        //    int iStudyFormId;
        //    int iStudyBasisId;
        //    if (!int.TryParse(studyform, out iStudyFormId))
        //        iStudyFormId = 1;
        //    if (!int.TryParse(studybasis, out iStudyBasisId))
        //        iStudyBasisId = 1;
        //    int iEntryId = 1;
        //    if (!int.TryParse(entry, out iEntryId))
        //        iEntryId = 1;

        //    string query = string.Format("SELECT DISTINCT FacultyId, FacultyName FROM {0} WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId " +
        //        "AND IsSecond=@IsSecond ORDER BY FacultyId", iEntryId == 2 ? "extStudyPlan" : "extStudyPlan1K");
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("@StudyFormId", iStudyFormId);
        //    dic.Add("@StudyBasisId", iStudyBasisId);
        //    dic.Add("@IsSecond", iEntryId == 3 ? true : false);

        //    DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
        //    var facs =
        //        from DataRow rw in tbl.Rows
        //        select new { Id = rw.Field<int>("FacultyId"), Name = rw.Field<string>("FacultyName") };
        //    return Json(facs);
        //}

        //[OutputCache(NoStore = true, Duration = 0)]
        //public ActionResult GetProfs(string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0")
        //{
        //    Guid PersonId;
        //    Util.CheckAuthCookies(Request.Cookies, out PersonId);

        //    int iStudyFormId;
        //    int iStudyBasisId;
        //    if (!int.TryParse(studyform, out iStudyFormId))
        //        iStudyFormId = 1;
        //    if (!int.TryParse(studybasis, out iStudyBasisId))
        //        iStudyBasisId = 1;
        //    int iEntryId = 1;
        //    if (!int.TryParse(entry, out iEntryId))
        //        iEntryId = 1;

        //    bool bIsSecond = isSecond == "1" ? true : false;
        //    bool bIsReduced = isReduced == "1" ? true : false;
        //    bool bIsParallel = isParallel == "1" ? true : false;

        //    string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = qEntry.StudyLevelId " +
        //        "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced AND DateOfClose>GETDATE() ";
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("@StudyFormId", iStudyFormId);
        //    dic.Add("@StudyBasisId", iStudyBasisId);
        //    dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);//2 == mag, 1 == 1kurs
        //    dic.Add("@IsSecond", bIsSecond);
        //    dic.Add("@IsParallel", bIsParallel);
        //    dic.Add("@IsReduced", bIsReduced);

        //    DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
        //    var profs =
        //        (from DataRow rw in tbl.Rows
        //         select new
        //         {
        //             Id = rw.Field<int>("LicenseProgramId"),
        //             Name = "(" + rw.Field<string>("LicenseProgramCode") + ") " + rw.Field<string>("LicenseProgramName")
        //         }).OrderBy(x => x.Name);
        //    return Json(profs);
        //}

        //[OutputCache(NoStore = true, Duration = 0)]
        //public ActionResult GetObrazPrograms(string prof, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0")
        //{
        //    Guid PersonId;
        //    bool Authorized = true;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        Authorized = false;

        //    int iStudyFormId;
        //    int iStudyBasisId;
        //    if (!int.TryParse(studyform, out iStudyFormId))
        //        iStudyFormId = 1;
        //    if (!int.TryParse(studybasis, out iStudyBasisId))
        //        iStudyBasisId = 1;
        //    int iEntryId = 1;
        //    if (!int.TryParse(entry, out iEntryId))
        //        iEntryId = 1;
        //    int iProfessionId = 1;
        //    if (!int.TryParse(prof, out iProfessionId))
        //        iProfessionId = 1;

        //    bool bIsSecond = isSecond == "1" ? true : false;
        //    bool bIsReduced = isReduced == "1" ? true : false;
        //    bool bIsParallel = isParallel == "1" ? true : false;

        //    string query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = qEntry.StudyLevelId " +
        //        "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
        //        "AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced AND DateOfClose>GETDATE() ";
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("@StudyFormId", iStudyFormId);
        //    dic.Add("@StudyBasisId", iStudyBasisId);
        //    dic.Add("@LicenseProgramId", iProfessionId);
        //    dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
        //    dic.Add("@IsSecond", bIsSecond);
        //    dic.Add("@IsParallel", bIsParallel);
        //    dic.Add("@IsReduced", bIsReduced);

        //    DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
        //    var OPs = from DataRow rw in tbl.Rows
        //              select new { Id = rw.Field<int>("ObrazProgramId"), Name = rw.Field<string>("ObrazProgramName") };

        //    return Json(new { NoFree = OPs.Count() > 0 ? false : true, List = OPs });
        //}

        //[OutputCache(NoStore = true, Duration = 0)]
        //public ActionResult GetSpecializations(string prof, string obrazprogram, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0")
        //{
        //    Guid PersonId;
        //    Util.CheckAuthCookies(Request.Cookies, out PersonId);

        //    int iStudyFormId;
        //    int iStudyBasisId;
        //    if (!int.TryParse(studyform, out iStudyFormId))
        //        iStudyFormId = 1;
        //    if (!int.TryParse(studybasis, out iStudyBasisId))
        //        iStudyBasisId = 1;
        //    int iEntryId = 1;
        //    if (!int.TryParse(entry, out iEntryId))
        //        iEntryId = 1;
        //    int iProfessionId = 1;
        //    if (!int.TryParse(prof, out iProfessionId))
        //        iProfessionId = 1;
        //    int iObrazProgramId = 1;
        //    if (!int.TryParse(obrazprogram, out iObrazProgramId))
        //        iObrazProgramId = 1;

        //    bool bIsSecond = isSecond == "1" ? true : false;
        //    bool bIsReduced = isReduced == "1" ? true : false;
        //    bool bIsParallel = isParallel == "1" ? true : false;

        //    string query = "SELECT DISTINCT ProfileId, ProfileName FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = qEntry.StudyLevelId WHERE StudyFormId=@StudyFormId " +
        //        "AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@StudyLevelGroupId " +
        //        "AND qEntry.Id NOT IN (SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL) " +
        //        "AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced  AND DateOfClose>GETDATE() ";

        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("@PersonId", PersonId);
        //    dic.Add("@StudyFormId", iStudyFormId);
        //    dic.Add("@StudyBasisId", iStudyBasisId);
        //    dic.Add("@LicenseProgramId", iProfessionId);
        //    dic.Add("@ObrazProgramId", iObrazProgramId);
        //    dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
        //    dic.Add("@IsSecond", bIsSecond);
        //    dic.Add("@IsParallel", bIsParallel);
        //    dic.Add("@IsReduced", bIsReduced);

        //    DataTable tblSpecs = Util.AbitDB.GetDataTable(query, dic);
        //    var Specs =
        //        from DataRow rw in tblSpecs.Rows
        //        select new { SpecId = rw.Field<Guid?>("ProfileId"), SpecName = rw.Field<string>("ProfileName") };

        //    var ret = new
        //    {
        //        NoFree = Specs.Count() == 0 ? true : false,
        //        List = Specs.Select(x => new { Id = x.SpecId, Name = x.SpecName })
        //    };
        //    return Json(ret);
        //}

        //[HttpPost]
        //public ActionResult DeleteFile(string id)
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
        //        return Json(res);
        //    }

        //    Guid fileId;
        //    if (!Guid.TryParse(id, out fileId))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
        //        return Json(res);
        //    }
        //    string attr = Util.AbitDB.GetStringValue("SELECT IsReadOnly FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
        //    if (string.IsNullOrEmpty(attr))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
        //        return Json(res);
        //    }
        //    if (attr == "True")
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
        //        return Json(res);
        //    }
        //    try
        //    {
        //        Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
        //    }
        //    catch
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
        //        return Json(res);
        //    }

        //    var result = new { IsOk = true, ErrorMessage = "" };
        //    return Json(result);
        //}

        //public JsonResult DeleteSharedFile(string id)
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
        //        return Json(res);
        //    }

        //    Guid fileId;
        //    if (!Guid.TryParse(id, out fileId))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
        //        return Json(res);
        //    }
        //    string attr = Util.AbitDB.GetStringValue("SELECT ISNULL([IsReadOnly], 'False') FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
        //    if (string.IsNullOrEmpty(attr))
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
        //        return Json(res);
        //    }
        //    if (attr == "True")
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
        //        return Json(res);
        //    }
        //    try
        //    {
        //        Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
        //    }
        //    catch
        //    {
        //        var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
        //        return Json(res);
        //    }

        //    var result = new { IsOk = true, ErrorMessage = "" };
        //    return Json(result);
        //}

        public ActionResult DeleteMsg(string id)
        {
            if (id == "0")//system messages
                return Json(new { IsOk = true });

            Guid MessageId;
            if (!Guid.TryParse(id, out MessageId))
                return Json(new { IsOk = false, ErrorMessage = "" });

            string query = "UPDATE PersonalMessage SET IsRead=@IsRead WHERE Id=@Id";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@IsRead", true);
            dic.Add("@Id", MessageId);

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch (Exception e)
            {
                return Json(new { IsOk = false, ErrorMessage = e.Message });//
            }

            return Json(new { IsOk = true });
        }

        public JsonResult AddVseross(string subjectId, string stageId, string statusId)
        {
            Guid PersonId, Id;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });

            int iSubjectId, iStageId, iStatusId;
            
            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });
            if (!int.TryParse(stageId, out iStageId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });
            if (!int.TryParse(statusId, out iStatusId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            Id = Guid.NewGuid();

            string query = @"SELECT COUNT(*) FROM PersonOtherOlympsVseross WHERE PersonId=@PersonId 
AND OtherOlympStageId=@OtherOlympStageId AND OtherOlympSubjectId=@OtherOlympSubjectId AND OtherOlympStatusId=@OtherOlympStatusId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", Id);
            dic.Add("@PersonId", PersonId);
            dic.Add("@OtherOlympSubjectId", iSubjectId);
            dic.Add("@OtherOlympStageId", iStageId);
            dic.Add("@OtherOlympStatusId", iStatusId);

            try
            {
                int cnt = (int)Util.AbitDB.GetValue(query, dic);
                if (cnt > 0)
                    return Json(new { IsOk = false, Message = "Ошибка: такая олимпиада уже внесена" });
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при чтении из базы" });
            }


            query = @"INSERT INTO PersonOtherOlympsVseross 
(Id, PersonId, OtherOlympStageId, OtherOlympSubjectId, OtherOlympStatusId) VALUES (@Id, @PersonId, @OtherOlympStageId, @OtherOlympSubjectId, @OtherOlympStatusId)";

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при записи в базу" });
            }

            return Json(new { IsOk = true, Id = Id.ToString("N") });
        }
        public JsonResult AddOtherOlymp(string subjectId, string vuzName, string statusId)
        {
            Guid PersonId, Id;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });

            int iSubjectId, iStatusId;

            if (!int.TryParse(subjectId, out iSubjectId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });
            if (!int.TryParse(statusId, out iStatusId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            Id = Guid.NewGuid();

            string query = @"SELECT COUNT(*) FROM PersonOtherOlymps WHERE PersonId=@PersonId AND VuzName=@VuzName AND OtherOlympSubjectId=@OtherOlympSubjectId AND OtherOlympStatusId=@OtherOlympStatusId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", Id);
            dic.Add("@PersonId", PersonId);
            dic.Add("@OtherOlympSubjectId", iSubjectId);
            dic.Add("@VuzName", vuzName);
            dic.Add("@OtherOlympStatusId", iStatusId);
            try
            {
                int cnt = (int)Util.AbitDB.GetValue(query, dic);
                if (cnt > 0)
                    return Json(new { IsOk = false, Message = "Ошибка: такая олимпиада уже внесена" });
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при чтении из базы" });
            }

            query = @"INSERT INTO PersonOtherOlymps
(Id, PersonId, VuzName, OtherOlympSubjectId, OtherOlympStatusId) VALUES (@Id, @PersonId, @VuzName, @OtherOlympSubjectId, @OtherOlympStatusId)";
            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при записи в базу" });
            }

            return Json(new { IsOk = true, Id = Id.ToString("N") });
        }
        public JsonResult DeleteVseross(string id)
        {
            Guid GuidId, PersonId;

            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });

            if (!Guid.TryParse(id, out GuidId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            string query = "SELECT COUNT(*) FROM PersonOtherOlympsVseross WHERE Id=@Id AND PersonId=@PersonId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Id", GuidId);
            int cnt = (int)Util.AbitDB.GetValue(query, dic);
            if (cnt == 0)
                return Json(new { IsOk = false, Message = "Запись не найдена" });

            query = "DELETE FROM PersonOtherOlympsVseross WHERE Id=@Id AND PersonId=@PersonId";

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при удалении" });
            }

            return Json(new { IsOk = true });
        }
        public JsonResult DeleteOtherOlymp(string id)
        {
            Guid GuidId, PersonId;

            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });

            if (!Guid.TryParse(id, out GuidId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            string query = "SELECT COUNT(*) FROM PersonOtherOlymps WHERE Id=@Id AND PersonId=@PersonId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Id", GuidId);
            int cnt = (int)Util.AbitDB.GetValue(query, dic);
            if (cnt == 0)
                return Json(new { IsOk = false, Message = "Запись не найдена" });

            query = "DELETE FROM PersonOtherOlymps WHERE Id=@Id AND PersonId=@PersonId";

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при удалении" });
            }

            return Json(new { IsOk = true });
        }

        #endregion

    }
}
