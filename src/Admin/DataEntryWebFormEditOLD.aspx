<%@ Page Title="" Language="C#" MasterPageFile="~/_master/AngularJS.master" AutoEventWireup="true" CodeFile="DataEntryWebFormEditOLD.aspx.cs" Inherits="DataEntryWebFormEditOLD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../_css/number-polyfill.css" type="text/css" />
    <style type="text/css">
        ul li { list-style: none; }
    </style>
    <%--    <script src="../_js/number-polyfill.js"></script>--%>

    <script src="../_js/ui-bootstrap-custom-tpls-0.9.0.js"></script>

    <script type="text/javascript">
        function LoadData() {
            app.controller('MainCtrl', ['$scope', function ($scope) {
                $scope.currForm = {
                    "date": "2010-09-07",
                    "des_version": "tretre",
                    "client_version": "xxx",
                    "Q105bSps1ID": 800503369,
                    "enumerator": "كفاح جهاد صلاح حجي",
                    "Latitude": "0",
                    "Longitude": "0",
                    "Q001SurvCode": 100,
                    "Q002SrvName": "F001",
                    "Q002AreaCode": "G3003F001",
                    "Q004SecSit": { "value": "Safe" },
                    "Q005TypInf": { "value": "Housing" },
                    "Q006Comm": "0",
                    "Q101": "G3003F001",
                    "Q102a": "كفاح",
                    "Q102b": "جهاد",
                    "Q102c": "صلاح",
                    "Q102d": "حجي",
                    "Q103ID": 803520303,
                    "Q104Mob": 598296821,
                    "Q1005": "Yes",
                    "Q105a": "وجيه حجي",
                    "Q106": "No",
                    "Q109": "No",
                    "Q1013": "No",
                    "Q204Gov": { "value": "Gaza غزة" },
                    "Q204bG": { "value": "المغراقة G30" },
                    "Q202Address": "بجوار شارع 16",
                    "Q203": "0",
                    "Q205DamDate": "2014-08-12",
                    "Q301TypeBldg": { "value": "Finished Concrete Building" },
                    "Q302NoFloors": 2,
                    "Q303DEMFloors": "Ground + the second floor",
                    "Q304UnitArea": 200,
                    "Q305BoundWall": 0,
                    "Q306a": "0",
                    "Q306b": "0",
                    "Q306c": "0",
                    "Q307STRUCTsAFE": "Yes",
                    "Q308": "No",
                    "Q309typedamage": { "value": "Repairable" },
                    "q3011a": "No",
                    "Q3011SheltiHab": "Yes",
                    "Q3012": { "value": "Owned" },
                    "Q3013Occupied": "Yes",
                    "Q3130Dem": "Yes",
                    "Q3013a": 5,
                    "Q3013c": 1,
                    "Q3013e": 1,
                    "Q3013i": 8,
                    "Q3140": "Yes",
                    "Q3014d": 5,
                    "Q30150": "Yes",
                    "Q3015f": 1,
                    "Q3160": "Yes",
                    "Q3016a": 20,
                    "Q3016b": 20,
                    "Q3016c": 20,
                    "Q3016e": 250,
                    "Q3016m": 10,
                    "Q3016r": 6,
                    "Q3170": "Yes",
                    "Q3017a": 1.5,
                    "Q3017k": 8,
                    "Q3180": "Yes",
                    "Q3018a": 4,
                    "Q3018d": 2,
                    "Q3190": "No",
                    "Q320Combreq": "Yes",
                    "Q3020a": 30,
                    "Q3020b": 10,
                    "Q320PluWorks": "No",
                    "Q320ELE": "No",
                    "Q30230": "Yes",
                    "repairof": "0",
                    "Recof": "0",
                    "Remarks": "0",
                    "Q3023mISC": [
                    {
                        "Q3023a": "قصارة إيطالية واجهات"
                    , "Q3023b": 200
                    , "Q3023C": 21
                    }, {
                        "Q3023a": "أعمال جبس أطراف جوانب السقف"
                    , "Q3023b": 30
                    , "Q3023C": 9
                    }, {
                        "Q3023a": "جبس زخرفة وسط الغرفة"
                    , "Q3023b": 1
                    , "Q3023C": 250
                    }]
                }
            }]);
            alert("click");
        }


    </script>

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
                    <input type="button" value="loaddata" onclick="LoadData()" />
                    <pre>RESULT: <br /><asp:Label ID="Literal2" runat="server">{{currForm | json}}</asp:Label></pre>
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


    <script type="text/javascript">





    </script>

</asp:Content>

