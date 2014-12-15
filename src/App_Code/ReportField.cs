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

/// <summary>
/// ReportField class contains auxiliary functions to query ReportField table on Grasp DB
/// </summary>
public partial class ReportField
{
    /// <summary>
    /// Creates a new pie chart for a report
    /// </summary>
    /// <param name="ReportID">The id of the report</param>
    /// <param name="FormFieldID">The id of the series name</param>
    /// <param name="ReportFieldLabel">An optional description for the series name</param>
    /// <param name="chartType">The type of the chart</param>
    /// <returns>The reportField created</returns>
    public static ReportField createNewReportField(int ReportID, int FormFieldID, string title, string ReportFieldLabel, string chartType)
    {
        return createNewReportField(ReportID, FormFieldID, title, ReportFieldLabel, chartType, null, null, null, null, null);
    }
    /// <summary>
    /// Creates a new bar chart for a report
    /// </summary>
    /// <param name="ReportID">The id of the report</param>
    /// <param name="FormFieldID">The id of the series name</param>
    /// <param name="ReportFieldLabel">An optional description for the series name</param>
    /// <param name="chartType">The type of the chart</param>
    /// <param name="ValueField">An optional description for the series value</param>
    /// <param name="ValueFormFieldID">The id of the series value</param>
    /// <param name="aggregate">A string representing an aggregate function</param>
    /// <param name="legend">1 if you want the legend to display</param>
    /// <param name="table">1 if you want the table to display</param>
    /// <returns>the reportField created</returns>
    public static ReportField createNewReportField(int ReportID, int FormFieldID, string title, string ReportFieldLabel, string chartType, string ValueField, int? ValueFormFieldID, string aggregate, int? legend, int? table)
    {
        GRASPEntities db = new GRASPEntities();

        var reportField = new ReportField();
        reportField.ReportID = ReportID;
        reportField.FormFieldID = FormFieldID;
        reportField.ReportFieldLabel = ReportFieldLabel;
        reportField.ReportFieldCreateDate = DateTime.Now;
        reportField.ChartType = chartType;
        reportField.ReportFieldValueLabel = ValueField;
        reportField.ValueFormFieldID = ValueFormFieldID;
        reportField.ReportFieldAggregate = aggregate;
        reportField.ReportFieldLegend = legend;
        reportField.ReportFieldTableData = table;
        reportField.ReportFieldTitle = title;
        db.ReportFields.Add(reportField);
        db.SaveChanges();

        return reportField;
    }
    /// <summary>
    /// Queries the DB to obtain information about a ReportField
    /// </summary>
    /// <param name="reportFieldID">The id of the report</param>
    /// <returns>The id of the formfield representing the series name</returns>
    public static int getFormFieldID(int reportFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        var ffID = (from rf in db.ReportFields
                    where rf.ReportFieldID == reportFieldID
                    select rf.FormFieldID).FirstOrDefault();
        if (ffID != null)
            return Convert.ToInt32(ffID);
        else return 0;
    }

    public static ReportField GetReportField(GRASPEntities db, int reportFieldId)
    {
        var item = (from rf in db.ReportFields
                    where rf.ReportFieldID == reportFieldId
                    select rf).FirstOrDefault();
        return item;
    }

    public static void ReorderReportField(GRASPEntities db,int reportFieldId, int startIdx, int endIdx,int step)
    {
        var items = (from rf in db.ReportFields
                    where rf.ReportFieldOrder >= startIdx && rf.ReportFieldOrder<= endIdx && rf.ReportFieldID!=reportFieldId
                    select rf);
        foreach(ReportField i in items)
        {
            i.ReportFieldOrder = i.ReportFieldOrder + step;
        }
    }


    /// <summary>
    /// </summary>
    /// <param name="ReportID">the id of the report</param>
    /// <returns>A list representing all the reportfields(charts) of a form</returns>
    public static IEnumerable<ReportField> getReportFields(int ReportID)
    {
        GRASPEntities db = new GRASPEntities();

        var items = from rf in db.ReportFields
                    where rf.ReportID == ReportID
                    select rf;

        return items;
    }
    /// <summary>
    /// Delete all the reportfields(charts) of a report
    /// </summary>
    /// <param name="ReportID">the id of a report</param>
    public static void deleteReportField(int ReportID)
    {
        GRASPEntities db = new GRASPEntities();

        var report = from rf in db.ReportFields
                      where rf.ReportID == ReportID
                      select rf;

        foreach (ReportField rep in report)
        {
            db.ReportFields.Remove(rep);
        }
        db.SaveChanges();
    }
    /// <summary>
    /// Deletes a specific chart of a report
    /// </summary>
    /// <param name="ReportFieldID">the id of the reportfield(chart)</param>
    /// <returns>The id of the reportfield deleted, 0 in it not exists</returns>
    public static int deleteReportFieldChart(int ReportFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        var report = (from rf in db.ReportFields
                     where rf.ReportFieldID == ReportFieldID
                     select rf).FirstOrDefault();

        if (report != null)
        {
            db.ReportFields.Remove(report);
            db.SaveChanges();

            return report.ReportFieldID;
        }
        else return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reportFieldID"></param>
    /// <returns></returns>
    public static int getReportID(int reportFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        int item = (from rf in db.ReportFields
                   where rf.ReportFieldID == reportFieldID
                   select rf.ReportID).FirstOrDefault();
        if (item != null)
            return item;
        else return 0;
    }
}