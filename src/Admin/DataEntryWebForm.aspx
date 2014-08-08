<%@ Page Title="" Language="C#" MasterPageFile="~/_master/AngularJS.master" AutoEventWireup="true" CodeFile="DataEntryWebForm.aspx.cs" Inherits="DataEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../_css/number-polyfill.css" type="text/css"></link>
    <style type="text/css">
        ul li {
            list-style: none;
        }
    </style>
    <%--    <script src="../_js/number-polyfill.js"></script>--%>
    <script src="../_js/ui-bootstrap-custom-tpls-0.9.0.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <script>
        var countRB = 0;
        function sendJson() {
            var json = document.getElementById('<%=Literal2.ClientID%>');
            var res = json.textContent;
            console.log("START POST");
            $.ajax({
                type: "POST",
                url: "DataEntryWebForm.aspx/getJSON",
                data: "{'result' : '" + res + "', 'formID' : '" + <%=formID%> + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    window.location = 'DataEntryWebForm.aspx?save=true&formID=' + msg.d;
                },
                error: function (msg) {
                    window.location = 'DataEntryWebForm.aspx?save=false&formID=' + msg.d;
                }
                
            });
        }
    </script>
    <div class="row">
        <div class="col-lg-12">
            <h1>Data Entry</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home </a></li>
                <li><i class="fa fa-pencil"></i><a href="Data_Entry.aspx">Data Entry </a></li>
                <li class="current"><i class="fa fa-check-square-o"></i>New Data Entry</li>
            </ol>
        </div>
    </div>
    <div id="success" class="row" runat="server" visible="false">
        <div class="col-lg-12">
            <div class="alert alert-dismissable alert-success">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                <strong>Well Done!</strong> You successfully sent the form.
            </div>
        </div>
    </div>
    <div id="error" class="row" runat="server" visible="false">
        <div class="col-lg-12">
            <div class="alert alert-dismissable alert-danger">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                Form not sent.
            </div>
        </div>
    </div>
    <div class="row" style="font-size: 12px;">
        <div class="col-lg-6">
            <div ng-form="mainForm" novalidate>
                <div class="clear left">
                    <hr />
                    <pre >RESULT: <br /><asp:Label ID="Literal2" runat="server">{{currForm | json}}</asp:Label></pre>
                </div>
                <asp:Literal ID="angJSForm" runat="server"></asp:Literal>
                <%--            <button ng-click="reset()" ng-disabled="isUnchanged(currForm)">RESET</button>--%>
                <br />
                <div class="left clear">
                    <button type="button" class="btn btn-default" ng-click="update(currForm)" ng-disabled="mainForm.$invalid || isUnchanged(currForm)">SAVE</button>
                </div>
            </div>

        </div>
    </div>

    <asp:Literal ID="ltlScript" runat="server"></asp:Literal>
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>

</asp:Content>

