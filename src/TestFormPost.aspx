<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestFormPost.aspx.cs" Inherits="TestFormPost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtToPost" runat="server" Rows="10" TextMode="MultiLine" Width="781px"></asp:TextBox>
        <br />
        <asp:Button ID="btnSendPost" runat="server" Text="POST" OnClick="btnSendPost_Click" />
    </div>
    </form>
</body>
</html>
