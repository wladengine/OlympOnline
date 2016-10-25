using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;

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
                isValid = pc.ValidateCredentials("myuser", "mypassword");
            }

            return isValid;
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


        }
    }
}