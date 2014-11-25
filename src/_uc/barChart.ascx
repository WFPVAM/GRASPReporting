<%@ Control Language="C#" AutoEventWireup="true" CodeFile="barChart.ascx.cs" Inherits="_uc_barChart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="col-lg-12">
    <div class="panel panel-primary">
<%--        <div class="panel-heading">
            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i><%= labelName%></h3>
        </div>--%>
        <div id="warning" class="alert alert-dismissable alert-warning" runat="server" visible="false">
            <button type="button" class="close" data-dismiss="alert">&times;</button>
            <p><i class="fa fa-warning"></i><strong>Warning! </strong>Data you choose for this report are empty.</p>
        </div>
        <div class="panel-body">
            <div class="flot-chart" style="margin-bottom:15px;">
                <div class="flot-chart-content" id='<%= "flot-chart-bar" + reportFieldID %>'>
                    <script>
                        function createChartBar<%= reportFieldID%>() {
                            $("<%= "#flot-chart-bar" + reportFieldID %>").kendoChart({
                                title: {
                                    text: <%= chartName%>,
                                    position: "top",
                                    font: "bold 16px  Arial,Helvetica,sans-serif"
                                },
                                legend: {
                                    visible: <%= legend%>
                                    },
                                seriesDefaults: {
                                    type: "bar"
                                },
                                series: [{
                                    name: "<%= aggregate%>",
                                    color: "#e52c22",
                                    data: <%= jsonData%>
                                    }],
                                valueAxis: {
                                    max: <%= maxValueAxis%>,
                                    line: {
                                        visible: false
                                    },
                                    minorGridLines: {
                                        visible: true
                                    }
                                },
                                categoryAxis: {
                                    categories: <%= jsonCategories%>,
                                    majorGridLines: {
                                        visible: false
                                    }
                                },
                                tooltip: {
                                    visible: true,
                                    template: "#= series.name #: #= value #"
                                }
                            });
                        }
                    </script>
                </div>
            </div>
            <div id="tableData" runat="server" visible="false" style="width: 50%; margin: 0 auto;">
                <telerik:RadGrid ID="tabularData" runat="server" AutoGenerateColumns="false" Height="100%" CellSpacing="0" GridLines="None" Skin="Metros"
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
