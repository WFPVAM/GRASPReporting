<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LineChart.ascx.cs" Inherits="UCLineChart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="col-lg-12">
    <div class="panel panel-primary">
        <span class="fake-link" onclick="<%= "exportChartImage('flot-chart-bar" + reportFieldID + "','" + ObjReport.ReportName + "','" + labelName + "')" %>"
            style="margin-left: 10px;">Export As Image</span>
        <div id="warning" class="alert alert-dismissable alert-warning" runat="server" visible="false">
            <button type="button" class="close" data-dismiss="alert">&times;</button>
            <p><i class="fa fa-warning"></i><strong>Warning! </strong>Data you have chosen for this report are empty.</p>
        </div>
        <div class="panel-body">
            <div class="flot-chart" style="margin-bottom: 15px">
                <div class="flot-chart-content" id='<%= "flot-chart-bar" + reportFieldID %>'>
                    <script>
                        
                        function createLineChart<%= reportFieldID %>() {
                            $("<%= "#flot-chart-bar" + reportFieldID %>").kendoChart({
                                title: {
                                    text: <%= ChartName %>,
                                    position: "top",
                                    font: "bold 16px  Arial,Helvetica,sans-serif"
                                },
                                legend: {
                                    visible: <%= legend %>,
                                },
                                seriesDefaults: {
                                    type: "line",
                                    labels: {
                                        visible: false,
                                        background: "transparent",
                                        template: "#= series.name #: #= value #"
                                    }
                                },
                                series: [{
                                    name: "<%= AggregateFunction %>",
                                    color: "#e52c22",
                                    data: <%= JsonSeriesData %>,
                                }],
                                valueAxis: {
                                    max: <%= MaxValueAxis%>,
                                    line: {
                                        visible: true
                                    },
                                    minorGridLines: {
                                        visible: true
                                    }
                                },
                                categoryAxis: {
                                    categories: <%= JsonCategories %>,
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
                <telerik:radgrid id="tabularData" runat="server" autogeneratecolumns="false" height="100%" cellspacing="0" gridlines="None" skin="Metro"
                    forecolor="#0058B1" bordercolor="White" alternatingitemstyle-backcolor="#CCE6FF" headerstyle-backcolor="#0058B1" headerstyle-forecolor="White">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridBoundColumn DataField="Key" />
                            <telerik:GridBoundColumn DataField="Value" />
                        </Columns>
                    </MasterTableView>
                </telerik:radgrid>
            </div>
        </div>
    </div>
</div>
