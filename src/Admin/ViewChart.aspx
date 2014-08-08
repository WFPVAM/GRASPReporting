<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="ViewChart.aspx.cs" Inherits="Admin_Statistics_ViewChart" %>

<%@ Register Src="../_uc/pieChart.ascx" TagName="pieChart" TagPrefix="uc1" %>

<%@ Register src="../_uc/barChart.ascx" tagname="barChart" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../_css/kendo.dataviz.min.css" rel="stylesheet" />

    <script src="../_js/kendo.dataviz.min.js"></script>
    <script src="../_js/console.js"></script>
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                createCharts();
            }, 400);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1><%= ReportName%> </h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="ViewReports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-tasks"></i>View Reports</li>
            </ol>
        </div>
    </div>
    <!--  INIZIO USERCONTROL --->
    <div id="row">
        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>

    </div>
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
</asp:Content>

