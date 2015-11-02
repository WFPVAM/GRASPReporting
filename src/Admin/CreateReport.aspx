<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="CreateReport.aspx.cs" Inherits="Admin_Statistics_CreateReport" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        #btnReset {
            float: right;
        }

        #btnNewLabel:hover {
            text-decoration: underline;
        }

        li.rcbItem, li.rcbHovered {
            font-size: 13px !important;
        }
    </style>
    <script type="text/javascript">
        var browserWidth = $(window).width();
        var browserHeight = $(window).height();
        function ViewHelp() {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwHelp");
            oWnd.setUrl("ViewReportHelp.aspx");
            oWnd.setSize(Math.ceil(browserWidth * 50 / 100), Math.ceil(browserHeight * 70 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function EnableDisableCustomLabels() {
            //var combo = $find("<%= rcbChartType.ClientID %>");
            var checkbo = $('#<%= chkTable.ClientID %>').is(':checked');
            if (checkbo) {
                $("#divCustomDataLabel").show();
                //if (combo.get_value() != "pie") {
                    $("#divCustomSeriesLabel").show();
                //}
            } else {
                $("#divCustomDataLabel").hide();
                $("#divCustomSeriesLabel").hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <telerik:RadWindowManager ID="RadWindow1" runat="server">
        <Windows>
            <telerik:RadWindow ID="wndwHelp" VisibleStatusbar="false" Modal="true" Behaviors="Default" runat="server" Overlay="true" ShowContentDuringLoad="false" VisibleOnPageLoad="false" ReloadOnShow="true"></telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <div class="row">
        <div class="col-lg-12">
            <h1>Reports</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="Reports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-edit"></i>New Report</li>
            </ol>
        </div>
    </div>
    <div id="success" class="row" runat="server" visible="false">
        <div class="col-lg-8">
            <div class="alert alert-dismissable alert-success">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                <strong>Chart successfully added. </strong> You successfully add a report. Add <asp:Button ID="btnNewLabel" runat="server" OnClick="btnNewLabel_Click" Text="another chart" ForeColor="#3a87ad" BackColor="Transparent" BorderColor="Transparent" />, or <a href="CreateReport.aspx">Create a New Report</a>.
            </div>
        </div>
    </div>
    <div id="error" class="row" runat="server" visible="false">
        <div class="col-lg-8">
            <div class="alert alert-dismissable alert-danger">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                &nbsp;Name you choose for this report already exists.
            </div>
        </div>
    </div>
    <div id="error2" class="row" runat="server" visible="false">
        <div class="col-lg-8">
            <div class="alert alert-dismissable alert-danger">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                &nbsp;Review your inserts.
            </div>
        </div>
    </div>
    <div class="row">
        <asp:Panel ID="pnlForm" runat="server">
            <div class="col-lg-4">

                <label>Report Name</label>
                <div class="form-group">

                    <asp:TextBox ID="tbReportName" runat="server" CssClass="form-control" placeholder="myReport"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ReportNameRequired" runat="server" ForeColor="Red" ControlToValidate="tbReportName" ErrorMessage="Report Name is required."
                        ToolTip="Report Name is required." ValidationGroup="CreatingReport"><i class="fa fa-warning"></i> Report Name is required</asp:RequiredFieldValidator>
                </div>
                <label>Report Description</label>
                <div class="form-group">

                    <asp:TextBox ID="tbReportDescription" runat="server" CssClass="form-control" placeholder="This is my report" TextMode="MultiLine" Rows="3"></asp:TextBox>
                </div>
                <label>Form</label>
                <div class="form-group">

                    <telerik:RadComboBox ID="ddlForm" runat="server" EmptyMessage="Select a form" EnableLoadOnDemand="true" Skin="MetroTouch" Width="100%" BorderColor="#66afe9" BackColor="White"
                        DataSourceID="ldsForm" CloseDropDownOnBlur="true" DataTextField="FormName" DataValueField="FormID" AppendDataBoundItems="true">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="FormRequired" runat="server" ForeColor="Red" ControlToValidate="ddlForm" ErrorMessage="Please choose a form"
                        ToolTip="Please choose a form." ValidationGroup="CreatingReport"><i class="fa fa-warning" ></i> Please choose a form</asp:RequiredFieldValidator>
                    <asp:LinqDataSource ID="ldsForm" runat="server" OnSelecting="ldsForm_Selecting" OrderBy="FormID"></asp:LinqDataSource>
                </div>
                <telerik:RadButton ID="btnReset" runat="server" Text="Reset Report" Skin="MetroTouch" BackColor="White" OnClick="btnReset_Click"></telerik:RadButton>
                <telerik:RadButton ID="btnForFields" runat="server" Skin="MetroTouch" BackColor="White" Text="Define Report" OnClick="btnForFields_Click" ValidationGroup="CreatingReport"></telerik:RadButton>

                <asp:HiddenField ID="hdnReportID" runat="server" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlReportFields" runat="server" Visible="false">
            <div id="reportFields" class="col-lg-6">
                <span style="font-size: 20px">Report name: <strong><%= reportName%></strong></span>
                <div style="margin: 10px;">
                    <h2 style="display: inline">Add Graph</h2>
                    <a href="javascript:ViewHelp();void(0);"><i class="fa fa-info-circle fa-2" id="linear"></i></a>
                    <telerik:RadToolTip ID="RadToolTip4" runat="server" RelativeTo="Element" Width="200px" AutoCloseDelay="10000" Skin="MetroTouch"
                        Text="Click here to view Help Page." TargetControlID="linear" IsClientID="true"
                        Position="MiddleRight">
                    </telerik:RadToolTip>
                </div>
                <div role="form">
                    <div class="form-group">                        
                        <label>Chart Title</label>
                        <asp:TextBox ID="TxtChartTitle" runat="server"></asp:TextBox>
                    </div>                        
                    <div class="form-group"> 
                        <label>Chart Type</label>
                        <telerik:RadComboBox ID="rcbChartType" runat="server" AutoPostBack="true" EnableLoadOnDemand="true" CloseDropDownOnBlur="true" OnSelectedIndexChanged="rcbChartType_SelectedIndexChanged"
                            EmptyMessage="Select a chart type" Skin="MetroTouch" OnClientItemChecked="EnableDisableCustomLabels()" Width="100%" BorderColor="#66afe9" BackColor="White">
                            <Items>
                                <telerik:RadComboBoxItem Text="Bar" Value="bar" />
                                <telerik:RadComboBoxItem Text="Pie" Value="pie" />
                                <telerik:RadComboBoxItem Text="Line" Value="line" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
                    <asp:Panel ID="pnlPie" runat="server" Visible="false">
                        <div class="form-group">
                            <label>Select Field</label>
                            <telerik:RadComboBox ID="ddlPieLabelFormField" runat="server" Skin="MetroTouch" Width="100%" BorderColor="#66afe9" BackColor="White" EnableLoadOnDemand="true"
                                CloseDropDownOnBlur="true" EmptyMessage="Select a label">
                            </telerik:RadComboBox>
                        </div>

                        <%--<div class="form-group">
                            <label>Custom Label</label>
                            <asp:TextBox ID="tbPieCustomReportFieldLabel" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>--%>
                    </asp:Panel>
                    <asp:Panel ID="pnlBar" runat="server" Visible="false">
                        <div class="form-group">
                            <label>Aggregate data by</label>
                            <telerik:RadComboBox ID="ddlSerieField" runat="server" Skin="MetroTouch" Width="100%" BorderColor="#66afe9" BackColor="White" EnableLoadOnDemand="true"
                                CloseDropDownOnBlur="true" EmptyMessage="Select a field">
                            </telerik:RadComboBox>
                        </div>
                        <div class="form-group">
                            <label>Select an aggregate function</label>
                            <telerik:RadComboBox ID="ddlAggregate" runat="server" AutoPostBack="true" EnableLoadOnDemand="true" CloseDropDownOnBlur="true" OnSelectedIndexChanged="ddlAggregate_SelectedIndexChanged"
                                EmptyMessage="Select an aggregate function" Skin="MetroTouch" Width="100%" BorderColor="#66afe9" BackColor="White">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Sum" Value="sum" />
                                    <telerik:RadComboBoxItem Text="Count" Value="count" />
                                    <telerik:RadComboBoxItem Text="Average" Value="average" />
                                    <telerik:RadComboBoxItem Text="Max" Value="max" />
                                    <telerik:RadComboBoxItem Text="Min" Value="min" />
                                    <telerik:RadComboBoxItem Text="St. Dev" Value="stdev" />
                                </Items>
                            </telerik:RadComboBox>
                        </div>
                        <div id="divSeriesField" class="form-group" runat="server">
                            <label>Select Series Value</label>
                            <telerik:RadComboBox ID="ddlValueField" runat="server" Skin="MetroTouch" Width="100%" BorderColor="#66afe9" BackColor="White" EnableLoadOnDemand="true"
                                CloseDropDownOnBlur="true" EmptyMessage="Select a field">
                            </telerik:RadComboBox>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlButton" runat="server" Visible="false">
                        <div class="form-group">
                            <asp:CheckBox ID="chkLegend" runat="server" Checked="true" />
                            Show Legend
                        </div>
                        <div id="divChkTabularData" runat="server" class="form-group">
                            <asp:CheckBox ID="chkTable" runat="server" OnClick="JavaScript:EnableDisableCustomLabels();" />
                            Show Tabular Data
                        </div>
                        <div id="divCustomDataLabel" class="form-group">
                            <label>Custom Data Column Label</label>
                            <asp:TextBox ID="tbCustomSeriesLabel" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div id="divCustomSeriesLabel" class="form-group">
                            <label id="lblCustomSeriesColumn" runat="server">Custom Series Column Label</label>
                            <asp:TextBox ID="tbCustomValueLabel" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div style="float: right">
                            <telerik:RadButton ID="btnResetReport" runat="server" Text="Cancel" Skin="MetroTouch" BackColor="White" OnClick="btnResetReport_Click"></telerik:RadButton>
                            <telerik:RadButton ID="btnCreateReport" runat="server" Skin="MetroTouch" BackColor="White" Text="Create Graph" OnClick="btnCreateReport_Click"></telerik:RadButton>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

