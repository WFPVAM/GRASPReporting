<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculatedFields.aspx.cs" Inherits="Admin_CalculatedFields" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <script type="text/javascript">
            function ConfirmDelete(sender, args) {
                if (!confirm('You are going to delete the Calculated Fields and all its values.\r\nAll the calculated values of this field will be permanently lost.\r\nAre you sure you want to proceed?')) {
                    args.set_cancel(true);
                }
            }
        </script>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
    <div>
        <telerik:RadGrid ID="GrdCalcFields" runat="server" AutoGenerateColumns="False" OnNeedDataSource="GrdCalcFields_NeedDataSource" Skin="Metro">
            <ClientSettings>
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            </ClientSettings>
            <MasterTableView DataKeyNames="FormFieldExtID">
                <Columns>
                    <%--                    <telerik:GridTemplateColumn UniqueName="CheckBoxTemplateColumn">
        <ItemTemplate>
          <asp:CheckBox ID="CheckBox1" runat="server" OnCheckedChanged="ToggleRowSelection"
            AutoPostBack="True" />
        </ItemTemplate>
        <HeaderTemplate>
          <asp:CheckBox ID="headerChkbox" runat="server" OnCheckedChanged="ToggleSelectedState"
            AutoPostBack="True" />
        </HeaderTemplate>
      </telerik:GridTemplateColumn>--%>
                    <telerik:GridTemplateColumn DataField="FormFieldExtID" DataType="System.Int32" FilterControlAltText="Filter FormFieldExtID column" HeaderText="id" Visible="false" UniqueName="FormFieldExtID">
                        <ItemTemplate>
                            <%# Eval("FormFieldExtID") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="FormFieldExtName" DataType="System.String" FilterControlAltText="Filter FormFieldExtName column" HeaderText="Name" UniqueName="FormFieldExtName">
                        <ItemTemplate>
                            <%# Eval("FormFieldExtName") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="FormFieldExtLabel" DataType="System.String" FilterControlAltText="Filter FormFieldExtLabel column" HeaderText="Label" UniqueName="FormFieldExtLabel">
                        <ItemTemplate>
                            <%# Eval("FormFieldExtLabel") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="FormFieldExtFormula" DataType="System.String" FilterControlAltText="Filter FormFieldExtFormula column" HeaderText="Formula" UniqueName="FormFieldExtFormula">
                        <ItemTemplate>
                            <%# Eval("FormFieldExtFormula") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="cmd">
                        <ItemTemplate>
                            <telerik:RadButton ID="BtnRecalculate" runat="server" Text="Calculate" CommandArgument='<%#Eval("FormFieldExtID")%>' CommandName="Calculate" OnCommand="BtnRecalculate_Command" ></telerik:RadButton>
                            <telerik:RadButton ID="BtnDelete" runat="server" Text="Delete" CommandArgument='<%#Eval("FormFieldExtID")%>' CommandName="Delete" OnClientClicking="ConfirmDelete" OnCommand="BtnDelete_Command" ></telerik:RadButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
    <asp:Literal ID="LitMessage" runat="server"></asp:Literal>


    </form>
</body>
</html>
