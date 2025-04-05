<%@ Page Title="Guest List" Language="C#" AutoEventWireup="true" CodeBehind="GuestList.aspx.cs" Inherits="WebApplication1.GuestList" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Guest List</h2>

  <div class="text-center">
      <asp:Button ID="LeftArrowButton" runat="server" Text="&lt;" OnClick="LeftArrowButton_Click" CssClass="btn btn-primary" />
      <asp:Label ID="DateLabel" runat="server" Text='<%# From.ToString("yyyy-MM-dd") %>' CssClass="mx-2"></asp:Label>
      <asp:Button ID="RightArrowButton" runat="server" Text="&gt;" OnClick="RightArrowButton_Click" CssClass="btn btn-primary" />
  </div>


    <asp:ObjectDataSource ID="GuestObjectDataSource" runat="server"
        TypeName="WebApplication1.Services.GuestRepository"
        SelectMethod="Select_GuestList" EnablePaging="True"
        SelectCountMethod="SelectCount_GuestList"
        StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows"
        SortParameterName="sortExpression">
        <SelectParameters>
            <asp:QueryStringParameter Name="from" QueryStringField="from" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    

    <asp:GridView ID="GuestGridView" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
        DataSourceID="GuestObjectDataSource"
        AllowSorting="True" AllowPaging="True" PageSize="10" OnSorting="GuestGridView_Sorting"
        OnPageIndexChanging="GuestGridView_PageIndexChanging">
      <EmptyDataTemplate>No guests that day.</EmptyDataTemplate>
      <Columns>
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" SortExpression="Date" />
      </Columns>
      <PagerSettings Mode="NumericFirstLast" PageButtonCount="3" FirstPageText="<<" LastPageText=">>" Position="TopAndBottom"  />
      <PagerStyle CssClass="pagination justify-content-center" />
  </asp:GridView>
  <asp:Label ID="TotalGuestsLabel" runat="server" Text='<%# paginationMessage %>' CssClass="total-guests-label"></asp:Label>

</asp:Content>
