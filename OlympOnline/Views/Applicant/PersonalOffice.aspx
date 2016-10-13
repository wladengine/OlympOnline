<%@ Import Namespace="OnlinePriem" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Applicant/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OlympOnline.Models.PersonalOffice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Личный кабинет
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Анкета</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script>
    $('#UILink').hide();
</script>
<% if (1 == 0)/* типа затычка, чтобы VS видела скрипты */
   {
%>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery.validate-vsdoc.js"></script>
<% } %>
<% if (Model.Stage == 1)
   {
%>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    <script language="javascript" type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            <% if (!Model.Enabled)
               { %>
            $('input').attr('readonly', 'readonly');
            $('select').attr('disabled', 'disabled');
            <% } %>
            
            <% if (Model.Enabled)
               { %>
            $("#PersonInfo_BirthDate").datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "focus",
                yearRange: '1920:2007',
                defaultDate: '-17y',
            });
            $.datepicker.regional["ru"];
            <% } %>

            $('#PersonInfo_Surname').keyup( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').keyup( function() { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').keyup( function() { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').keyup( function() { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').keyup( function() { setTimeout(CheckBirthPlace) });
            $('#PersonInfo_Surname').blur( function() { setTimeout(CheckSurname) });
            $('#PersonInfo_Name').blur( function() { setTimeout(CheckName) });
            $('#PersonInfo_SecondName').blur( function() { setTimeout(CheckSecondName) });
            $('#PersonInfo_BirthDate').blur( function() { setTimeout(CheckBirthDate) });
            $('#PersonInfo_BirthPlace').blur( function() { setTimeout(CheckBirthPlace) });

        });

        function CheckForm() {
            var res = true;
            if (!CheckSurname()) { res = false; }
            if (!CheckName()) { res = false; }
            if (!CheckBirthPlace()) { res = false; }
            if (!CheckBirthDate()) { res = false; }
            return res;
        }
    </script>
    <script language="javascript" type="text/javascript">
        var PersonInfo_Surname_Message = $('#PersonInfo_Surname_Message').text();
        var PersonInfo_Name_Message = $('#PersonInfo_Name_Message').text();
        var regexp = /^[А-Яа-яё\-\'\s]+$/i;
        function CheckSurname() {
            var ret = true;
            var val = $('#PersonInfo_Surname').val();
            if (val == '') {
                ret = false;
                $('#PersonInfo_Surname').addClass('input-validation-error');
                $('#PersonInfo_Surname_Message').show();
            }
            else {
                $('#PersonInfo_Surname').removeClass('input-validation-error');
                $('#PersonInfo_Surname_Message').hide();
                if (!regexp.test(val)) {
                    ret = false;
                    $('#PersonInfo_Surname_Message').text('Использование латинских символов не допускается');
                    $('#PersonInfo_Surname_Message').show();
                    $('#PersonInfo_Surname').addClass('input-validation-error');
                }
                else {
                    $('#PersonInfo_Surname_Message').text(PersonInfo_Surname_Message);
                    $('#PersonInfo_Surname_Message').hide();
                    $('#PersonInfo_Surname').removeClass('input-validation-error');
                }
            }
            return ret;
        }
        function CheckName() {
            var ret = true;
            var val = $('#PersonInfo_Name').val();
            if (val == '') {
                ret = false;
                $('#PersonInfo_Name').addClass('input-validation-error');
                $('#PersonInfo_Name_Message').show();
            }
            else {
                $('#PersonInfo_Name').removeClass('input-validation-error');
                $('#PersonInfo_Name_Message').hide();
                if (!regexp.test(val)) {
                    $('#PersonInfo_Name_Message').text('Использование латинских символов не допускается');
                    $('#PersonInfo_Name_Message').show();
                    $('#PersonInfo_Name').addClass('input-validation-error');
                    ret = false;
                }
                else {
                    $('#PersonInfo_Name_Message').text(PersonInfo_Name_Message);
                    $('#PersonInfo_Name_Message').hide();
                    $('#PersonInfo_Name').removeClass('input-validation-error');
                }
            }
            return ret;
        }
        function CheckSecondName() {
            var val = $('#PersonInfo_SecondName').val();
            if (!regexp.test(val)) {
                $('#PersonInfo_SecondName_Message').show();
                $('#PersonInfo_SecondName').addClass('input-validation-error');
                ret = false;
            }
            else {
                $('#PersonInfo_SecondName_Message').hide();
                $('#PersonInfo_SecondName').removeClass('input-validation-error');
            }
        }
        function CheckBirthDate() {
            var ret = true;
            var reg_exp = /^(0[1-9]|[12][0-9]|3[01]).(0[1-9]|1[012]).(19\d\d|200[0-7])$/i;
            if (!reg_exp.test($('#PersonInfo_BirthDate').val())) {
                ret = false;
                $('#PersonInfo_BirthDate').addClass('input-validation-error');
                $('#PersonInfo_BirthDate_Message').text('Некорректный формат даты');
                $('#PersonInfo_BirthDate_Message').show();
            }
            else if ($('#PersonInfo_BirthDate').val() == '') {
                ret = false;
                $('#PersonInfo_BirthDate').addClass('input-validation-error');
                $('#PersonInfo_BirthDate_Message').text('Введите дату рождения');
                $('#PersonInfo_BirthDate_Message').show();
            }
            else {
                $('#PersonInfo_BirthDate').removeClass('input-validation-error');
                $('#PersonInfo_BirthDate_Message').hide();
            }
            return ret;
        }
        function CheckBirthPlace() {
            var ret = true;
            if ($('#PersonInfo_BirthPlace').val() == '') {
                ret = false;
                $('#PersonInfo_BirthPlace').addClass('input-validation-error');
                $('#PersonInfo_BirthPlace_Message').show();
            }
            else {
                $('#PersonInfo_BirthPlace').removeClass('input-validation-error');
                $('#PersonInfo_BirthPlace_Message').hide();
            }
            return ret;
        }
    </script>
    <div class="grid">
        <div class="wrapper">
            <div class="grid_4 first">
            <% if (!Model.Enabled)
               { %>
                <div id="Message" class="message warning">
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                </div>
            <% } %>
            <form id="form" class="form panel" action="../../Applicant/NextStep" method="post" onsubmit="return CheckForm();">
                <h4><%= GetGlobalResourceObject("PersonInfo", "HeaderPersonalInfo").ToString()%></h4>
                <hr />
                <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                <fieldset>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Surname, GetGlobalResourceObject("PersonInfo", "Surname").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Surname)%>
                        <br />
                        <span id="PersonInfo_Surname_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Surname_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Name, GetGlobalResourceObject("PersonInfo", "Name").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.Name)%>
                        <br />
                        <span id="PersonInfo_Name_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_Name_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.SecondName, GetGlobalResourceObject("PersonInfo", "SecondName").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.SecondName)%>
                        <span id="PersonInfo_SecondName_Message" class="Red" style="display:none">
                            Использование латинских символов не допускается
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Sex, GetGlobalResourceObject("PersonInfo", "Sex").ToString())%>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Sex, Model.PersonInfo.SexList)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.BirthDate, GetGlobalResourceObject("PersonInfo", "BirthDate").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthDate)%>
                        <br />
                        <span id="PersonInfo_BirthDate_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthDate_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.BirthPlace, GetGlobalResourceObject("PersonInfo", "BirthPlace").ToString())%>
                        <%= Html.TextBoxFor(x => x.PersonInfo.BirthPlace)%>
                        <br />
                        <span id="PersonInfo_BirthPlace_Message" class="Red" style="display:none">
                            <%= GetGlobalResourceObject("PersonInfo", "PersonInfo_BirthPlace_Message").ToString()%>
                        </span>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.PersonInfo.Nationality, GetGlobalResourceObject("PersonInfo", "Nationality").ToString())%>
                        <%= Html.DropDownListFor(x => x.PersonInfo.Nationality, Model.PersonInfo.NationalityList)%>
                    </div>
                </fieldset>
                <hr />
                <div class="clearfix">
                    <input id="btnSubmit" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                </div>
            </form>
            </div>
            <div class="grid_2">
                    <ol>
                        <li><a href="../../Applicant?step=1">Личные данные</a></li>
                       <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                        <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                        <li><a href="../../Applicant?step=4">Образование</a></li>
                        <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                        <li><a href="../../Applicant?step=6">Сведения о родителях</a></li>
                    </ol>
                </div>
        </div>
    </div>
