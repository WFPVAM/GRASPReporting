<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserFilter.aspx.cs" Inherits="Admin_UserFilter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        body { font-family: Arial; }
        .rcbHeader ul,
        .rcbFooter ul,
        .rcbItem ul,
        .rcbHovered ul,
        .rcbDisabled ul { margin: 0; padding: 0; width: 100%; display: inline-block; list-style-type: none; }
        .rcbHovered { background: #bbeaf3; }
        .col1,
        .col2,
        .col3 { margin: 0; padding: 0 3px 0 0; width: 110px; line-height: 13px; float: left; font-size: 11px; }
        .col2 { width: 300px; font-size: 11px; }
        .col1 { font-weight: bold;width:160px; }
        .rcbHeader ul li { font-weight: bold; }
        .rcbItem ul { border-bottom: 1px dashed #ddd; }
        #filterSummary { width: 620px; border: 1px solid #666; background: #eee; padding: 10px; height: 100px; margin: 2px 0; }
        #btnBox, #filterBox { width: 620px; text-align: center; background: #CCE6FF; padding: 10px; border: 1px solid #0058B1; }
            #btnBox .btn { margin-right: 50px; }
        .testResult { font-size: 12px; margin: 15px 5px; }
        #infoField { position: absolute; top: 35px; left: 660px; width: 200px; height: 200px; background: #fef9a4; font-size: 11px; padding: 5px; }
            #infoField label { display: inline-block; width: 100px; }
        .pnlSave { text-align: center; width: 620px; border: 1px solid #ddd; padding: 10px; }
        #pnlSelectUser { margin-bottom: 5px; }
            #pnlSelectUser label { font-size: 12px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ddlFormFields">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ddlOperator" UpdatePanelCssClass="" />
                    <telerik:AjaxUpdatedControl ControlID="txtFilterVal" LoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelCssClass="" />
                    <telerik:AjaxUpdatedControl ControlID="litFieldInfo" UpdatePanelCssClass="" />
                    <telerik:AjaxUpdatedControl ControlID="ddlSurveyValues" LoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelCssClass="" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

            <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
        <img src="../_images/preloader1.gif" />
    </telerik:RadAjaxLoadingPanel>

    <div>
        <asp:Literal ID="litInfo" runat="server"></asp:Literal>

        <asp:Panel ID="pnlSelectUser" runat="server">
            <label>Select a User:</label>
            <telerik:RadComboBox ID="DdlUsers" runat="server" DataTextField="username" EmptyMessage="Select a User"
                DataValueField="user_id" Skin="Metro" ForeColor="#222" HighlightTemplatedItems="true" DropDownWidth="650px" Width="250px">
                <HeaderTemplate>
                    <ul>
                        <li class="col1">User Name</li>
                        <li class="col2">Role</li>
                        <li class="col3">Existing Filter</li>
                    </ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <ul>
                        <li class="col1">
                            <%# Eval("username") %></li>
                        <li class="col2">
                            <%# Eval("supervisor") %></li>
                        <li class="col3">
                            <%# Eval("UserFilterDescription") %></li>
                    </ul>
                </ItemTemplate>
            </telerik:RadComboBox>
        </asp:Panel>
        <div>
            <asp:HiddenField ID="hdnFilterCount" runat="server" />
            <div id="filterBox">
                <telerik:RadComboBox ID="ddlSQLOperator" runat="server" Skin="Metro" Width="90px" Visible="false">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="ddlFormFields" runat="server" DataTextField="name" EmptyMessage="Select a form field" AutoPostBack="true"
                    DataValueField="id" Skin="Metro" ForeColor="#222" HighlightTemplatedItems="true" DropDownWidth="650px" Width="250px" OnSelectedIndexChanged="ddlFormFields_SelectedIndexChanged">
                    <HeaderTemplate>
                        <ul>
                            <li class="col1">Name</li>
                            <li class="col2">Label</li>
                            <li class="col3">Type</li>
                        </ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <ul>
                            <li class="col1">
                                <%# Eval("name") %></li>
                            <li class="col2">
                                <%# Eval("label") %></li>
                            <li class="col3">
                                <asp:Label ID="lblType" runat="server" Text='<%# Eval("type") %>'></asp:Label></li>
                        </ul>
                    </ItemTemplate>

                </telerik:RadComboBox>
                <telerik:RadComboBox ID="ddlOperator" runat="server" Skin="Metro" Width="100px">
                </telerik:RadComboBox>
                <asp:TextBox ID="txtFilterVal" runat="server"></asp:TextBox>
                <telerik:RadComboBox ID="ddlSurveyValues" runat="server" Skin="Metro" Width="150px" DataTextField="value">
                </telerik:RadComboBox>
                <telerik:RadButton ID="btnAddFilter" runat="server" Text="Add Filter" OnClick="btnAddFilter_Click"></telerik:RadButton>
            </div>
            <div id="infoField">
                <asp:Literal ID="litFieldInfo" runat="server"></asp:Literal>
            </div>
            <div style="display: none;">
                <asp:Label ID="lblFilter" runat="server" Text=""></asp:Label>
            </div>
            <div id="filterSummary">
                <asp:Label ID="lblFilterSummary" runat="server" Text=""></asp:Label>
            </div>
            <div id="btnBox">
                <telerik:RadButton ID="btnRemoveLastEntry" Visible="false" runat="server" CssClass="btn" Text="Remove Last Entry" OnClick="btnRemoveLastEntry_Click"></telerik:RadButton>
                <telerik:RadButton ID="btnClearAll" runat="server" CssClass="btn" Text="Clear" OnClick="btnClearAll_Click"></telerik:RadButton>
                <telerik:RadButton ID="btnTest" runat="server" Text="Test Filter" CssClass="btn" OnClick="btnTest_Click"></telerik:RadButton>
            </div>
            <asp:Label ID="lblTestResult" runat="server" Text="" Visible="false" CssClass="testResult"></asp:Label>
        </div>
    </div>
    <asp:Panel ID="pnlSave" runat="server" CssClass="pnlSave" Visible="false">
        <span style="font-size:13px;"><strong>In order to activate the filter on this user you must click on "Apply Filter to User" button</strong></span><br />
        <br />
        <telerik:RadButton ID="BtnSave" runat="server" Text="Apply Filter to User" SingleClick="true" SingleClickText=" Applying Filter... PLEASE WAIT " ToolTip="Click this button to apply the filter to the selected user" OnClick="BtnSave_Click">
        </telerik:RadButton>

    </asp:Panel>

    </form>
</body>
</html>
