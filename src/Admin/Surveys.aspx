<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Surveys.aspx.cs" Inherits="Admin_Surveys" %>



<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .formLink { margin-left: 5px; color: #0058B1; text-decoration: none; }
            .formLink:hover { text-decoration: underline; }
    </style>

    <script type="text/javascript">

        var browserWidth = $(window).width();
        var browserHeight = $(window).height();

        function ExportForm(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwExport");
            oWnd.setUrl("ExportData.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 70 / 100), Math.ceil(browserHeight * 60 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function ViewForm(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("ViewFormResponses.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 85 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function ViewFilter(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("CustomFilters.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 90 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function ViewCalculatedField(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("CalculatedFieldInsert.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 80 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        function CheckDuplicates(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("CHeckDuplicates.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 80 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }
        function UsersFilter(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("UserFilter.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 80 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }
        function ReviewRestore(id, name) {
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("wndwForm");
            oWnd.setUrl("ReviewRestore.aspx?FormID=" + id + "&FormName=" + name);
            oWnd.setSize(Math.ceil(browserWidth * 80 / 100), Math.ceil(browserHeight * 80 / 100));
            oWnd.center();
            oWnd.Show();
            return false;
        }

        //Not yet working
        //function ImportForm(id, name) {
        //    var oManager = GetRadWindowManager();
        //    var oWnd = oManager.GetWindowByName("wndwForm");
        //    oWnd.setUrl("ImportForm.aspx?FormID=" + id + "&FormName=" + name);
        //    oWnd.setSize(Math.ceil(browserWidth * 50 / 100), Math.ceil(browserHeight * 60 / 100));
        //    oWnd.center();
        //    oWnd.Show();
        //    return false;
        //}

        function closeWin() {
            var tree = $find('<%=rgForm.ClientID %>');
            location.reload(true);
        }

<%--        function closeFormWindow() {
            var ss = '<%= Session["DeleteResponse"] %>';
            alert(ss);
            if (ss != null
                && ss == "True") {
                alert(ss);
                document.location.reload();
            }


        }--%>

        /**
           * Calls when closes the window form, it reloades the page if there are deleted responses to update the responses count.
		   */
        function OnClientCloseWindowForm(oWnd) {
            var deletedStatus = oWnd.argument;
            if (deletedStatus != null
                && deletedStatus == "responsesDeleted") {
                window.location.reload();
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <telerik:RadWindowManager ID="RadWindow1" runat="server">
        <Windows>
            <telerik:RadWindow ID="wndwExport" Skin="Metro" VisibleStatusbar="false" Modal="true" Behaviors="Default" runat="server" Overlay="true" ShowContentDuringLoad="false" VisibleOnPageLoad="false" ReloadOnShow="true"></telerik:RadWindow>
            <telerik:RadWindow ID="wndwForm" Skin="Metro" VisibleStatusbar="false" Modal="true" Behaviors="Default" 
                runat="server" Overlay="true" ShowContentDuringLoad="false" VisibleOnPageLoad="false" ReloadOnShow="true"
                OnClientClose="OnClientCloseWindowForm"></telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <div class="row">
        <div class="col-lg-12">
            <h1>Surveys</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home</a></li>
                <li class="current"><i class="fa fa-check-square-o"></i>Surveys</li>
            </ol>
        </div>
    </div>
    <div>
        <telerik:RadGrid ID="rgForm" runat="server" Height="100%" DataSourceID="ldsForm" Skin="Metro" ClientSettings-AllowDragToGroup="true" AllowPaging="True" AllowSorting="True"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White" AutoGenerateColumns="False">
            <ExportSettings>
                <Pdf PageWidth="">
                </Pdf>
            </ExportSettings>

            <AlternatingItemStyle BackColor="#CCE6FF"></AlternatingItemStyle>

            <MasterTableView DataSourceID="ldsForm" DataKeyNames="Id">
                <Columns>
                    <telerik:GridTemplateColumn DataField="Name" FilterControlAltText="Filter Name column" HeaderText="Form Name" UniqueName="Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <%# "<a class=\"formLink\" href=\"javascript:ViewForm('" +Eval("Id") + "','" + Eval("Name") + "');void(0);\">" + Eval("Name") + "</a>"  %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Create Date"
                        UniqueName="CreateDate">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Owner" HeaderText="Owner"
                        UniqueName="Owner">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Group" HeaderText="Group"
                        UniqueName="Group">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Responses" HeaderText="Responses Count"
                        UniqueName="Responses">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Actions">
                        <ItemTemplate>
                            <div class="dropdown">
                                <button class="btn btn-primary btn-xs dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown">
                                    More <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu1" style="left: auto; right: 0; margin-right: -10px;">
                                    <%--                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ImportForm('" +  Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-upload fa-2\"></i>Import</a>"%>
                                    </li>--%>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ViewForm('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-eye fa-2\"></i> View</a>"%>
                                    </li>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ViewFilter('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-filter fa-2\"></i> Custom Filter</a>"%>
                                    </li>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ViewCalculatedField('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-calculator fa-2\"></i> Add Calculated Fields</a>"%>
                                    </li>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ExportForm('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-download fa-2\"></i> Export</a>"%>
                                    </li>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:CheckDuplicates('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-check fa-2\"></i> Check Duplicates</a>"%>
                                    </li>
                                    <li role="presentation" class="divider"></li>
                                    
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:UsersFilter('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-users fa-2\"></i> Users Filter</a>"%>
                                    </li>
                                    <li role="presentation">
                                        <%# "<a role=\"menuitem\" tabindex=\"-1\" href=\"javascript:ReviewRestore('" + Eval("Id") + "','" +  Eval("Name") + "');void(0);\"><i class=\"fa fa-refresh fa-2\"></i> Review Restore</a>"%>
                                    </li>
                                </ul>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>

            <HeaderStyle BackColor="#0058B1" ForeColor="White"></HeaderStyle>
        </telerik:RadGrid>
        <asp:LinqDataSource ID="ldsForm" runat="server" OnSelecting="ldsForm_Selecting"></asp:LinqDataSource>
    </div>
</asp:Content>

