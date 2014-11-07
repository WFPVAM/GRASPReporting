<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckDuplicates.aspx.cs" Inherits="Admin_CheckDuplicates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <link rel="stylesheet" href="../_css/font-awesome2/css/font-awesome.min.css" />

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <h2>
            <asp:Literal ID="litFormName" runat="server"></asp:Literal></h2>

        <div id="navbar">
            <span>Index: </span>
            <telerik:RadComboBox ID="ddlIndexes" runat="server" Skin="Metro" OnSelectedIndexChanged="ddlIndexes_SelectedIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
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
        </div>




        <asp:Literal ID="litInfo" runat="server"></asp:Literal>

        <telerik:RadGrid ID="grdDuplicatedResponses" runat="server" AllowPaging="True" AllowCustomPaging="True" AllowSorting="True" CellSpacing="0"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White"
            EnableViewState="False" GridLines="None" OnNeedDataSource="grdDuplicatedResponses_NeedDataSource" Skin="Metro" AutoGenerateColumns="False" OnDetailTableDataBind="grdDuplicatedResponses_DetailTableDataBind" OnDeleteCommand="grdDuplicatedResponses_DeleteCommand">
            <ExportSettings>
                <Pdf PageWidth="" />
            </ExportSettings>
            <AlternatingItemStyle BackColor="#CCE6FF"></AlternatingItemStyle>
            <MasterTableView DataKeyNames="id">
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="id" Name="DuplicatedResponses" Width="100%" AllowCustomPaging="false"
                        HeaderStyle-BackColor="#0048A1" HeaderStyle-ForeColor="White" BorderColor="#0048A1" ItemStyle-BackColor="#dddddd" AlternatingItemStyle-BackColor="#cccccc">
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
                            <telerik:GridBoundColumn DataField="CompileDate" DataType="System.DateTime" HeaderText="Compiled on">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text=""></ModelErrorMessage>
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="senderMsisdn" DataType="System.String" HeaderText="Sender">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text=""></ModelErrorMessage>
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Enumerator" DataType="System.String" HeaderText="Enumerator">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text=""></ModelErrorMessage>
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="clientVersion" DataType="System.String" HeaderText="Client">
                                <ColumnValidationSettings>
                                    <ModelErrorMessage Text=""></ModelErrorMessage>
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="">
                                <ItemTemplate>
                                    <i class="fa fa-eye fa-2"></i>&nbsp;<a href='viewForm.aspx?id=<%# Eval("id") %>' target="_blank">View</a>
                                    &nbsp;|&nbsp;
                                    <i class="fa fa-remove fa-2"></i>&nbsp;
                                    <asp:LinkButton runat="server" Text="Delete" CommandName="Delete" OnClientClick="javascript:if(!confirm('This action will DELETE the selected response permanently. Are you sure?')){return false;}"></asp:LinkButton>
<%--                                    <a href="javascript:if(!confirm('This action will DELETE the selected response permanently. Are you sure?')){return false;}">Delete
                                    </a>--%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
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
                    <telerik:GridBoundColumn DataField="CompileDate" DataType="System.DateTime" HeaderText="Compiled on">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="senderMsisdn" DataType="System.String" HeaderText="Sender">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Enumerator" DataType="System.String" HeaderText="Enumerator">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="clientVersion" DataType="System.String" HeaderText="Client">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="">
                        <ItemTemplate>
                            <a href='CompareResponses.aspx?id=<%# Eval("id") %>&h=<%# Eval("hash") %>' target="_blank">Compare</a>
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
    </form>
</body>
</html>
