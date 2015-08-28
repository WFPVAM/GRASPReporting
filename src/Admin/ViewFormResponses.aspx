<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewFormResponses.aspx.cs" Inherits="Admin_ViewFormResponses" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/sb-admin.css" rel="stylesheet">
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
        .col3 { margin: 0; padding: 0 3px 0 0; width: 110px; line-height: 13px; float: left; }
        .col2 { width: 300px; }
        #navbar {margin-bottom:5px;padding:5px;border:1px solid #ccc;background:#eee;font-size:12px; }
        #btnApplyQuickFilter { margin-left: 15px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <asp:Literal ID="litInfo" runat="server"></asp:Literal>


        <div id="navbar">
            <span><strong>Quick Filter | </strong></span>
            <span>Status: </span>
            <telerik:RadComboBox ID="ddlReviewStatus" runat="server" EnableLoadOnDemand="True" Skin="Metro" AppendDataBoundItems="true" BorderColor="#66AFE9" BackColor="White" DataSourceID="edsResponseStatus" DataTextField="ResponseStatusName" DataValueField="ResponseStatusID" OnSelectedIndexChanged="ddlReviewStatus_SelectedIndexChanged">
                <Items>
                    <telerik:RadComboBoxItem Text="Any" Value="0" />
                </Items>
            </telerik:RadComboBox>
            <span>Date Filter: </span>
            <telerik:RadComboBox ID="ddlDate" runat="server" Skin="Metro" OnSelectedIndexChanged="ddlDate_SelectedIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
            <telerik:RadDatePicker ID="rdpCustomDate" runat="server" Width="140px" AutoPostBack="true"
                DateInput-EmptyMessage="Select a start date" MinDate="01/01/1000" MaxDate="01/01/3000">
                <Calendar>
                    <SpecialDays>
                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday">
                        </telerik:RadCalendarDay>
                    </SpecialDays>                    
                </Calendar>
            </telerik:RadDatePicker>
            <span>Sender: </span>
            <asp:TextBox ID="txtSender" runat="server"></asp:TextBox>

            <telerik:RadButton ID="btnApplyQuickFilter" runat="server" Text="Apply Quick Filter" Skin="Metro" CssClass="btnApplyQuickFilter" OnClick="btnApplyQuickFilter_Click"></telerik:RadButton>
        </div>

        <div>
            <asp:HiddenField ID="hdnFilterCount" runat="server" />
            <asp:Panel ID="filterSummary" runat="server" Visible="false" CssClass="customFilterSummaryLabel">
                <asp:Label ID="lblFilterSummary" runat="server" Text=""></asp:Label>
            </asp:Panel>
        </div>

        <telerik:RadGrid ID="RadGrid1" runat="server" AllowPaging="True" AllowCustomPaging="True" AllowSorting="false" CellSpacing="0"
            EnableViewState="true" GridLines="None" OnNeedDataSource="RadGrid1_NeedDataSource" Skin="Metro" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White" AutoGenerateColumns="False">
            <ExportSettings>
                <Pdf PageWidth="" />
            </ExportSettings>
            <MasterTableView DataKeyNames="id">
                <Columns>
                    <telerik:GridBoundColumn DataField="id" DataType="System.Int32" HeaderText="Response ID" UniqueName="id">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FRCreateDate" DataType="System.DateTime" HeaderText="Received on" UniqueName="FRCreateDate">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
<%--                    <telerik:GridBoundColumn DataField="CompileDate" DataType="System.DateTime" HeaderText="Compiled on">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>--%>
                    <telerik:GridBoundColumn DataField="senderMsisdn" DataType="System.String" HeaderText="Sender">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
<%--                    <telerik:GridBoundColumn DataField="Enumerator" DataType="System.String" HeaderText="Enumerator">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>--%>
                    <telerik:GridBoundColumn DataField="clientVersion" DataType="System.String" HeaderText="Client">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ResponseStatus" DataType="System.String" HeaderText="Status">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="">

                        <ItemTemplate>
                            <a href='viewForm.aspx?id=<%# Eval("id") %>' target="_blank">View</a>
                        </ItemTemplate>

                    </telerik:GridTemplateColumn>



                </Columns>


                <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>

                <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>

                <EditFormSettings>
                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                </EditFormSettings>

                <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
            </MasterTableView>
            <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
            <FilterMenu EnableImageSprites="False"></FilterMenu>
        </telerik:RadGrid>

    </div>

    <asp:EntityDataSource ID="edsResponseStatus" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableFlattening="False" EntitySetName="FormResponseStatus" Select="it.[ResponseStatusName], it.[ResponseStatusID]">
    </asp:EntityDataSource>
    </form>
</body>
</html>
