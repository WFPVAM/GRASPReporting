<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewForm.aspx.cs" Inherits="Admin_ViewForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/bootstrap.css" rel="stylesheet">
    <link href="../_css/sb-admin.css" rel="stylesheet">
    <style type="text/css">
        #formContainer { width: 100%; font-size: 13px; text-align: center;height:90%; }
        #tblContainer { width: 90%; margin: auto; }
        #blankMargin { height: 250px;width:90%; }
        #tblContainer h1 { margin: 0 0 10px 0; padding: 5px 0; border: 3px solid #ccc; width: 100%; background: #eee; border-bottom: 1px solid #ddd; }
        .repContainer { height: 100%; min-height: 100%; overflow: hidden; border-top: 2px solid #0058b1; border-bottom: 2px solid #0058b1; }
        .repTitle { font-size: 16px; font-weight: bold; }
        .left { font-weight: bold; width: 50%; text-align: right; border-bottom: 1px solid #ddd; margin: 0; padding: 2px 0 0px 0; }
        .right { width: 49%; border-bottom: 1px dashed #ddd; margin: 0; padding: 2px 0 4px 0; text-align: left; }
        .inline { display: inline-block; padding: 0 2px; width: 100px; border-left: 1px solid #ddd; }
        .overflowTable { overflow-y: hidden; }
        img { width: 110px; height: auto; border: 0; }
        .pnlResponseStatus { width: 100%; height:45px; border: 1px solid #666; background: #eee; padding: 10px; text-align: center; position: fixed; bottom: 0px; }
        .currentStatus { padding: 6px; border-bottom: 1px solid #ccc; }
        .statusName { font-weight: bold; padding: 0 15px 0 5px; }
        .pnlHistory { text-align: left; }
        #editlnk { position: absolute; top: 5px; right: 5px; padding: 5px; border: 1px solid #ccc; background: #014; }
            #editlnk a { color: #fff; }
                #editlnk a:hover { background: #012; }
        #historyHideShow {position: absolute; top: 5px; left: 5px; padding: 5px; border: 1px solid #ccc; background: #014;color:#fff;cursor:pointer; }
        .up { height: 260px; }
    </style>
    
    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.5.1.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#historyHideShow').click(function () {
                $('.pnlResponseStatus').toggleClass('up');
            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <a ti></a>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>
    <div id="formContainer">
        <div id="tblContainer">
            <h1>
                <asp:Literal ID="litFormTitle" runat="server"></asp:Literal>
            </h1>
            <asp:Literal ID="litTableResult" runat="server"></asp:Literal>
        </div>
    </div>
    
    <br clear="all" />
        <div id="blankMargin"></div>
    <asp:Panel ID="pnlResponseStatus" runat="server" CssClass="pnlResponseStatus">
        <div id="historyHideShow">Show/Hide History</div>
        <div class="currentStatus">
            <label>Current Response Status</label>:
        <asp:Label ID="lblFormResponseStatus" runat="server" Text="" CssClass="statusName"></asp:Label>
            
            <telerik:RadButton ID="btnChangeStatus" runat="server" Text=" Change Status " OnClick="btnChangeStatus_Click"></telerik:RadButton>
        </div>
        <asp:Panel ID="pnlChangeStatus" runat="server" Visible="false">
            <telerik:RadComboBox ID="ddlFormResponseStatus" runat="server" EmptyMessage="Select a Status..."
                DataTextField="ResponseStatusName" DataValueField="ResponseStatusID" Skin="Metro" Width="280px">
            </telerik:RadComboBox>
            <br />
            <asp:TextBox ID="txtDetails" runat="server" TextMode="MultiLine" Height="60px" Width="270px"></asp:TextBox>
            <asp:Literal ID="LitMessage" runat="server"></asp:Literal>
           <br />
            <telerik:RadButton ID="btnGoBack" runat="server" Text=" Cancel " OnClick="btnGoBack_Click"></telerik:RadButton>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <telerik:RadButton ID="btnSaveStatusChange" runat="server" Text=" Save Changes " OnClick="btnSaveStatusChange_Click"></telerik:RadButton>
        </asp:Panel>
        <div id="editlnk">
            <asp:Literal ID="litEditLink" runat="server"></asp:Literal></div>
        <asp:Panel ID="pnlHistory" runat="server" Visible="true" CssClass="pnlHistory">
            <telerik:RadGrid ID="grdHistory" runat="server" OnNeedDataSource="grdHistory_NeedDataSource" Skin="Metro" Height="150px"
                AutoGenerateColumns="False" CellSpacing="-1" GridLines="Both">
                <ExportSettings>
                    <Pdf PageWidth="">
                    </Pdf>
                </ExportSettings>
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                </ClientSettings>
                <MasterTableView DataKeyNames="FormResponseReviewID">
                    <Columns>
                        <telerik:GridTemplateColumn DataField="FormResponseReviewID" DataType="System.Int32" FilterControlAltText="Filter FormResponseReviewID column" HeaderText="ReviewID" SortExpression="FormResponseReviewID" UniqueName="FormResponseReviewID">
                            <ItemTemplate>
                                <asp:Label ID="FormResponseReviewIDLabel" runat="server" Text='<%# Eval("FormResponseReviewID") %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="FormResponseID" DataType="System.Decimal" FilterControlAltText="Filter FormResponseID column" HeaderText="FormResponseID" SortExpression="FormResponseID" UniqueName="FormResponseID" Visible="false">
                            <ColumnValidationSettings>
                                <ModelErrorMessage Text="" />
                            </ColumnValidationSettings>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FRRUserName" FilterControlAltText="Filter FRRUserName column" HeaderText="Reviewer" SortExpression="FRRUserName" UniqueName="FRRUserName">
                            <ColumnValidationSettings>
                                <ModelErrorMessage Text="" />
                            </ColumnValidationSettings>
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="FormResponseReviewDate" DataType="System.DateTime" FilterControlAltText="Filter FormResponseReviewDate column" HeaderText="Review Date" SortExpression="FormResponseReviewDate" UniqueName="FormResponseReviewDate">
                            <ItemTemplate>
                                <asp:Label ID="FormResponseReviewDateLabel" runat="server" Text='<%# Eval("FormResponseReviewDate") %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="prevStatusName" DataType="System.Int32" FilterControlAltText="Filter FormResponsePreviousStatusID column" HeaderText="Previous Status" SortExpression="FormResponsePreviousStatusID" UniqueName="FormResponsePreviousStatusID">
                            <ItemTemplate>
                                <asp:Label ID="FormResponsePreviousStatusIDLabel" runat="server" Text='<%# Eval("prevStatusName") %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="currStatusName" DataType="System.Int32" FilterControlAltText="Filter FormResponseCurrentStatusID column" HeaderText="Current Status" SortExpression="FormResponseCurrentStatusID" UniqueName="FormResponseCurrentStatusID">
                            <ItemTemplate>
                                <asp:Label ID="FormResponseCurrentStatusIDLabel" runat="server" Text='<%# Eval("currStatusName") %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="FormResponseReviewDetail" FilterControlAltText="Filter FormResponseReviewDetail column" HeaderText="Comment" SortExpression="FormResponseReviewDetail" UniqueName="FormResponseReviewDetail">
                            <ColumnValidationSettings>
                                <ModelErrorMessage Text="" />
                            </ColumnValidationSettings>
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="FormResponseReviewSeqNo" DataType="System.Int32" FilterControlAltText="Filter FormResponseReviewSeqNo column" HeaderText="Rev No." SortExpression="FormResponseReviewSeqNo" UniqueName="FormResponseReviewSeqNo">
                            <ItemTemplate>
                                <a href='viewform.aspx?id=<%# Eval("FormResponseID") %>&amp;v=<%# Eval("FormResponseReviewID") %>'><%# Eval("FormResponseReviewSeqNo") %></a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:Literal ID="litViewLatest" runat="server"></asp:Literal>
        </asp:Panel>

    </asp:Panel>



    </form>
</body>
</html>
