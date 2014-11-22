<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculatedFields.aspx.cs" Inherits="Admin_CalculatedFields" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calculated Fields </title>

    <script type="text/javascript">
        function ConfirmDelete(sender, args) {
            if (!confirm('You are going to delete the Calculated Fields and all its values.\r\nAll the calculated values of this field will be permanently lost.\r\nAre you sure you want to proceed?')) {
                args.set_cancel(true);
            }
        }
    </script>

    <style type="text/css">
        body { font-family: Arial; font-size: 12px; }
        div.formula { font-size: 10px; }
        #nav { margin: 4px; width: 98%; height: 30px; padding: 5px; background: #0058B1; color: #fff; line-height: 30px; }
            #nav a { color: #fff; text-decoration: none; }
                #nav a:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
    <div id="nav">
        <a id="lnkCalcFieldList" runat="server" href="">Insert New Calculated Field</a>
    </div>

    <div>
        <telerik:RadGrid ID="GrdCalcFields" runat="server" AutoGenerateColumns="False" OnNeedDataSource="GrdCalcFields_NeedDataSource" Skin="Metro">
            <ClientSettings>
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            </ClientSettings>
            <MasterTableView DataKeyNames="FormFieldExtID">
                <Columns>
                    <telerik:GridTemplateColumn DataField="FormFieldExtID" DataType="System.Int32" FilterControlAltText="Filter FormFieldExtID column" HeaderText="id" Visible="false" UniqueName="FormFieldExtID">
                        <ItemTemplate>
                            <%# Eval("FormFieldExtID") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="FormFieldExtName" HeaderStyle-Width="120" DataType="System.String" FilterControlAltText="Filter FormFieldExtName column" HeaderText="Name" UniqueName="FormFieldExtName">
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
                            <div class="formula"><%# Eval("FormFieldExtFormula") %></div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="cmd">
                        <ItemTemplate>
                            <telerik:RadButton ID="BtnRecalculate" runat="server" Text="Calculate" CommandArgument='<%#Eval("FormFieldExtID")%>' CommandName="Calculate" SingleClick="true" SingleClickText="Calculating..." OnCommand="BtnRecalculate_Command"></telerik:RadButton>
                            <telerik:RadButton ID="BtnDelete" runat="server" Text="Delete" CommandArgument='<%#Eval("FormFieldExtID")%>' CommandName="Delete"  SingleClick="true" OnClientClicking="ConfirmDelete" OnCommand="BtnDelete_Command"></telerik:RadButton>
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
