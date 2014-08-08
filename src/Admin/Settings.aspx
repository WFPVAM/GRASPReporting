<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Settings" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Settings</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home</a></li>
                <li class="current"><i class="fa fa-cogs"></i>Settings</li>
            </ol>
        </div>
    </div>
    <div class="row">
        <div class="col-lg-6">
            <div class="form-group">
                <h3>HomePage Content</h3>
                <telerik:RadEditor ID="RadEditor1" Skin="Metro" runat="server" ContentAreaCssFile="~/_css/RadEditorCustomCss.css"></telerik:RadEditor>
                <telerik:RadButton ID="saveInfo" runat="server" Skin="MetroTouch" BackColor="White" Text="Save" OnClick="saveInfo_Click"></telerik:RadButton>
            </div>
        </div>
    </div>
</asp:Content>

