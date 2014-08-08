<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="ReportDetails.aspx.cs" Inherits="Admin_ReportDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .color {
            color: #0058B1;
            margin-left: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Report Details</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="ViewReports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-tasks"></i>Report Detail</li>
            </ol>
        </div>
    </div>
    <div>
        <telerik:RadGrid ID="rgReports" runat="server" DataSourceID="ldsReport" CellSpacing="0" GridLines="None" Skin="MetroTouch" OnDeleteCommand="rgReports_DeleteCommand"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="ReportID" DataSourceID="ldsReport">
                <Columns>
                    <telerik:GridBoundColumn DataField="ChartType" HeaderText="Chart Type" ReadOnly="True"
                        UniqueName="ChartType">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ReportDescription" HeaderText="Report Description"
                        UniqueName="ReportDescription">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Aggregate" HeaderText="Aggregate Function"
                        UniqueName="Aggregate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Create Date"
                        UniqueName="CreateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="DeleteColumn" HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="javascript:if(!confirm('This action will delete the selected report. Are you sure?')){return false;}"
                                CommandName="Delete"><i class="color fa fa-trash-o fa-2"></i><span class="color">Delete Chart</span></asp:LinkButton>
                            <%--<asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Chart"
                                OnClientClick="javascript:if(!confirm('This action will delete the selected chart. Are you sure?')){return false;}"
                                 CommandName="Delete" ImageUrl="../_images/Cross.png" />--%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <asp:LinqDataSource ID="ldsReport" runat="server" OnSelecting="ldsReport_Selecting" OrderBy="CreateDate"></asp:LinqDataSource>
    </div>
</asp:Content>

