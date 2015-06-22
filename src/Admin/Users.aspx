<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Admin_Users" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .color {
            color: #0058B1;
            margin-left: 5px;
        }

        .rgEditForm {
            border: 1px solid #666;
            font-size: 12px;
            width: 500px !important;
        }

            .rgEditForm .rgHeader {
                background: #0058B1;
                height: 30px !important;
            }

            .rgEditForm table {
                margin: 3px 0 15px 10px;
                width: 500px;
            }

                .rgEditForm table td {
                    vertical-align: top !important;
                }

                .rgEditForm table tr td:first-child table tr td:first-child {
                    width: 110px !important;
                }

                .rgEditForm table tr td label {
                    font-size: 12px;
                }

                .rgEditForm table tr td a {
                    font-weight: bold;
                    border: 1px solid #ccc;
                    padding: 5px;
                }

                    .rgEditForm table tr td a:hover {
                        background: #0058B1;
                        color: #fff;
                    }

                .rgEditForm table tr td:first-child table tr td input[type="text"],
                .rgEditForm table tr td:first-child table tr td input[type="password"] {
                    height: 21px;
                    padding: 2px;
                    font-size: 12px;
                    width: 200px;
                }

        .errMsg {
            color: #f00;
            font-size: 11px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Users</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home</a></li>
                <li class="current"><i class="fa fa-bar-chart-o"></i>Users</li>
            </ol>
        </div>
    </div>
    <div>
        <telerik:radgrid id="rgUser" runat="server" datasourceid="OdsUsers" skin="Metro"
            forecolor="#0058B1" bordercolor="White" alternatingitemstyle-backcolor="#CCE6FF" headerstyle-backcolor="#0058B1" headerstyle-forecolor="White"
            allowpaging="True" allowsorting="True" allowautomaticinserts="True" allowautomaticdeletes="True" allowautomaticupdates="True" onitemcommand="rgUser_ItemCommand">
            <ExportSettings>
                <Pdf PageWidth="">
                </Pdf>
            </ExportSettings>
            <AlternatingItemStyle BackColor="#CCE6FF"></AlternatingItemStyle>

            <MasterTableView DataSourceID="OdsUsers" AutoGenerateColumns="False" EditMode="PopUp" CommandItemDisplay="Top" DataKeyNames="user_id">
                <EditFormSettings>
                    <PopUpSettings Modal="true" />
                </EditFormSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="user_id" DataType="System.Int32" FilterControlAltText="Filter user_id column" HeaderText="ID" ReadOnly="True" SortExpression="user_id" UniqueName="user_id">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn ColumnEditorID="txtName" DataField="name" FilterControlAltText="Filter name column" HeaderText="First Name" SortExpression="name" UniqueName="name">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage=" *"></RequiredFieldValidator>
                            <ModelErrorMessage BackColor="Red" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="surname" FilterControlAltText="Filter surname column" HeaderText="Last Name" SortExpression="surname" UniqueName="surname">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn DataField="username" FilterControlAltText="Filter username column" HeaderText="UserName" SortExpression="username" UniqueName="username">
                        <EditItemTemplate>
                            <asp:TextBox ID="usernameTextBox" runat="server" Text='<%# Bind("username") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ForeColor="Red" Text="*" ControlToValidate="usernameTextBox"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="usernameLabel" runat="server" Text='<%# Eval("username") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Password" DataField="password" UniqueName="password" Visible="false">
                        <EditItemTemplate>
                            <asp:TextBox ID="passwordTextBox" TextMode="Password" runat="server" Text='<%# Bind("password") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ForeColor="Red" Text="*" ControlToValidate="passwordTextBox"></asp:RequiredFieldValidator>
                            <%--<asp:RegularExpressionValidator ID="Regex2" runat="server" ControlToValidate="passwordTextBox" CssClass="errMsg"
                                ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$"
                                ErrorMessage="Weak Password" ForeColor="Red" />--%>
                            <br />
                            <label>Confirm Password</label>
                            <br/>
                            <asp:TextBox ID="TxtPwdConfirm" TextMode="Password" runat="server" Text=''></asp:TextBox>
                            <asp:CompareValidator ID="CvPwd" runat="server" ErrorMessage="Paswords doesn't match" CssClass="errMsg" ControlToCompare="TxtPwdConfirm" ControlToValidate="passwordTextBox"></asp:CompareValidator>
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="phone_number" FilterControlAltText="Filter phone_number column" HeaderText="Phone" SortExpression="phone_number" UniqueName="phone_number" Visible="false">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="email" FilterControlAltText="Filter email column" HeaderText="Email" SortExpression="email" UniqueName="email" Visible="false">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridDropDownColumn DataField="roles_id" FilterControlAltText="Filter supervisor column" HeaderText="Role" SortExpression="roles_id" UniqueName="roles_id" DataSourceID="ldsRoles" ListValueField="id" ListTextField="description">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridDropDownColumn>
                    <telerik:GridTemplateColumn HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Edit"><i class="color fa fa-edit fa-2"></i><span class="color">Edit</span></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="javascript:if(!confirm('This action will delete the selected user. Are you sure?')){return false;}"
                                CommandName="Delete"><i class="color fa fa-trash-o fa-2"></i><span class="color">Delete</span></asp:LinkButton>
                            <%--<asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit User"
                                CommandName="Edit" ImageUrl="../_images/Edit.png" />
                            <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete User"
                                OnClientClick="javascript:if(!confirm('This action will delete the selected user. Are you sure?')){return false;}"
                                 CommandName="Delete" ImageUrl="../_images/Cross.png" />--%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                </Columns>
            </MasterTableView>

            <HeaderStyle BackColor="#0058B1" ForeColor="White"></HeaderStyle>
        </telerik:radgrid>

        <asp:ObjectDataSource ID="OdsUsers" runat="server" SelectMethod="getUsers"
            TypeName="User_Credential" UpdateMethod="updateUser" InsertMethod="addUser" DeleteMethod="deleteUser">
            <DeleteParameters>
                <asp:Parameter Name="user_id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="email" Type="String" />
                <asp:Parameter Name="name" Type="String" />
                <asp:Parameter Name="surname" Type="String" />
                <asp:Parameter Name="username" Type="String" />
                <asp:Parameter Name="password" Type="String" />
                <asp:Parameter Name="phone_number" Type="String" />
                <asp:Parameter Name="roles_id" Type="Int32" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="email" Type="String" />
                <asp:Parameter Name="name" Type="String" />
                <asp:Parameter Name="surname" Type="String" />
                <asp:Parameter Name="username" Type="String" />
                <asp:Parameter Name="password" Type="String" />
                <asp:Parameter Name="phone_number" Type="String" />
                <asp:Parameter Name="roles_id" Type="Int32" />
                <asp:Parameter Name="user_id" Type="Int32" />
            </UpdateParameters>
        </asp:ObjectDataSource>
        <asp:LinqDataSource ID="ldsRoles" runat="server" OnSelecting="ldsRoles_Selecting"></asp:LinqDataSource>
    </div>
</asp:Content>

