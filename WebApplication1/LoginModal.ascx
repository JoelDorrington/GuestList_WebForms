<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginModal.ascx.cs" Inherits="WebApplication1.LoginModal" %>

<div class="modal fade" id="loginModal" tabindex="-1" role="dialog" aria-labelledby="loginModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header" style="justify-content: space-between;">
                <h5 class="modal-title" id="loginModalLabel">Login</h5>
                <button class="float-right btn btn-outline btn-circle btn-sm close" type="button" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Username"></asp:TextBox>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Password"></asp:TextBox>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary" Text="Login" OnClientClick="triggerLogin(); return false;" />
            </div>
            <script type="text/javascript">
                function triggerLogin() {
                    var username = document.getElementById('<%= txtUsername.ClientID %>').value;
                    var password = document.getElementById('<%= txtPassword.ClientID %>').value;

                    $.ajax({
                        type: "POST",
                        url: "SignIn/LoginSubmit",
                        data: JSON.stringify({ username: username, password: password }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "text",
                        success: function (response) {
                            console.log(response)
                            if (response.d === "success") {
                                window.location.href = "~/GuestList.aspx";
                            } else {
                                alert("Invalid login credentials.");
                            }
                        },
                        failure: function (response) {
                            alert("Error: " + response.d);
                        }
                    });
                }
            </script>
        </div>
    </div>
</div>
