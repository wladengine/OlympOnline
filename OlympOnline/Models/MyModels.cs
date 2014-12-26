using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OlympOnline.Models
{
    public class BaseModel
    {
        public string UILanguage { get; set; }
    }

    public class HomeModel : BaseModel
    {
        public bool IsLogged { get; set; }
    }

    public enum MessageType
    {
        /// <summary>
        /// = 1 in DB
        /// </summary>
        CommonMessage = 1,
        /// <summary>
        /// = 2 in DB
        /// </summary>
        CriticalMessage = 2,
        /// <summary>
        /// = 3 in DB
        /// </summary>
        TipMessage = 3
    }

    public class PersonalMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsClosed { get; set; }
        public MessageType Type { get; set; }
        public DateTime Time { get; set; }
    }

    //-------------------------------------------
    //PERSONAL INFO CLASS
    public class PersonalOffice : BaseModel
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public EducationPerson EducationInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public ParentsPersonInfo ParentInfo { get; set; }
    }

    public class InfoPerson
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }

        public string BirthPlace { get; set; }
        public string BirthDate { get; set; }

        public string Nationality { get; set; }
        public List<SelectListItem> NationalityList { get; set; }
        public string Sex { get; set; }
        public List<SelectListItem> SexList { get; set; }
    }

    public class PassportPerson
    {
        public string PassportType { get; set; }
        public List<SelectListItem> PassportTypeList { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportAuthor { get; set; }
        public string PassportDate { get; set; }
        public string PassportCode { get; set; }
    }

    public class ContactsPerson
    {
        public string MainPhone { get; set; }
        public string SecondPhone { get; set; }

        public string CountryId { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public string Code { get; set; }

        public string RegionId { get; set; }
        public List<SelectListItem> RegionList { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Korpus { get; set; }
        public string Flat { get; set; }

        public bool IsCountryside { get; set; }
    }

    public class EducationPerson
    {
        public string SchoolTypeId { get; set; }
        
        [Required]
        public string SchoolName { get; set; }

        public string SchoolNumber { get; set; }
        public string SchoolCity { get; set; }
        
        public string CountryEducId { get; set; }
        public string RegionEducId { get; set; }
        public string SchoolClassId { get; set; }

        public List<SelectListItem> SchoolTypeList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> RegionList { get; set; }
        public List<SelectListItem> SchoolClassList { get; set; }
    }

    public class AdditionalInfoPerson
    {
        public List<SelectListItem> OlympVserosSubjects { get; set; }
        public List<SelectListItem> OlympVserosStages { get; set; }
        public List<SelectListItem> OlympStatuses { get; set; }
        public List<OtherVseross> VserossOlympBase { get; set; }
        public List<OtherOlympics> OtherOlympBase { get; set; }
        public List<SelectListItem> OtherOlympSubjects { get; set; }
        

        public bool IsDisabled { get; set; }
        public bool IsSirota { get; set; }
    }

    public class ParentsPersonInfo
    {
        public string ParentName { get; set; }
        public string ParentAddress { get; set; }
        [Required]
        public bool FZ_152Agree { get; set; }
    }

    public class OtherOlympics
    {
        public Guid Id { get; set; }
        public string VuzName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }

    public class OtherVseross
    {
        public Guid Id { get; set; }
        public string Level { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
    }
    //-------------------------------------------

    //-------------------------------------------
    //PERSON CLASS
    public class SimplePerson : BaseModel
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public List<SimpleApplication> Applications { get; set; }
        public List<AppendedFile> Files { get; set; }
        public List<PersonalMessage> Messages { get; set; }
    }

    public class SimpleApplication
    {
        public Guid Id { get; set; }
        public string City { get; set; }
        public string Subject { get; set; }
        public string Stage { get; set; }
        public string Date { get; set; }
        public bool Enabled { get; set; }
    }
    public enum ApprovalStatus
    {
        Approved,
        Rejected,
        NotSet
    }
    public class AppendedFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string Comment { get; set; }
        public string FileExtention { get; set; }
        public bool IsShared { get; set; }
        public ApprovalStatus IsApproved { get; set; }
    }
    //-------------------------------------------

    public class ApplicationModel : BaseModel
    {
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> Subjects { get; set; }
    }

    //-------------------------------------------
    public class AppendFilesModel : BaseModel
    {
        public List<AppendedFile> Files { get; set; }
    }

    public class ShortFileInfo
    {
        public string Path { get; set; }
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
    }
    
    //-------------------------------------------
    // ./Application/Index.aspx
    public class ExtApplicationModel : BaseModel
    {
        public Guid Id { get; set; }
        public string City { get; set; }
        public string Subject { get; set; }
        public string Stage { get; set; }
        public string Date { get; set; }
        public bool Enabled { get; set; }
        public string DateOfDisable { get; set; }
        //public List<AppendedFile> Files { get; set; }

        public bool IsFullTime { get; set; }

        public string ComissionAddress { get; set; }
        public string ComissionYaCoord { get; set; }
        public string BlackBoardURL { get; set; }
        public DateTime DateOfApply { get; set; }

        public string TeacherName { get; set; }
        public string BBLogin { get; set; }
    }

    //-------------------------------------------
    // ./Abiturient/MotivateMail.aspx
    public class MotivateMailModel : BaseModel
    {
        public List<SimpleApplication> Apps { get; set; }
        public List<string> lstApps { get; set; }
    }

    public enum EmailConfirmationStatus
    {
        Confirmed,
        WrongTicket,
        WrongEmail,
        FirstEmailSent
    }

    public class EmailConfirmationModel : BaseModel
    {
        public EmailConfirmationStatus RegStatus { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
    }

    public class AccountErrorModel : BaseModel
    {
        public string ErrorHtmlString { get; set; }
    }

    public class DormsModel
    {
        public int? DormsId { get; set; }
        public bool isRegistered { get; set; }
        public bool hasRightsToTimetable { get; set; }
        public DateTime? regDate { get; set; }
        public List<TimetableRow> Rows { get; set; }
        /// <summary>
        /// Юзер из Спб
        /// </summary>
        public bool isSPB { get; set; }
        /// <summary>
        /// Юзер первокурсник
        /// </summary>
        public bool isFirstCourse { get; set; }
        /// <summary>
        /// юзер находится в списке зачисленных
        /// </summary>
        public bool hasInEntered { get; set; }
    }

    public class TimetableRow
    {
        public int Hour { get; set; }
        public List<TimetableCell> Cells { get; set; }
    }

    public class TimetableCell
    {
        public int Minute { get; set; }
        public bool isLocked { get; set; }
        public int CountAbits { get; set; }
        public bool isRegistered { get; set; }
    }

    public class DataRegModel
    {
        public List<SelectListItem> listRegion { get; set; }
    }
}