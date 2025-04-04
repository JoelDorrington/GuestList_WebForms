using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Services;

namespace WebApplication1
{
    public partial class SignIn : Page
    {
        public string lastLog { get; set; } = "";
        private IGuestRepository _guestRepository;
        private SignInService _signInService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _guestRepository = new GuestRepository(new SQLConnectionFactory(
                ConfigurationManager.ConnectionStrings["GuestLogDb"].ConnectionString
            ));
            _signInService = new SignInService(new SessionService());
            if (!IsPostBack)
            {
                lastLog = _signInService.GetLastLog();
                welcomePanel.Visible = HasName();
                DataBind();
            }
        }

        private bool HasName()
        {
            return !string.IsNullOrEmpty(nameTextBox.Text);
        }

        protected void PostData(object sender, EventArgs e)
        {
            // Use SignInService to set the last log
            _guestRepository.AddGuest(nameTextBox.Text);
            _signInService.SetLastLog(nameTextBox.Text);
            lastLog = nameTextBox.Text;
            nameTextBox.Text = "";
            welcomePanel.Visible = HasName();
        }
    }
}