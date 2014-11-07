<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ExportData.aspx.cs" Inherits="Admin_Surveys_ExportSettings" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../_css/bootstrap.css" rel="stylesheet">
    <link href="../_css/sb-admin.css" rel="stylesheet">
    <link rel="stylesheet" href="../_css/font-awesome/css/font-awesome2.min.css">
    <style type="text/css">
        body, html { margin: 0; padding: 0; }
        label { display: inline-block; margin-bottom: 5px; font-weight: bold; }
        #error { display: none; }
        .filterSummary { border: 1px solid #666; background: #eee; padding: 4px; width: 500px; font-size: 11px; }
        .errMsg { color: #f00; background: #eee; padding: 4px; font-size: 11px; }
    </style>

    <script src="../_js/jquery.min.js"></script>

    <script src="../_js/bootstrap.js"></script>

    <!--<link rel="stylesheet" href="http://cdn.oesmith.co.uk/morris-0.4.3.min.css">-->

</head>
<body>
    <form id="form1" runat="server">

    <script type="text/javascript">
        function Export() {
            var type = $('input[name="type"]:checked').val();
            var separator = $find('<%=ddlCharacter.ClientID %>').get_selectedItem();
            var filter = "";
            var fc = "";
            var hdnFilterByName = "";

            fc = $('#hdnFilterCount').val();
            filter = $('#hdnFilter').val();
            //filter = $('#hdnFilterByName').val;

            var startFromResponseID = $('input[name="startFromResponseID"]').val();

            var table = $('input[name="linear"]:checked').val();

            if (separator != null) {
                separator = separator.get_value();
                if (type == "CSV") {
                    var urlToGo = "<%="CSVExport.aspx?FormID=" + formID + "&FormName=" + name%>" + "&separator=" + separator + "&linear=" + table + "&startFRID=" + startFromResponseID;
                    if (filter.length != 0) {
                        urlToGo += "&f=" + filter + "&fc=" + fc;
                    }
                    //alert(urlToGo);
                    window.location = urlToGo;
                    //alert("<%="CSVExport.aspx?FormID=" + formID + "&FormName=" + name%>" + "&separator=" + separator + "&linear=" + table);
                    //setTimeout(window.close(), 3000);
                }
                else {
                    window.location = "<%="SPSSExport.aspx?FormID=" + formID + "&FormName=" + name%>" + "&separator=" + separator + "&linear=" + table;
                    setTimeout(window.close(), 3000);
                }
            }
            else {
                $("#error").fadeIn("slow");
                separator = ",";
            }
        }

        function RemoveErrorPanel() {
            var separator = $find('<%=ddlCharacter.ClientID %>').get_selectedItem();
            if (separator != null) {
                $("#error").fadeOut("slow");
            }
        }
    </script>

    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js"></asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js"></asp:ScriptReference>
        </Scripts>
    </telerik:RadScriptManager>
    <div class="row" style="margin: 0">
        <div class="col-lg-12">
            <h1>Export Data</h1>
            <ol class="breadcrumb">
                <li class="current"><i class="fa fa-check-square-o"></i>Export</li>
            </ol>
        </div>
    </div>
    <div id="error" class="row" style="margin: 0">
        <div class="col-lg-12">
            <div class="alert alert-dismissable alert-danger">
                <button type="button" class="close" data-dismiss="alert">&times;</button>
                You need to select a separator to export data.
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnFilterCount" runat="server" />
    <asp:HiddenField ID="hdnFilter" runat="server" />
    <asp:HiddenField ID="hdnFilterByName" runat="server" />


    <asp:Literal ID="litDownloadExport" runat="server"></asp:Literal>

    <asp:Panel ID="pnlExportSettings" runat="server" Visible="true">
        <div class="row" style="margin: 0; border: 1px solid #0058b1; padding: 20px 20px 20px 0; margin: 0 15px;">
            <div class="col-lg-6 col-sm-6">
                <asp:Panel ID="filterSummary" runat="server" Visible="false">
                    <label>Filter</label>
                    <div class="col-lg-3 form-group">
                        <div class="filterSummary">
                            <asp:Label ID="lblFilterSummary" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </asp:Panel>
                <label>Starting Response ID</label>
                <div class="col-lg-3 form-group">

                    <label class="input-sm">
                        <asp:TextBox ID="txtStartFromResponseID" runat="server" Text="0"></asp:TextBox>
                    </label>
                </div>
                <label>Separator</label>
                <div class="col-lg-3 form-group">
                    <telerik:RadComboBox ID="ddlCharacter" OnClientSelectedIndexChanged="RemoveErrorPanel" runat="server" EmptyMessage="Select a separator" EnableLoadOnDemand="True" Skin="MetroTouch" BorderColor="#66AFE9" BackColor="White">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="," Value="," />
                            <telerik:RadComboBoxItem runat="server" Text=";" Value=";" />
                            <telerik:RadComboBoxItem runat="server" Text="|" Value="|" />
                        </Items>

                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvddlCharacter" runat="server" CssClass="errMsg" ErrorMessage="Select a Separator" ControlToValidate="ddlCharacter"></asp:RequiredFieldValidator>
                </div>
                <label>Review Status</label>
                <div class="col-lg-3 form-group">
                    <telerik:RadComboBox ID="ddlReviewStatus" runat="server" EnableLoadOnDemand="True" Skin="MetroTouch" AppendDataBoundItems="true" BorderColor="#66AFE9" BackColor="White" DataSourceID="edsResponseStatus" DataTextField="ResponseStatusName" DataValueField="ResponseStatusID">
                    <Items>
                        <telerik:RadComboBoxItem Text="Any" Value="0" />
                    </Items>
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvddlReviewStatus" runat="server" CssClass="errMsg" ErrorMessage="Select a Review Status" ControlToValidate="ddlReviewStatus"></asp:RequiredFieldValidator>
                </div>
                <label>Type</label>
                <div class="col-lg-3 form-group">

                    <label class="radio">
                        <input type="radio" name="type" id="CSV" value="CSV" checked />CSV
                    </label>
                    <label class="radio">
                        <input type="radio" name="type" id="SPSS" value="SPSS" disabled />SPSS
                    </label>
                </div>
                <div class="col-lg-3 form-group">
                    <input type="checkbox" name="linear" value="Tables on a single record" />
                    <i class="fa fa-info-circle" id="linear"></i>
                    <telerik:RadToolTip ID="RadToolTip4" runat="server" RelativeTo="Element" Width="200px" AutoCloseDelay="10000" Skin="MetroTouch"
                        Text="Each table and roster is exported as a separate file containing an ID that can be used to join it to the main form in a relational database design. Select this option if you prefer table records to be exported inside the main data of the form in a single record." TargetControlID="linear" IsClientID="true"
                        Position="BottomRight">
                    </telerik:RadToolTip>
                </div>
                <telerik:RadButton ID="btnExport" runat="server" SingleClick="true" SingleClickText="Exporting... Please Wait" Text="Export" Skin="MetroTouch" OnClick="btnExport_Click"></telerik:RadButton>

            </div>
            <div class="col-lg-6 col-sm-6">
                <div class="alert alert-dismissable alert-info">
                    Most statistical and database application will import a CSV (Comma Separated Value) file. Select a <b>comma</b> (,)
                        for English computers or <b>semicolon</b> (;) for French, Spanish and Portuguese. If you are having problems importing the data
                        with these settings into another application, select <b>pipe</b> ( | ) and import the files as "delimited", making sure to specify <b>pipe</b>
                    as the delimiter in the import function of the application.
                </div>
            </div>

        </div>

    </asp:Panel>

    <asp:EntityDataSource ID="edsResponseStatus" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableFlattening="False" EntitySetName="FormResponseStatus" Select="it.[ResponseStatusName], it.[ResponseStatusID]">
    </asp:EntityDataSource>

    </form>
</body>
</html>
