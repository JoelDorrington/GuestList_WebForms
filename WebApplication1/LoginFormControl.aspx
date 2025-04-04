<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginFormControl.aspx.cs" Inherits="WebApplication1.LoginFormControl" %>

<asp:Panel ID="loginPanel" runat="server" DefaultButton="btnLogin">
    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
    <div class="form-group">
        <label for="<%= txtUsername.ClientID %>">Username</label>
        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Username"></asp:TextBox>
    </div>
    <div class="form-group">
        <label for="<%= txtPassword.ClientID %>">Password</label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Password"></asp:TextBox>
    </div>
    <div class="form-group">
        <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary" Text="Login" OnClick="btnLogin_Click" />
    </div>
</asp:Panel>
