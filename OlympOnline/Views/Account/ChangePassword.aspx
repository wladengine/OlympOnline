<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="OlympOnline.Models" %>
<%@ Page Language="C#" MasterPageFile="~/Views/Applicant/PersonalOffice.Master" Inherits="System.Web.Mvc.ViewPage<ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Смена пароля
</asp:Content>

<asp:Content ContentPlaceHolderID="Subheader" runat="server">
    <h2>Смена пароля</h2>
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <p>Для смены пароля введите данные в форме ниже.</p>
    <p>Длина нового пароля не должна быть менее 6 символов.</p>

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Не удалось сменить пароль:")%>
        <div class="form">
            <fieldset>
                <legend>Информация об учётной записи</legend>
                <hr />
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.OldPassword, "Старый пароль")%>
                    <%: Html.PasswordFor(m => m.OldPassword) %>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.NewPassword, "Новый пароль")%>
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                <div class="clearfix">
                    <%: Html.LabelFor(m => m.ConfirmPassword, "Подтвердите пароль")%>
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <input class="button button-green" type="submit" value="Сменить пароль" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
