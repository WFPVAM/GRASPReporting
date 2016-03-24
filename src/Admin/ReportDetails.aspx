<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="ReportDetails.aspx.cs" Inherits="Admin_ReportDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        //<![CDATA[
        function onRowDropping(sender, args) {
            if (sender.get_id() == "<%=rgReports.ClientID %>") {
                    var node = args.get_destinationHtmlElement();
                    if (!isChildOf('<%=rgReports.ClientID %>', node) && !isChildOf('<%=rgReports.ClientID %>', node)) {
                        args.set_cancel(true);
                    }
                }
                else {
                    var node = args.get_destinationHtmlElement();
                    if (!isChildOf('trashCan', node)) {
                        args.set_cancel(true);
                    }
                    else {
                        if (confirm("Are you sure you want to delete this order?"))
                            args.set_destinationHtmlElement($get('trashCan'));
                        else
                            args.set_cancel(true);
                    }
                }
            }

            function isChildOf(parentId, element) {
                while (element) {
                    if (element.id && element.id.indexOf(parentId) > -1) {
                        return true;
                    }
                    element = element.parentNode;
                }
                return false;
            }
            //]]>
        </script>
    <style type="text/css">
        .color {
            color: #0058B1;
            margin-left: 5px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Report Details</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home </a></li>
                <li><i class="fa fa-bar-chart-o"></i><a href="ViewReports.aspx">Reports </a></li>
                <li class="current"><i class="fa fa-tasks"></i>Report Detail</li>
            </ol>
        </div>
    </div>
    <div>
        <telerik:RadGrid ID="rgReports" runat="server" CellSpacing="0" GridLines="None" Skin="Metro" 
            OnDeleteCommand="rgReports_DeleteCommand" OnRowDrop="rgReports_RowDrop"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White" OnNeedDataSource="rgReports_NeedDataSource">
            
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="ReportFieldID" >                
                <Columns>
                    <telerik:GridBoundColumn DataField="ChartType" HeaderText="Chart Type" ReadOnly="True"
                        UniqueName="ChartType">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ChartTitle" HeaderText="Chart Title"
                        UniqueName="ChartTitle">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Aggregate" HeaderText="Aggregate Function"
                        UniqueName="Aggregate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Create Date"
                        UniqueName="CreateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="DeleteColumn" HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="javascript:if(!confirm('This action will delete the selected chart. Are you sure?')){return false;}"
                                CommandName="Delete"><i class="color fa fa-trash-o fa-2"></i><span class="color">Delete Chart</span></asp:LinkButton>
                            <%--<asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Chart"
                                OnClientClick="javascript:if(!confirm('This action will delete the selected chart. Are you sure?')){return false;}"
                                 CommandName="Delete" ImageUrl="../_images/Cross.png" />--%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>

            
                <ClientSettings AllowRowsDragDrop="True">
                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                    <ClientEvents OnRowDropping="onRowDropping"></ClientEvents>
                </ClientSettings>
        </telerik:RadGrid>
        <telerik:RadButton ID="BtnAddNewChart" runat="server" Text=" Add New Chart " OnClick="BtnAddNewChart_Click" Skin="Metro"></telerik:RadButton>

    </div>
</asp:Content>

