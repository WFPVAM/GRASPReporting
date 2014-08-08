<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Surveys.aspx.cs" Inherits="Admin_Surveys" %>



<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">

        var browserWidth = $(window).width();
        var browserHeight = $(window).height();

        function ExportSettings(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwExport");
            oWnd.setUrl("ExportData.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 50 / 100), Math.ceil(browserHeight * 60 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function ViewForm(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("ViewFormResponse.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 80 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function ImportForm(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("ImportForm.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 50 / 100), Math.ceil(browserHeight * 60 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function closeWin() {
            var tree = $find('<%=rgForm.ClientID %>');
            location.reload(true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <telerik:RadWindowManager ID="RadWindow1" runat="server">
        <Windows>
            <telerik:RadWindow ID="wndwExport" VisibleStatusbar="false" Modal="true" Behaviors="Default" runat="server" Overlay="true" ShowContentDuringLoad="false" VisibleOnPageLoad="false" ReloadOnShow="true"></telerik:RadWindow>
            <telerik:RadWindow ID="wndwForm" VisibleStatusbar="false" Modal="true" Behaviors="Default" runat="server" Overlay="true" ShowContentDuringLoad="false" VisibleOnPageLoad="false" ReloadOnShow="true" OnClientClose="closeWin"></telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <div class="row">
        <div class="col-lg-12">
            <h1>Surveys</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx" >Home</a></li>
                <li class="current"><i class="fa fa-check-square-o"></i>Surveys</li>
            </ol>
        </div>
    </div>
    <div>
        <telerik:RadGrid ID="rgForm" runat="server" Height="100%" DataSourceID="ldsForm" CellSpacing="0" GridLines="None" Skin="MetroTouch" PageSize="10" ClientSettings-AllowDragToGroup="true" AllowPaging="True" AllowSorting="True"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White" ShowGroupPanel="True">
        <MasterTableView DataSourceID="ldsForm" AutoGenerateColumns="False" DataKeyNames="Id">
               <Columns>
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Form Name" ReadOnly="True"
                        UniqueName="Name">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Create Date"
                        UniqueName="CreateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Owner" HeaderText="Owner"
                        UniqueName="Owner">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Group" HeaderText="Group"
                        UniqueName="Group">
                    </telerik:GridBoundColumn>
                   <telerik:GridBoundColumn DataField="Responses" HeaderText="Responses Count"
                        UniqueName="Responses">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Actions">
                        <ItemTemplate>
                            <%# "<a style=\"color:#0058B1\" href=\"javascript:ImportForm('" +  Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-upload fa-2\"></i>Import</a>"%>
                            <%# getLinks(Eval("Responses").ToString(), Eval("Id").ToString(), Eval("Name").ToString())%>
                            <%--<asp:ImageButton ID="btnImport" runat="server" AutoPostBack="false" AlternateText="Import Form"
                                ImageUrl="../_images/Upload.png" OnClientClick='<%# String.Format("ImportForm({0},\"{1}\");return false;",Eval("Id"), Eval("Name"))%> ' ToolTip="Import"/>
                            <asp:ImageButton ID="btnView" runat="server" AutoPostBack="false" AlternateText="View Form"  ImageUrl="../_images/View.png" ToolTip="View Responses"
                                OnClientClick='<%# String.Format("ViewForm({0},\"{1}\");return false;",Eval("Id"), Eval("Name"))%> ' Visible='<%# !string.Equals(Eval("Responses").ToString(), "0") %>'/>
                            <asp:ImageButton ID="btnExport" runat="server" AutoPostBack="false" AlternateText="Export Form" Visible='<%# !string.Equals(Eval("Responses").ToString(), "0") %>'
                                ImageUrl="../_images/Download.png" OnClientClick='<%# String.Format("ExportSettings({0},\"{1}\");return false;",Eval("Id"), Eval("Name"))%> ' ToolTip="Export"/>--%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <asp:LinqDataSource ID="ldsForm" runat="server" OnSelecting="ldsForm_Selecting" OrderBy="CreateDate"></asp:LinqDataSource>
    </div>
</asp:Content>

