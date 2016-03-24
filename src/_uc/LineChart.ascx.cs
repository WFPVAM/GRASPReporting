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
public partial class UCLineChart : System.Web.UI.UserControl
{
#region "Puclic Variables"
    public string ChartName { get; private set; }
    public string JsonSeriesData { get; private set; }
    public string JsonCategories { get; private set; }
    public string MaxValueAxis { get; private set; }
    public string legend = "false";
    public string table = "false";
    public string AggregateFunction { get; private set; }
    public string firstColumn { get; private set; }
    public string secondColumn { get; private set; }
    public Report ObjReport { set; get; }
    public int reportFieldID { get; set; }
    public string labelName { get; set; }
    public int ResponseStatusID { get; set; }
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
        List<double> seriesData = new List<double>();
        Dictionary<string, double?> axisValues = new Dictionary<string, double?>();
        
        //Saves the response ids with each series item. Uses to match the series item with its corresponding value item (category) where 
        //the response id is the common field between the two records in [FormFieldResponses] table.
        Dictionary<int?, string> seriesResponseIds = new Dictionary<int?, string>();
        Dictionary<string, int> countAverage = new Dictionary<string, int>();
        Dictionary<string, List<double>> stDev = new Dictionary<string, List<double>>();
        InitializeChartProperties();