<%  }
    if (Model.Stage == 2)
    {
%> 
        <script language="javascript" type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script language="javascript" type="text/javascript" src="../../Scripts/jquery.ui.datepicker-ru.js"></script>
        <script language="javascript" type="text/javascript">
            $(function () {
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% } %>
                <% if (Model.Enabled)
                   { %>
                $("#PassportInfo_PassportDate").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showOn: "focus",
                    yearRange: '1967:2014',
                    maxDate: "+1D",
                    defaultDate: '-3y',
                });
                $.datepicker.regional["ru"];
                <% } %>

                $("form").submit(function () {
                    return CheckForm();
                });
                $('#PassportInfo_PassportType').change(CheckForm);
                $('#PassportInfo_PassportSeries').keyup( function() { setTimeout(CheckSeries); });
                $('#PassportInfo_PassportNumber').keyup( function() { setTimeout(CheckNumber); });
                $('#PassportInfo_PassportAuthor').keyup( function() { setTimeout(CheckAuthor); });
                $('#PassportInfo_PassportDate').keyup( function() { setTimeout(CheckDate); });
                $('#PassportInfo_PassportDate').keyup( function() { setTimeout(CheckDate); });
                $('#PassportInfo_PassportCode').keyup( function() { setTimeout(CheckCode); });

                $('#PassportInfo_PassportSeries').blur( function() { setTimeout(CheckSeries); });
                $('#PassportInfo_PassportNumber').blur( function() { setTimeout(CheckNumber); });
                $('#PassportInfo_PassportAuthor').blur( function() { setTimeout(CheckAuthor); });
                $('#PassportInfo_PassportDate').blur( function() { setTimeout(CheckDate); });
                $('#PassportInfo_PassportCode').blur( function() { setTimeout(CheckCode); });

            });
            function CheckForm() {
                var res = true;
                if (!CheckSeries()) { res = false; }
                if (!CheckNumber()) { res = false; }
                if (!CheckAuthor()) { res = false; }
                if (!CheckDate()) { res = false; }
                return res;
            }
        </script>
        <script type="text/javascript">
            var PassportInfo_PassportSeries_Message = $('#PassportInfo_PassportSeries_Message').text();
            var PassportInfo_PassportNumber_Message = $('#PassportInfo_PassportNumber_Message').text();
            var PassportInfo_PassportDate_Message = $('#PassportInfo_PassportDate_Message').text();
            function CheckSeries() {
                var ret = true;
                var val = $('#PassportInfo_PassportSeries').val();
                var ruPassportRegex = /^\d{4}$/i;
                if ($('#PassportInfo_PassportType').val() == '1' && val == '') {
                    ret = false;
                    $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').show();
                }
                else {
                    $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                    $('#PassportInfo_PassportSeries_Message').hide();
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.test(val)) {
                        ret = false;
                        $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                        $('#PassportInfo_PassportSeries_Message').text('Серия паспорта РФ должна состоять из 4 цифр без пробелов');
                        $('#PassportInfo_PassportSeries_Message').show();
                    }
                    else {
                        $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                        $('#PassportInfo_PassportSeries_Message').hide();
                        $('#PassportInfo_PassportSeries_Message').text(PassportInfo_PassportSeries_Message);
                        if (val.length > 10) {
                            ret = false;
                            $('#PassportInfo_PassportSeries').addClass('input-validation-error');
                            $('#PassportInfo_PassportSeries_Message').text('Слишком длинное значение');
                            $('#PassportInfo_PassportSeries_Message').show();
                        }
                        else {
                            $('#PassportInfo_PassportSeries').removeClass('input-validation-error');
                            $('#PassportInfo_PassportSeries_Message').hide();
                            $('#PassportInfo_PassportSeries_Message').text(PassportInfo_PassportSeries_Message);
                        }
                    }
                }
                return ret;
            }
            function CheckNumber() {
                var ret = true;
                var val = $('#PassportInfo_PassportNumber').val();
                var ruPassportRegex = /^\d{6}$/i;
                if ($('#PassportInfo_PassportNumber').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').show();
                }
                else {
                    $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                    $('#PassportInfo_PassportNumber_Message').hide();
                    if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.test(val)) {
                        $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                        $('#PassportInfo_PassportNumber_Message').text('Номер паспорта РФ должен состоять из 6 цифр без пробелов');
                        $('#PassportInfo_PassportNumber_Message').show();
                    }
                    else {
                        $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                        $('#PassportInfo_PassportNumber_Message').hide();
                        $('#PassportInfo_PassportNumber_Message').text(PassportInfo_PassportNumber_Message);
                        if (val.length > 20) {
                            $('#PassportInfo_PassportNumber').addClass('input-validation-error');
                            $('#PassportInfo_PassportNumber_Message').text('Слишком длинное значение');
                            $('#PassportInfo_PassportNumber_Message').show();
                        }
                        else {
                            $('#PassportInfo_PassportNumber').removeClass('input-validation-error');
                            $('#PassportInfo_PassportNumber_Message').hide();
                            $('#PassportInfo_PassportNumber_Message').text(PassportInfo_PassportNumber_Message);
                        }
                    }
                }
                return ret;
            }
            function CheckDate() {
                var ret = true;
                if ($('#PassportInfo_PassportDate').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportDate').addClass('input-validation-error');
                    $('#PassportInfo_PassportDate_Message').show();
                }
                else {
                    $('#PassportInfo_PassportDate').removeClass('input-validation-error');
                    $('#PassportInfo_PassportDate_Message').hide();
                }
                return ret;
            }
            function CheckAuthor() {
                var ret = true;
                
                if ($('#PassportInfo_PassportType').val() == '1' && $('#PassportInfo_PassportAuthor').val() == '') {
                    ret = false;
                    $('#PassportInfo_PassportAuthor').addClass('input-validation-error');
                    $('#PassportInfo_PassportAuthor_Message').show();
                }
                else {
                    $('#PassportInfo_PassportAuthor').removeClass('input-validation-error');
                    $('#PassportInfo_PassportAuthor_Message').hide();
                }
                return ret;
            }
            function CheckCode() {
                var ret = true;
                var val = $('#PassportInfo_PassportCode').val();
                var ruPassportRegex = /^[0-9\-]+$/i;
                
                if ($('#PassportInfo_PassportType').val() == '1' && !ruPassportRegex.test(val)) {
                    ret = false;
                    $('#PassportInfo_PassportCode').addClass('input-validation-error');
                    $('#PassportInfo_PassportCode_Message').text('Номер подразделения должен состоять из цифр и знака "-"');
                    $('#PassportInfo_PassportCode_Message').show();
                }
                else {
                    $('#PassportInfo_PassportCode').removeClass('input-validation-error');
                    $('#PassportInfo_PassportCode_Message').hide();
                }
                return ret;
            }
        </script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                    <% if (!Model.Enabled)
                       { %>
                        <div id="Message" class="message warning">
                            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                        </div>
                    <% } %>
                    <form id="form" class="form panel" action="../../Applicant/NextStep" method="post" onsubmit="return CheckForm();">
                        <h4><%= GetGlobalResourceObject("PassportInfo", "HeaderPassport").ToString()%></h4>
                        <hr />
                        <%= Html.ValidationSummary(GetGlobalResourceObject("PersonInfo", "ValidationSummaryHeader").ToString())%>
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <input name="Enabled" type="hidden" value="<%= Model.Enabled %>" />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportType, GetGlobalResourceObject("PassportInfo", "PassportType").ToString())%>
                            <%= Html.DropDownListFor(x => x.PassportInfo.PassportType, Model.PassportInfo.PassportTypeList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportSeries, GetGlobalResourceObject("PassportInfo", "PassportSeries").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportSeries)%>
                            <br /><p></p>
                            <span id="PassportInfo_PassportSeries_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportSeries_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportNumber, GetGlobalResourceObject("PassportInfo", "PassportNumber").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportNumber)%>
                            <br /><p></p> 
                            <span id="PassportInfo_PassportNumber_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportNumber_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportAuthor, GetGlobalResourceObject("PassportInfo", "PassportAuthor").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportAuthor)%>
                            <br /><p></p> 
                            <span id="PassportInfo_PassportAuthor_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportAuthor_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportDate, GetGlobalResourceObject("PassportInfo", "PassportDate").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportDate)%>
                            <br /><p></p> 
                            <span id="PassportInfo_PassportDate_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportDate_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.PassportInfo.PassportCode, GetGlobalResourceObject("PassportInfo", "PassportCode").ToString())%>
                            <%= Html.TextBoxFor(x => x.PassportInfo.PassportCode)%>
                            <br /><p></p> 
                            <span id="PassportInfo_PassportCode_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PassportInfo", "PassportInfo_PassportCode_Message").ToString()%>
                            </span>
                        </div>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit1" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../Applicant?step=1">Личные данные</a></li>
                        <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                        <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                        <li><a href="../../Applicant?step=4">Образование</a></li>
                        <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                        <li><a href="../../Applicant?step=6">Сведения о родителях</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 3)
    {
%>
        <style>
	        .ui-autocomplete {
		        max-height: 200px;
		        max-width: 400px;
		        overflow-y: auto;
		        /* prevent horizontal scrollbar */
		        overflow-x: hidden;
		        /* add padding to account for vertical scrollbar */
		        padding-right: 20px;
	        }
	        /* IE 6 doesn't support max-height
	         * we use height instead, but this forces the menu to always be this tall
	         */
	        * html .ui-autocomplete {
		        height: 200px;
	        }
	     </style>
        <script type="text/javascript" language="javascript">
            $(function () {
                $('#form').submit(function () {
                    return CheckForm();
                })
                GetCities();
                <% if (!Model.Enabled)
                   { %>
                $('input').attr('readonly', 'readonly');
                $('select').attr('disabled', 'disabled');
                <% }
                   else
                   { %>
                $('#ContactsInfo_CountryId').change(function () { setTimeout(ValidateCountry); });
                function ValidateCountry() {
                    var countryid = $('#ContactsInfo_CountryId').val();
                    if (countryid == '1') {
                        $('#Region').show();
                    }
                    else {
                        $('#Region').hide();
                    }
                }
                ValidateCountry();
                <% } %>
                
                $('#ContactsInfo_RegionId').change(function () { setTimeout(GetCities); });
                $('#ContactsInfo_City').blur(function () { setTimeout(GetStreets); });
                $('#ContactsInfo_Street').blur(function () { setTimeout(GetHouses); });

                $('#ContactsInfo_MainPhone').keyup(function() { setTimeout(CheckPhone); } );
                $('#ContactsInfo_SecondPhone').keyup(function() { setTimeout(CheckSecondPhone); } );

                $('#ContactsInfo_Code').keyup(function() { setTimeout(CheckIndex); } );
                $('#ContactsInfo_City').keyup(function() { setTimeout(CheckCity); } );
              // $('#ContactsInfo_Street').keyup(function() { setTimeout(CheckStreet); } );
                $('#ContactsInfo_House').keyup(function() { setTimeout(CheckHouse); } );
                $('#ContactsInfo_Flat').keyup(function() { setTimeout(CheckFlat); } );
                $('#ContactsInfo_Korpus').keyup(function() { setTimeout(CheckKorpus); } );

                $('#ContactsInfo_MainPhone').blur(function() { setTimeout(CheckPhone); } );
                $('#ContactsInfo_SecondPhone').blur(function() { setTimeout(CheckSecondPhone); } );
                $('#ContactsInfo_Code').blur(function() { setTimeout(CheckIndex); } );
                $('#ContactsInfo_City').blur(function () { setTimeout(CheckCity); });
                
                //$('#ContactsInfo_Street').blur(function() { setTimeout(CheckStreet); } );
                $('#ContactsInfo_House').blur(function() { setTimeout(CheckHouse); } );
                $('#ContactsInfo_Flat').blur(function() { setTimeout(CheckFlat); } );
                $('#ContactsInfo_Korpus').blur(function() { setTimeout(CheckKorpus); } );


            });
        </script>
        <script type="text/javascript" language="javascript">
            var ContactsInfo_MainPhone_Message = $('#ContactsInfo_MainPhone_Message').text();
            var ContactsInfo_SecondPhone_Message = $('#ContactsInfo_SecondPhone_Message').text(); 

            function CheckPhone() {
                var ret = true;
                var val = $('#ContactsInfo_MainPhone').val();
                var ruPassportRegex = /^((8|0|((\+|00)\d{1,2}))[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,16}$/i;
                $('#ContactsInfo_MainPhone_Message').text('Введите основной номер');
                if ( val == '' ) {
                    ret = false;
                    $('#ContactsInfo_MainPhone').addClass('input-validation-error'); 
                    $('#ContactsInfo_MainPhone_Message').show();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_MainPhone').addClass('input-validation-error'); 
                        $('#ContactsInfo_MainPhone_Message').text('Номер телефона должен быть в формате 8-ХХХ-ХХХХХХХ либо +Х-ХХХ-ХХХХХХХ');
                        $('#ContactsInfo_MainPhone_Message').show();
                    }
                    else {
                        $('#ContactsInfo_MainPhone').removeClass('input-validation-error');
                        $('#ContactsInfo_MainPhone_Message').hide();
                    }
                }
                return ret;
            }
            function CheckSecondPhone() {
                var ret = true;
                var val = $('#ContactsInfo_SecondPhone').val();
                var ruPassportRegex = /^[0-9]+$/i;
                $('#ContactsInfo_SecondPhone_Message').text(ContactsInfo_SecondPhone_Message);
                if (val == '') {
                    $('#ContactsInfo_SecondPhone').removeClass('input-validation-error');
                    $('#ContactsInfo_SecondPhone_Message').hide();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_SecondPhone').addClass('input-validation-error');
                        $('#ContactsInfo_SecondPhone_Message').text('Номер телефона должен состоять из цифр');
                        $('#ContactsInfo_SecondPhone_Message').show();
                    }
                    else {
                        $('#ContactsInfo_SecondPhone').removeClass('input-validation-error');
                        $('#ContactsInfo_SecondPhone_Message').hide();
                    }
                }
                return ret;
            }
            function CheckIndex() {
                var ret = true;
                var ruPassportRegex = /^[0-9]+$/i;
                var val = $('#ContactsInfo_Code').val();

                if (val == '') { 
                    $('#ContactsInfo_Code').removeClass('input-validation-error'); 
                    $('#ContactsInfo_Code_Message').hide();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_Code').addClass('input-validation-error');
                        $('#ContactsInfo_Code_Message').text('Почтовый код должен содержать только цифры');
                        $('#ContactsInfo_Code_Message').show();
                    }
                    else{
                        $('#ContactsInfo_Code').removeClass('input-validation-error');
                        $('#ContactsInfo_Code_Message').hide();
                    }
                }
                return ret;
            }
            function CheckCity() {
                var ret = true;
                if ($('#ContactsInfo_City').val() == '') {
                    ret = false;
                    $('#ContactsInfo_City').addClass('input-validation-error');
                    $('#ContactsInfo_City_Message').show();
                }
                else {
                    $('#ContactsInfo_City').removeClass('input-validation-error');
                    $('#ContactsInfo_City_Message').hide();
                }
                return ret;
            }
            function CheckStreet() {
                var ret = true;
                var val = $('#ContactsInfo_Street').val();

                if ( val == '') {
                    ret = false;
                    $('#ContactsInfo_Street').addClass('input-validation-error');
                    $('#ContactsInfo_Street_Message').show();
                }
                else {
                    $('#ContactsInfo_Street').removeClass('input-validation-error');
                    $('#ContactsInfo_Street_Message').hide();
                }
                return ret;
            }
            function CheckHouse() {
                var ret = true;
                var ruPassportRegex = /^[0-9\A-яа-я\/\\\_\-]+$/i;
                var val = $('#ContactsInfo_House').val();
                if ( val == '') {
                    ret = false;
                    $('#ContactsInfo_House').addClass('input-validation-error');
                    $('#ContactsInfo_House_Message').text('Введите дом')
                    $('#ContactsInfo_House_Message').show();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_House').addClass('input-validation-error');
                        $('#ContactsInfo_House_Message').text('Номер дома должен состоять из цифр и (или) букв')
                        $('#ContactsInfo_House_Message').show();
                    }
                    else {
                        $('#ContactsInfo_House').removeClass('input-validation-error');
                        $('#ContactsInfo_House_Message').hide();
                    }
                }
                return ret;
            }
             function CheckFlat() {
                var ret = true;
                var ruPassportRegex = /^[0-9\A-яа-я\/\\\_\-]+$/i;
                var val = $('#ContactsInfo_Flat').val();
                if ( val == '') { 
                    $('#ContactsInfo_Flat').removeClass('input-validation-error');
                    $('#ContactsInfo_Flat_Message').hide();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_Flat').addClass('input-validation-error');
                        $('#ContactsInfo_Flat_Message').text('Номер квартиры должен состоять из цифр')
                        $('#ContactsInfo_Flat_Message').show();
                    }
                    else {
                        $('#ContactsInfo_Flat').removeClass('input-validation-error');
                        $('#ContactsInfo_Flat_Message').hide();
                    }
                }
                return ret;
            }
            function CheckKorpus() {
                var ret = true;
                var ruPassportRegex = /^[0-9\A-яа-я\/\\\_\-]+$/i;
                var val = $('#ContactsInfo_Korpus').val();
                if (val == '') {
                    $('#ContactsInfo_Korpus').removeClass('input-validation-error');
                    $('#ContactsInfo_Korpus_Message').hide();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#ContactsInfo_Korpus').addClass('input-validation-error');
                        $('#ContactsInfo_Korpus_Message').text('Номер корпуса должен состоять из цифр')
                        $('#ContactsInfo_Korpus_Message').show();
                    }
                    else {
                        $('#ContactsInfo_Korpus').removeClass('input-validation-error');
                        $('#ContactsInfo_Korpus_Message').hide();
                    }
                }
                return ret;
            }
            function CheckForm() {
                var res = true;
                if (!CheckPhone()) { res = false; }
                if (!CheckIndex()) { res = false; }
                if (!CheckSecondPhone()) { res = false; }
                if (!CheckCity()) { res = false; }
                //if (!CheckStreet()) { res = false; }
                if (!CheckHouse()) { res = false; }
                if (!CheckFlat()) { res = false; }
                if (!CheckKorpus()) { res = false; }

                return res;
            }
        </script>
        <script type ="text/javascript">
            function GetCities() {
                $.post('../../Olymp/GetCityNames', { regionId: $('#ContactsInfo_RegionId').val() }, function (data) {
                    if (data.IsOk) {
                        $('#ContactsInfo_City').autocomplete({
                            source: data.List
                        });
                    }
                }, 'json');
            }
            function GetStreets() {
                $.post('../../Olymp/GetStreetNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val() }, function (data) {
                    if (data.IsOk) {
                        $('#ContactsInfo_Street').autocomplete({
                            source: data.List
                        });
                    }
                }, 'json');
            }
            function GetHouses() {
                $.post('../../Olymp/GetHouseNames', { regionId: $('#ContactsInfo_RegionId').val(), cityName: $('#ContactsInfo_City').val(), streetName: $('#ContactsInfo_Street').val() }, function (data) {
                    if (data.IsOk) {
                        $('#ContactsInfo_House').autocomplete({
                            source: data.List
                        });
                    }
                }, 'json');
            }
        </script>
        <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                <% if (!Model.Enabled)
                   { %>
                    <div id="Message" class="message warning">
                        <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                    </div>
                <% } %>
                    <form id="form" class="form panel" action="../../Applicant/NextStep" method="post" onsubmit="return CheckForm();">
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <h3>Контактные телефоны:</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.MainPhone, GetGlobalResourceObject("ContactsInfo", "MainPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.MainPhone) %>
                            <br /><p></p>
                            <span id="ContactsInfo_MainPhone_Message" class="Red" style="display:none">Введите основной номер</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.SecondPhone, GetGlobalResourceObject("ContactsInfo", "SecondPhone").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.SecondPhone)%>
                            <br /><p></p>
                            <span id="ContactsInfo_SecondPhone_Message" class="Red" style="display:none"></span>
                        </div>
                        <h3>Адрес регистрации:</h3>
                        <hr />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.CountryId, GetGlobalResourceObject("ContactsInfo", "CountryId").ToString())%>
                            <%= Html.DropDownListFor(x => x.ContactsInfo.CountryId, Model.ContactsInfo.CountryList) %>
                        </div>
                        <div class="clearfix" id="Region">
                            <%= Html.LabelFor(x => x.ContactsInfo.RegionId, GetGlobalResourceObject("ContactsInfo", "RegionId").ToString())%>
                            <%= Html.DropDownListFor(x => x.ContactsInfo.RegionId, Model.ContactsInfo.RegionList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.City, GetGlobalResourceObject("ContactsInfo", "City").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.City) %>
                            <br /><p></p>
                            <span id="ContactsInfo_City_Message" class="Red" style="display:none">Введите город</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Street, GetGlobalResourceObject("ContactsInfo", "Street").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Street)%>
                            <br /><p></p>
                            <span id="ContactsInfo_Street_Message" class="Red" style="display:none">Введите улицу</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.House, GetGlobalResourceObject("ContactsInfo", "House").ToString()) %>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.House) %>
                            <br /><p></p>
                            <span id="ContactsInfo_House_Message" class="Red" style="display:none">Введите дом</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Korpus, GetGlobalResourceObject("ContactsInfo", "Korpus").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Korpus) %>
                             <br /><p></p>
                            <span id="ContactsInfo_Korpus_Message" class="Red" style="display:none"></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Flat, GetGlobalResourceObject("ContactsInfo", "Flat").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Flat) %>
                            <br /><p></p>
                            <span id="ContactsInfo_Flat_Message" class="Red" style="display:none"></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.Code, GetGlobalResourceObject("ContactsInfo", "PostIndex").ToString())%>
                            <%= Html.TextBoxFor(x => x.ContactsInfo.Code)%>
                            <br /><p></p>
                            <span id="ContactsInfo_Code_Message" class="Red" style="display:none">Введите почтовый код</span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ContactsInfo.IsCountryside, "Проживаю в сельской местности")%>
                            <%= Html.CheckBoxFor(x => x.ContactsInfo.IsCountryside)%>
                            <br /><p></p>
                            <span id="ContactsInfo_IsCountryside_Message" class="Red" style="display:none"></span>
                        </div>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit2" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../Applicant?step=1">Личные данные</a></li>
                        <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                        <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                        <li><a href="../../Applicant?step=4">Образование</a></li>
                        <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                        <li><a href="../../Applicant?step=6">Сведения о родителях</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 4)
    {
%>
        <style>
	        .ui-autocomplete {
		        max-height: 200px;
		        max-width: 400px;
		        overflow-y: auto;
		        /* prevent horizontal scrollbar */
		        overflow-x: hidden;
		        /* add padding to account for vertical scrollbar */
		        padding-right: 20px;
	        }
	        /* IE 6 doesn't support max-height
	         * we use height instead, but this forces the menu to always be this tall
	         */
	        * html .ui-autocomplete {
		        height: 200px;
	        }
	     </style>
        <script language="javascript" type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
        <script type="text/javascript" language="javascript">
            function CheckForm() {
                var ret = true;
                if (!CheckSchoolName()) { ret = false; }
                if (!CheckSchoolNumber()) { ret = false; }
                //if (!CheckSchoolExitYear()) { ret = false; }
               // if (!CheckAttestatRegion()) { ret = false; }
                return ret;
            }
            function CheckSchoolName() {
                var ret = true;
                if ($('#EducationInfo_SchoolName').val() == '') {
                    ret = false;
                    $('#EducationInfo_SchoolName').addClass('input-validation-error');
                    $('#EducationInfo_SchoolName_Message').show();
                }
                else {
                    $('#EducationInfo_SchoolName').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolName_Message').hide();
                }
                return ret;
            }
            function CheckSchoolNumber() {
                var ret = true;
                var ruPassportRegex = /^[0-9\А-Яа-я\-]+$/i;
                var val = $('#EducationInfo_SchoolNumber').val();
                if (val== '') { 
                    $('#EducationInfo_SchoolNumber').removeClass('input-validation-error');
                    $('#EducationInfo_SchoolNumber_Message').hide();
                }
                else {
                    if (!ruPassportRegex.test(val)) {
                        ret = false;
                        $('#EducationInfo_SchoolNumber').addClass('input-validation-error');
                        $('#EducationInfo_SchoolNumber_Message').text('Номер школы должен содержать только цифры и(или) буквы');
                        $('#EducationInfo_SchoolNumber_Message').show();
                    }
                    else
                    {
                        $('#EducationInfo_SchoolNumber').removeClass('input-validation-error');
                        $('#EducationInfo_SchoolNumber_Message').hide();
                    }
                }
                return ret;
            }
            $(function () {
                $('#EducationInfo_SchoolName').keyup(function () { setTimeout(CheckSchoolName); });
                $('#EducationInfo_SchoolName').blur(function () { setTimeout(CheckSchoolName); });

                $('#EducationInfo_SchoolNumber').keyup(function () { setTimeout(CheckSchoolNumber); });
                $('#EducationInfo_SchoolNumber').blur(function () { setTimeout(CheckSchoolNumber); });

                $('#EducationInfo_CountryEducId').change(function () {
                    if ($('#EducationInfo_CountryEducId').val() != 6) {
                        $('#CountryMessage').hide();
                    }
                    else {
                        $('#CountryMessage').show();
                    }
                });
            });
        </script>
        <div class="grid">
            <div class="wrapper">
                <div class="grid_4 first">
                    <% if (!Model.Enabled)
                       { %>
                        <div id="Message" class="message warning">
                            <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                        </div>
                    <% } %>
                    <form id="form" class="form panel" action="../../Applicant/NextStep" method="post" onsubmit="return CheckForm();">
                        <h3>Данные об образовании</h3>
                        <hr />
                        <input name="Stage" type="hidden" value="<%= Model.Stage %>" />
                        <fieldset><br />
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolTypeId, GetGlobalResourceObject("EducationInfo", "SchoolTypeId").ToString())%>
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolTypeId, Model.EducationInfo.SchoolTypeList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolName, GetGlobalResourceObject("EducationInfo", "SchoolName").ToString())%>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolName)%>
                            <br /><p></p>
                            <span id="EducationInfo_SchoolName_Message" class="Red" style="display:none">Укажите название образовательного учреждения</span>
                        </div>
                        <div id="_SchoolNumber" class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolNumber, "Номер школы") %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolNumber) %>
                            <br /><p></p>
                            <span id="EducationInfo_SchoolNumber_Message" class="Red" style="display:none"></span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolCity, "Населённый пункт") %>
                            <%= Html.TextBoxFor(x => x.EducationInfo.SchoolCity) %>
                        </div>
                        
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.CountryEducId, GetGlobalResourceObject("EducationInfo", "CountryEducId").ToString()) %>
                            <%= Html.DropDownListFor(x => x.EducationInfo.CountryEducId, Model.EducationInfo.CountryList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.RegionEducId, "Регион, в котором находится образовательное учреждение") %>
                            <%= Html.DropDownListFor(x => x.EducationInfo.RegionEducId, Model.EducationInfo.RegionList) %>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.EducationInfo.SchoolClassId, "Класс") %>
                            <%= Html.DropDownListFor(x => x.EducationInfo.SchoolClassId, Model.EducationInfo.SchoolClassList)%>
                        </div>
                        <div id="CountryMessage" class="message info" style="display:none; border-collapse:collapse;">
                            Пожалуйста, укажите в названии школы страну, где Вы обучались (например, "Oxford, UK", "Oberwolfach, Germany")
                        </div>
                        
                        </fieldset>
                        <hr />
                        <div class="clearfix">
                            <input id="Submit3" type="submit" class="button button-green" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                        </div>
                    </form>
                </div>
                <div class="grid_2">
                    <ol>
                        <li><a href="../../Applicant?step=1">Личные данные</a></li>
                        <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                        <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                        <li><a href="../../Applicant?step=4">Образование</a></li>
                        <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                        <li><a href="../../Applicant?step=6">Сведения о родителях</a></li>
                    </ol>
                </div>
            </div>
        </div>
