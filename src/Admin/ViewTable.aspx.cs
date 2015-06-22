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
/// Used to create the HTML structure of a table field
/// </summary>
public partial class Admin_ViewTable : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads a table with all the formresponses of a table field is shown
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

            foreach (var l in getLabelForTableHeader(FormFieldParentID))
            {
                sb.Append("<th>" + l + " <i class=\"fa fa-sort\"></i></th>");
            }

            sb.Append("</tr></thead><tbody>");
            int SurveyID = FormFieldExport.getSurveyID(FormFieldParentID);
            IEnumerable<FormResponse> formResponse = FormResponse.getFormResponse(FormID);
            foreach (FormResponse res in formResponse)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + res.id.ToString() + "</td>");

                int formResID = Convert.ToInt32(res.id);
                int repeatCount = 1;

                List<string> surveys = new List<string>();
                var surEl = (from s in db.Survey
                             join ff in db.FormField on s.id equals ff.survey_id
                             join rv in db.ResponseValue on ff.id equals (int)rv.formFieldId
                             where s.id == SurveyID
                             select s).FirstOrDefault();
                foreach (var el in Survey.GetSurveyListElements((int)surEl.id))
                {
                    surveys.Add(el.value);
                }
                int surveysCount = 0;
                sb.Append("<td>" + surveys[surveysCount++] + "</td>");

                IEnumerable<ResponseValue> results = getResponseValuesForRoasters(formResID, FormFieldParentID);
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
                        sb.Append("<td>" + surveys[surveysCount++] + "</td>");
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
    /// Populates the list which forms the header of a csv file with all the children
    /// of the table whose id is FormFieldParentID.
    /// </summary>
    /// <param name="FormFieldParentID">The id of the table field</param>
    /// <returns>A list of strings representing the header</returns>
    private List<string> getLabelForTableHeader(int FormFieldParentID)
    {
        List<string> labels = new List<string>();

        GRASPEntities db = new GRASPEntities();

        var roasters = from ff in db.FormFieldExport
                       where ff.FormFieldParentID == FormFieldParentID
                       orderby ff.positionIndex ascending
                       select ff;

        var surveyid = (from ffe in db.FormFieldExport
                        where ffe.id == FormFieldParentID
                        select ffe.survey_id).FirstOrDefault();


        labels.Add("ListValue");
        foreach (FormFieldExport lbl in roasters)
        {
            labels.Add(lbl.label);
        }
        return labels;
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