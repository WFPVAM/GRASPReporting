<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewTable.aspx.cs" Inherits="Admin_ViewTable" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <link href="../_css/bootstrap.css" rel="stylesheet">

    <link href="../_css/sb-admin.css" rel="stylesheet">
    <link rel="stylesheet" href="../_css/font-awesome/css/font-awesome.min.css">
    <style type="text/css">
        body, html {
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ImageButton runat="server" ImageUrl="../_images/back.png" ID="btnBack" Text="Back" OnClick="btnBack_Click" />
            <asp:Literal ID="tableForms" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