<%
    }
    if (Model.Stage == 5) //олимпиады ( ФЗ 152 перехало в низ)
    {
%>
    <script type="text/javascript">
        var cntVseross = parseInt('<%= Model.AddInfo.VserossOlympBase.Count %>');
        var cntOtherOlymp = parseInt('<%= Model.AddInfo.OtherOlympBase.Count %>');
        $(function () {
            $('form').submit(function () {
                return true;
            });
            if (cntVseross == 0) {
                $('#Vseros').hide();
            }
            if (cntOtherOlymp == 0) {
                $('#OtherOlymp').hide();
            }
        });
        function CheckOlympName() {
                var ret = true;
                if ($('#OtherOlympUniverName').val() == '') {
                    ret = false;
                    $('#OtherOlympUniverName').addClass('input-validation-error');
                    $('#OtherOlympUniverName_Message').text('Введите название олимпиады');
                    $('#OtherOlympUniverName_Message').show();
                }
                else {
                    $('#OtherOlympUniverName').removeClass('input-validation-error');
                    $('#OtherOlympUniverName_Message').hide();
                }
                return ret;
            }
        function AddVseross() {
            var SubjId = $('#OlympVserosSubject').val();
            var StageId = $('#OlympVserosStage').val();
            var StatusId = $('#OlympVserosStatus').val();
            var tblbodyText = $('#Vseros tbody').html();
            $.post('/Applicant/AddVseross', { subjectId: SubjId, stageId: StageId, statusId: StatusId }, function (data) {
                if (data.IsOk) {
                    tblbodyText += '<tr id="Vs' + data.Id + '">';
                    tblbodyText += '<td>' + $('#OlympVserosSubject option:selected').text() + '</td>';
                    tblbodyText += '<td>' + $('#OlympVserosStage option:selected').text() + '</td>';
                    tblbodyText += '<td>' + $('#OlympVserosStatus option:selected').text() + '</td>';
                    tblbodyText += '<td><span class="link Red" onclick="DeleteVseross(\'' + data.Id + '\')">Удалить</span></td>';
                    tblbodyText += '</tr>';

                    cntVseross += 1;
                    $('#Vseros').show();
                    $('#Vseros_Message').hide();
                }
                else {
                    $('#Vseros_Message').text(data.Message);
                    $('#Vseros_Message').show();
                }
                $('#Vseros tbody').html(tblbodyText);
            }, 'json');

        }
        function DeleteVseross(olympId) {
            $('#Vseros_Message').hide();
            $.post('/Applicant/DeleteVseross', { id: olympId }, function (data) {
                if (data.IsOk) {
                    $("#Vs" + olympId).html('').hide();
                    cntVseross -= 1;
                    if (cntVseross == 0) {
                        $('#Vseros').hide();
                    }
                }
            }, 'json');
        }
        function AddOther() {
            if (CheckOlympName()) 
            {
                var SubjId = $('#OtherOlympSubject').val();
                var VuzName = $('#OtherOlympUniverName').val();
                var StatusId = $('#OtherOlympStatus').val();
                var tblbodyText = $('#OtherOlymp tbody').html();
                $.post('/Applicant/AddOtherOlymp', { subjectId: SubjId, vuzName: VuzName, statusId: StatusId }, function (data) {
                    if (data.IsOk) {
                        tblbodyText += '<tr id="Oth' + data.Id + '">';
                        tblbodyText += '<td>' + $('#OtherOlympSubject option:selected').text() + '</td>';
                        tblbodyText += '<td>' + $('#OtherOlympUniverName').val() + '</td>';
                        tblbodyText += '<td>' + $('#OtherOlympStatus option:selected').text() + '</td>';
                        tblbodyText += '<td><span class="link Red" onclick="DeleteOther(\'' + data.Id + '\')">Удалить</span></td>';
                        tblbodyText += '</tr>';

                        cntOtherOlymp += 1;
                        $('#OtherOlymp').show();
                        $('#OtherOlymp_Message').hide();
                    }
                    else {
                        $('#OtherOlymp_Message').text(data.Message);
                        $('#OtherOlymp_Message').show();
                    }
                    $('#OtherOlymp tbody').html(tblbodyText);
                }, 'json');
            }
        }
        function DeleteOther(olympId) {
            $('#OtherOlymp_Message').hide();
            $.post('/Applicant/DeleteOtherOlymp', { id: olympId }, function (data) {
                if (data.IsOk) {
                    $("#Oth" + olympId).html('').hide();
                    cntOtherOlymp -= 1;
                    if (cntOtherOlymp == 0) {
                        $('#OtherOlymp').hide();
                    }
                }
            }, 'json');
        }
    </script>
    <div class="grid">
        <div class="wrapper">
            <div class="grid_4 first">
            <% if (!Model.Enabled)
               { %>
                <div id="Message" class="message warning">
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                </div>
            <% } %>
                <form class="panel form" action="../../Applicant/NextStep" method="post">
                    <%= Html.ValidationSummary()%>
                    <%= Html.HiddenFor(x => x.Stage)%>
                    <h4>Претендую на участие в программе интеллектуального попечительства детей и молодежи с  ограниченными возможностями здоровья «Талант преодоления»:</h4>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.AddInfo.IsDisabled, "Ребенок-инвалид")%>
                        <%= Html.CheckBoxFor(x => x.AddInfo.IsDisabled)%>
                    </div>
                    <div class="clearfix">
                        <%= Html.LabelFor(x => x.AddInfo.IsSirota, "Сирота")%>
                        <%= Html.CheckBoxFor(x => x.AddInfo.IsSirota)%>
                    </div>
                    <h4>Участие во Всероссийской олимпиаде школьников</h4>
                    <hr />
                    <div class="clearfix">
                        <label>Предмет</label>
                        <%= Html.DropDownList("OlympVserosSubject", Model.AddInfo.OlympVserosSubjects)%>
                    </div>
                    <div class="clearfix">
                        <label>Уровень</label>
                        <%= Html.DropDownList("OlympVserosStage", Model.AddInfo.OlympVserosStages)%>
                    </div>
                    <div class="clearfix">
                        <label>Статус</label>
                        <%= Html.DropDownList("OlympVserosStatus", Model.AddInfo.OlympStatuses)%>
                    </div>
                    <button type="button" class="button button-blue" onclick="AddVseross();">Добавить</button>
                    <br />
                    <table id="Vseros" class="paginate">
                        <thead>
                            <tr>
                                <th>Предмет</th>
                                <th>Уровень</th>
                                <th>Статус</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% foreach (var Ol in Model.AddInfo.VserossOlympBase.OrderBy(x => x.Subject).ThenBy(x => x.Status))
                           { %>
                            <tr id="<%= "Vs" + Ol.Id.ToString("N") %>">
                               <td><%= Ol.Subject%></td>
                               <td><%= Ol.Level%></td>
                               <td><%= Ol.Status%></td>
                               <td><span class="link Red" onclick="DeleteVseross('<%= Ol.Id.ToString("N") %>')">Удалить</span></td>
                            </tr>
                        <% } %>
                        </tbody>
                    </table>
                    <span id ="Vseros_Message" class="Red" style="display: none;">
                    </span>
                    <br />

                    <h4>Участие в олимпиадах школьников других вузов</h4>
                    <hr />
                    <div class="clearfix">
                        <label>Предмет</label>
                        <%= Html.DropDownList("OtherOlympSubject", Model.AddInfo.OtherOlympSubjects)%>
                    </div>
                    <div class="clearfix">
                        <label>Название олимпиады</label>
                        <%= Html.TextBox("OtherOlympUniverName")%>
                        <br /><p></p>
                            <span id="OtherOlympUniverName_Message" class="Red" style="display:none"></span>
                    </div>
                    <div class="clearfix">
                        <label>Статус</label>
                        <%= Html.DropDownList("OtherOlympStatus", Model.AddInfo.OlympStatuses)%>
                    </div>
                    <button type="button" class="button button-blue" onclick="AddOther();">Добавить</button>
                    <br />
                    <table id="OtherOlymp" class="paginate">
                        <thead>
                            <tr>
                                <th>Предмет</th>
                                <th>Название олимпиады</th>
                                <th>Статус</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% foreach (var Ol in Model.AddInfo.OtherOlympBase.OrderBy(x => x.Subject).ThenBy(x => x.Status))
                           { %>
                            <tr id="<%= "Oth" + Ol.Id.ToString("N") %>">
                               <td><%= Ol.Subject%></td>
                               <td><%= Ol.VuzName%></td>
                               <td><%= Ol.Status%></td>
                               <td><span class="link Red" onclick="DeleteOther('<%= Ol.Id.ToString("N") %>')">Удалить</span></td>
                            </tr>
                        <% } %>
                        </tbody>
                    </table>
                    <span id ="OtherOlymp_Message" class="Red" style="display: none;">
                    </span>
                    <br />
                    <hr />
                    <div class="clearfix">
                        <input id="Submit4" type="submit" class="button button-green" value="<%= GetGlobalResourceObject("PersonInfo", "ButtonSubmitText").ToString()%>" />
                    </div>
                </form>
            </div>
            <div class="grid_2">
                <ol>
                    <li><a href="../../Applicant?step=1">Личные данные</a></li>
                    <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                    <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                    <li><a href="../../Applicant?step=4">Образование</a></li>
                    <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                    <li><a href="../../Applicant?step=6">Сведения о родителях</a></li>
                </ol>
            </div>
        </div>
    </div>
