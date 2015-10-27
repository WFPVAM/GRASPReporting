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
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

/// <summary>
/// Used to call an user control based on the chart type of the ReportFields
/// </summary>
public partial class Admin_Statistics_ViewChart : System.Web.UI.Page
{
    public int FormID { get; set; }
    //public int ReportID { get; set; }
    public Report ObjReport { set; get; }

    /// <summary>
    /// Checks all the reportFields for a report, then it calls a specified function to show pie or bar charts
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["IsReportSessionSet"] = false; //Whether the Report session is set from query string coming from custom filters popup.
            IsDBUpdated();
            InitializePage();
            MaintainFiltersButtonsEnabiling();
            SetFilterSummaryLabelText();
        }
        InitializeReportObject();
    }

    private void InitializePage()
    {
        InitializeReportObject();
        reportNameHeader.Text = ObjReport.ReportName;
        //ReportID = Convert.ToInt32(Request["reportID"]);
        FormID = Report.getFormID(ObjReport.ReportID);
        int responseStatusID = 0;

        if (ddlResponseStatus.SelectedValue != null 
            && ddlResponseStatus.SelectedValue != "")
        {
            responseStatusID = Convert.ToInt32(ddlResponseStatus.SelectedValue);
        }

        IEnumerable<ReportField> repFlds = ReportField.getReportFields(ObjReport.ReportID);
        Literal1.Text = "<script>function createCharts(){";

        foreach (ReportField rep in repFlds.OrderBy(o => o.ReportFieldOrder))
        {
            switch (rep.ChartType)
            {
                case "pie":
                    _uc_pieChart c = (_uc_pieChart)Page.LoadControl("../_uc/pieChart.ascx");
                    c.reportFieldID = Convert.ToInt32(rep.ReportFieldID);
                    c.labelName = rep.ReportFieldTitle;
                    c.ResponseStatusID = responseStatusID;
                    c.ObjReport = ObjReport;
                    //if (dateFrom.SelectedDate != null) c.ResponseValueDate = dateFrom.SelectedDate.Value.Date;
                    PlaceHolder1.Controls.Add(c);
                    Literal1.Text += "createChartPie" + rep.ReportFieldID + "(); ";
                    break;
                case "bar":
                    _uc_barChart c2 = (_uc_barChart)Page.LoadControl("../_uc/barChart.ascx");
                    c2.reportFieldID = Convert.ToInt32(rep.ReportFieldID);
                    c2.ObjReport = ObjReport;
                    c2.labelName = rep.ReportFieldTitle;
                    c2.ResponseStatusID = responseStatusID;
                    //if (dateFrom.SelectedDate != null) c2.ResponseValueDate = dateFrom.SelectedDate.Value.Date;
                    PlaceHolder1.Controls.Add(c2);
                    Literal1.Text += "createChartBar" + rep.ReportFieldID + "(); ";
                    break;
                case "line":
                    UCLineChart ucLineChartUserControl = (UCLineChart)Page.LoadControl("../_uc/LineChart.ascx");
                    ucLineChartUserControl.reportFieldID = Convert.ToInt32(rep.ReportFieldID);
                    ucLineChartUserControl.ObjReport = ObjReport;
                    ucLineChartUserControl.labelName = rep.ReportFieldTitle;
                    ucLineChartUserControl.ResponseStatusID = responseStatusID;
                    //if (dateFrom.SelectedDate != null) c2.ResponseValueDate = dateFrom.SelectedDate.Value.Date;
                    PlaceHolder1.Controls.Add(ucLineChartUserControl);
                    Literal1.Text += "createLineChart" + rep.ReportFieldID + "(); ";
                    break;
                default:
                    break;
            }
        }
        Literal1.Text += "}</script>";
    }

    private void SetFilterSummaryLabelText()
    {
        lblFilterSummary.Text = !string.IsNullOrEmpty(ObjReport.FiltersSummary) ? Server.HtmlDecode(Server.UrlDecode(ObjReport.FiltersSummary)) : "There are no Filters.";
    }

    private void InitializeReportObject()
    {
        try
        {
            if (((bool) ViewState["IsReportSessionSet"]) == false) //First time, or open a new report (change name).
            {
                ObjReport = ObjReport ?? new Report();
                ObjReport.ReportID = Convert.ToInt32(Request["reportID"]);
                ObjReport.ReportName = Request["reportName"];

                if (Request["customFilter"] == null) //There is no custom filters comming from the custom filters popup, then we fetch object from DB.
                {
                    ObjReport = Report.GetReportByID(ObjReport.ReportID);
                    Session["IsFiltersChanged"] = false;
                }
                else
                {
                    ObjReport.Filters = Request["customFilter"];
                    ObjReport.FiltersCount = int.Parse(Request["filterCount"]);
                    ObjReport.FiltersSummary = Request["filterSummary"];
                }

                Session["ReportObject"] = ObjReport;
                ViewState["IsReportSessionSet"] = true;
            }
            else
                ObjReport = (Report) Session["ReportObject"];
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }
    }

    private void IsDBUpdated()
    {
        if (Session["ReportsDatabaseIsOutdated"] == null
            || ((bool)Session["ReportsDatabaseIsOutdated"]))
        {
            Report objReport = new Report();
            var filtersSummaryProperty = objReport.GetType().GetProperty("FiltersSummary");

            if (filtersSummaryProperty == null)
            {
                pnlFilter.Enabled = false;
                string noteMsg = "You can't use Report Filters. Please, contact your administrator to update the database.";
                pnlFilter.ToolTip = noteMsg;
                Session["ReportsDatabaseIsOutdated"] = true;
                ucResultMsgBar.ShowResultMsg(false, noteMsg);
            }else
                Session["ReportsDatabaseIsOutdated"] = false;
        }
    }

    private void MaintainFiltersButtonsEnabiling()
    {
        if ((ObjReport != null
             && !string.IsNullOrEmpty(ObjReport.FiltersSummary)))
        {
            btnClearFilters.Enabled = true;
        }
        else
        {
            btnClearFilters.Enabled = false;
        }

        if (Session["IsFiltersChanged"] != null
           && ((bool)Session["IsFiltersChanged"]))
        {
            btnSaveFilter.Enabled = true;
        }
    }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ObjReport.Filters = "";
        ObjReport.FiltersSummary = "";
        ObjReport.FiltersCount = null;
        Session["ReportObject"] = ObjReport;
        SetFilterSummaryLabelText();
        InitializePage();
        btnSaveFilter.Enabled = true;
        MaintainFiltersButtonsEnabiling();
    }

    protected void btnSaveFilters_Click(object sender, EventArgs e)
    {
        btnSaveFilter.Enabled = false;
        bool saveFiltersResult = Report.UpdateFilters(ObjReport);
        ucResultMsgBar.ShowResultMsg(saveFiltersResult);
        InitializePage();
    }

    protected void ddlResponseStatus_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        InitializePage();
    }
}