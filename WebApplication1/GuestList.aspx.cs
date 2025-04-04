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
        private GuestRepository _guestRepository;
        private SessionService _sessionService;
        protected GuestRepository.GetGuestsParams tableParams;
        protected int totalGuests;
        protected string paginationMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionService = new SessionService();
            if (_sessionService.Get("Authenticated") != "true")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            _guestRepository = new GuestRepository(new SQLConnectionFactory());
            if (!IsPostBack)
            {
                tableParams = new GuestRepository.GetGuestsParams
                {
                    From = DateTime.Today,
                    To = DateTime.Today.AddDays(1).AddTicks(-1)
                };
                CountGuests();
                LoadGuests();
            }
            else
            {
                tableParams = GuestRepository.GetGuestsParams.FromJson(
                    _sessionService.Get("GuestListTableParams")
                );
            }
        }

        private void LoadGuests()
        {
            totalGuests = int.Parse(_sessionService.Get("TotalGuests") ?? "0");
            GuestGridView.VirtualItemCount = totalGuests;
            GuestGridView.PageSize = tableParams.Limit;
            GuestGridView.AllowPaging = true;
            GuestGridView.AllowSorting = true;

            var guests = _guestRepository.GetGuests(tableParams);
            GuestGridView.DataSource = guests;
            GuestGridView.DataBind();
            UpdatePaginationButtons();
            DataBind();
            _sessionService.Set("GuestListTableParams", tableParams.ToJson());
        }
        private void CountGuests()
        {
            totalGuests = _guestRepository.CountGuests(tableParams);
            _sessionService.Set("TotalGuests", totalGuests.ToString());
        }

        protected void GuestGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (tableParams.OrderBy.ToString() == e.SortExpression)
            {
                tableParams.Ascending = !tableParams.Ascending;
            }
            else
            {
                tableParams.OrderBy = (GuestRepository.SortKeys)Enum.Parse(typeof(GuestRepository.SortKeys), e.SortExpression);
                tableParams.Ascending = true;
            }
            tableParams.Offset = 0;
            LoadGuests();
        }

        protected void LeftArrowButton_Click(object sender, EventArgs e)
        {
            tableParams.From = tableParams.From?.AddDays(-1);
            tableParams.To = tableParams.From?.AddDays(1).AddTicks(-1);
            tableParams.Offset = 0;
            CountGuests();
            LoadGuests();
        }

        protected void RightArrowButton_Click(object sender, EventArgs e)
        {
            tableParams.From = tableParams.From?.AddDays(1);
            tableParams.To = tableParams.From?.AddDays(1).AddTicks(-1);
            tableParams.Offset = 0;
            CountGuests();
            LoadGuests();
        }
        protected void FirstPageButton_Click(object sender, EventArgs e)
        {
            tableParams.Offset = 0;
            LoadGuests();
        }

        protected void PreviousPageButton_Click(object sender, EventArgs e)
        {
            if (tableParams.Offset > 0)
            {
                tableParams.Offset -= tableParams.Limit;
                if (tableParams.Offset < 0) tableParams.Offset = 0;
                LoadGuests();
            }
        }

        protected void NextPageButton_Click(object sender, EventArgs e)
        {
            tableParams.Offset += tableParams.Limit;
            LoadGuests();
        }

        protected void LastPageButton_Click(object sender, EventArgs e)
        {
            int pageCount = (int)Math.Ceiling(GuestGridView.VirtualItemCount / (float)GuestGridView.PageSize);
            tableParams.Offset = tableParams.Limit * (pageCount-1);
            LoadGuests();
        }

        private void UpdatePaginationButtons()
        {
            int pageCount = (int)Math.Ceiling(GuestGridView.VirtualItemCount / (float)GuestGridView.PageSize);
            int pageIndex = (int)Math.Ceiling(tableParams.Offset / (float)tableParams.Limit);
            bool isFirstPage = pageIndex > 0;
            bool isLastPage = pageIndex >= pageCount-1;

            paginationMessage = $"Showing Guests: {tableParams.Offset+1} to {tableParams.Offset+tableParams.Limit} of {totalGuests}";
            FirstPageButton.Enabled = isFirstPage;
            PreviousPageButton.Enabled = isFirstPage;
            NextPageButton.Enabled = !isLastPage;
            LastPageButton.Enabled = !isLastPage;
        }
    }

}