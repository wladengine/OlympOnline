﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OlympOnline.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI;
using Recaptcha;
using System.IO;

namespace OlympOnline.Controllers
{
    public static partial class Util
    {
        #region Fields

        private static SQLClass _abitDB;
        private static SQLClass _studDB;
        private static SQLClass _offlineWorkBase;

        public static int iOlympYear { get; private set; }
        public static string sOlympYear { get; private set; }
        public static bool bUseRedirection { get; private set; }

        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass AbitDB { get { return _abitDB; } }
        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass StudDB { get { return _studDB; } }
        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass OfflineWorkBase { get { return _offlineWorkBase; } }

        public static string ServerAddress { get; set; }
        public static string FilesPath { get; set; }

        public static Dictionary<int, string> SchoolTypesAll { get; set; }
        public static Dictionary<int, string> CountriesAll { get; set; }
        public static Dictionary<int, string> RegionsAll { get; set; }
        public static Dictionary<int, string> SchoolClassesAll { get; set; }

        #endregion

        //статический конструктор
        static Util()
        {
            //abDB = new AbitDB();
            //studDB = new StudDB();
            _abitDB = new SQLClass("Data Source=SRVPRIEM1;Initial Catalog=OlympOnline;Integrated Security=False;User ID=OnlinePriemUser;Password=AllYourBaseAreBelongToUs666+;MultipleActiveResultSets=True;");
            _studDB = new SQLClass("Data Source=81.89.183.21;Initial Catalog=EducationUR;Integrated Security=False;User ID=faculty;Password=parolfaculty;MultipleActiveResultSets=True;");
            _offlineWorkBase = new SQLClass("Data Source=srveducation.ad.pu.ru;Initial Catalog=Priem2012;Integrated Security=false;User ID=PriemReader; Password=kukushonok");

            string query = "SELECT Id, Name FROM {0} WHERE 1=@x ORDER BY Id";
            Dictionary<string, object> dic = new Dictionary<string, object>() { { "@x", 1 } };
            DataTable tbl = new DataTable();

            tbl = _abitDB.GetDataTable(string.Format(query, "SchoolType"), dic);
            SchoolTypesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "Country"), dic);
            CountriesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "Region"), dic);
            RegionsAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "SchoolClass"), dic);
            SchoolClassesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            //FilesPath = @"C:\Users\v.chikhira\Documents\Visual Studio 2010\Projects\OnlinePriem\OnlinePriem\Content\Files\";
            ServerAddress = WebConfigurationManager.AppSettings["ServerName"];//in web.config
            bool bTmp = false;
	        if (!bool.TryParse(WebConfigurationManager.AppSettings["bUseRedirection"], out bTmp))//in web.config
	            bUseRedirection = false;
	        else
	            bUseRedirection = bTmp;
            sOlympYear = WebConfigurationManager.AppSettings["OlympYear"];

            int iTmp = 0;
            if (!int.TryParse(sOlympYear, out iTmp))
                iOlympYear = DateTime.Now.AddMonths(5).Year;
            else
                iOlympYear = iTmp;
        }

        /// <summary>
        /// Возвращает Guid пользователя в базе по указанному SID. Если такого пользователя нет, возвращается пустой Guid
        /// </summary>
        /// <param name="SID"></param>
        /// <returns></returns>
        public static Guid GetIdBySID(string SID)
        {
            Guid id = Guid.Empty;
            string sId = AbitDB.GetStringValue("SELECT Id FROM [User] WHERE SID=@SID", new Dictionary<string, object>() { { "@SID", SID } });
            try
            {
                id = Guid.Parse(sId);
            }
            catch { }

            return id;
        }

        /// <summary>
        /// Возвращает SID пользователя по указанному Guid. Если такого не найдено, возвращается пустая строка
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetSIDById(Guid id)
        {
            return AbitDB.GetStringValue("SELECT SID FROM User WHERE Id=@Id", new Dictionary<string, object>() { { "@Id", id } });
        }

        public static bool CheckRegistrationInfo(string password, string email, out List<string> errList)
        {
            bool res = true;
            errList = new List<string>();
            
            string query = "SELECT Id FROM [User] WHERE [Email]=@Email";
            string result = AbitDB.GetStringValue(query, new Dictionary<string, object>() { { "@Email", email } });
            if (!string.IsNullOrEmpty(result))
            {
                res = false;
                errList.Add("Пользователь с данным адресом электронной почты уже существует");
            }
            if (password.Length < 6)
            {
                res = false;
                errList.Add("Пароль слишком короткий");
            }
            if (!Regex.IsMatch(email, @"^[-a-zA-Z0-9!#$%&'*+/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-zA-Z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-zA-Z][a-zA-Z])$"))
            {
                res = false;
                errList.Add("Некорректный адрес электронной почты");
            }
            return res;
        }

        /// <summary>
        /// Создаёт нового пользователя в базе абитуриентов. Возвращает Guid нового пользователя. Exception в случае проблем при создании.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Guid CreateNewUser(string password, string email)
        {
            Guid id = Guid.NewGuid();
            string sid = MD5Str(id.ToByteArray());
            string md5pwd = MD5Str(password);

            string query = "INSERT INTO [User] (Id, Password, SID, Email, IsApproved, EmailTicket) VALUES (@Id, @Password, @SID, @Email, @IsApproved, @EmailTicket)";
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                {"@Id", id},
                {"@Password", md5pwd},
                {"@SID", sid},
                {"@Email", email},
                {"@IsApproved", false},
                {"@EmailTicket", Guid.NewGuid().ToString("N")},
            };
            AbitDB.ExecuteQuery(query, dic);

            query = "INSERT INTO AuthTicket (UserId, Ticket) VALUES (@UserId, @Ticket)";
            dic.Clear();
            dic.Add("@UserId", id);
            dic.Add("@Ticket", Guid.NewGuid().ToString("N"));
            AbitDB.ExecuteQuery(query, dic);

            query = "INSERT INTO BBUser (Id, UserId, BBPassword, Email) VALUES (@Id, @Id, @BBPassword, @Email)";
            dic.Clear();
            dic.Add("@Id", id);
            dic.Add("@BBPassword", password);
            dic.Add("@Email", email);
            AbitDB.ExecuteQuery(query, dic);

            return id;
        }

        public static Guid CreateNewUserFromAD(string password, string email, string ST)
        {
            Guid id = Guid.NewGuid();
            string sid = MD5Str(id.ToByteArray());
            string md5pwd = MD5Str(password);

            string query = "INSERT INTO [User] (Id, Password, SID, Email, ST, IsApproved, EmailTicket) VALUES (@Id, @Password, @SID, @Email, @ST, @IsApproved, @EmailTicket)";
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                { "@Id", id },
                { "@Password", md5pwd },
                { "@SID", sid },
                { "@ST", ST },
                { "@Email", email },
                { "@IsApproved", false },
                { "@EmailTicket", Guid.NewGuid().ToString("N") },
            };
            AbitDB.ExecuteQuery(query, dic);

            query = "INSERT INTO AuthTicket (UserId, Ticket) VALUES (@UserId, @Ticket)";
            dic.Clear();
            dic.Add("@UserId", id);
            dic.Add("@Ticket", Guid.NewGuid().ToString("N"));
            AbitDB.ExecuteQuery(query, dic);

            query = "INSERT INTO BBUser (Id, UserId, BBLogin, BBPassword, Email) VALUES (@Id, @Id, @BBLogin, @BBPassword, @Email)";
            dic.Clear();
            dic.Add("@Id", id);
            dic.Add("@BBLogin", ST);
            dic.Add("@BBPassword", password);
            dic.Add("@Email", email);
            AbitDB.ExecuteQuery(query, dic);

            query = "UPDATE BBUser SET BBLogin=@BBLogin WHERE Id=@Id";
            AbitDB.ExecuteQuery(query, dic);

            return id;
        }

        public static bool CreateNewSimpleUser(string password, string email, Guid id)
        {
            using (System.Transactions.TransactionScope tran = new System.Transactions.TransactionScope())
            {
                try
                {
                    string sid = MD5Str(id.ToByteArray());
                    string md5pwd = MD5Str(password);

                    string query = "INSERT INTO [User] (Id, Password, SID, Email, IsApproved, EmailTicket) VALUES (@Id, @Password, @SID, @Email, @IsApproved, @EmailTicket)";
                    Dictionary<string, object> dic = new Dictionary<string, object>()
                    {
                        {"@Id", id},
                        {"@Password", md5pwd},
                        {"@SID", sid},
                        {"@Email", email},
                        {"@IsApproved", false},
                        {"@EmailTicket", Guid.NewGuid().ToString("N")}
                    };
                    AbitDB.ExecuteQuery(query, dic);

                    query = "INSERT INTO AuthTicket (UserId, Ticket) VALUES (@UserId, @Ticket)";
                    dic.Clear();
                    dic.Add("@UserId", id);
                    dic.Add("Ticket", Guid.NewGuid().ToString("N"));
                    AbitDB.ExecuteQuery(query, dic);

                    tran.Complete();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Проверяет состояние регистрации пользователя и, если у него не заполнен до конца профиль, то возвращает false. 
        /// IntStage = этап регистрации, на который нужно перекинуть пользователя
        /// </summary>
        /// <param name="PersonId"></param>
        /// <param name="IntStage"></param>
        /// <returns></returns>
        public static bool CheckPersonRegStatus(Guid PersonId, out int IntStage)
        {
            IntStage = 1;
            string query = "SELECT RegistrationStage FROM Person WHERE Id=@Id";
            string stage = AbitDB.GetStringValue(query, new Dictionary<string, object>() { { "@Id", PersonId } });
            if (string.IsNullOrEmpty(stage))
                return false;

            int iStage = 0;
            if (!int.TryParse(stage, out iStage))
                return false;

            IntStage = iStage;
            if (iStage < 100)
                return false;
            else
                return true;
        }

        //public static bool CheckIsForeign(Guid PersonId)
        //{
        //    string query = "SELECT IsForeign FROM [User] WHERE Id=@Id";
        //    string res = AbitDB.GetStringValue(query, new Dictionary<string, object>() { { "@Id", PersonId } });
        //    if (!string.IsNullOrEmpty(res) && string.Compare(res, "true", StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Проверяет пользователя на необходимость блокировки
        /// </summary>
        /// <param name="PersonId"></param>
        /// <returns></returns>
        public static bool CheckPersonReadOnlyStatus(Guid PersonId)
        {
            string query = "SELECT COUNT(Application.Id) FROM [Application] INNER JOIN Olympiad ON Olympiad.Id=[Application].OlympiadId WHERE PersonId=@PersonId AND Enabled=@Enabled";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Enabled", true);

            int res = (int)AbitDB.GetValue(query, dic);
            if (res > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Возвращает MD5-строку от строки-источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Str(string source)
        {
            byte[] md5 = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(source));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает SSHA-строку от строки-источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA512Str(string source)
        {
            byte[] hash = SHA512CryptoServiceProvider.Create("SHA512").ComputeHash(Encoding.Unicode.GetBytes(source));
            //MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(source));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает MD5-строку от byte[] источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Str(byte[] source)
        {
            byte[] md5 = MD5.Create().ComputeHash(source);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GetFileSize(int fileSize)
        {
            if (fileSize > 1024)
            {
                string fSizeRaw = ((double)fileSize / 1024.0).ToString();
                int limpos = fSizeRaw.IndexOf(',');
                int delta = fSizeRaw.Length - limpos;
                if (delta > 2)
                    return fSizeRaw.Substring(0, limpos + 2) + " Kb";
                else
                    return fSizeRaw + " Kb";
            }
            else if (fileSize > 1024 * 1024)
            {
                string fSizeRaw = ((double)fileSize / (1024.0 * 1024.0)).ToString();
                int limpos = fSizeRaw.IndexOf(',');
                int delta = fSizeRaw.Length - limpos;
                if (delta > 2)
                    return fSizeRaw.Substring(0, limpos + 2) + " Mb";
                else
                    return fSizeRaw + " Mb";
            }
            return fileSize + " b";
        }

        public static bool CheckAuthCookies(HttpCookieCollection cookies, out Guid personId)
        {
            SetThreadCultureByCookies(cookies);
            personId = Guid.Empty;
            string sid = "";
            if (cookies["sid"] != null)
                sid = cookies["sid"].Value;
            else
                return false;

            string ticket = "";
            if (cookies["t"] != null)
                ticket = cookies["t"].Value;
            else
                return false;

            if (!string.IsNullOrEmpty(sid))
                personId = GetIdBySID(sid);
            else
                return false;

            Guid uid = personId;

            string t = AbitDB.GetStringValue("SELECT Ticket FROM AuthTicket WHERE UserId=@UserId", new Dictionary<string, object>() { { "@UserId", personId } });
            //string t = ABDB.AuthTicket.Where(x => x.UserId == uid).Select(x => x.Ticket).FirstOrDefault();
            if (string.IsNullOrEmpty(t) || (t != ticket))
                return false;

            if (personId == Guid.Empty)
                return false;

            //Оптимистичное блокирование - только проверка ограничений
            return true;
        }

        public static bool CheckAdminRights(Guid personId)
        {
            string query = "SELECT Id, Approved FROM Admins WHERE Id=@Id AND Approved='True'";
            try
            {
                DataTable tbl = AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", personId } });
                if (tbl.Rows.Count == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static void SetThreadCultureByCookies(this HttpCookieCollection cookies)
        {
            string uilang = cookies["uilang"] != null ? cookies["uilang"].Value : "";
            if (uilang.StrCmp("en"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture
                    = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture
                    = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
            }
        }

        public static bool SetUILang(Guid PersonId, string lang)
        {
            if (lang.StrCmp("ru"))
                lang = "ru";
            else if (lang.StrCmp("en"))
                lang = "en";

            string query = "UPDATE [User] SET UILanguage=@Language WHERE Id=@Id";
            try
            {
                AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Language", lang }, { "@Id", PersonId } });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetUILang(Guid PersonId)
        {
            string query = "SELECT UILanguage, IsForeign FROM [User] WHERE Id=@Id";
            DataTable tbl = AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", PersonId } });
            if (tbl.Rows.Count == 0)
                return "ru";
            string UILang = tbl.Rows[0].Field<string>("UILanguage");
            bool IsForeigner = tbl.Rows[0].Field<bool?>("IsForeign").HasValue ? tbl.Rows[0].Field<bool>("IsForeign") : false;
            if (string.IsNullOrEmpty(UILang))
            {
                if (IsForeigner)
                    UILang = "en";
                else
                    UILang = "ru";
            }

            return UILang;
        }

        public static string StrictUILang(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return "ru";

            if (lang.StrCmp("ru"))
                return "ru";
            if (lang.StrCmp("en"))
                return "en";
            
            return "ru";
        }

        /// <summary>
        /// Парсит строку в число. Если строка кривая, то 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ParseSafe(string str)
        {
            int ret;
            if (!int.TryParse(str, out ret))
                ret = 0;

            return ret;
        }

        /// <summary>
        /// Возвращает контент-тип файла по указанному расширению
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetMimeFromExtention(string ext)
        {
            switch (ext)
            {
                case ".323": return "text/h323";
                case ".3g2": return "video/3gpp2";
                case ".3gp": return "video/3gpp";
                case ".3gp2": return "video/3gpp2";
                case ".3gpp": return "video/3gpp";
                case ".7z": return "application/x-7z-compressed";
                case ".aa": return "audio/audible";
                case ".AAC": return "audio/aac";
                case ".aaf": return "application/octet-stream";
                case ".aax": return "audio/vnd.audible.aax";
                case ".ac3": return "audio/ac3";
                case ".aca": return "application/octet-stream";
                case ".accda": return "application/msaccess.addin";
                case ".accdb": return "application/msaccess";
                case ".accdc": return "application/msaccess.cab";
                case ".accde": return "application/msaccess";
                case ".accdr": return "application/msaccess.runtime";
                case ".accdt": return "application/msaccess";
                case ".accdw": return "application/msaccess.webapplication";
                case ".accft": return "application/msaccess.ftemplate";
                case ".acx": return "application/internet-property-stream";
                case ".AddIn": return "text/xml";
                case ".ade": return "application/msaccess";
                case ".adobebridge": return "application/x-bridge-url";
                case ".adp": return "application/msaccess";
                case ".ADT": return "audio/vnd.dlna.adts";
                case ".ADTS": return "audio/aac";
                case ".afm": return "application/octet-stream";
                case ".ai": return "application/postscript";
                case ".aif": return "audio/x-aiff";
                case ".aifc": return "audio/aiff";
                case ".aiff": return "audio/aiff";
                case ".air": return "application/vnd.adobe.air-application-installer-package+zip";
                case ".amc": return "application/x-mpeg";
                case ".application": return "application/x-ms-application";
                case ".art": return "image/x-jg";
                case ".asa": return "application/xml";
                case ".asax": return "application/xml";
                case ".ascx": return "application/xml";
                case ".asd": return "application/octet-stream";
                case ".asf": return "video/x-ms-asf";
                case ".ashx": return "application/xml";
                case ".asi": return "application/octet-stream";
                case ".asm": return "text/plain";
                case ".asmx": return "application/xml";
                case ".aspx": return "application/xml";
                case ".asr": return "video/x-ms-asf";
                case ".asx": return "video/x-ms-asf";
                case ".atom": return "application/atom+xml";
                case ".au": return "audio/basic";
                case ".avi": return "video/x-msvideo";
                case ".axs": return "application/olescript";
                case ".bas": return "text/plain";
                case ".bcpio": return "application/x-bcpio";
                case ".bin": return "application/octet-stream";
                case ".bmp": return "image/bmp";
                case ".c": return "text/plain";
                case ".cab": return "application/octet-stream";
                case ".caf": return "audio/x-caf";
                case ".calx": return "application/vnd.ms-office.calx";
                case ".cat": return "application/vnd.ms-pki.seccat";
                case ".cc": return "text/plain";
                case ".cd": return "text/plain";
                case ".cdda": return "audio/aiff";
                case ".cdf": return "application/x-cdf";
                case ".cer": return "application/x-x509-ca-cert";
                case ".chm": return "application/octet-stream";
                case ".class": return "application/x-java-applet";
                case ".clp": return "application/x-msclip";
                case ".cmx": return "image/x-cmx";
                case ".cnf": return "text/plain";
                case ".cod": return "image/cis-cod";
                case ".config": return "application/xml";
                case ".contact": return "text/x-ms-contact";
                case ".coverage": return "application/xml";
                case ".cpio": return "application/x-cpio";
                case ".cpp": return "text/plain";
                case ".crd": return "application/x-mscardfile";
                case ".crl": return "application/pkix-crl";
                case ".crt": return "application/x-x509-ca-cert";
                case ".cs": return "text/plain";
                case ".csdproj": return "text/plain";
                case ".csh": return "application/x-csh";
                case ".csproj": return "text/plain";
                case ".css": return "text/css";
                case ".csv": return "application/octet-stream";
                case ".cur": return "application/octet-stream";
                case ".cxx": return "text/plain";
                case ".dat": return "application/octet-stream";
                case ".datasource": return "application/xml";
                case ".dbproj": return "text/plain";
                case ".dcr": return "application/x-director";
                case ".def": return "text/plain";
                case ".deploy": return "application/octet-stream";
                case ".der": return "application/x-x509-ca-cert";
                case ".dgml": return "application/xml";
                case ".dib": return "image/bmp";
                case ".dif": return "video/x-dv";
                case ".dir": return "application/x-director";
                case ".disco": return "text/xml";
                case ".dll": return "application/x-msdownload";
                case ".dll.config": return "text/xml";
                case ".dlm": return "text/dlm";
                case ".doc": return "application/msword";
                case ".docm": return "application/vnd.ms-word.document.macroEnabled.12";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".dot": return "application/msword";
                case ".dotm": return "application/vnd.ms-word.template.macroEnabled.12";
                case ".dotx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".dsp": return "application/octet-stream";
                case ".dsw": return "text/plain";
                case ".dtd": return "text/xml";
                case ".dtsConfig": return "text/xml";
                case ".dv": return "video/x-dv";
                case ".dvi": return "application/x-dvi";
                case ".dwf": return "drawing/x-dwf";
                case ".dwp": return "application/octet-stream";
                case ".dxr": return "application/x-director";
                case ".eml": return "message/rfc822";
                case ".emz": return "application/octet-stream";
                case ".eot": return "application/octet-stream";
                case ".eps": return "application/postscript";
                case ".etl": return "application/etl";
                case ".etx": return "text/x-setext";
                case ".evy": return "application/envoy";
                case ".exe": return "application/octet-stream";
                case ".exe.config": return "text/xml";
                case ".fdf": return "application/vnd.fdf";
                case ".fif": return "application/fractals";
                case ".filters": return "Application/xml";
                case ".fla": return "application/octet-stream";
                case ".flr": return "x-world/x-vrml";
                case ".flv": return "video/x-flv";
                case ".fsscript": return "application/fsharp-script";
                case ".fsx": return "application/fsharp-script";
                case ".generictest": return "application/xml";
                case ".gif": return "image/gif";
                case ".group": return "text/x-ms-group";
                case ".gsm": return "audio/x-gsm";
                case ".gtar": return "application/x-gtar";
                case ".gz": return "application/x-gzip";
                case ".h": return "text/plain";
                case ".hdf": return "application/x-hdf";
                case ".hdml": return "text/x-hdml";
                case ".hhc": return "application/x-oleobject";
                case ".hhk": return "application/octet-stream";
                case ".hhp": return "application/octet-stream";
                case ".hlp": return "application/winhlp";
                case ".hpp": return "text/plain";
                case ".hqx": return "application/mac-binhex40";
                case ".hta": return "application/hta";
                case ".htc": return "text/x-component";
                case ".htm": return "text/html";
                case ".html": return "text/html";
                case ".htt": return "text/webviewhtml";
                case ".hxa": return "application/xml";
                case ".hxc": return "application/xml";
                case ".hxd": return "application/octet-stream";
                case ".hxe": return "application/xml";
                case ".hxf": return "application/xml";
                case ".hxh": return "application/octet-stream";
                case ".hxi": return "application/octet-stream";
                case ".hxk": return "application/xml";
                case ".hxq": return "application/octet-stream";
                case ".hxr": return "application/octet-stream";
                case ".hxs": return "application/octet-stream";
                case ".hxt": return "text/html";
                case ".hxv": return "application/xml";
                case ".hxw": return "application/octet-stream";
                case ".hxx": return "text/plain";
                case ".i": return "text/plain";
                case ".ico": return "image/x-icon";
                case ".ics": return "application/octet-stream";
                case ".idl": return "text/plain";
                case ".ief": return "image/ief";
                case ".iii": return "application/x-iphone";
                case ".inc": return "text/plain";
                case ".inf": return "application/octet-stream";
                case ".inl": return "text/plain";
                case ".ins": return "application/x-internet-signup";
                case ".ipa": return "application/x-itunes-ipa";
                case ".ipg": return "application/x-itunes-ipg";
                case ".ipproj": return "text/plain";
                case ".ipsw": return "application/x-itunes-ipsw";
                case ".iqy": return "text/x-ms-iqy";
                case ".isp": return "application/x-internet-signup";
                case ".ite": return "application/x-itunes-ite";
                case ".itlp": return "application/x-itunes-itlp";
                case ".itms": return "application/x-itunes-itms";
                case ".itpc": return "application/x-itunes-itpc";
                case ".IVF": return "video/x-ivf";
                case ".jar": return "application/java-archive";
                case ".java": return "application/octet-stream";
                case ".jck": return "application/liquidmotion";
                case ".jcz": return "application/liquidmotion";
                case ".jfif": return "image/pjpeg";
                case ".jnlp": return "application/x-java-jnlp-file";
                case ".jpb": return "application/octet-stream";
                case ".jpe": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".jpg": return "image/jpeg";
                case ".js": return "application/x-javascript";
                case ".jsx": return "text/jscript";
                case ".jsxbin": return "text/plain";
                case ".latex": return "application/x-latex";
                case ".library-ms": return "application/windows-library+xml";
                case ".lit": return "application/x-ms-reader";
                case ".loadtest": return "application/xml";
                case ".lpk": return "application/octet-stream";
                case ".lsf": return "video/x-la-asf";
                case ".lst": return "text/plain";
                case ".lsx": return "video/x-la-asf";
                case ".lzh": return "application/octet-stream";
                case ".m13": return "application/x-msmediaview";
                case ".m14": return "application/x-msmediaview";
                case ".m1v": return "video/mpeg";
                case ".m2t": return "video/vnd.dlna.mpeg-tts";
                case ".m2ts": return "video/vnd.dlna.mpeg-tts";
                case ".m2v": return "video/mpeg";
                case ".m3u": return "audio/x-mpegurl";
                case ".m3u8": return "audio/x-mpegurl";
                case ".m4a": return "audio/m4a";
                case ".m4b": return "audio/m4b";
                case ".m4p": return "audio/m4p";
                case ".m4r": return "audio/x-m4r";
                case ".m4v": return "video/x-m4v";
                case ".mac": return "image/x-macpaint";
                case ".mak": return "text/plain";
                case ".man": return "application/x-troff-man";
                case ".manifest": return "application/x-ms-manifest";
                case ".map": return "text/plain";
                case ".master": return "application/xml";
                case ".mda": return "application/msaccess";
                case ".mdb": return "application/x-msaccess";
                case ".mde": return "application/msaccess";
                case ".mdp": return "application/octet-stream";
                case ".me": return "application/x-troff-me";
                case ".mfp": return "application/x-shockwave-flash";
                case ".mht": return "message/rfc822";
                case ".mhtml": return "message/rfc822";
                case ".mid": return "audio/mid";
                case ".midi": return "audio/mid";
                case ".mix": return "application/octet-stream";
                case ".mk": return "text/plain";
                case ".mmf": return "application/x-smaf";
                case ".mno": return "text/xml";
                case ".mny": return "application/x-msmoney";
                case ".mod": return "video/mpeg";
                case ".mov": return "video/quicktime";
                case ".movie": return "video/x-sgi-movie";
                case ".mp2": return "video/mpeg";
                case ".mp2v": return "video/mpeg";
                case ".mp3": return "audio/mpeg";
                case ".mp4": return "video/mp4";
                case ".mp4v": return "video/mp4";
                case ".mpa": return "video/mpeg";
                case ".mpe": return "video/mpeg";
                case ".mpeg": return "video/mpeg";
                case ".mpf": return "application/vnd.ms-mediapackage";
                case ".mpg": return "video/mpeg";
                case ".mpp": return "application/vnd.ms-project";
                case ".mpv2": return "video/mpeg";
                case ".mqv": return "video/quicktime";
                case ".ms": return "application/x-troff-ms";
                case ".msi": return "application/octet-stream";
                case ".mso": return "application/octet-stream";
                case ".mts": return "video/vnd.dlna.mpeg-tts";
                case ".mtx": return "application/xml";
                case ".mvb": return "application/x-msmediaview";
                case ".mvc": return "application/x-miva-compiled";
                case ".mxp": return "application/x-mmxp";
                case ".nc": return "application/x-netcdf";
                case ".nsc": return "video/x-ms-asf";
                case ".nws": return "message/rfc822";
                case ".ocx": return "application/octet-stream";
                case ".oda": return "application/oda";
                case ".odc": return "text/x-ms-odc";
                case ".odh": return "text/plain";
                case ".odl": return "text/plain";
                case ".odp": return "application/vnd.oasis.opendocument.presentation";
                case ".ods": return "application/oleobject";
                case ".odt": return "application/vnd.oasis.opendocument.text";
                case ".one": return "application/onenote";
                case ".onea": return "application/onenote";
                case ".onepkg": return "application/onenote";
                case ".onetmp": return "application/onenote";
                case ".onetoc": return "application/onenote";
                case ".onetoc2": return "application/onenote";
                case ".orderedtest": return "application/xml";
                case ".osdx": return "application/opensearchdescription+xml";
                case ".p10": return "application/pkcs10";
                case ".p12": return "application/x-pkcs12";
                case ".p7b": return "application/x-pkcs7-certificates";
                case ".p7c": return "application/pkcs7-mime";
                case ".p7m": return "application/pkcs7-mime";
                case ".p7r": return "application/x-pkcs7-certreqresp";
                case ".p7s": return "application/pkcs7-signature";
                case ".pbm": return "image/x-portable-bitmap";
                case ".pcast": return "application/x-podcast";
                case ".pct": return "image/pict";
                case ".pcx": return "application/octet-stream";
                case ".pcz": return "application/octet-stream";
                case ".pdf": return "application/pdf";
                case ".pfb": return "application/octet-stream";
                case ".pfm": return "application/octet-stream";
                case ".pfx": return "application/x-pkcs12";
                case ".pgm": return "image/x-portable-graymap";
                case ".pic": return "image/pict";
                case ".pict": return "image/pict";
                case ".pkgdef": return "text/plain";
                case ".pkgundef": return "text/plain";
                case ".pko": return "application/vnd.ms-pki.pko";
                case ".pls": return "audio/scpls";
                case ".pma": return "application/x-perfmon";
                case ".pmc": return "application/x-perfmon";
                case ".pml": return "application/x-perfmon";
                case ".pmr": return "application/x-perfmon";
                case ".pmw": return "application/x-perfmon";
                case ".png": return "image/png";
                case ".pnm": return "image/x-portable-anymap";
                case ".pnt": return "image/x-macpaint";
                case ".pntg": return "image/x-macpaint";
                case ".pnz": return "image/png";
                case ".pot": return "application/vnd.ms-powerpoint";
                case ".potm": return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case ".potx": return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case ".ppa": return "application/vnd.ms-powerpoint";
                case ".ppam": return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case ".ppm": return "image/x-portable-pixmap";
                case ".pps": return "application/vnd.ms-powerpoint";
                case ".ppsm": return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case ".ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".pptm": return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".prf": return "application/pics-rules";
                case ".prm": return "application/octet-stream";
                case ".prx": return "application/octet-stream";
                case ".ps": return "application/postscript";
                case ".psc1": return "application/PowerShell";
                case ".psd": return "application/octet-stream";
                case ".psess": return "application/xml";
                case ".psm": return "application/octet-stream";
                case ".psp": return "application/octet-stream";
                case ".pub": return "application/x-mspublisher";
                case ".pwz": return "application/vnd.ms-powerpoint";
                case ".qht": return "text/x-html-insertion";
                case ".qhtm": return "text/x-html-insertion";
                case ".qt": return "video/quicktime";
                case ".qti": return "image/x-quicktime";
                case ".qtif": return "image/x-quicktime";
                case ".qtl": return "application/x-quicktimeplayer";
                case ".qxd": return "application/octet-stream";
                case ".ra": return "audio/x-pn-realaudio";
                case ".ram": return "audio/x-pn-realaudio";
                case ".rar": return "application/octet-stream";
                case ".ras": return "image/x-cmu-raster";
                case ".rat": return "application/rat-file";
                case ".rc": return "text/plain";
                case ".rc2": return "text/plain";
                case ".rct": return "text/plain";
                case ".rdlc": return "application/xml";
                case ".resx": return "application/xml";
                case ".rf": return "image/vnd.rn-realflash";
                case ".rgb": return "image/x-rgb";
                case ".rgs": return "text/plain";
                case ".rm": return "application/vnd.rn-realmedia";
                case ".rmi": return "audio/mid";
                case ".rmp": return "application/vnd.rn-rn_music_package";
                case ".roff": return "application/x-troff";
                case ".rpm": return "audio/x-pn-realaudio-plugin";
                case ".rqy": return "text/x-ms-rqy";
                case ".rtf": return "application/rtf";
                case ".rtx": return "text/richtext";
                case ".ruleset": return "application/xml";
                case ".s": return "text/plain";
                case ".safariextz": return "application/x-safari-safariextz";
                case ".scd": return "application/x-msschedule";
                case ".sct": return "text/scriptlet";
                case ".sd2": return "audio/x-sd2";
                case ".sdp": return "application/sdp";
                case ".sea": return "application/octet-stream";
                case ".searchConnector-ms": return "application/windows-search-connector+xml";
                case ".setpay": return "application/set-payment-initiation";
                case ".setreg": return "application/set-registration-initiation";
                case ".settings": return "application/xml";
                case ".sgimb": return "application/x-sgimb";
                case ".sgml": return "text/sgml";
                case ".sh": return "application/x-sh";
                case ".shar": return "application/x-shar";
                case ".shtml": return "text/html";
                case ".sit": return "application/x-stuffit";
                case ".sitemap": return "application/xml";
                case ".skin": return "application/xml";
                case ".sldm": return "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                case ".sldx": return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case ".slk": return "application/vnd.ms-excel";
                case ".sln": return "text/plain";
                case ".slupkg-ms": return "application/x-ms-license";
                case ".smd": return "audio/x-smd";
                case ".smi": return "application/octet-stream";
                case ".smx": return "audio/x-smd";
                case ".smz": return "audio/x-smd";
                case ".snd": return "audio/basic";
                case ".snippet": return "application/xml";
                case ".snp": return "application/octet-stream";
                case ".sol": return "text/plain";
                case ".sor": return "text/plain";
                case ".spc": return "application/x-pkcs7-certificates";
                case ".spl": return "application/futuresplash";
                case ".src": return "application/x-wais-source";
                case ".srf": return "text/plain";
                case ".SSISDeploymentManifest": return "text/xml";
                case ".ssm": return "application/streamingmedia";
                case ".sst": return "application/vnd.ms-pki.certstore";
                case ".stl": return "application/vnd.ms-pki.stl";
                case ".sv4cpio": return "application/x-sv4cpio";
                case ".sv4crc": return "application/x-sv4crc";
                case ".svc": return "application/xml";
                case ".swf": return "application/x-shockwave-flash";
                case ".t": return "application/x-troff";
                case ".tar": return "application/x-tar";
                case ".tcl": return "application/x-tcl";
                case ".testrunconfig": return "application/xml";
                case ".testsettings": return "application/xml";
                case ".tex": return "application/x-tex";
                case ".texi": return "application/x-texinfo";
                case ".texinfo": return "application/x-texinfo";
                case ".tgz": return "application/x-compressed";
                case ".thmx": return "application/vnd.ms-officetheme";
                case ".thn": return "application/octet-stream";
                case ".tif": return "image/tiff";
                case ".tiff": return "image/tiff";
                case ".tlh": return "text/plain";
                case ".tli": return "text/plain";
                case ".toc": return "application/octet-stream";
                case ".tr": return "application/x-troff";
                case ".trm": return "application/x-msterminal";
                case ".trx": return "application/xml";
                case ".ts": return "video/vnd.dlna.mpeg-tts";
                case ".tsv": return "text/tab-separated-values";
                case ".ttf": return "application/octet-stream";
                case ".tts": return "video/vnd.dlna.mpeg-tts";
                case ".txt": return "text/plain";
                case ".u32": return "application/octet-stream";
                case ".uls": return "text/iuls";
                case ".user": return "text/plain";
                case ".ustar": return "application/x-ustar";
                case ".vb": return "text/plain";
                case ".vbdproj": return "text/plain";
                case ".vbk": return "video/mpeg";
                case ".vbproj": return "text/plain";
                case ".vbs": return "text/vbscript";
                case ".vcf": return "text/x-vcard";
                case ".vcproj": return "Application/xml";
                case ".vcs": return "text/plain";
                case ".vcxproj": return "Application/xml";
                case ".vddproj": return "text/plain";
                case ".vdp": return "text/plain";
                case ".vdproj": return "text/plain";
                case ".vdx": return "application/vnd.ms-visio.viewer";
                case ".vml": return "text/xml";
                case ".vscontent": return "application/xml";
                case ".vsct": return "text/xml";
                case ".vsd": return "application/vnd.visio";
                case ".vsi": return "application/ms-vsi";
                case ".vsix": return "application/vsix";
                case ".vsixlangpack": return "text/xml";
                case ".vsixmanifest": return "text/xml";
                case ".vsmdi": return "application/xml";
                case ".vspscc": return "text/plain";
                case ".vss": return "application/vnd.visio";
                case ".vsscc": return "text/plain";
                case ".vssettings": return "text/xml";
                case ".vssscc": return "text/plain";
                case ".vst": return "application/vnd.visio";
                case ".vstemplate": return "text/xml";
                case ".vsto": return "application/x-ms-vsto";
                case ".vsw": return "application/vnd.visio";
                case ".vsx": return "application/vnd.visio";
                case ".vtx": return "application/vnd.visio";
                case ".wav": return "audio/wav";
                case ".wave": return "audio/wav";
                case ".wax": return "audio/x-ms-wax";
                case ".wbk": return "application/msword";
                case ".wbmp": return "image/vnd.wap.wbmp";
                case ".wcm": return "application/vnd.ms-works";
                case ".wdb": return "application/vnd.ms-works";
                case ".wdp": return "image/vnd.ms-photo";
                case ".webarchive": return "application/x-safari-webarchive";
                case ".webtest": return "application/xml";
                case ".wiq": return "application/xml";
                case ".wiz": return "application/msword";
                case ".wks": return "application/vnd.ms-works";
                case ".WLMP": return "application/wlmoviemaker";
                case ".wlpginstall": return "application/x-wlpg-detect";
                case ".wlpginstall3": return "application/x-wlpg3-detect";
                case ".wm": return "video/x-ms-wm";
                case ".wma": return "audio/x-ms-wma";
                case ".wmd": return "application/x-ms-wmd";
                case ".WMD": return "application/x-ms-wmd";
                case ".wmf": return "application/x-msmetafile";
                case ".wml": return "text/vnd.wap.wml";
                case ".wmlc": return "application/vnd.wap.wmlc";
                case ".wmls": return "text/vnd.wap.wmlscript";
                case ".wmlsc": return "application/vnd.wap.wmlscriptc";
                case ".wmp": return "video/x-ms-wmp";
                case ".wmv": return "video/x-ms-wmv";
                case ".wmx": return "video/x-ms-wmx";
                case ".wmz": return "application/x-ms-wmz";
                case ".wpl": return "application/vnd.ms-wpl";
                case ".wps": return "application/vnd.ms-works";
                case ".wri": return "application/x-mswrite";
                case ".wrl": return "x-world/x-vrml";
                case ".wrz": return "x-world/x-vrml";
                case ".wsc": return "text/scriptlet";
                case ".wsdl": return "text/xml";
                case ".wvx": return "video/x-ms-wvx";
                case ".x": return "application/directx";
                case ".xaf": return "x-world/x-vrml";
                case ".xaml": return "application/xaml+xml";
                case ".xap": return "application/x-silverlight-app";
                case ".xbap": return "application/x-ms-xbap";
                case ".xbm": return "image/x-xbitmap";
                case ".xdr": return "text/plain";
                case ".xht": return "application/xhtml+xml";
                case ".xhtml": return "application/xhtml+xml";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlam": return "application/vnd.ms-excel.addin.macroEnabled.12";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xld": return "application/vnd.ms-excel";
                case ".xlk": return "application/vnd.ms-excel";
                case ".xll": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsb": return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case ".xlsm": return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xltm": return "application/vnd.ms-excel.template.macroEnabled.12";
                case ".xltx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case ".xlw": return "application/vnd.ms-excel";
                case ".xml": return "text/xml";
                case ".xmta": return "application/xml";
                case ".xof": return "x-world/x-vrml";
                case ".XOML": return "text/plain";
                case ".xpm": return "image/x-xpixmap";
                case ".xps": return "application/vnd.ms-xpsdocument";
                case ".xrm-ms": return "text/xml";
                case ".xsc": return "application/xml";
                case ".xsd": return "text/xml";
                case ".xsf": return "text/xml";
                case ".xsl": return "text/xml";
                case ".xslt": return "text/xml";
                case ".xsn": return "application/octet-stream";
                case ".xss": return "application/xml";
                case ".xtp": return "application/octet-stream";
                case ".xwd": return "image/x-xwindowdump";
                case ".z": return "application/x-compress";
                case ".zip": return "application/x-zip-compressed";

                default:
                    return "application/octet-stream";
            }
        }

        /// <summary>
        /// Добавляет куки авторизации к указанной HttpCookieCollection
        /// </summary>
        /// <param name="outCookies"></param>
        /// <param name="userId"></param>
        /// <param name="usrTime"></param>
        /// <param name="createPersistCookie"></param>
        public static void SetAuthCookies(this HttpCookieCollection outCookies, Guid userId, DateTime usrTime, bool createPersistCookie)
        {
            //User person = ABDB.User.Where(x => x.Id == userId).FirstOrDefault();
            string query = "SELECT SID, Ticket FROM [User] LEFT JOIN AuthTicket ON AuthTicket.UserId=[User].Id " + 
                "WHERE [User].Id=@Id";
            DataTable tbl = AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", userId } });
            string sid = tbl.Rows[0].Field<string>("SID");
                //AbitDB.GetStringValue("SELECT SID FROM [User] WHERE Id=@Id", new Dictionary<string, object>() { { "@Id", userId } });
            if (string.IsNullOrEmpty(sid))
                return;

            string ticket = tbl.Rows[0].Field<string>("Ticket");
                //AbitDB.GetStringValue("SELECT Ticket FROM AuthTicket WHERE UserId=@Id", new Dictionary<string, object>() { { "@Id", userId } });
            //AuthTicket ticket = ABDB.AuthTicket.Where(x => x.UserId == userId).FirstOrDefault();
            if (string.IsNullOrEmpty(ticket))
                return;

            if (outCookies["sid"] != null)
            {
                outCookies["sid"].Value = sid;
                outCookies["sid"].Path = "/";
                outCookies["sid"].Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2);
            }
            else
                outCookies.Add(new HttpCookie("sid") { Path = "/", Value = sid, Expires = createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2) });

            if (outCookies["t"] != null)
            {
                outCookies["t"].Value = ticket;
                outCookies["t"].Path = "/";
                outCookies["t"].Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2);
            }
            else
                outCookies.Add(new HttpCookie("t") { Path = "/", Value = ticket, Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2) });
        }

        public static bool StrCmp(this string source, string str)
        {
            return (source.IndexOf(str, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Добавляет в коллекцию Пару ключ-значение. Если value == null, то  value = DBNull
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItem(this Dictionary<string, object> dic, string key, object value)
        {
            if (dic.ContainsKey(key))
                return;

            if (value == null)
                value = DBNull.Value;

            dic.Add(key, value);
        }

        public static bool CheckCaptcha(HttpRequestBase request, out string errMsg)
        {

            string ChallengeFieldKey = "recaptcha_challenge_field";
            string ResponseFieldKey = "recaptcha_response_field";

            var captchaChallengeValue = request.Form[ChallengeFieldKey];
            var captchaResponseValue = request.Form[ResponseFieldKey];

            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {

                PrivateKey = WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
                RemoteIP = request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };
            try
            {
                var recaptchaResponse = captchaValidtor.Validate();
                errMsg = recaptchaResponse.ErrorMessage;
                return recaptchaResponse.IsValid;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        public static List<PersonalMessage> GetNewPersonalMessages(Guid PersonId)
        {
            List<PersonalMessage> lst = new List<PersonalMessage>();

            string query = "SELECT Id, Type, Text, Time FROM PersonalMessage WHERE PersonId=@PersonId AND IsRead=@IsRead ORDER BY Time";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@IsRead", false);
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var rawMessages =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Id = rw.Field<Guid>("Id").ToString("N"),
                         Type = rw.Field<int>("Type"),
                         Text = rw.Field<string>("Text"),
                         Time = rw.Field<DateTime>("Time")
                     }).ToList();

                foreach (var tmp in rawMessages)
                {
                    PersonalMessage msg = new PersonalMessage();
                    msg.Id = tmp.Id;
                    switch (tmp.Type)
                    {
                        case 1: { msg.Type = MessageType.CommonMessage; break; }
                        case 2: { msg.Type = MessageType.CriticalMessage; break; }
                        case 3: { msg.Type = MessageType.TipMessage; break; }
                        default: { msg.Type = MessageType.CommonMessage; break; }
                    }
                    msg.Text = tmp.Text;
                    msg.Time = tmp.Time;

                    lst.Add(msg);
                }
            }
            catch { }


            return lst;
        }
    }
}
    /*
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string ChallengeFieldKey = "recaptcha_challenge_field";
        private const string ResponseFieldKey = "recaptcha_response_field";

        /// <summary>
        /// Called before the action method is invoked
        /// </summary>
        /// <param name="filterContext">Information about the current request and action</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[ChallengeFieldKey];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[ResponseFieldKey];
            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {

                PrivateKey = WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };

            var recaptchaResponse = captchaValidtor.Validate();

            // this will push the result value into a parameter in our Action
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

            base.OnActionExecuting(filterContext);

            // Add string to Trace for testing
            //filterContext.HttpContext.Trace.Write("Log: OnActionExecuting", String.Format("Calling {0}", filterContext.ActionDescriptor.ActionName));
        }
    }
*/