<%
    }
    if (Model.Stage == 6) //сведения о родителе или законном представителе ФЗ 152
    {
%>
    <script type="text/javascript">
        $(function () {
            $('form').submit(function () {
                if (CheckForm()) {
                    var FZAgree = $('#ParentInfo_FZ_152Agree').is(':checked');
                    if (FZAgree) {
                        $('#FZ').hide();
                        return true;
                    }
                    else {
                        $('#FZ').show();
                        return false;
                    }
                }
                else
                    return false;
            });
        });
        function CheckForm() {
            var res = true;
            if (!CheckParentName()) { res = false; }
            if (!CheckParentAdress()) { res = false; } 
            return res;
        }
    </script>
    <script language="javascript" type="text/javascript">
        var ParentInfo_ParentName_Message = $('#ParentInfo_ParentName_Message').text(); 
        var regexp = /^[А-Яа-яё\-\'\s]+$/i;
        function CheckParentName() {
            var ret = true;
            var val = $('#ParentInfo_ParentName').val();
            if (val == '') {
                ret = false;
                $('#ParentInfo_ParentName').addClass('input-validation-error');
                $('#ParentInfo_ParentName_Message').show();
            }
            else { 
                if (!regexp.test(val)) {
                    ret = false;
                    $('#ParentInfo_ParentName_Message').text('Использование латинских символов не допускается');
                    $('#ParentInfo_ParentName_Message').show();
                    $('#ParentInfo_ParentName').addClass('input-validation-error');
                }
                else {
                    $('#ParentInfo_ParentName_Message').text(ParentInfo_ParentName_Message);
                    $('#ParentInfo_ParentName_Message').hide();
                    $('#ParentInfo_ParentName').removeClass('input-validation-error');
                }
            }
            return ret;
        }
        function CheckParentAdress() {
            var ret = true;
            var val = $('#ParentInfo_ParentAddress').val();
            if (val == '') {
                ret = false;
                $('#ParentInfo_ParentAddress').addClass('input-validation-error');
                $('#ParentInfo_ParentAddress_Message').show();
            }
            else {
                    $('#ParentInfo_ParentAddress_Message').hide();
                    $('#ParentInfo_ParentAddress').removeClass('input-validation-error');
                }
            return ret;
        }
    </script>
    <div class="grid">
        <div class="wrapper">
            <div class="grid_4 first">
            <% if (!Model.Enabled)
                { %>
                <div id="Message" class="message warning">
                    <span class="ui-icon ui-icon-alert"></span><%= GetGlobalResourceObject("PersonInfo", "WarningMessagePersonLocked").ToString()%>
                </div>
            <% } %>
                <form class="panel form" action="../../Applicant/NextStep" method="post">
                    <%= Html.ValidationSummary() %>
                    <%= Html.HiddenFor(x => x.Stage) %>
                     <fieldset>
                     <h4>Сведения о родителе или законном представителе</h4>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ParentInfo.ParentName, GetGlobalResourceObject("PersonInfo", "ParentInfo_Name").ToString())%>
                            <%= Html.TextBoxFor(x => x.ParentInfo.ParentName)%>
                            <br /><p></p>
                            <span id="ParentInfo_ParentName_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PersonInfo", "ParentInfo_Name_Message").ToString()%>
                            </span>
                        </div>
                        <div class="clearfix">
                            <%= Html.LabelFor(x => x.ParentInfo.ParentAddress, GetGlobalResourceObject("PersonInfo", "ParentInfo_Adress").ToString())%>
                            <%= Html.TextBoxFor(x => x.ParentInfo.ParentAddress)%>
                            <br /><p></p>
                            <span id="ParentInfo_ParentAddress_Message" class="Red" style="display:none">
                                <%= GetGlobalResourceObject("PersonInfo", "ParentInfo_Adress_Message").ToString()%>
                            </span>
                        </div>
                     </fieldset>
                    <br />
                    <hr />
                    <div class="clearfix">
                        <h4>Я подтверждаю, что предоставленная мной информация корректна и достоверна. Даю согласие на обработку предоставленных персональных данных в порядке, установленном Федеральным законом от 27 июля 2006 года № 152-ФЗ «О персональных данных».</h4>
                        <%= Html.CheckBoxFor(x => x.ParentInfo.FZ_152Agree) %>
                        <span>Подтверждаю и согласен</span>    
                    </div>
                    <span id="FZ" class="Red" style="display:none;">Вы должны принять условия</span>
                    <hr />
                    <div class="clearfix">
                        <input id="Submit5" class="button button-green" type="submit" value="Закончить регистрацию" />
                    </div>
                </form>
            </div>
            <div class="grid_2">
                <ol>
                    <li><a href="../../Applicant?step=1">Личные данные</a></li>
                    <% if (1 == 1)
                       { %><li><a href="../../Applicant?step=2">Паспорт</a></li> <%}%>
                    <li><a href="../../Applicant?step=3">Контактная информация</a></li>
                    <li><a href="../../Applicant?step=4">Образование</a></li>
                    <li><a href="../../Applicant?step=5">Доп. сведения</a></li>
                    <li><a href="../../Applicant?step=6">Сведения о родителях</a></li> 
                </ol>
            </div>
        </div>
    </div>
<%
    }
%>  
</asp:Content>
