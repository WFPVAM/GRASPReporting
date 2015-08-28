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
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Dynamic;
/// <summary>
/// User Control to create the structure for a bar chart with KendoUI
/// </summary>
public partial class _uc_barChart : System.Web.UI.UserControl
{
#region "Puclic Variables"
    public string chartName = "";
    public string jsonData = "";
    public string jsonCategories = "";
    public string maxValueAxis = "";
    public string legend = "false";
    public string table = "false";
    public string aggregate = "";
    public string firstColumn = "";
    public string secondColumn = "";
    public Report ObjReport { set; get; }
    //public string ReportName { get; set; }

    public int reportFieldID { get; set; }
    public string labelName { get; set; }
    public int ResponseStatusID { get; set; }
    //public DateTime? ResponseValueDate { get; set; }
    //public string CustomFilters { get; set; }
#endregion

    /// <summary>
    /// Shows a bar charts on the selected report, taking the data from the DB
    /// and passing them to the javascript as json
    /// </summary>
    /// <param name="sender"></param>forn
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        List<string> categories = new List<string>();
        List<double> data = new List<double>();
        Dictionary<string, double> axisValues = new Dictionary<string, double>();
        Dictionary<string, int> countAverage = new Dictionary<string, int>();
        Dictionary<string, List<double>> stDev = new Dictionary<string, List<double>>();

        GRASPEntities db = new GRASPEntities();


        var ReportData = (from rf in db.ReportFields
                          where rf.ReportFieldID == reportFieldID
                          select rf).FirstOrDefault();

        int reportID = ReportData.ReportID;
        int serieID = Convert.ToInt32(ReportData.FormFieldID);
        int valueID = Convert.ToInt32(ReportData.ValueFormFieldID);
        int formID = Report.getFormID(reportID);
        aggregate = ReportData.ReportFieldAggregate.ToLower();
        firstColumn = ReportData.ReportFieldLabel;
        secondColumn = ReportData.ReportFieldValueLabel;
        //chartName = "\"" + ReportData.ReportFieldLabel + " / " + ReportData.ReportFieldValueLabel + "\"";

        chartName = "\"" + labelName + "\"";

        if(ReportData.ReportFieldLegend == 1)
            legend = "true";
        if(ReportData.ReportFieldTableData == 1)
            table = "true";

        var deleteRespStatusID = (from rs in db.FormResponseStatus
                                      where rs.ResponseStatusName == "Deleted"
                                      select new { rs.ResponseStatusID }).FirstOrDefault();

        if(aggregate == "count" || aggregate == "")
        {
            IEnumerable<FormFieldResponses> items = from ffr in db.FormFieldResponses.AsEnumerable()
                where ffr.formFieldId == serieID && 
                (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                      && ffr.ResponseStatusID != deleteRespStatusID.ResponseStatusID
                //&& (ffr.RVCreateDate.Value.Date == (ResponseValueDate != null ? ResponseValueDate.Value.Date : ffr.RVCreateDate.Value.Date))
                select ffr;

            FilterFormResponses(db, formID, ref items, deleteRespStatusID.ResponseStatusID);

            var items2 = from ffr in items
                        group ffr by ffr.value into g
                        select new
                        {
                            category = g.Key,
                            value = g.Count()
                        };

            foreach(var item in items2)
            {
                data.Add(item.value);
                categories.Add(item.category);
                axisValues.Add(item.category, item.value);
            }
        }
        else
        {
            var resVal = from ffr in db.FormFieldResponses.AsEnumerable()
                where ffr.parentForm_id == formID && (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                      && ffr.ResponseStatusID != deleteRespStatusID.ResponseStatusID
                      && (ffr.formFieldId == serieID || ffr.formFieldId == valueID)
                //s3 why is OR between serieID and valueID
                //&& (ffr.RVCreateDate.Value.Date == (ResponseValueDate != null ? ResponseValueDate.Value.Date : ffr.RVCreateDate.Value.Date))
                select ffr; //new { ffr.value, formFieldID = ffr.formFieldId, ffr.nvalue, ffr.FormResponseID };

            FilterFormResponses(db, formID, ref resVal, deleteRespStatusID.ResponseStatusID);
            
            string tmpValueKey = "";
            foreach(var r in resVal)
            {
                if(r.formFieldId == serieID)
                {
                    // Series Field
                    tmpValueKey = r.value;
                }
                else
                {
                    // Value Field
                    try
                    {
                        double val = axisValues[tmpValueKey];
                        switch(aggregate)
                        {
                            case "average":
                                countAverage[tmpValueKey] = countAverage[tmpValueKey] + 1;
                                axisValues[tmpValueKey] += Convert.ToDouble(r.value);
                                break;
                            case "sum":
                                if(r.nvalue != null)
                                {
                                    axisValues[tmpValueKey] += r.nvalue.Value;
                                }
                                break;
                            case "stdev":
                                List<double> list;
                                if(!stDev.TryGetValue(tmpValueKey, out list))
                                    stDev.Add(tmpValueKey, list = new List<double>());
                                list.Add(Convert.ToDouble(r.value));
                                break;
                            case "min":
                                if(Convert.ToDouble(r.value) < val)
                                    axisValues[tmpValueKey] = Convert.ToDouble(r.value);
                                break;
                            case "max":
                                if(Convert.ToDouble(r.value) > val)
                                    axisValues[tmpValueKey] = Convert.ToDouble(r.value);
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        if(aggregate != "max" && aggregate != "min")
                        {
                            countAverage.Add(tmpValueKey, 1);
                        }
                        axisValues.Add(tmpValueKey, Convert.ToDouble(r.value));

                        if(aggregate == "stdev")
                        {
                            List<double> list;
                            if(!stDev.TryGetValue(tmpValueKey, out list))
                                stDev.Add(tmpValueKey, list = new List<double>());
                            list.Add(Convert.ToDouble(r.value));
                        }
                    }
                }
            }


            foreach(var item in axisValues.ToList())
            {
                if(aggregate == "average")
                {
                    axisValues[item.Key] /= countAverage[item.Key];
                }
                else if(aggregate == "stdev")
                {
                    double average = stDev[item.Key].Average();
                    double sumOfSquaresOfDifferences = stDev[item.Key].Select(val => (val - average) * (val - average)).Sum();
                    double sd = Math.Sqrt(sumOfSquaresOfDifferences / stDev[item.Key].Count());
                    axisValues[item.Key] = sd;
                }
            }


            foreach(var item in axisValues)
            {
                data.Add(item.Value);
                categories.Add(item.Key.Length > 20 ? item.Key.Substring(0, 20) : item.Key);
            }
        }

        if(table == "true")
        {
            tableData.Visible = true;
            tabularData.DataSource = axisValues;
            tabularData.Columns[0].HeaderText = firstColumn;
            tabularData.Columns[1].HeaderText = secondColumn;
        }
        else tableData.Visible = false;

        if(data.Count() > 0)
            maxValueAxis = data.Max().ToString().Replace(",", ".");
        else maxValueAxis = "0";

        var serializer = new JavaScriptSerializer();
        jsonData = serializer.Serialize(data);
        jsonCategories = serializer.Serialize(categories);
        if(jsonCategories == "[]")
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