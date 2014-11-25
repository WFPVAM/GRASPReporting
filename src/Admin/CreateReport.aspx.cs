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
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
/// <summary>
/// Used for the creation of a new report
/// </summary>
public partial class Admin_Statistics_CreateReport : System.Web.UI.Page
{
    int FormID = 0;
    public string pathNewReport = "";
    public string reportName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request["rid"] != null)
        {
            hdnReportID.Value = Request["rid"].ToString();
            int reportID = Convert.ToInt32(Request["rid"].ToString());

            using(GRASPEntities db = new GRASPEntities())
            {
                Report report = (from r in db.Reports
                                 where r.ReportID == reportID
                                 select r).FirstOrDefault();
                tbReportName.Text = report.ReportName;
                FormID = (int)report.FormID;
            }
        }
        if(!IsPostBack)
        {
            if(Request["rid"] != null)
            {
                //Add chart to an existing report.

                error.Visible = false;
                tbReportName.ReadOnly = true;
                tbReportDescription.ReadOnly = true;
                ddlForm.Enabled = false;
                btnForFields.Enabled = false;
                btnReset.Enabled = false;
                pnlReportFields.Visible = true;
                pnlForm.Visible = false;
            }
            else
            {
                hdnReportID.Value = "";
            }
        }
        reportName = tbReportName.Text;
    }
    /// <summary>
    /// Checks if there is a report with that name for that form.
    /// If it not exists, the panel for the compilation of the report fields is shown,
    /// otherwise the user is notified with an error message.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnForFields_Click(object sender, EventArgs e)
    {
        if(FormID == 0)
        {
            FormID = Convert.ToInt32(ddlForm.SelectedValue);
        }
        reportName = tbReportName.Text;

        using(GRASPEntities db = new GRASPEntities())
        {
            var report = (from r in db.Reports
                          where r.FormID == FormID && r.ReportName == reportName
                          select r).FirstOrDefault();
            if(report == null)
            {
                error.Visible = false;
                tbReportName.ReadOnly = true;
                tbReportDescription.ReadOnly = true;
                ddlForm.Enabled = false;
                btnForFields.Enabled = false;
                btnReset.Enabled = false;

                pnlReportFields.Visible = true;
                pnlForm.Visible = false;
                //serieFieldBind();
            }
            else
            {
                error.Visible = true;
            }
        }

    }
    /// <summary>
    /// Fills the dropdown list for selecting the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ldsForm_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var forms = from f in db.Form
                    join fr in db.FormResponse on f.id equals fr.parentForm_id
                    where f.finalised == 1
                    select new { FormID = f.id, FormName = f.name };

        e.Result = forms.Distinct();
    }
    /// <summary>
    /// Creates the report for the choosen form and save all the data on the DB
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCreateReport_Click(object sender, EventArgs e)
    {
        int FormFieldID = 0;
        int FormFieldValueID = 0;
        string ReportFieldLabel = "";
        string chartType = "";
        int legend = 0;
        int table = 0;
        string ReportFieldValue = "";
        string aggregate = "";
        try
        {
            if(FormID == 0)
            {
                FormID = Convert.ToInt32(ddlForm.SelectedValue);
            }
            chartType = rcbChartType.SelectedValue;
            if(chartType == "bar")
            {

                FormFieldID = Convert.ToInt32(ddlSerieField.SelectedValue);
                FormFieldValueID = Convert.ToInt32(ddlValueField.SelectedValue);
                ReportFieldLabel = (tbReportFieldSerie.Text != "") ? tbReportFieldSerie.Text : ddlSerieField.Text;
                ReportFieldValue = (tbReportFieldValue.Text != "") ? tbReportFieldValue.Text : ddlValueField.Text;
                aggregate = ddlAggregate.SelectedValue;
                if(chkLegend.Checked)
                    legend = 1;
                if(chkTable.Checked)
                    table = 1;
                if(hdnReportID.Value == "")
                {
                    hdnReportID.Value = Report.createNewReport(tbReportName.Text, tbReportDescription.Text, FormID).ToString();
                }
                ReportField newReport = ReportField.createNewReportField(Convert.ToInt32(hdnReportID.Value), FormFieldID, TxtChartTitle.Text, ReportFieldLabel, chartType, ReportFieldValue, FormFieldValueID, aggregate, legend, table);
            }
            else if(chartType == "pie")
            {
                if(chkLegend.Checked)
                    legend = 1;
                if(chkTable.Checked)
                    table = 1;
                ReportFieldLabel = (tbReportFieldLabel.Text != "") ? tbReportFieldLabel.Text : ddlLabelFormField.Text;
                FormFieldID = Convert.ToInt32(ddlLabelFormField.SelectedValue);
                if(hdnReportID.Value == "")
                {
                    hdnReportID.Value = Report.createNewReport(tbReportName.Text, tbReportDescription.Text, FormID).ToString();
                }
                ReportField newReport = ReportField.createNewReportField(Convert.ToInt32(hdnReportID.Value), FormFieldID, TxtChartTitle.Text, ReportFieldLabel, chartType, null, null, null, legend, table);

            }

            success.Visible = true;
            error2.Visible = false;
            //pathNewReport = newReport.ChartType + "Chart.aspx?formID=" +  Convert.ToString(FormID).TrimStart() + "&reportID=" + Convert.ToString(newReport.ReportID).TrimStart();
            pnlReportFields.Enabled = false;
        }
        catch(Exception ex)
        {
            error2.Visible = true;
        }
        if(Request["rid"] != null)
        {
            Response.Redirect("ReportDetails.aspx?id=" + Request["rid"].ToString(), true);
        }
    }
    /// <summary>
    /// Cleans up the panel with the selected form and the name of the report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        tbReportName.Text = "";
        tbReportDescription.Text = "";
        ddlForm.Text = "";
        ddlForm.ClearSelection();
        ddlLabelFormField.Items.Clear();
        ddlSerieField.Items.Clear();
        ddlAggregate.ClearSelection();
        ddlValueField.Items.Clear();
        tbReportFieldLabel.Text = "";
        tbReportFieldSerie.Text = "";
        tbReportFieldValue.Text = "";
        rcbChartType.Text = "";
        rcbChartType.ClearSelection();
        chkTable.Checked = false;
        chkLegend.Checked = true;
        pnlReportFields.Visible = false;
    }
    /// <summary>
    /// Cleans up the panel with the selected data for the report (chart type, axis values)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnResetReport_Click(object sender, EventArgs e)
    {
        ddlLabelFormField.Items.Clear();
        ddlSerieField.Items.Clear();
        ddlAggregate.ClearSelection();
        ddlValueField.Items.Clear();
        tbReportFieldLabel.Text = "";
        tbReportFieldSerie.Text = "";
        tbReportFieldValue.Text = "";
        rcbChartType.Text = "";
        rcbChartType.ClearSelection();
        chkTable.Checked = false;
        chkLegend.Checked = true;
        pnlBar.Visible = false;
        pnlPie.Visible = false;
        pnlButton.Visible = false;
        success.Visible = false;
    }
    /// <summary>
    /// Cleans up the panel with the selected data for the report  in order to enter new data for that report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnNewLabel_Click(object sender, EventArgs e)
    {
        ddlLabelFormField.Items.Clear();
        ddlSerieField.Items.Clear();
        ddlAggregate.ClearSelection();
        ddlValueField.Items.Clear();
        tbReportFieldLabel.Text = "";
        tbReportFieldSerie.Text = "";
        tbReportFieldValue.Text = "";
        rcbChartType.Text = "";
        rcbChartType.ClearSelection();
        chkTable.Checked = false;
        chkLegend.Checked = true;
        success.Visible = false;
        pnlReportFields.Enabled = true;
        pnlBar.Visible = false;
        pnlPie.Visible = false;
        pnlButton.Visible = false;
    }
    /// <summary>
    /// Shows the panel for the selected chart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rcbChartType_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        string chartType = rcbChartType.SelectedValue;
        if(chartType == "pie")
        {
            ddlSerieField.Text = "";
            ddlSerieField.ClearSelection();
            ddlSerieField.Items.Clear();
            ddlAggregate.ClearSelection();
            ddlValueField.Text = "";
            ddlValueField.ClearSelection();
            ddlValueField.Items.Clear();
            tbReportFieldLabel.Text = "";
            tbReportFieldSerie.Text = "";
            tbReportFieldValue.Text = "";
            chkTable.Checked = false;
            chkLegend.Checked = true;
            pnlPie.Visible = true;
            pnlButton.Visible = true;
            pnlBar.Visible = false;
            error2.Visible = false;
            if(ddlLabelFormField.SelectedValue == "")
                serieFieldBind(ddlLabelFormField);

        }
        else if(chartType == "bar")
        {
            error2.Visible = false;
            ddlLabelFormField.Text = "";
            ddlLabelFormField.ClearSelection();
            ddlLabelFormField.Items.Clear();
            tbReportFieldLabel.Text = "";
            tbReportFieldSerie.Text = "";
            tbReportFieldValue.Text = "";
            chkTable.Checked = false;
            chkLegend.Checked = true;
            pnlBar.Visible = true;
            pnlButton.Visible = true;
            pnlPie.Visible = false;
            if(ddlSerieField.SelectedValue == "")
                serieFieldBind(ddlSerieField);
            if(ddlValueField.SelectedValue == "")
                serieValueBind();
        }

    }
    /// <summary>
    /// Fills the dropdwon list for selecting the Series Value in bar chart.
    /// Only numeric fields are shown.
    /// </summary>
    private void serieValueBind()
    {

        if(FormID == 0)
        {
            FormID = Convert.ToInt32(ddlForm.SelectedValue);
        }

        GRASPEntities db = new GRASPEntities();

        IEnumerable<FormFieldExport> fields = from ff in db.FormFieldExport
                                              where ff.form_id == FormID && ff.FormFieldParentID == null
                                              orderby ff.positionIndex ascending
                                              select ff;
        var ffs = ((from f in db.FormFieldExport
                   where f.form_id == FormID && f.type != "TRUNCATED_TEXT" && f.type != "WRAPPED_TEXT" && f.type != "IMAGE" && f.FormFieldParentID == null
                   select new { name = f.name, label = f.label, type = f.type, id = f.id, positionIndex = f.positionIndex }).Union(
                    from fe in db.FormFieldExt
                    where fe.FormID == FormID
                    select new { name = fe.FormFieldExtName, label = fe.FormFieldExtLabel, type = "SERVERSIDE_CALCULATED", 
                        id = fe.FormFieldID.Value, positionIndex=fe.PositionIndex.Value })).OrderBy(o=>o.positionIndex);


        List<FormFieldExport> rosterFields = (from i in db.FormFieldExport
                                                    where i.FormFieldParentID != null
                                                    orderby i.positionIndex ascending
                                                    select i).ToList();
        int tmp = fields.Count();

        foreach(var ff in ffs)
        {
            if(ff.type == "SEPARATOR")
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = "-- " + ff.name;
                item.Value = ff.id.ToString();
                item.Enabled = false;
                ddlValueField.Items.Add(item);
            }
            else if(ff.type == "REPEATABLES" || ff.type == "REPEATABLES_BASIC")
            {
                foreach(FormFieldExport f in rosterFields.Where(x => x.FormFieldParentID == ff.id))
                {
                    if(f.type == "NUMERIC_TEXT_FIELD" || f.type == "CURRENCY_FIELD")
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        item.Text = f.name + " (" + f.label + ")";
                        item.Value = f.id.ToString();
                        ddlValueField.Items.Add(item);
                    }
                }
            }
            else if(ff.type == "NUMERIC_TEXT_FIELD" || ff.type == "CURRENCY_FIELD" || ff.type == "SERVERSIDE_CALCULATED")
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = ff.name + " (" + ff.label + " " + ff.type + ")";
                item.Value = ff.id.ToString();
                ddlValueField.Items.Add(item);
            }
        }
    }
    /// <summary>
    /// Fills the dropdwon list for selecting the Series Name in a chart (pie or bar).
    /// Only dropdown and radio button fields are shown.
    /// </summary>
    /// <param name="ddl">The dropdown list where the elements are added</param>
    private void serieFieldBind(RadComboBox ddl)
    {
        if(FormID==0)
        {
            FormID = Convert.ToInt32(ddlForm.SelectedValue);
        }

        GRASPEntities db = new GRASPEntities();

        IEnumerable<FormFieldExport> fields = from ff in db.FormFieldExport
                                              where ff.form_id == FormID && ff.FormFieldParentID == null
                                              orderby ff.positionIndex ascending
                                              select ff;

        IEnumerable<FormFieldExport> rosterFields = from i in db.FormFieldExport
                                                    where i.FormFieldParentID != null
                                                    orderby i.positionIndex ascending
                                                    select i;

        foreach(FormFieldExport ff in fields)
        {
            if(ff.type == "SEPARATOR")
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = "-- " + ff.name;
                item.Value = ff.id.ToString();
                item.Enabled = false;
                ddl.Items.Add(item);
            }
            else if(ff.type == "REPEATABLES" || ff.type == "REPEATABLES_BASIC")
            {
                foreach(FormFieldExport f in rosterFields.Where(x => x.FormFieldParentID == ff.id))
                {
                    if(f.type == "DROP_DOWN_LIST" || f.type == "RADIO_BUTTON")
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        item.Text = f.name + " (" + f.label + ")";
                        item.Value = f.id.ToString();
                        ddl.Items.Add(item);
                    }
                }
            }
            else if(ff.type == "DROP_DOWN_LIST" || ff.type == "RADIO_BUTTON")
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = ff.name + " (" + ff.label + ")";
                item.Value = ff.id.ToString();
                ddl.Items.Add(item);
            }
        }
    }
}