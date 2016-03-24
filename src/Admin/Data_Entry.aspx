<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Data_Entry.aspx.cs" Inherits="Admin_Data_Entry" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Data Entry</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home </a></li>
                <li class="current"><i class="fa fa-bar-chart-o"></i>Data Entry</li>
            </ol>
        </div>
    </div>
    
    <div>
        <telerik:RadGrid ID="rgForm" runat="server" DataSourceID="ldsForm" CellSpacing="0" GridLines="None" Skin="MetroTouch" AllowPaging="True" AllowSorting="True"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White">
        </telerik:RadGrid>
        <asp:LinqDataSource ID="ldsForm" runat="server" OnSelecting="ldsForm_Selecting" OrderBy="CreateDate"></asp:LinqDataSource>
    </div>
</asp:Content>

