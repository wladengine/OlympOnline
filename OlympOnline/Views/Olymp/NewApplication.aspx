<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="OlympOnline.Models" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Applicant/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<ApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Создание нового заявления
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
   <h2>Новое заявление</h2>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var entry;
    $(function () {
        $('#UILink').hide();
        $('#FinishBtn').hide();
        $('#ApplicationInfo').hide();
        GetSubjects();
    });

    function GetSubjects() {
        $('#Errors').hide();
        $('#Classes').hide();
        $('#OlympForms').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#Cities').hide();
        $('#ApplicationInfo').hide();
        $('#TeacherInfo').hide();

        $.post('/Olymp/GetSubjects', null, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                $('#Subject').html(options);
                $('#Subjects').show();
            }
            else {
                $('#Errors').text(data.ErrorMessage);
                $('#Errors').show();
            }
        }, 'json');
    }

    function GetOlympForms() {
        $('#Errors').hide();
        $('#FinishBtn').hide();
        $('#Classes').hide();
        $('#OlympForms').hide();
        $('#OlympPlaces').hide();
        $('#Cities').hide();
        $('#ApplicationInfo').hide();
        $('#TeacherInfo').hide();

        $.post('/Olymp/GetOlympForms', { subjectId: $('#Subject').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                if (data.List.length == 1) {
                    $('#OlympForm').html(options);
                    $('#OlympForm').val(data.List[0].Id);
                    $('#hOlympForm').val(data.List[0].Id);
                    GetClasses();
                }
                else {
                    $('#OlympForm').html(options);
                    $('#OlympForms').show();
                }
                $('#OlympForm').html(options);
                $('#OlympForms').show();
            }
            else {
                $('#Errors').text(data.ErrorMessage);
                $('#Errors').show();
            }
        }, 'json');
    }

    function GetClasses() {
        $('#Errors').hide();
        $('#Classes').hide();
        //$('#OlympForms').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#Cities').hide();
        $('#ApplicationInfo').hide();
        $('#TeacherInfo').hide();

        $.post('/Olymp/GetSchoolClassIntervals', { subjectId: $('#Subject').val(), olympFormId: $('#hOlympForm').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                if (data.List.length == 1) {
                    $('#Class').html(options);
                    $('#Class').val(data.List[0].Id);
                    $('#hClass').val(data.List[0].Id);
                    GetCities();
                }
                else {
                    $('#Class').html(options);
                    $('#Classes').show();
                }
            }
            else {
                $('#Errors').text(data.ErrorMessage);
                $('#Errors').show();
            }
        }, 'json');
    }

    function GetCities() {
        $('#Errors').hide();
        $('#Classes').hide();
        //$('#OlympForms').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#Cities').hide();
        $('#ApplicationInfo').hide();
        $('#TeacherInfo').hide();
        $.post('/Olymp/GetCities', { subjectId: $('#Subject').val(), olympFormId: $('#hOlympForm').val(), schoolClassInterval: $('#Class').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                if (data.List.length == 1) {
                    $('#City').html(options);
                    $('#City').val(data.List[0].Id);
                    $('#hCity').val(data.List[0].Id);
                    MkBtn();
                }
                else {
                    $('#City').html(options);
                    $('#Cities').show();
                }
            }
            else {
                $('#Errors').text(data.ErrorMessage);
                $('#Errors').show();
            }
        }, 'json');
    }

    function MkBtn() {
        $('#Errors').hide();
        $('#Classes').hide();
        $('#OlympForms').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#Cities').hide();
        $.post('/Olymp/CheckInfo', { subjectId: $('#Subject').val(), olympFormId: $('#hOlympForm').val(), schoolClassInterval: $('#hClass').val(), cityId: $('#hCity').val() }, function (data) {
            if (data.IsOk) {
                var options = '<b>Данные по заявлению</b><br />';
                options += 'Предмет: ' + data.Vals.Subject + '<br />';
                options += 'Группа классов: ' + data.Vals.Class + '<br />';
                options += 'Форма проведения: ' + data.Vals.Form + '<br />';
                options += 'Город проведения: ' + data.Vals.City + '<br />';
                $('#ApplicationInfo').html(options).show();
                $('#TeacherInfo').show();
                $('#FinishBtn').show();
            }
            else {
                $('#Errors').text(data.ErrorMessage);
                $('#Errors').show();
            }
        }, 'json');
    }
</script>
<% using (Html.BeginForm("NewApp", "Olymp", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    <p class="message warning">Обращаем ваше внимание, что в соответствии с п.7.3 Регламента проведения олимпиады школьников СПбГУ в 2013/2014 учебном году, допускается однократное участие в олимпиаде по соответствующему предмету.</p>
    <p id="pCities">
        <span>Выберите предмет олимпиады</span><br />
        <%= Html.DropDownList("Subject", Model.Subjects, new Dictionary<string, object>() { { "onchange", "GetOlympForms()" }, { "size", "12" }, { "style", "min-width:500px;" } })%>
    </p>
    
    <%--<p id="Subjects" style="border-collapse:collapse; display:none;">
        <span>Выберите предмет</span><br />
        <select id="Subject" size="5" name="Subject" style="min-width:500px;" onchange="GetStages()"></select>
    </p>--%>
    <p id="OlympForms" style="border-collapse:collapse; display:none;">
        <span>Выберите форму проведения олимпиады</span><br />
        <select id="OlympForm" size="5" name="OlympForm" style="min-width:500px;" onchange="$('#hOlympForm').val($('#OlympForm').val()); GetClasses()"></select>
    </p>
    <p id="Classes" style="border-collapse:collapse; display:none;">
        <span>Выберите группу классов олимпиады</span><br />
        <select id="Class" size="5" name="Class" style="min-width:500px;" onchange="$('#hClass').val($('#Class').val()); GetCities()"></select>
    </p>
    <p id="Cities" style="border-collapse:collapse; display:none;">
        <span>Выберите город</span><br />
        <select id="City" size="7" name="City" style="min-width:500px;" onchange="$('#hCity').val($('#City').val()); MkBtn()"></select>
    </p>

    <input type="hidden" id ="hOlympForm" name="hOlympForm" />
    <input type="hidden" id ="hClass" name="hClass" />
    <input type="hidden" id ="hCity" name="hCity" />

    <p id="ApplicationInfo" class="message info" style="border-collapse:collapse; display:none;">
    </p>
    
    <p id = "TeacherInfo" class="message info" style="border-collapse:collapse; display:none;">
        ФИО преподавателя, подготовившего участника:<br />
        <input name = "teacherName" type="text" style="width: 500px;"/>
    </p>

    <p id="FinishBtn" style=" display:none;">
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </p>
    <span id="Errors" class="message error" style="display:none;"></span>
<% 
   }
%>

</asp:Content>
