<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculatedFieldInsert.aspx.cs" Inherits="Admin_CalculatedFieldInsert" UICulture="en" Culture="en-US" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body { font-family: Arial; margin: 0; }
        #content { padding: 10px; }
        #nav {margin:4px; width: 98%; height: 30px; padding: 5px; background: #0058B1; color: #fff;line-height:30px; }
            #nav a { color: #fff; text-decoration: none; }
                #nav a:hover { text-decoration: underline; }
        #fields { font-size: 11px; color: #222; height: 250px; width: 620px; overflow: auto; border: 1px solid #ccc; padding: 4px; }
            #fields table tr td { margin-bottom: 5px; border-bottom: 1px dashed #ddd; }
                #fields table tr td:first-child { font-weight: bold; cursor: pointer; }
        .errMsg { color: #f00; background: #eee; padding: 4px; font-size: 11px; }
        label { font-size: 12px; }
        .result, .msg { font-size: 12px; line-height: 15px; margin-top: 20px; width: 620px; padding: 10px; }
        .result { border-top: 1px solid #ccc; border-bottom: 1px solid #ccc; }
        .msg { font-weight: bold; text-align: center; }
        .pnlSaveOrEdit { width: 620px; text-align: center; background: #CCE6FF; padding: 10px;border:1px solid #0058B1}
        .pnlInsert label { float: left; display: block; width: 100px; text-align: right; padding-right: 8px;height:21px;line-height:21px;border-bottom:1px dashed #ddd; }
    </style>

    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.5.1.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $('.f').click(function () {
                var htmlString = $(this).html();
                var val = $('#txtFormula').val();
                $('#txtFormula').val(val + htmlString);
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>




    <asp:HiddenField ID="hdnFormFieldExtID" runat="server" />
    <div id="nav">
        <a id="lnkCalcFieldList" runat="server" href="">calculated fields list</a>
    </div>

    <div id="content">
        <div>
            <label>Formula =</label><asp:TextBox ID="txtFormula" runat="server" Width="463px"></asp:TextBox>
            <asp:Button ID="btnTestFormula" runat="server" OnClick="btnTestFormula_Click" Text="Test the formula" />
            <br />
            <asp:RequiredFieldValidator ID="rfvFormula" runat="server" CssClass="errMsg" ErrorMessage="No formula has been inserted!" ControlToValidate="txtFormula"></asp:RequiredFieldValidator>
            <br />
            <asp:Panel ID="pnlResult" Visible="false" runat="server">
                <div class="result">
                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                </div>
            </asp:Panel>
            <div class="msg">
                <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
            </div>
            <asp:Panel ID="pnlSaveOrEdit" CssClass="pnlSaveOrEdit" Visible="false" runat="server">
                <telerik:RadButton ID="btnGoToSave" runat="server" Text="Yes, it's fine" OnClick="btnGoToSave_Click"></telerik:RadButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
            <telerik:RadButton ID="btnTestAgain" runat="server" Text="No, I want to modify it" OnClick="btnTestAgain_Click"></telerik:RadButton>
            </asp:Panel>
        </div>




        <asp:Panel ID="pnlFieldList" runat="server">
            <div id="fields">
                <asp:Literal ID="litFields" runat="server"></asp:Literal>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlInsert" CssClass="pnlInsert" runat="server">
            <label>Field Name:</label><asp:TextBox ID="txtName" runat="server" Width="250px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvFieldName" runat="server" CssClass="errMsg" ErrorMessage="Field Name is required" ControlToValidate="txtName"></asp:RequiredFieldValidator>
            <br /><br />
            <label>Field label:</label><asp:TextBox ID="txtLabel" runat="server" Width="250px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvFieldlabel" runat="server" CssClass="errMsg" ErrorMessage="Field Label is required" ControlToValidate="txtLabel"></asp:RequiredFieldValidator>
            <br /><br />
            <label></label>
            <telerik:RadButton ID="btnSaveNewField" runat="server" Text="Create New Calculated Field" OnClick="btnSaveNewField_Click"></telerik:RadButton>
            <br />
        </asp:Panel>
        <asp:Panel ID="pnlGenerateValues" CssClass="pnlInsert" runat="server" Visible="false">
            <div id="generateValues">
                <asp:Literal ID="litGenerate" runat="server"></asp:Literal>
                <br />
            </div>
            <telerik:RadButton ID="btnGenerate" runat="server" SingleClick="true" SingleClickText=" Calculating... Please Wait " Text="Calculate for all the fields..." OnClick="btnGenerate_Click"></telerik:RadButton>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
