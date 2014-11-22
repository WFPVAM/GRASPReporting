<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomFilters.aspx.cs" Inherits="Admin_CustomFilter" %>

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
        .col3 { margin: 0; padding: 0 3px 0 0; width: 110px; line-height: 13px; float: left;font-size:11px; }
        .col2 { width: 300px;font-size:11px; }
        .col1 { font-weight: bold; }
        .rcbHeader ul li { font-weight: bold; }
        .rcbItem ul { border-bottom: 1px dashed #ddd; }
        #filterSummary {width: 620px; border: 1px solid #666; background: #eee; padding: 10px;  height: 200px;margin:2px 0; }
        #btnBox,#filterBox { width: 620px; text-align: center; background: #CCE6FF; padding: 10px;border:1px solid #0058B1 }
            #btnBox .btn { margin-right: 50px; }
        .testResult { font-size: 12px; margin: 15px 5px; }
        #infoField { position: absolute; top: 10px; left: 660px; width: 200px; height: 200px;background:#fef9a4;font-size:11px;padding:5px; }
            #infoField label { display: inline-block; width: 100px; }
        .pnlExportView { text-align: center; width: 620px; border: 1px solid #ddd; padding: 10px; }
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

        <div>
            <asp:HiddenField ID="hdnFilterCount" runat="server" />
            <div id="filterBox">
                <telerik:RadComboBox ID="ddlSQLOperator" runat="server" Skin="Metro" Width="90px" Visible="false">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="ddlFormFields" runat="server" DataTextField="name" EmptyMessage="Select a form field" AutoPostBack="true"
                    DataValueField="id" Skin="Metro" ForeColor="#222" HighlightTemplatedItems="true" DropDownWidth="600px" Width="250px" OnSelectedIndexChanged="ddlFormFields_SelectedIndexChanged">
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
                <telerik:RadButton ID="btnRemoveLastEntry" runat="server" CssClass="btn" Text="Remove Last Entry" OnClick="btnRemoveLastEntry_Click"></telerik:RadButton>
                <telerik:RadButton ID="btnClearAll" runat="server" CssClass="btn" Text="Clear All" OnClick="btnClearAll_Click"></telerik:RadButton>
                <telerik:RadButton ID="btnTest" runat="server" Text="Test Filter" CssClass="btn" OnClick="btnTest_Click"></telerik:RadButton>
            </div>
            <asp:Label ID="lblTestResult" runat="server" Text="" Visible="false" CssClass="testResult"></asp:Label>
        </div>
    </div>
    <asp:Panel ID="pnlExportView" runat="server" CssClass="pnlExportView" Visible="false">

    <telerik:RadButton ID="btnExportData" runat="server" OnClick="btnExportData_Click" Text="Export Data">
    </telerik:RadButton>
    <telerik:RadButton ID="btnViewData" runat="server" OnClick="btnViewData_Click" Text="View Data">
    </telerik:RadButton>

    </asp:Panel>

    </form>
</body>
</html>
