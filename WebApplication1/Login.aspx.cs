using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        SessionService _sessionService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionService = new SessionService();
            if (IsPostBack)
            {
                btnLogin_Click(sender, e);
            } else
            {
                if(_sessionService.Get("Authenticated") == "true")
                {
                    Response.Redirect("~/GuestList");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "admin" && password == "pass")
            {
                // In production, this would be a session ID corresponding to a server-side session cache (redis)
                // And verified on the server. Never trust the client!
                _sessionService.Set("Authenticated", "true");
                Response.Redirect("~/GuestList.aspx");
            }
            else
            {
                lblMessage.Text = "Invalid login credentials.";
            }
        }
    }
}