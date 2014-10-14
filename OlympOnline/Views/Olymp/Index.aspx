<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Applicant/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<OlympOnline.Models.ExtApplicationModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Просмотр заявления
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Subheader" runat="server">
    <h2>Просмотр заявления</h2>
</asp:Content>

<asp:Content ID="HeaderScripts" ContentPlaceHolderID="HeaderScriptsContent" runat="server">
    <%--<script src="http://api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU"
            type="text/javascript"></script>
    <script type="text/javascript">
        ymaps.ready(init);

        function init() {
            var myMap = new ymaps.Map("map", {
                center: [<%= Model.ComissionYaCoord %>],
                zoom: 16
            });

            myMap.controls.add('typeSelector');

            myMap.balloon.open(
                [<%= Model.ComissionYaCoord %>], {
                    contentBody: '<%= Model.ComissionAddress %>'
                }, {
                    closeButton: false
                });
        }
    </script>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% if (1 == 2)
   { %>
   <script type="text/javascript" src="../../Scripts/jquery-1.5.1-vsdoc.js"></script>
<% } %>

    <%--<script type="text/javascript" language="javascript">
        $(function () {
            $('#UILink').hide();
            $('#fileAttachment').change(ValidateInput);
            $("#rejectBtn")
                .button().click(function () {
                    $("#dialog-form").dialog("open");
                });
                $("#dialog:ui-dialog").dialog("destroy");
                $('#fileAttachment').change(ValidateInput);
                $("#dialog-form").dialog(
                {
                    autoOpen: false,
                    height: 400,
                    width: 350,
                    modal: true,
                    buttons: 
                    {
                        "Да": function () 
                        {
                            $.post('/Application/Disable', { id: '<%= Model.Id.ToString("N") %>' }, function (res) 
                            {
                                if (res.IsOk) 
                                {
                                    if (!res.Enabled) 
                                    {
                                        $('#appStatus').removeClass("Green").addClass("Red").text("Отозвано");
                                        $('#rejectApp').html('').hide();
                                        $("#dialog-form").dialog("close");
                                    }
                                }
                                else 
                                {
                                    //message to the user
                                    $('#errMessage').text(res.ErrorMessage).addClass("ui-state-highlight");
                                    setTimeout(function () 
                                    {
                                        $('#errMessage').removeClass("ui-state-highlight", 1500);
                                    }, 500);
                                }
                            }, 'json');
                        },
                        "Нет": function () 
                        {
                            $(this).dialog("close");
                        }
                    }
                });
        }); 
    function ValidateInput() {
        if ($.browser.msie) {
            var myFSO = new ActiveXObject("Scripting.FileSystemObject");
            var filepath = document.getElementById('fileAttachment').value;
            var thefile = myFSO.getFile(filepath);
            var size = thefile.size;
        } else {
            var fileInput = $("#fileAttachment")[0];
            var size = fileInput.files[0].size; // Size returned in bytes.
        }
        if (size > 4194304) {// 4194304 = 5Mb
            alert('To big file for uploading (4Mb - max)');
            //Очищаем поле ввода файла
            document.getElementById('fileAttachment').parentNode.innerHTML = document.getElementById('fileAttachment').parentNode.innerHTML;
        }
    }
    </script>--%>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.11.js"></script>
    
<h4>Основные сведения</h4>
<hr />
<table class="paginate" style="width: 90%">
    <% if (Model.Stage != "1 этап") 
       { %>
    <tr>
        <td width="30%" align="right">Город</td>
        <td align="left"><%= Html.Encode(Model.City) %></td>
    </tr>
    <% } %>
    <tr>
        <td width="30%" align="right">Предмет</td>
        <td align="left"><%= Html.Encode(Model.Subject) %></td>
    </tr>
    <tr>
        <td width="30%" align="right">Этап</td>
        <td align="left"><%= Html.Encode(Model.Stage) %></td>
    </tr>
<%--
    <tr>
        <td width="30%" align="right">Дата проведения</td>
        <td align="left"><%= Html.Encode(Model.Date) %></td>
    </tr>
--%>
    <tr>
        <td width="30%" align="right">Скачать заявление</td>
        <td align="left"><a href="<%= string.Format("../../Olymp/GetPrint/{0}", Model.Id.ToString("N")) %>"><img src="../../Content/themes/base/images/PDF.png" alt="Скачать (PDF)" /></a></td>
    </tr>

</table>
<br />
<% if (Model.Stage == "Отборочный" && !Model.IsFullTime) { %>
   <div class="message info">
       <% if (DateTime.Now - Model.DateOfApply > new TimeSpan(0, 7, 0)) { %>
    <b>Вам следует <a href="<%= Model.BlackBoardURL %>">пройти по ссылке</a> для прохождения олимпиады</b> 
       <% } else 
          { %>
       <b>Через <%= (Model.DateOfApply.AddMinutes(7) - DateTime.Now).Minutes.ToString() %> мин. <%= (Model.DateOfApply.AddMinutes(7) - DateTime.Now).Seconds.ToString() %> сек. Вам следует <a href="<%= Model.BlackBoardURL %>">пройти по ссылке</a> для прохождения олимпиады</b> 
       <% } %>
   </div>
<% }
   else
   { %>
    <div class="message info">
        <b>Вам следует <a href="<%= string.Format("../../Olymp/GetPrint/{0}", Model.Id.ToString("N")) %>">распечатать заявление</a> и с ним прийти на олимпиаду</b> 
    </div>
<% } %>
   
</asp:Content>
