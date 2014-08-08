/*
 *GRASP(Geo-referential Real-time Acquisition Statistics Platform) Reporting Tool <http://www.brainsen.com>
 * Developed by Brains Engineering s.r.l (marco.giorgi@brainsen.com)
 * This file is part of GRASP Reporting Tool.  
 *  GRASP Reporting Tool is free software: you can redistribute it and/or modify it
 *  under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or (at
 *  your option) any later version.  
 *  GRASP Reporting Tool is distributed in the hope that it will be useful, but
 *  WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser  General Public License for more details.  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GRASP Reporting Tool. 
 *  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Used to call an user control based on the chart type of the ReportFields
/// </summary>
public partial class Admin_Statistics_ViewChart : System.Web.UI.Page
{
    public string ReportName = "";
    /// <summary>
    /// Checks all the reportFields for a report, then it calls a specified function to show pie or bar charts
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ReportName = Request["reportName"];
        int ReportID = Convert.ToInt32(Request["reportID"]);
        IEnumerable<ReportField> repFlds = ReportField.getReportFields(ReportID);
        Literal1.Text="<script>function createCharts(){";
        foreach (ReportField rep in repFlds)
        {
            switch (rep.ChartType)
            {
                case "pie":
                    _uc_pieChart c = (_uc_pieChart)Page.LoadControl("../_uc/pieChart.ascx");
                    c.reportFieldID = Convert.ToInt32(rep.ReportFieldID);
                    c.labelName = rep.ReportFieldLabel;
                    PlaceHolder1.Controls.Add(c);
                    Literal1.Text += "createChartPie" + rep.ReportFieldID + "(); ";
                    break;
                case "bar":
                    _uc_barChart c2 = (_uc_barChart)Page.LoadControl("../_uc/barChart.ascx");
                    c2.reportFieldID = Convert.ToInt32(rep.ReportFieldID);
                    c2.labelName = rep.ReportFieldLabel;
                    PlaceHolder1.Controls.Add(c2);
                    Literal1.Text += "createChartBar" + rep.ReportFieldID + "(); ";
                    break;
                default:
                    break;
            }
        }
        Literal1.Text += "}</script>";
    }
}