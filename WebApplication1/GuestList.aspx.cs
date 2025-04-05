using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Services;

namespace WebApplication1
{
    public partial class GuestList : Page
    {
        private SessionService _sessionService;
        protected DateTime From { get; set; } = DateTime.Today;
        protected string paginationMessage
        {
            get
            {
                return $"Showing Page: {GuestGridView.PageIndex + 1} of {GuestGridView.PageCount}";
            }
        }
        protected SortDirection sortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionService = new SessionService();
            if (_sessionService.Get("Authenticated") != "true")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (string.IsNullOrEmpty(Request.QueryString["from"]))
            {
                string defaultFromDate = DateTime.Today.ToString("yyyy-MM-dd");
                Response.Redirect($"~/GuestList.aspx?from={defaultFromDate}");
                return;
            }
            if (DateTime.TryParse(Request.QueryString["from"], out DateTime fromDate))
            {
                From = fromDate;
            }
            GuestGridView.DataBind();
            DataBind();
        }
        protected void GuestGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GuestGridView.PageIndex = e.NewPageIndex;
            GuestGridView.DataBind();
            DataBind();
        }

        protected void GuestGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortDirection d = e.SortDirection;
            string f = e.SortExpression;
            GridViewSortDirection(GuestGridView, e, out sortDirection, out f);
        }
        private void GridViewSortDirection(GridView g, GridViewSortEventArgs e, out SortDirection d, out string f)
        {
            f = e.SortExpression;
            d = e.SortDirection;

            //Check if GridView control has required Attributes
            if (g.Attributes["CurrentSortField"] != null && g.Attributes["CurrentSortDir"] != null)
            {
                if (f == g.Attributes["CurrentSortField"])
                {
                    d = SortDirection.Descending;
                    if (g.Attributes["CurrentSortDir"] == "ASC")
                    {
                        d = SortDirection.Ascending;
                    }
                }

                g.Attributes["CurrentSortField"] = f;
                g.Attributes["CurrentSortDir"] = (d == SortDirection.Ascending ? "DESC" : "ASC");
            }

        }

        protected void LeftArrowButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/GuestList.aspx?from={From.AddDays(-1).ToString("yyyy-MM-dd")}");
        }

        protected void RightArrowButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/GuestList.aspx?from={From.AddDays(1).ToString("yyyy-MM-dd")}");
        }
    }

}