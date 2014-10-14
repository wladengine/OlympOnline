using OlympOnline.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OlympOnline.Controllers
{
    public static partial class Util
    {
        public static List<SelectListItem> GetNationalityList()
        {
            return CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
        }
        public static List<SelectListItem> GetSexList(string Lang)
        {
            return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = LangPack.GetValue(5, Lang), Value = "M" }, 
                    new SelectListItem() { Text = LangPack.GetValue(6, Lang), Value = "F" }
                };
        }
        public static List<SelectListItem> GetPassportTypeList()
        {
            DataTable tblPsp = Util.AbitDB.GetDataTable("SELECT Id, Name FROM PassportType WHERE 1=@x", new Dictionary<string, object>() { { "@x", 1 } });
            return (from DataRow rw in tblPsp.Rows
                    select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>("Name") }).
                    ToList();
        }
        public static List<SelectListItem> GetCountryList()
        {
            string query = "SELECT Id, Name FROM [Country]";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return (from DataRow rw in tbl.Rows
                    select new SelectListItem()
                    {
                        Value = rw.Field<int>("Id").ToString(),
                        Text = rw.Field<string>("Name")
                    }).ToList();
        }
        public static List<SelectListItem> GetRegionList()
        {
            string query = "SELECT Id, Name FROM Region";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return (from DataRow rw in tbl.Rows
                    select new SelectListItem()
                    {
                        Value = rw.Field<int>("Id").ToString(),
                        Text = rw.Field<string>("Name")
                    }).ToList();
        }
        public static List<SelectListItem> GetSchoolTypeList()
        {
            return SchoolTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
        }
        public static List<SelectListItem> GetSchoolClassList()
        {
            return SchoolClassesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
        }

        public static List<SelectListItem> GetOlympVserosSubjects()
        {
            string quer = "SELECT Id, Name FROM OtherOlympSubject WHERE OlympTypeId = 1";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, null);
            return (from DataRow rw in tblOther.Rows
                    select new SelectListItem() 
                    { 
                        Value = rw["Id"].ToString(), 
                        Text = rw["Name"].ToString() 
                    }).ToList();
        }
        public static List<SelectListItem> GetOtherOlympSubjects()
        {
            string quer = "SELECT Id, Name FROM OtherOlympSubject WHERE OlympTypeId = 2";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, null);
            return (from DataRow rw in tblOther.Rows
                    select new SelectListItem()
                    {
                        Value = rw["Id"].ToString(),
                        Text = rw["Name"].ToString()
                    }).ToList();
        }
        public static List<SelectListItem> GetOtherOlympStatus()
        {
            string quer = "SELECT Id, Name FROM OtherOlympStatus";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, null);
            return (from DataRow rw in tblOther.Rows
                    select new SelectListItem() 
                    { 
                        Value = rw["Id"].ToString(), 
                        Text = rw["Name"].ToString() 
                    }).ToList();
        }
        public static List<SelectListItem> GetOtherOlympStage()
        {
            string quer = "SELECT Id, Name FROM OtherOlympStage";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, null);
            return (from DataRow rw in tblOther.Rows
                    select new SelectListItem()
                    {
                        Value = rw["Id"].ToString(),
                        Text = rw["Name"].ToString()
                    }).ToList();
        }
        public static List<OtherVseross> GetVserossOlympBase(Guid PersonId)
        {
            string quer = "SELECT Id, OtherOlympStatus, OtherOlympSubject, OtherOlympStage FROM extPersonOtherOlympsVseross WHERE PersonId=@PersonId";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            return (from DataRow rw in tblOther.Rows
                    select new OtherVseross()
                    {
                        Id = rw.Field<Guid>("Id"),
                        Status = rw["OtherOlympStatus"].ToString(),
                        Subject = rw["OtherOlympSubject"].ToString(),
                        Level = rw["OtherOlympStage"].ToString()
                    }).ToList();
        }
        public static List<OtherOlympics> GetOtherOlympBase(Guid PersonId)
        {
            string quer = "SELECT Id, VuzName, OtherOlympStatus, OtherOlympSubject FROM extPersonOtherOlymps WHERE PersonId=@PersonId";
            DataTable tblOther = Util.AbitDB.GetDataTable(quer, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            return (from DataRow rw in tblOther.Rows
                    select new OtherOlympics()
                    {
                        Id = rw.Field<Guid>("Id"),
                        Status = rw["OtherOlympStatus"].ToString(),
                        Subject = rw["OtherOlympSubject"].ToString(),
                        VuzName = rw["VuzName"].ToString()
                    }).ToList();
        }
    }
}