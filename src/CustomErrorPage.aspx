<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomErrorPage.aspx.cs" Inherits="CustomErrorPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title></title>
    <!-- Bootstrap core CSS -->
    <link href="_css/bootstrap.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="_css/signin.css" rel="stylesheet" />
    <link href="_css/sb-admin.css" rel="stylesheet" />
    <link rel="stylesheet" href="_css/font-awesome/css/font-awesome.min.css" />

    <style type="text/css">
        #login {
            margin: 0 auto;
        }
    </style>

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.3.0/respond.min.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">

        <nav class="navbar navbar-inverse navbar-fixed-top" style="padding-top: 25px;" role="navigation">
            <!-- Brand and toggle get grouped for better mobile display -->
                <table>
                <tr>
                    <td>
                        <div style="padding-left: 25px;color: white">
                            <strong>GRASP</strong>
                            <br/>
                            <span>Version: <asp:Literal ID="lblVersion" runat="server"></asp:Literal></span>
                        </div>
                    </td>
                </tr>
            </table>
                <!--<div style="width: 160px; float:left">-->
                <%--<a href="index.html">--%>
                    <%--<img style="padding-left: 25px; padding-top: 8px" src="ErrorPage.JPG" height="70px" alt="World Food Programme" /></a>--%>
                
                <!--</div>-->
                <!--<div style="color: #f00;text-align: center;margin: 0 auto;width: 160px;padding-top: 8px;">PROVA</div>-->

            <%--</div>--%>
        </nav>
        
        <div style="padding-left: 25px">
                <h2>Error!</h2>
                <p>Sorry, the Server has encountered an error and could not complete your request.</p>
                <p>You can send this error ID to your support. <strong>Error ID: '<asp:Literal id="lblErrorID" runat="server"></asp:Literal>'</strong></p>
        </div>

        <!-- Bootstrap core JavaScript -->
        <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
        <script src="http://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
        <script src="_js/bootstrap.js"></script>
    </form>
</body>
</html>
