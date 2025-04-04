<%@ Page Title="Guest List" Language="C#" AutoEventWireup="true" CodeBehind="GuestList.aspx.cs" Inherits="WebApplication1.GuestList" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Guest List</h2>

  <div class="text-center">
      <asp:Button ID="LeftArrowButton" runat="server" Text="&lt;" OnClick="LeftArrowButton_Click" CssClass="btn btn-primary" />
      <asp:Label ID="DateLabel" runat="server" Text='<%# tableParams.From?.ToString("yyyy-MM-dd") %>' CssClass="mx-2"></asp:Label>
      <asp:Button ID="RightArrowButton" runat="server" Text="&gt;" OnClick="RightArrowButton_Click" CssClass="btn btn-primary" />
  </div>

  <asp:GridView ID="GuestGridView" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
        OnSorting="GuestGridView_Sorting">
      <EmptyDataTemplate>No guests that day.</EmptyDataTemplate>
      <Columns>
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" SortExpression="Date" />
      </Columns>
      <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" />
      <PagerStyle CssClass="pagination" />
  </asp:GridView>
  <asp:Label ID="TotalGuestsLabel" runat="server" Text='<%# paginationMessage %>' CssClass="total-guests-label"></asp:Label>

  <div class="text-center mt-3">
      <asp:Button ID="FirstPageButton" runat="server" Text="First" OnClick="FirstPageButton_Click" CssClass="btn btn-secondary" />
      <asp:Button ID="PreviousPageButton" runat="server" Text="Previous" OnClick="PreviousPageButton_Click" CssClass="btn btn-secondary" />
      <asp:Button ID="NextPageButton" runat="server" Text="Next" OnClick="NextPageButton_Click" CssClass="btn btn-secondary" />
      <asp:Button ID="LastPageButton" runat="server" Text="Last" OnClick="LastPageButton_Click" CssClass="btn btn-secondary" />
  </div>
</asp:Content>
