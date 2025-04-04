using System;
using System.Web.Services;
using System.Web.UI;

namespace WebApplication1
{
    public partial class LoginModal : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Show()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "$('#loginModal').modal('show');", true);
        }

        public void Hide()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "HideModal", "$('#loginModal').modal('hide');", true);
        }
    }
}
