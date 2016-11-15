<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OlympOnline.Models.LogOnModelST>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= GetGlobalResourceObject("LogOn", "HeaderLogOn").ToString()%>
</asp:Content>

<asp:Content ID="SubheaderContent" ContentPlaceHolderID="Subheader" runat="server">
    <h2><%= GetGlobalResourceObject("LogOn", "HeaderLogOn").ToString()%></h2>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="NavigationList" runat="server">
    <ul class="clearfix">
        <li class="active"><a href="../../Applicant/Main"><%= GetGlobalResourceObject("Common", "MainNavLogon").ToString()%></a></li>
        <li><a href="../../Account/Register"><%= GetGlobalResourceObject("Common", "MainNavRegister").ToString()%></a></li>
    </ul>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <a href="../../Account/Register" style="float:right; text-decoration:underline; color:#888;font-weight:400; font-size:1.3em;">
        <%: GetGlobalResourceObject("LogOn", "Register").ToString() %>
    </a>
    <%--<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>--%>
    <script type="text/javascript">
        $(function mkTimeClient() {
            var time = new Date();
            var value = time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds() + " " + time.toDateString();
            $('#time').val(value);
        });
    </script>
    <div class="grid">
        <div class="wrapper">
        <div class=" grid_5 first">
            <form class="form" method="post" action="../../Account/LogOnAndConfirmST">
                <%: Html.ValidationSummary(true, GetGlobalResourceObject("LogOn", "ValidationSummaryHeader").ToString()) %>
                <fieldset>
                    <h5><%= GetGlobalResourceObject("LogOn", "HeaderEmailConfirmOrChange").ToString()%></h5>
                    <hr />
                    <div class="clearfix">
                        <%: Html.TextBoxFor(m => m.EmailToConfirm, new Dictionary<string, object>() { { "required", "required" } }) %>
                    </div>
                    <br />
                    <hr />
                    <input id="time" name="time" type="hidden" />
                    <%= Html.HiddenFor(x => x.UserID) %>
                    <div class="clearfix">
                        <input id="submitBtn" class="button button-green" type="submit" value="<%= GetGlobalResourceObject("LogOn", "btnLogOn").ToString() %>" />
                    </div>
                </fieldset>
            </form>
        </div>
        </div>
    </div>
    
</asp:Content>
