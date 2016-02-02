<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewFormResponse.aspx.cs" Inherits="Admin_ViewFormResponse" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/bootstrap.css" rel="stylesheet">
    
    <link href="../_css/sb-admin.css" rel="stylesheet">
    <link rel="stylesheet" href="../_css/font-awesome/css/font-awesome.min.css">
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="tableForms" runat="server" EnableViewState="false"></asp:Literal>
    </div>
    </form>
</body>
</html>
