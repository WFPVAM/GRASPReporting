<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Admin_Users" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .color {
            color: #0058B1;
            margin-left: 5px;
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
        <telerik:RadGrid ID="rgUser" runat="server" DataSourceID="ObjectDataSource1" CellSpacing="0" GridLines="None" Skin="MetroTouch" 
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White"
            AllowPaging="True" AllowSorting="True" AllowAutomaticInserts="True" AllowAutomaticDeletes="true" AllowAutomaticUpdates="true">
            <AlternatingItemStyle BackColor="#CCE6FF"></AlternatingItemStyle>

            <MasterTableView DataSourceID="ObjectDataSource1" AutoGenerateColumns="False" CommandItemDisplay="Top" DataKeyNames="user_id">
                
                <Columns>
                    <telerik:GridBoundColumn DataField="user_id" DataType="System.Int32" FilterControlAltText="Filter user_id column" HeaderText="ID" ReadOnly="True" SortExpression="user_id" UniqueName="user_id">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="email" FilterControlAltText="Filter email column" HeaderText="Email" SortExpression="email" UniqueName="email">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="name" FilterControlAltText="Filter name column" HeaderText="First Name" SortExpression="name" UniqueName="name">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="surname" FilterControlAltText="Filter surname column" HeaderText="Last Name" SortExpression="surname" UniqueName="surname">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="username" FilterControlAltText="Filter username column" HeaderText="UserName" SortExpression="username" UniqueName="username">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="password" DataField="password" FilterControlAltText="Filter password column" SortExpression="password" UniqueName="password">
                        <EditItemTemplate>
                            <asp:TextBox ID="passwordTextBox" runat="server" Text='<%# Bind("password") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="phone_number" FilterControlAltText="Filter phone_number column" HeaderText="Phone" SortExpression="phone_number" UniqueName="phone_number">
                    </telerik:GridBoundColumn>
                    <telerik:GridDropDownColumn DataField="roles_id" FilterControlAltText="Filter supervisor column" HeaderText="Role" SortExpression="roles_id" UniqueName="roles_id" DataSourceID="ldsRoles" ListValueField="id" ListTextField="description">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text=""></ModelErrorMessage>
                        </ColumnValidationSettings>
                    </telerik:GridDropDownColumn>
                    <telerik:GridTemplateColumn HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Edit"><i class="color fa fa-edit fa-2"></i><span class="color">Edit User</span></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="javascript:if(!confirm('This action will delete the selected report. Are you sure?')){return false;}"
                                CommandName="Delete"><i class="color fa fa-trash-o fa-2"></i><span class="color">Delete User</span></asp:LinkButton>
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
        </telerik:RadGrid>

        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="getUsers"  
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

