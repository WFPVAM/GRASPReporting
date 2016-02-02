<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="ViewReports.aspx.cs" Inherits="Admin_Statistics_ViewReports" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        #success {
            display: none;
        }

        .color {
            color: #0058B1;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Reports</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="Reports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-tasks"></i>View Reports</li>
            </ol>
        </div>
    </div>
    <div id="success" class="row">
        <div class="col-lg-12">
            <div class="alert alert-dismissable alert-success">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                You successfully delete a report.
            </div>
        </div>
    </div>
    <div>
        <telerik:RadGrid ID="rgReports" runat="server" DataSourceID="ldsReport" CellSpacing="0" GridLines="None" Skin="MetroTouch" OnDeleteCommand="rgReports_DeleteCommand"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="ReportID" DataSourceID="ldsReport">
                <Columns>
                    <telerik:GridBoundColumn DataField="ReportName" HeaderText="Report Name" ReadOnly="True"
                        UniqueName="ReportName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ReportDescription" HeaderText="Report Description"
                        UniqueName="ReportDescription">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FormName" HeaderText="Form Name"
                        UniqueName="FormName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Create Date"
                        UniqueName="CreateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="DeleteColumn" HeaderText="Actions">
                        <ItemTemplate>
<%--                            <asp:ImageButton ID="btnInfo" runat="server" AlternateText="View Detail Report" ToolTip="View Report Details"
                                CommandName="Detail" ImageUrl="../_images/List.png" PostBackUrl='<%# "ReportDetails.aspx?id=" + Eval("ReportID") %>' />
                            <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Report"
                                
                                 CommandName="Delete" ImageUrl="../_images/Cross.png" />--%>
                            <%# "<a style=\"color:#0058B1; margin-right:5px;\" href=\"ReportDetails.aspx?id=" + Eval("ReportID") + "\"><i class=\"fa fa-eye fa-2\"></i>Edit</a>"%>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="javascript:if(!confirm('This action will delete the selected report. Are you sure?')){return false;}"
                                CommandName="Delete"><i class="color fa fa-trash-o fa-2"></i><span class="color">Delete Report</span></asp:LinkButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <asp:LinqDataSource ID="ldsReport" runat="server" OnSelecting="ldsReport_Selecting" OrderBy="CreateDate"></asp:LinqDataSource>
    </div>
</asp:Content>
