<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Settings" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .settingsPropPnl { margin-top: 50px; }
        .cbGroup { padding: 15px; border: 1px solid #ccc; margin: 0 0 25px 0; position: relative; }
            .cbGroup h4 { font-size: 12px; font-weight: bold; display: block; position: absolute; top: -10px; margin: 0; border: 1px solid #ccc; padding: 2px; background: #eee; width: 150px; text-align: center; }
            .cbGroup label { margin: 0 20px 0 5px; }
            .cbGroup input[type="checkbox"] { width: 15px; height: 15px; padding: 0; margin: 0; vertical-align: bottom; position: relative; top: -1px; *overflow: hidden; }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <div class="row">
        <div class="col-lg-12">
            <h1>Settings</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home</a></li>
                <li class="current"><i class="fa fa-cogs"></i>Settings</li>
            </ol>
        </div>
    </div>

    <telerik:RadTabStrip ID="TsSettingsMenu" runat="server" MultiPageID="MpSettings" SelectedIndex="0" Skin="Metro" OnTabClick="TsSettingsMenu_TabClick">
        <Tabs>
            <telerik:RadTab runat="server" Text="Roles" PageViewID="PvRoles" Selected="True">
            </telerik:RadTab>
            <telerik:RadTab runat="server" Text="HomePage Content" PageViewID="PvHomePageContent">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <div class="row">
        <div class="col-lg-12">
            <div class="form-group">
                <telerik:RadMultiPage ID="MpSettings" runat="server">
                    <telerik:RadPageView ID="PvHomePageContent" runat="server">
                        <h3>HomePage Content</h3>
                        <telerik:RadEditor ID="RadEditor1" Skin="Metro" runat="server" ContentAreaCssFile="~/_css/RadEditorCustomCss.css"></telerik:RadEditor>
                        <telerik:RadButton ID="saveInfo" runat="server" Skin="MetroTouch" BackColor="White" Text="Save" OnClick="saveInfo_Click"></telerik:RadButton>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="PvRoles" runat="server" Selected="True">
                        <h3>Roles</h3>
                        Select a Role to manage:
                        <telerik:RadComboBox ID="DdlRoles" runat="server" AutoPostBack="true" EmptyMessage="Select a role..." Width="300px" DataTextField="description" DataValueField="id" OnSelectedIndexChanged="DdlRoles_SelectedIndexChanged"></telerik:RadComboBox>
                        <br />
                        <asp:Panel ID="PnlRoleReviewAssociation" CssClass="settingsPropPnl" runat="server" Visible="false">                            
                            <div class="cbGroup">
                                <h4>Reviewable Status</h4>
                                <asp:CheckBoxList ID="CblReviewableStatus" runat="server" RepeatDirection="Horizontal" DataTextField="ResponseStatusName" DataValueField="ResponseStatusID">
                                </asp:CheckBoxList>
                            </div>
                            <div class="cbGroup">
                                <h4>Selectable Status</h4>
                                <asp:CheckBoxList ID="CblSelectableStatus" runat="server" RepeatDirection="Horizontal" DataTextField="ResponseStatusName" DataValueField="ResponseStatusID">
                                </asp:CheckBoxList>
                            </div>
                            <div class="cbGroup">
                                <h4>Permissions</h4>
                                <asp:CheckBoxList ID="cblPermissions" runat="server" RepeatDirection="Horizontal" DataTextField="Description" DataValueField="id">
                                </asp:CheckBoxList>
                            </div>
                            <telerik:RadButton ID="BtnSaveRoleChanges" runat="server" Text=" Save Changes " SingleClick="true" SingleClickText="Saving..." OnClick="BtnSaveRoleChanges_Click"></telerik:RadButton>
                            <asp:Label ID="LblMessage" runat="server"></asp:Label>
                        </asp:Panel>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>

            </div>
        </div>
    </div>
</asp:Content>