        using (GRASPEntities db = new GRASPEntities())
        {
            ReportField reportField = ReportField.GetReportFieldById(reportFieldID);
            int reportID = reportField.ReportID;
            int serieID = Convert.ToInt32(reportField.FormFieldID);
            int valueID = Convert.ToInt32(reportField.ValueFormFieldID);
            int formID = Report.getFormID(reportID);
            AggregateFunction = reportField.ReportFieldAggregate.ToLower();
            firstColumn = reportField.ReportFieldLabel;
            secondColumn = reportField.ReportFieldValueLabel;
            ChartName = "\"" + labelName + "\"";

            if (reportField.ReportFieldLegend == 1)
                legend = "true";
            if (reportField.ReportFieldTableData == 1)
                table = "true"; 

            //gets the deleted status id.
            var deleteRespStatusID = (from rs in db.FormResponseStatus
                                      where rs.ResponseStatusName == "Deleted"
                                      select new { rs.ResponseStatusID }).FirstOrDefault();

            if (AggregateFunction.Equals(GeneralEnums.AggregateFuns.count.ToString()))
            {
                IEnumerable<FormFieldResponses> ffResponseses = (from ffr in db.FormFieldResponses
                    where ffr.formFieldId == serieID
                          && (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                          && (ffr.ResponseStatusID != deleteRespStatusID.ResponseStatusID)
                    select ffr).AsEnumerable();

                FilterFormResponses(db, formID, ref ffResponseses, deleteRespStatusID.ResponseStatusID);

                var groupedResponseses = from ffr in ffResponseses
                    group ffr by ffr.value
                    into g
                    select new
                    {
                        category = g.Key,
                        value = g.Count()
                    };

                foreach (var item in groupedResponseses)
                {
                    seriesData.Add(item.value);
                    categories.Add(item.category);
                    axisValues.Add(item.category, item.value);
                }
            }
            else
            {
                //fields responses for both series and values fields.
                var ffResponses = (from ffr in db.FormFieldResponses
                    where ffr.parentForm_id == formID
                          && (ResponseStatusID == 0 || ffr.ResponseStatusID == ResponseStatusID)
                          && ffr.ResponseStatusID != deleteRespStatusID.ResponseStatusID
                          && (ffr.formFieldId == serieID || ffr.formFieldId == valueID)
                    select ffr).AsEnumerable();

                FilterFormResponses(db, formID, ref ffResponses, deleteRespStatusID.ResponseStatusID);

                //Saves all series items, and their corresponding form response ids.
                string tmpValueKey = "";
                foreach (var r in ffResponses)
                {
                    // Series Field
                    if (r.formFieldId == serieID)
                    {
                        tmpValueKey = r.value;
                        if (!axisValues.ContainsKey(tmpValueKey))
                        {
                            axisValues.Add(tmpValueKey, null);
                        }

                        //Saves the response ids with each series item. Uses to match the series item with its corresponding value item (category) where 
                        //the response id is the common field between the two records in [FormFieldResponses] table.
                        if (!seriesResponseIds.ContainsKey(r.FormResponseID))
                        {
                            seriesResponseIds.Add(r.FormResponseID, tmpValueKey);
                        }
                    }
                }

                foreach (var r in ffResponses)
                {
                    // Value Field
                    if (r.formFieldId == valueID)
                    {
                        string seriesItem = null;
                        double? seriesValue = null;
                        if (seriesResponseIds.ContainsKey(r.FormResponseID))
                        {
                            //get the corresponding series item of the value item.
                            seriesItem = seriesResponseIds[r.FormResponseID];
                            if (axisValues.ContainsKey(seriesItem))
                            {
                                seriesValue = axisValues[seriesItem];

                                switch (AggregateFunction)
                                {
                                    case "average":
                                        if (!countAverage.ContainsKey(seriesItem))
                                        {
                                            countAverage.Add(seriesItem, 1);
                                        }else
                                            countAverage[seriesItem] += 1;
                                        
                                        if (axisValues[seriesItem] == null)
                                            axisValues[seriesItem] = Convert.ToDouble(r.value);
                                        else
                                            axisValues[seriesItem] += Convert.ToDouble(r.value);
                                        break;
                                    case "sum":
                                        if (r.nvalue != null)
                                        {
                                            if (axisValues[seriesItem] == null)
                                            {
                                                axisValues[seriesItem] = r.nvalue.Value;
                                            }
                                            else
                                                axisValues[seriesItem] += r.nvalue.Value;
                                        }
                                        break;
                                    case "stdev":
                                        List<double> list;
                                        if (!stDev.TryGetValue(seriesItem, out list))
                                            stDev.Add(seriesItem, list = new List<double>());
                                        list.Add(Convert.ToDouble(r.value));
                                        break;
                                    case "min":
                                        if (seriesValue == null)
                                        {
                                            axisValues[seriesItem] = Convert.ToDouble(r.value);
                                        }
                                        else if (Convert.ToDouble(r.value) < seriesValue)
                                        {
                                            axisValues[seriesItem] = Convert.ToDouble(r.value);
                                        }
                                        break;
                                    case "max":
                                        if (seriesValue == null)
                                        {
                                            axisValues[seriesItem] = Convert.ToDouble(r.value);
                                        }
                                        else if (Convert.ToDouble(r.value) > seriesValue)
                                        {
                                            axisValues[seriesItem] = Convert.ToDouble(r.value);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                foreach (var item in axisValues.ToList())
                {
                    if (AggregateFunction == "average")
                    {
                        axisValues[item.Key] /= countAverage[item.Key];
                    }
                    else if (AggregateFunction == "stdev")
                    {
                        double average = stDev[item.Key].Average();
                        double sumOfSquaresOfDifferences = stDev[item.Key].Select(val => (val - average) * (val - average)).Sum();
                        double sd = Math.Sqrt(sumOfSquaresOfDifferences / stDev[item.Key].Count());
                        axisValues[item.Key] = sd;
                    }
                }

                foreach (var item in axisValues)
                {
                    seriesData.Add(item.Value.GetValueOrDefault());
                    categories.Add(item.Key.Length > 20 ? item.Key.Substring(0, 20) : item.Key);
                }
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

        if(seriesData.Count() > 0)
            MaxValueAxis = (seriesData.Max() + 1).ToString().Replace(",", "."); //increase it by 1, to not cross the highest value with the chart name.
        else MaxValueAxis = "0";

        var serializer = new JavaScriptSerializer();
        JsonSeriesData = serializer.Serialize(seriesData);
        JsonCategories = serializer.Serialize(categories);
        if(JsonCategories == "[]")
            warning.Visible = true;
    }

    private void InitializeChartProperties()
    {
        
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