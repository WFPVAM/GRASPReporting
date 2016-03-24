<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportFormPrototype.aspx.cs" Inherits="Admin_ImportFormPrototype" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/bootstrap.css" rel="stylesheet">
    <link href="../_css/sb-admin.css" rel="stylesheet">
    <link rel="stylesheet" href="../_css/font-awesome/css/font-awesome.min.css">
    <style type="text/css">
        body, html {
            margin: 0;
            padding: 0;
        }

        label {
            display: inline-block;
            margin-bottom: 5px;
            font-weight: bold;
        }
    </style>

    <script src="../_js/jquery.min.js"></script>
    <script src="../_js/bootstrap.js"></script>
    <!--<link rel="stylesheet" href="http://cdn.oesmith.co.uk/morris-0.4.3.min.css">-->
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
                <Scripts>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
                </Scripts>
            </telerik:RadScriptManager>
            <div class="row" style="margin: 0">
                <div class="col-lg-12">
                    <h1>Import Form Prototype</h1>
                    <ol class="breadcrumb">
                        <li class="current"><i class="fa fa-upload"></i>Import Form Prototype</li>
                    </ol>
                </div>
            </div>
            <div class="row" style="margin: 0; border: 1px solid #0058b1; padding: 20px 20px 20px 0; margin: 0 15px;">
                <div class="col-lg-12 col-sm-12">
                    <label>Select the file to import</label>
                    <div class="col-lg-2 form-group" style="margin-bottom: 10px; border: 1px solid #0058b1; padding: 20px;">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </div>
                    <telerik:RadButton ID="btnImport" runat="server" Skin="MetroTouch" BackColor="White" Text="Import" OnClick="btnImport_Click"></telerik:RadButton>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
