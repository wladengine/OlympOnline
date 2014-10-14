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
    });

    function GetSubjects() {
        $('#Stages').hide();
        $('#Dates').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#ApplicationInfo').hide();
        $.post('/Olymp/GetSubjects', { cityId: $('#City').val() }, function (data) {
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
            }
        }, 'json');
    }

    function GetStages() {
        $('#Dates').hide();
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#ApplicationInfo').hide();
        
        $.post('/Olymp/GetStages', { cityId: $('#City').val(), subjectId: $('#Subject').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                $('#Stage').html(options);
                if (data.List.length == 1) {
                    $('#Stage').val(data.List[0].Id);
                    $('#Stages').hide();
                    GetDates();
                }
                if (data.List.length > 1) {
                    $("#Stages").show();
                }
            }
            else {
                $('#Errors').text(data.ErrorMessage);
            }
        }, 'json');
    }

    function GetDates() {
        $('#FinishBtn').hide();
        $('#OlympPlaces').hide();
        $('#ApplicationInfo').hide();
        $('#Dates').show();
        $.post('/Olymp/GetDates', { cityId: $('#City').val(), subjectId: $('#Subject').val(), stageId: $('#Stage').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                $('#Date').html(options);
                if (data.List.length == 1) {
                    $('#Date').val(data.List[0].Id);
                    $('#Dates').hide();
                    GetOlympPlaces();
                }
                if (data.List.length > 1) {
                    $("#Dates").show();
                }
            }
            else {
                $('#Errors').text(data.ErrorMessage);
            }
        }, 'json');
    }

    function MkBtn() {
        $('#FinishBtn').hide();
        $.post('/Olymp/CheckInfo', { cityId: $('#City').val(), subjectId: $('#Subject').val(), stageId: $('#Stage').val(), date: $('#Date').val(), placeId: $('#OlympPlace').val() }, function (data) {
            if (data.IsOk) {
                var options = '<b>Данные по заявлению</b><br />';
                options += 'Город: ' + data.Vals.City + '<br />';
                options += 'Предмет: ' + data.Vals.Subject + '<br />';
                options += 'Дата: ' + data.Vals.Date + '<br />';
                options += 'Этап: ' + data.Vals.Stage + '<br />';
                options += 'Место: ' + data.Vals.OlympPlace + '<br />';

                $('#ApplicationInfo').html(options).show();
            }
            else {
                $('#Errors').text(data.ErrorMessage);
            }
        }, 'json');
        $('#FinishBtn').show();
    }

    function GetOlympPlaces() {
        $('#FinishBtn').hide();
        $('#ApplicationInfo').hide();
        $('#OlympPlaces').show();
        $.post('/Olymp/GetOlympPlaces', { cityId: $('#City').val(), subjectId: $('#Subject').val(), stageId: $('#Stage').val(), date: $('#Date').val() }, function (data) {
            if (data.IsOk) {
                var options = '';
                for (var i = 0; i < data.List.length; i++) {
                    options += '<option value="' + data.List[i].Id + '">' + data.List[i].Name + '</option>';
                }
                $('#OlympPlace').html(options);
                if (data.List.length == 1) {
                    $('#OlympPlace').val(data.List[0].Id);
                    $('#OlympPlaces').hide();
                    MkBtn();
                }
                if (data.List.length > 1) {
                    $("#OlympPlaces").show();
                }
            }
            else {
                $('#Errors').text(data.ErrorMessage);
            }
        }, 'json');
    }
</script>
<% using (Html.BeginForm("NewApp", "Olymp", FormMethod.Post))
   { 
%> 
    <%= Html.ValidationSummary() %>
    
    
    <p id="pCities">
        <span>Город</span><br />
        <%= Html.DropDownList("City", Model.Cities, new Dictionary<string, object>() { { "onchange", "GetSubjects()" }, { "size", "12" }, { "style", "min-width:500px;" } })%>
    </p>
    
    <p id="Subjects" style="border-collapse:collapse; display:none;">
        <span>Выберите предмет</span><br />
        <select id="Subject" size="5" name="Subject" style="min-width:500px;" onchange="GetStages()"></select>
    </p>
    <p id="Stages" style="border-collapse:collapse; display:none;">
        <span>Выберите этап</span><br />
        <select id="Stage" size="2" name="Stage" style="min-width:500px;" onchange="GetDates()"></select>
    </p>
    <p id="Dates" style="border-collapse:collapse; display:none;">
        <span>Выберите дату проведения олимпиады</span><br />
        <select id="Date" size="5" name="Date" style="min-width:500px;" onchange="GetOlympPlaces()"></select>
    </p>
    <p id="OlympPlaces" style="border-collapse:collapse; display:none;">
        <span>Выберите дату проведения олимпиады</span><br />
        <select id="OlympPlace" size="5" name="OlympPlace" style="min-width:500px;" onchange="MkBtn()"></select>
    </p>
    <p id="ApplicationInfo" class="message info" style="border-collapse:collapse; display:none;">
    </p>
    <p id="FinishBtn" style=" display:none;">
        <input id="Submit" type="submit" value="Подать заявление" class="button button-green"/>
    </p>
    <span id="Errors" class="message error" style="display:none;"></span>
<% 
   }
%>

</asp:Content>
