<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewForm.aspx.cs" Inherits="Admin_ViewForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/bootstrap.css" rel="stylesheet">
    <link href="../_css/sb-admin.css" rel="stylesheet">
    <style type="text/css">
        #formContainer { width: 100%;font-size:13px;text-align:center; }
        #tblContainer { width: 90%; border: 3px solid #ccc;margin:auto; }
        #tblContainer h1 { margin: 0 0 5px 0; padding: 5px 0; width: 100%; background: #eee; border-bottom: 1px solid #ddd;}
        .rosterContainer { height: 100%; min-height: 100%; overflow: hidden; border-top: 1px solid #0058b1;border-bottom: 1px solid #0058b1;}
        .roasterTitle { font-size: 16px;font-weight:bold; }
        .left {font-weight: bold;width:50%;text-align:right;border-bottom:1px solid #ddd;margin:0;padding:2px 0 0px 0;}
        .right {width: 49%;border-bottom:1px dashed #ddd;margin:0;padding:2px 0 4px 0;text-align:left;}
        .inline {display: inline-block;padding:0 2px;width:100px;border-left:1px solid #ddd;}
        .overflowTable {overflow-y: hidden;}
    </style>
</head>
<body>
    <form id="form1" runat="server">        
        <div id="formContainer">
            <div id="tblContainer">
                <h1><asp:Literal ID="litFormTitle" runat="server"></asp:Literal></h1>
                <asp:Literal ID="litTableResult" runat="server"></asp:Literal>
            </div>
        </div>
    </form>
</body>
</html>
