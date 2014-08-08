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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Used to create the HTML structure of a roster field
/// </summary>
public partial class Admin_ViewRoster : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads a table with all the formresponses of a roster is shown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) //check if the webpage is loaded for the first time.
        {
            ViewState["PreviousPage"] = Request.UrlReferrer;//Saves the Previous page url in ViewState
        }
        
        tableForms.Text = "";
        StringBuilder sb = new StringBuilder();

        if ((Request["ffID"] != null || Request["ffID"] != "") && (Request["FormID"] != null || Request["FormID"] != ""))
        {
            int FormFieldParentID = Convert.ToInt32(Request["ffID"]);
            int FormID = Convert.ToInt32(Request["FormID"]);
            sb.Append("<div class=\"table-responsive\"><table class=\"table table-bordered table-hover table-striped tablesorter\"><thead><tr>");
            sb.Append("<th>ResponseID <i class=\"fa fa-sort\"></i></th>");

            GRASPEntities db = new GRASPEntities();

            IEnumerable<FormFieldExport> roasters = from ff in db.FormFieldExport
                                                    where ff.FormFieldParentID == FormFieldParentID
                                                    orderby ff.positionIndex ascending
                                                    select ff;

            foreach (var l in roasters)
            {
                sb.Append("<th>" + l.label + " <i class=\"fa fa-sort\"></i></th>");
            }

            sb.Append("</tr></thead><tbody>");

            IEnumerable<FormResponse> formResponse = FormResponse.getFormResponse(FormID);
            foreach (FormResponse res in formResponse)
            {
                int formResID = Convert.ToInt32(res.id);
                int repeatCount = 1;
                IEnumerable<ResponseValue> results = getResponseValuesForRoasters(formResID, FormFieldParentID);
                if (results.Count() > 0)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + res.id.ToString() + "</td>");                    
                }
                foreach (ResponseValue resVal in results)
                {
                    if (repeatCount == resVal.RVRepeatCount)
                    {
                        sb.Append("<td>" + resVal.value + "</td>");
                        repeatCount = (int)resVal.RVRepeatCount;
                    }
                    else
                    {
                        sb.Append("</tr><tr>");
                        sb.Append("<td>" + res.id.ToString() + "</td>");
                        sb.Append("<td>" + resVal.value + "</td>");
                        repeatCount = (int)resVal.RVRepeatCount;
                    }
                }
                //write a single ResponseValue to file
                if (results.Count() > 0)
                {
                    sb.Append("</tr>");
                }
            }
            sb.Append("</tbody></table></div>");
            tableForms.Text = sb.ToString();
        }
    }
    /// <summary>
    /// </summary>
    /// <param name="FormResponseID">The id of a formresponse</param>
    /// <param name="formFieldParentID">The id of the roster field</param>
    /// <returns>A list representing all the responsevalue for that roster</returns>
    protected IEnumerable<ResponseValue> getResponseValuesForRoasters(int FormResponseID, int formFieldParentID)
    {
        GRASPEntities db = new GRASPEntities();

        var responseValue = from rv in db.ResponseValue
                            join ffe in db.FormFieldExport on rv.formFieldId equals (int)ffe.id
                            where rv.FormResponseID == FormResponseID && ffe.FormFieldParentID == formFieldParentID &&
                            rv.formFieldId != null && rv.RVRepeatCount > 0
                            orderby rv.RVRepeatCount, rv.positionIndex ascending
                            select rv;

        return responseValue;
    }
    /// <summary>
    /// </summary>
    /// <param name="FormID">The id of the form</param>
    /// <returns>A list representing all the formResponse for a form</returns>
    protected IEnumerable<FormResponse> getCompiledForms(int FormID)
    {
        GRASPEntities db = new GRASPEntities();

        var formResponse = from fr in db.FormResponse
                           where fr.parentForm_id == FormID
                           select fr;
        return formResponse;
    }
    /// <summary>
    /// </summary>
    /// <param name="ffID">The id of a field</param>
    /// <param name="FormResponseID">The id of a formResponse</param>
    /// <returns>A list representing all the responsevalue for that field</returns>
    protected ResponseValue getResponseValues(int ffID, int FormResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var responseValue = (from rv in db.ResponseValue
                             where rv.FormResponseID == FormResponseID && rv.formFieldId == ffID
                             select rv).FirstOrDefault();

        return responseValue;
    }
    /// <summary>
    /// </summary>
    /// <param name="FormResponseID">The id of a formresponse</param>
    /// <param name="formFieldParentID">The id of the roster field</param>
    /// <param name="ffID"></param>
    /// <returns>A list representing all the responsevalue for a field of that roster</returns>
    protected IEnumerable<ResponseValue> getResponseValuesForRoasters(int FormResponseID, int formFieldParentID, int ffID)
    {
        GRASPEntities db = new GRASPEntities();

        var responseValue = from rv in db.ResponseValue
                            join ffe in db.FormFieldExport on rv.formFieldId equals (int)ffe.id
                            where rv.FormResponseID == FormResponseID && ffe.FormFieldParentID == formFieldParentID &&
                            rv.formFieldId == ffID
                            select rv;

        return responseValue;
    }
    /// <summary>
    /// Shows the previous page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (ViewState["PreviousPage"] != null)	//Check if the ViewState 
        //contains Previous page URL
        {
            Response.Redirect(ViewState["PreviousPage"].ToString());//Redirect to 
            //Previous page by retrieving the PreviousPage Url from ViewState.
        }
    }
}