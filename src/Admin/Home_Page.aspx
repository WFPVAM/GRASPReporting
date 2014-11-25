<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Home_Page.aspx.cs" Inherits="Admin_Dashboard" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Home Page</h1>
            <ol class="breadcrumb">
                <li class="current"><i class="fa fa-home"></i>Home</li>
            </ol>
        </div>
    </div>
    <div class="row">

        <div runat="server" id="PnlResponseProcessing" class="col-lg-4">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-xs-4">
                            <i class="fa fa-android fa-5x"></i>
                        </div>
                        <div class="col-xs-8 text-right">
                            <p class="announcement-heading">Incoming</p>
                            <p class="announcement-text">
                                <asp:Literal ID="LitIncomingInfo" runat="server"></asp:Literal>
                            </p>
                        </div>
                    </div>
                </div>

                <div class="panel-footer announcement-bottom">
                    <div class="row">
                        <div class="col-xs-6">
                            <telerik:RadButton ID="BtnProcessIncomingResponse" runat="server" Text=" Process Responses " SingleClick="true" SingleClickText=" Processing - Please Wait " Skin="Metro" OnClick="BtnProcessIncomingResponse_Click"></telerik:RadButton>
                        </div>
                        <div class="col-xs-6 text-right">
                            <i class="fa fa-download"></i>
                        </div>
                    </div>
                </div>
                <div class="panel-footer announcement-bottom">
                    <div class="row">
                        <div class="col-xs-6">
                            <telerik:RadProgressManager ID="RadProgressManager" runat="server" RefreshPeriod="1000" />
                            <telerik:RadProgressArea ID="RadProgressArea" Skin="Metro" ProgressIndicators="TotalProgressBar, TotalProgressPercent, TotalProgress, CurrentFileName, TimeEstimated" runat="server"></telerik:RadProgressArea>

                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div class="col-lg-4">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-xs-4">
                            <i class="fa fa-android fa-5x"></i>
                        </div>
                        <div class="col-xs-8 text-right">
                            <p class="announcement-heading">Mobile</p>
                            <p class="announcement-text">
                                <asp:Literal ID="ltrlInfo" runat="server"></asp:Literal>
                            </p>
                        </div>
                    </div>
                </div>

                <div class="panel-footer announcement-bottom">
                    <asp:HyperLink ID="lnkdwnload" runat="server" NavigateUrl="~/public/GraspMobile.apk">
                        <div class="row">
                            <div class="col-xs-6">
                                Download GRASP APK
                            </div>
                            <div class="col-xs-6 text-right">
                                <i class="fa fa-download"></i>
                            </div>
                        </div>
                    </asp:HyperLink>
                </div>
                <div class="panel-footer announcement-bottom">
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/public/BarcodeScanner.apk">
                        <div class="row">
                            <div class="col-xs-8">
                                Download BARCODE SCANNER
                            </div>
                            <div class="col-xs-4 text-right">
                                <i class="fa fa-download"></i>
                            </div>
                        </div>
                    </asp:HyperLink>

                    <div class="row" style="border-top: 1px solid #d9edf7; margin-top: 15px">
                        <div class="col-xs-12">
                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>

