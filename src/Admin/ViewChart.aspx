<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="ViewChart.aspx.cs" Inherits="Admin_Statistics_ViewChart" %>

<%@ Register Src="../_uc/pieChart.ascx" TagName="pieChart" TagPrefix="uc1" %>

<%@ Register Src="../_uc/barChart.ascx" TagName="barChart" TagPrefix="uc2" %>
<%@ Register TagPrefix="uc3" TagName="ResultMsgBar" Src="~/_uc/ResultMsgBar.ascx" %>
<%@ Reference Control="~/_uc/LineChart.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../_css/kendo.dataviz.min.css" rel="stylesheet" />

    <script src="../_js/kendo.dataviz.min.js"></script>

    <script src="../_js/console.js"></script>
    <script src="../_js/kendo.all.min.js"></script>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            setTimeout(function () {
                createCharts();
            }, 400);
        });

        function exportChartImage(chartID, reportName, chartName) {
            var chart = $("#" + chartID).getKendoChart();
            chart.exportImage().done(function (data) {
                kendo.saveAs({
                    dataURI: data,
                    fileName: reportName + "_" + chartName + ".png"
                });
            });
        }

        /**
           * Shows the custom filters form in a new popup window.
           *
		   */
        function ViewFiltersForm() {
            var browserWidth = $(window).width();
            var browserHeight = $(window).height();
            var oManager = window.GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndFilterForm");
            oWnd.setUrl("CustomFilters.aspx?FormID=" + <%= FormID %>
                + "&reportID=" + <%= ObjReport.ReportID %>
                + "&reportName=" + document.getElementById('h1ReportName').innerText);
            oWnd.setSize(Math.ceil(browserWidth * 90 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        /**
           * Calls when closes the custom filters form, it refreshes the report page with the selected filters.
		   */
        function OnClientCloseFiltersForm(sender, args) {
            var url = args.get_argument();
            if (url != null) {
                window.location.href = args.get_argument();     
            }
        }

        <%--function ClearFilters() {
            alert("s");
            $("#btnSaveFilter").enable = true;
            document.getElementById('lblFilterSummary').enable = false;
            <% ObjReport.Filters = ""; %>
            <% ObjReport.FiltersSummary = ""; %>
            <% ObjReport.FiltersCount = null; %>
        }--%>
    </script>
        </telerik:RadScriptBlock>
    <style type="text/css">
        #filter { margin-bottom: 4px; border-bottom: 1px solid #ccc;font-size:12px;padding:0 0 4px 0; }
    </style>
    
    
</asp:Content>  
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1 id="h1ReportName">
                <asp:Literal runat="server" ID="reportNameHeader"></asp:Literal>
            </h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="ViewReports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-tasks"></i>View Reports</li>
            </ol>
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlFilters">
        <div id="divResultMsg">
            <uc3:ResultMsgBar runat="server" ID="ucResultMsgBar" />
        </div>
        Review Status: 
        <telerik:radcombobox id="ddlResponseStatus" runat="server" enableloadondemand="True" skin="Metro"
            autopostback="True" appenddatabounditems="true" bordercolor="#66AFE9" backcolor="White"
            datasourceid="EdsResponseStatus" datatextfield="ResponseStatusName" datavaluefield="ResponseStatusID"
            onselectedindexchanged="ddlResponseStatus_SelectedIndexChanged">
            <Items>
                <telerik:RadComboBoxItem Text="Any" Value="0" />
            </Items>
        </telerik:radcombobox>
        <br />
        <asp:Panel runat="server" ID="pnlFilter">
            <div style="margin-bottom: 3px;">
                <asp:Panel ID="pnlFilterSummary" runat="server" CssClass="customFilterSummaryLabel" Style="margin-top: 3px; margin-bottom: 3px;" ToolTip="Report Filters">
                    <asp:Label ID="lblFilterSummary" runat="server" Text=""></asp:Label>
                </asp:Panel>
                <input id="btnAddFilters" type="button" value="Add Filters" onclick="ViewFiltersForm()" class="btn btn-primary btn-sm" />
                <asp:Button ID="btnSaveFilter" runat="server" Text="Save Report Filters" OnClick="btnSaveFilters_Click" Enabled="False"
                    CssClass="btn btn-primary btn-sm"></asp:Button>
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" OnClick="btnClearFilters_Click" CssClass="btn btn-primary btn-sm"></asp:Button>
            </div>
        </asp:Panel>
    </asp:Panel>
    <hr style="margin-top: 5px; margin-bottom: 10px;" />
    <%--<asp:Label ID="lblDateFilter" runat="server" Text="Date"></asp:Label>
            <asp:Label ID="lblFromDate" Visible="False" runat="server" Text="From"></asp:Label>
            <telerik:raddatepicker id="dateFrom" width="150px" skin="Metro" runat="server">
            <DateInput DateFormat="dd-MM-yyyy"> 
            </DateInput>
        </telerik:raddatepicker>
            <asp:Label ID="lblFromTo" runat="server" Visible="False" Text="To"></asp:Label>
            <telerik:raddatepicker id="RadDatePicker1" visible="False" width="150px" skin="Metro" runat="server">
            <DateInput DateFormat="yyyy-MM-dd"> 
            </DateInput>
        </telerik:raddatepicker>--%>
    <%--<telerik:radbutton id="btnApplyFilters" runat="server" text="Apply Filters" UseSubmitBehavior="True">
        </telerik:radbutton>--%>

    <!--  INIZIO USERCONTROL --->
    <asp:Panel runat="server" ID="pnlCharts">
        <div id="row">
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </div>
        <asp:Literal ID="Literal1" runat="server" EnableViewState="false"></asp:Literal>
        <hr />

        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
        <asp:EntityDataSource ID="EdsResponseStatus" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableFlattening="False" EntitySetName="FormResponseStatus" Select="it.[ResponseStatusName], it.[ResponseStatusID]">
        </asp:EntityDataSource>
    </asp:Panel>
    <telerik:radwindowmanager id="RadWindow1" runat="server">
        <Windows>
            <telerik:RadWindow ID="wndFilterForm" Skin="Metro" VisibleStatusbar="false" Modal="true" 
                Behaviors="Default" runat="server" Overlay="true" ShowContentDuringLoad="false" 
                VisibleOnPageLoad="false" ReloadOnShow="true" OnClientClose="OnClientCloseFiltersForm"></telerik:RadWindow>
        </Windows>
    </telerik:radwindowmanager>
<%--    <telerik:radskinmanager id="RadSkinManager1" runat="server" showchooser="true" />
    <telerik:radajaxloadingpanel id="RadAjaxLoadingPanel1" runat="server" transparency="10" updatepanelrendermode="Block">
        <img src="../_images/preloader1.gif" />
    </telerik:radajaxloadingpanel>
    <telerik:radajaxmanager runat="server" id="RadAjaxManager1" defaultloadingpanelid="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSaveFilter">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlFilters" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnClearFilters">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Content2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ddlResponseStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Content2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:radajaxmanager>--%>
</asp:Content>
