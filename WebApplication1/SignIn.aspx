<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="WebApplication1.SignIn" EnableSessionState="true" %>

<asp:Content ID="SignIn" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <fieldset>
            <legend style="display:none;">Sign in</legend>
            <div class="form-group">
                <label for="nameTextBox">Name</label>
                <asp:TextBox ID="nameTextBox" CssClass="form-control" required="true" runat="server" />
            </div>
            <div class="form-group">
                <asp:Button ID="submitButton" cssClass="btn btn-primary" Text="Sign in" runat="server" onclick="PostData" />
                <button class="btn btn-secondary" type="button" 
                        onclick="window.location.href='Login.aspx'; return false;">
                    View Guest List
                </button>
            </div>
            <asp:Panel runat="server" ID="welcomePanel" cssClass="form-group">
                <div class="alert alert-success">
                    <strong>Welcome <%= lastLog %>!</strong>
                </div>
            </asp:Panel>
        </fieldset>
    </main>
</asp:Content>
