<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="Admin_Statistics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Reports</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home</a></li>
                <li class="current"><i class="fa fa-bar-chart-o"></i>Reports</li>
            </ol>
        </div>
    </div>
    <div class="row">
        <div class="col-lg-4">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-xs-6">
                            <i class="fa fa-bar-chart-o fa-5x"></i>
                        </div>
                        <div class="col-xs-6 text-right">
                            <p class="announcement-heading"><i class="fa fa-plus"></i></p>
                            <p class="announcement-text">New Report</p>
                        </div>
                    </div>
                </div>
                <a href="CreateReport.aspx">
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-10">
                                Create new Report
                            </div>
                            <div class="col-xs-2 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </a>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-xs-6">
                            <i class="fa fa-tasks fa-5x"></i>
                        </div>
                        <div class="col-xs-6 text-right">
                            <p class="announcement-heading">
                                <asp:Literal ID="nOfReports" runat="server"></asp:Literal></p>
                            <p class="announcement-text">Reports</p>
                        </div>
                    </div>
                </div>
                <a href="ViewReports.aspx">
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-10">
                                View all Reports
                            </div>
                            <div class="col-xs-2 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </a>
            </div>
        </div>
    </div>
</asp:Content>

