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
/// Report class contains auxiliary functions to query Report table on Grasp DB
/// </summary>
public partial class Report
{
    /// <summary>
    /// Creates a new Report for a form
    /// </summary>
    /// <param name="name">The report name</param>
    /// <param name="description">The description for the report (can be empty)</param>
    /// <param name="FormID">The id of the form</param>
    /// <returns>The id of the created report</returns>
    public static int createNewReport(string name, string description, int FormID)
    {
        GRASPEntities db = new GRASPEntities();

        var report = new Report();
        report.ReportName = name;
        report.ReportDescription = description;
        report.FormID = FormID;
        report.ReportCreateDate = DateTime.Now;

        db.Reports.Add(report);
        db.SaveChanges();

        return report.ReportID;

    }
    /// <summary>
    /// </summary>
    /// <returns>The number of all the reports</returns>
    public static int getReportsNumber()
    {
        GRASPEntities db = new GRASPEntities();

        var reports = from r in db.Reports
                      join rf in db.ReportFields on r.ReportID equals rf.ReportID
                      join f in db.Form on r.FormID equals f.id
                      where f.finalised == 1
                      select r;

        if (reports != null)
            return reports.Distinct().Count();
        else return 0;
    }
    /// <summary>
    /// Delete the report for a form
    /// </summary>
    /// <param name="ReportID">The id of a report</param>
    /// <returns>The id of the deleted report, 0 if there isn't a report with that id</returns>
    public static int deleteReport(int ReportID)
    {
        ReportField.deleteReportField(ReportID);
        GRASPEntities db = new GRASPEntities();

        var report = (from r in db.Reports
                      where r.ReportID == ReportID
                      select r).FirstOrDefault();

        if (report != null)
        {
            db.Reports.Remove(report);
            db.SaveChanges();

            return report.ReportID;
        }
        else return 0;
    }
    /// <summary>
    /// </summary>
    /// <param name="ReportID">hte id of the report</param>
    /// <returns>The id of the form associated with the report</returns>
    public static int getFormID(int ReportID)
    {
        GRASPEntities db = new GRASPEntities();

        var report = (from r in db.Reports
                      where r.ReportID == ReportID
                      select r).FirstOrDefault();

        if (report != null)
            return Convert.ToInt32(report.FormID);
        else return 0;
    }
}