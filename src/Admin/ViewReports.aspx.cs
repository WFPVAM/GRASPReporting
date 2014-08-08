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
using System.Data.Objects.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
/// <summary>
/// Shows a list of all the Report previously created
/// </summary>
public partial class Admin_Statistics_ViewReports : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    /// <summary>
    /// Fills the grid with all the reports
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ldsReport_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var reports = from r in db.Reports
                      join rf in db.ReportFields on r.ReportID equals rf.ReportID
                      join f in db.Form on r.FormID equals f.id
                      where f.finalised == 1
                      select new { r, f };

        e.Result = from x in reports.Distinct().AsEnumerable()
                   select new
        {
            ReportName = "<a style=\"color:#0058B1\" href=\"ViewChart.aspx?reportName=" + x.r.ReportName + "&reportID=" + x.r.ReportID.ToString().TrimStart() + "\"><i class=\"fa fa-tasks\"></i> " + x.r.ReportName + "</a>",
            ReportDescription = x.r.ReportDescription,
            FormName = x.f.name,
            CreateDate = x.r.ReportCreateDate,
            ReportID = x.r.ReportID.ToString().TrimStart()
        };
    }
    /// <summary>
    /// The delete command associated with the grid that allows users to delete a report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rgReports_DeleteCommand(object sender, GridCommandEventArgs e)
    {        
        int rID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ReportID"].ToString());
        if (rID == Report.deleteReport(rID))
            rgReports.DataBind();
    }
}