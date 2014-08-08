<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

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

        <nav class="navbar navbar-inverse navbar-fixed-top" role="navigation">
            <!-- Brand and toggle get grouped for better mobile display -->
            <div class="navbar-header">
                <!--<div style="width: 160px; float:left">-->
                <a href="index.html">
                    <img style="padding-left: 25px; padding-top: 8px" src="_images/logo_wfp.gif" height="70px" alt="World Food Programme" /></a>
                <span style="color: white">Version: <asp:Literal ID="lblVersion" runat="server"></asp:Literal></span><!--</div>-->
                <!--<div style="color: #f00;text-align: center;margin: 0 auto;width: 160px;padding-top: 8px;">PROVA</div>-->
            </div>
        </nav>
        <asp:Login ID="login" runat="server" OnLoggedIn="login_LoggedIn">
            <LayoutTemplate>
                <div class="container">
                    <div class="form-signin">
                        <h2 class="form-signin-heading">Please sign in</h2>
                        <asp:TextBox ID="UserName" runat="server" class="form-control" placeholder="Username"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ForeColor="Red" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="login"><i class="fa fa-warning"></i> User Name is required</asp:RequiredFieldValidator>
                        <asp:TextBox ID="Password" runat="server" TextMode="Password" class="form-control" placeholder="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ForeColor="Red" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="login"><i class="fa fa-warning"></i> Password is required</asp:RequiredFieldValidator>
                        <label class="checkbox">
                            <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me next time." />
                        </label>
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="login" class="btn btn-lg btn-primary btn-block" type="submit" />
                        <div style="margin-top:20px" class="panel panel-info">
                            <div class="panel-footer announcement-bottom">
                                <asp:HyperLink ID="lnkdwnload" runat="server" NavigateUrl="~/Public/GraspMobile.apk">
                                    <div class="row">
                                        <div class="col-xs-6">
                                            Download GRASP APK
                                        </div>
                                        <div class="col-xs-6 text-right">
                                            <i class="fa fa-download"></i>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                            <div class="panel-footer announcement-bottom">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Public/BarcodeScanner.apk">
                                    <div class="row">
                                        <div class="col-xs-8">
                                            Download BARCODE SCANNER
                                        </div>
                                        <div class="col-xs-4 text-right">
                                            <i class="fa fa-download"></i>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>

        <!-- Bootstrap core JavaScript -->
        <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
        <script src="http://code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
        <script src="_js/bootstrap.js"></script>
    </form>
</body>
</html>
