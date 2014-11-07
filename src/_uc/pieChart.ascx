<%@ Control Language="C#" AutoEventWireup="true" CodeFile="pieChart.ascx.cs" Inherits="_uc_pieChart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<div class="col-lg-6">
    <div class="panel panel-primary">
        <div class="panel-heading">
            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i><%= labelName%></h3>
        </div>
        <div id="warning" class="alert alert-dismissable alert-warning" runat="server" visible="false">
            <button type="button" class="close" data-dismiss="alert">&times;</button>
            <p><i class="fa fa-warning"></i><strong>Warning! </strong>Data you choose for this report are empty.</p>
        </div>
        <div class="panel-body">
            <div class="flot-chart">
                <div class="flot-chart-content" id="<%= "flot-chart-pie" + reportFieldID %>">
                    <script>
                        var colors= ['#D7191C','#FDAE61','#2B83BA','green','silver','purple', 'olive','maroon', 'fuchsia', 'teal', 'yellow' ,'lightpink', 'aqua','gray','lime','navy','khaki','mediumorchid', 'lightslateblue', 'springgreen', 'darkturquoise', 'greenyellow','blue', 'orange', 'red'];
                            
                        function shuffle(o){
                            for(var j, x, i = o.length; i; j = Math.floor(Math.random() * i), x = o[--i], o[i] = o[j], o[j] = x);
                            return o;
                        };
                        function createChartPie<%= reportFieldID%>() {
                            $("<%= "#flot-chart-pie" + reportFieldID %>").kendoChart({
                                title: {
                                    text: <%= chartName%>,
                                    position: "top"
                                },
                                legend: {
                                    visible: <%= legend%>,
                                    position: "bottom"
                                },
                                seriesDefaults: {
                                    labels: {
                                        visible: true,
                                        background: "transparent",
                                        template: "#= value# - #= kendo.format('{0:P}', percentage)#"
                                    }
                                },
                                series: [{
                                    type: "pie",
                                    startAngle: 150,
                                    data: <%= json %>
                                    }],
                                seriesColors: shuffle(colors),
                                tooltip: {
                                    visible: true,
                                    template: "#= category #: #= value#"
                                }
                            });
                        }
                    </script>
                </div>
            </div>
            <div id="tableData" runat="server" visible="false" style="width: 50%; margin: 0 auto;">
                <telerik:RadGrid ID="tabularData" runat="server" AutoGenerateColumns="false" Height="100%" CellSpacing="0" GridLines="None" Skin="MetroTouch"
            ForeColor="#0058B1" BorderColor="White" AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridBoundColumn DataField="Key" />
                            <telerik:GridBoundColumn DataField="Value" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                
            </div>
            <asp:Literal ID="Literal2" runat="server"></asp:Literal>
        </div>
    </div>
</div>
