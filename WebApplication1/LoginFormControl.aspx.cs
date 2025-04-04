using System;
using System.Diagnostics;
using System.Web.UI;

namespace WebApplication1
{
    public partial class LoginFormControl : System.Web.UI.UserControl
    {
        public event EventHandler<LoginEventArgs> LoginSuccessful;
        public event EventHandler<LoginEventArgs> LoginFailed;

        private SessionService _sessionService;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionService = new SessionService();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            Debug.WriteLine($"{username} {password}");

            if (username == "admin" && password == "pass")
            {
                _sessionService.Set("Authenticated", "true");

                // Raise the success event
                LoginSuccessful?.Invoke(this, new LoginEventArgs { Username = username });
            }
            else
            {
                lblMessage.Text = "Invalid login credentials.";

                // Raise the failure event
                LoginFailed?.Invoke(this, new LoginEventArgs { Username = username });
            }
        }

        public void ClearForm()
        {
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            lblMessage.Text = string.Empty;
        }

        public void SetErrorMessage(string message)
        {
            lblMessage.Text = message;
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public string Username { get; set; }
    }
}
