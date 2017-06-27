using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.Data;

namespace OlympOnline.Controllers
{
    public static partial class Util
    {
        public static bool GetIsValidAccountInActiveDirectory(string username, string password)
        {
            bool isValid = false;

            // create a "principal context" - e.g. your domain (could be machine, too)
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "RECTORAT"))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(username, password);
            }

            return isValid;
        }

        public static Guid CheckOrCreatePersonFromAccountInActiveDirectory(string username)
        {
            string query = "SELECT [User].Id AS UserId, BBUser.UserId AS BBUserId FROM [User] LEFT JOIN BBUser ON BBUser.UserId=[User].Id WHERE BBUser.BBLogin=@Email";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Email", username);
            DataTable tbl = AbitDB.GetDataTable(query, dic);

            if (tbl.Rows.Count == 0) //если пользователя в базе нет совсем, то создаём записи в User и BBUser
            {
                Guid UserId = CreateNewUserFromAD("", GetUserEmailFromAD(username), username);

                return UserId;
            }
            else
            {
                return tbl.Rows[0].Field<Guid>("UserId");
            }
        }

        public static void GetAccountInfo(string username)
        {
            //PrincipalContext - контекст домена, который используется для всех операций поиска, добавления/изменения данных в ActiveDirectory
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            //Principal ppp = Principal.FindByIdentity(ctx, IdentityType.Name, username);
            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.Name, username);
            string sBirthDate = usr.Description;
            string sSurname = usr.Surname;
            string sName = usr.MiddleName;
            string sSecondName = usr.Name;

            //StudentIMService.Student stud = new StudentIMService.Student();
            //stud.Surname = sSurname;
            //stud.FirstName = sName;
            //stud.SecondName = sSecondName;
            //DateTime dtBirth = DateTime.Now;
            //if (DateTime.TryParse(sBirthDate, out dtBirth))
            //    stud.Birthday = dtBirth.Date.ToShortDateString();
        }

        public static string GetUserEmailFromAD(string username)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            //Principal ppp = Principal.FindByIdentity(ctx, IdentityType.Name, username);
            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.Name, username);
            return usr.EmailAddress;
        }
    }
}