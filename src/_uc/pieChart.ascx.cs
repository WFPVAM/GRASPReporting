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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Data.Objects;
using System.Linq.Dynamic;

/// <summary>
/// User Control to create the structure for a pie chart with KendoUI
/// </summary>
public partial class _uc_pieChart : System.Web.UI.UserControl
{

    public string json = "";
    public string chartName = "ss";
    public string legend = "false";
    public string table = "false";
    public string firstColumn = "";
    public string secondColumn = "";

    public int reportFieldID { get; set; }
    public string labelName { get; set; }
    public int ResponseStatusID { get; set; }
    //public DateTime? ResponseValueDate { get; set; }
    public Report ObjReport { set; get; }

    /// <summary>
    /// Shows a bar charts on the selected report, taking the data from the DB
    /// and passing them to the javascript as json
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var ReportData = (from rf in db.ReportFields
                          where rf.ReportFieldID == reportFieldID
                          select rf).FirstOrDefault();

        int reportID = ReportField.getReportID(reportFieldID);
        int formFieldID = ReportField.getFormFieldID(reportFieldID);
        int formID = Report.getFormID(reportID);
        firstColumn = ReportData.ReportFieldLabel;
        secondColumn = ReportData.ReportFieldValueLabel; //it is always count, when make pie to accept aggregate func, change it.

        var deleteRespStatusID = (from rs in db.FormResponseStatus
                                  where rs.ResponseStatusName == "Deleted"
                                  select new { rs.ResponseStatusID }).FirstOrDefault();

        IEnumerable<FormFieldResponses> items = from ffr in db.FormFieldResponses.AsEnumerable()
            where ffr.formFieldId == formFieldID
                  && (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                  && ffr.ResponseStatusID != deleteRespStatusID.ResponseStatusID
            //&& (rv.RVCreateDate.Value.Date == (ResponseValueDate != null ? ResponseValueDate.Value.Date : rv.RVCreateDate.Value.Date))
            select ffr;

        FilterFormResponses(db, formID, ref items, deleteRespStatusID.ResponseStatusID);

        var items2 = from ffr in items
                     group ffr by ffr.value into g
            select new
            {
                category = g.Key,
                value = g.Count()
            };

        //ResponseValueDate != null ? ResponseValueDate : rv.RVCreateDate.Value.Date))
        Dictionary<string, string> response = new Dictionary<string, string>();
        List<Object> newItems = new List<Object>();
        foreach (var r in items2)
        {
            try
            {
                Dictionary<string, string> tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(r.category.ToString());
                response.Add(tmp.FirstOrDefault().Value, r.value.ToString());
                newItems.Add(new { category = tmp.FirstOrDefault().Value, value = r.value });
            }
            catch (Exception ex)
            {
                response.Add(r.category, r.value.ToString());
                newItems.Add(new { category = r.category, value = r.value });
            }
        }
        
        //var report = (from r in db.Reports
        //              where r.ReportID == reportID
        //              select r).FirstOrDefault();
        //chartName = (report.ReportDescription != "") ? "\"" + report.ReportDescription + "\"" : "\"" + report.ReportName + "\"";
        chartName = "\"" + labelName + "\"";

        if (ReportData.ReportFieldLegend == 1)
            legend = "true";
        if (ReportData.ReportFieldTableData == 1)
            table = "true";

        if (table == "true")
        {
            tableData.Visible = true;
            tabularData.DataSource = response;
            tabularData.Columns[0].HeaderText = firstColumn;
            tabularData.Columns[1].HeaderText = "Count";
        }
        else tableData.Visible = false;

        var serializer = new JavaScriptSerializer();
        json = serializer.Serialize(newItems);
        if (json == "[]")
            warning.Visible = true;

    }

    private void FilterFormResponses(GRASPEntities db, int formID, ref IEnumerable<FormFieldResponses> items, int deleteRespStatusID)
    {
        try
        {
            string customFilter = !string.IsNullOrEmpty(ObjReport.Filters) ? ObjReport.Filters : null;
            int filterCount = (int)ObjReport.FiltersCount;

            if (!string.IsNullOrEmpty(customFilter))
            {
                var allFormFieldResponses = from ffr in db.FormFieldResponses
                                            where
                                                ffr.parentForm_id == formID &&
                                                (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                                                && ffr.ResponseStatusID != deleteRespStatusID
                                            select new { ffr.value, formFieldID = ffr.formFieldId, ffr.nvalue, ffr.FormResponseID };

                //This...
                var filteredFormResponseIDs = (from r in allFormFieldResponses.Where(customFilter)
                                               group r by r.FormResponseID
                                                   into grp
                                                   where grp.Count() == filterCount
                                                   select grp.Key).ToList();

                items = from ffr in items
                        where filteredFormResponseIDs.Contains(ffr.FormResponseID)
                        select ffr;
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }
    }
}