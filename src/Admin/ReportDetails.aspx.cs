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
using Telerik.Web.UI;
/// <summary>
/// Shows a list of the charts associated with a report
/// </summary>
public partial class Admin_ReportDetails : System.Web.UI.Page
{
    int reportID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request["id"] != "" && Request["id"] != null)
        {
            reportID = Convert.ToInt32(Request["id"]);
        }
    }

    /// <summary>
    /// The delete command associated with the grid that allows users to delete a chart for that report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rgReports_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        int rID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ReportFieldID"].ToString());
        if(rID == ReportField.deleteReportFieldChart(rID))
        {
            rgReports.DataBind();
        }
    }
    protected void BtnAddNewChart_Click(object sender, EventArgs e)
    {
        if(Request["id"] != null && Request["id"] != "")
        {
            Response.Redirect("CreateReport.aspx?rid=" + Request["id"].ToString(), true);
        }
    }
    protected void rgReports_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            var reports = from rf in db.ReportFields
                          where rf.ReportID == reportID
                          select rf;

            rgReports.DataSource = (from x in reports.Distinct().AsEnumerable()
                                    select new
                                    {
                                        ChartType = x.ChartType,
                                        Aggregate = x.ReportFieldAggregate,
                                        CreateDate = x.ReportFieldCreateDate,
                                        ReportFieldID = x.ReportFieldID.ToString().TrimStart(),
                                        ChartTitle = x.ReportFieldTitle,
                                        Order = x.ReportFieldOrder
                                    }).ToList().OrderBy(o=>o.Order);
        }
    }
    protected void rgReports_RowDrop(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
    {
        if(string.IsNullOrEmpty(e.HtmlElement))
        {
            if(e.DestDataItem != null && e.DestDataItem.OwnerGridID == rgReports.ClientID)
            {
                //reorder items in pending grid
                int reportFieldId = Convert.ToInt32(e.DraggedItems[0].GetDataKeyValue("ReportFieldID").ToString());                

                using(GRASPEntities db = new GRASPEntities())
                {
                    ReportField reportField = ReportField.GetReportField(db,reportFieldId);
                    int orderIdx = 0;
                    if(reportField.ReportFieldOrder != null)
                    {
                        orderIdx = reportField.ReportFieldOrder.Value;

                        if(e.DropPosition == GridItemDropPosition.Above && e.DestDataItem.ItemIndex < e.DraggedItems[0].ItemIndex)
                        {
                            int position = e.DestDataItem.ItemIndex - e.DraggedItems[0].ItemIndex;
                            orderIdx += position;
                            reportField.ReportFieldOrder = orderIdx;
                            ReportField.ReorderReportField(db, reportField.ReportFieldID, e.DestDataItem.ItemIndex+1, e.DraggedItems[0].ItemIndex, 1);
                            db.SaveChanges();
                            //todo: reorder element, (order+1) where < e.DraggedItems[0].ItemIndex and >= e.DestDataItem.ItemIndex
                        }
                        if(e.DropPosition == GridItemDropPosition.Below && e.DestDataItem.ItemIndex > e.DraggedItems[0].ItemIndex)
                        {
                            int position = e.DestDataItem.ItemIndex - e.DraggedItems[0].ItemIndex;
                            orderIdx += position;
                            reportField.ReportFieldOrder = orderIdx;
                            ReportField.ReorderReportField(db, reportField.ReportFieldID, e.DraggedItems[0].ItemIndex+1, e.DestDataItem.ItemIndex+1, -1);
                            db.SaveChanges();
                            //todo: reorder element, (order-1) where > e.DraggedItems[0].ItemIndex and <= e.DestDataItem.ItemIndex
                        }
                    }
                    else
                    {

                    }
                }
                rgReports.Rebind();

                //int destinationItemIndex = orderIdx - (rgReports.PageSize * rgReports.CurrentPageIndex);
                //e.DestinationTableView.Items[destinationItemIndex].Selected = true;
            }
        }

    }
}