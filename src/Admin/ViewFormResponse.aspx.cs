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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
/// <summary>
/// Used to create the HTML structure of all the ResponseValue of a FormResponse
/// </summary>
public partial class Admin_ViewFormResponse : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads a table with all the formResponse for a form is shown.
    /// This table follows the structure of the exported CSV files.
    /// By clicking on the roster or table headers another table is shown with the responses value for that field.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        tableForms.Text = "";
        StringBuilder sb = new StringBuilder();
        if (Request["FormID"] != null || Request["FormID"] != "")
        {
            int FormID = Convert.ToInt32(Request["FormID"]);
            sb.Append("<div class=\"table-responsive\"><table class=\"table table-bordered table-hover table-striped tablesorter\"><thead><tr>");

            List<string> labels = new List<string>();
            int r = 0;
            int t = 0;

            sb.Append("<th>ResponseID <i class=\"fa fa-sort\"></i></th>");
            sb.Append("<th>Sender <i class=\"fa fa-sort\"></i></th>");

            GRASPEntities db = new GRASPEntities();

            var formFieldsLabels = from ff in db.FormFieldExport
                                   where ff.form_id == FormID && ff.FormFieldParentID == null &&
                                   ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                                   orderby ff.positionIndex ascending
                                   select ff;

            foreach (var l in formFieldsLabels)
            {
                if (l.type == "REPEATABLES_BASIC" || l.type == "REPEATABLES")
                {
                    if (l.type == "REPEATABLES_BASIC")
                    {
                        sb.Append("<th><a href=\"ViewRoster.aspx?FormID=" + FormID.ToString() + "&ffID=" + l.id.ToString() + "\">" + l.label + " (R" + ++r + ")</a></th>");
                    }
                    else if (l.type == "REPEATABLES")
                    {
                        sb.Append("<th><a href=\"ViewTable.aspx?FormID=" + FormID.ToString() + "&ffID=" + l.id.ToString() + "\">" + l.label + " (T" + ++t + ")</a></th>");
                    }
                }
                else sb.Append("<th>" + l.label + " <i class=\"fa fa-sort\"></i></th>");
            }

            sb.Append("</tr></thead><tbody>");

            IEnumerable<FormResponse> formResponse = FormResponse.getFormResponse(FormID);
            foreach (FormResponse resp in formResponse)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + resp.id.ToString() + "</td>");
                sb.Append("<td>" + resp.senderMsisdn + "</td>");
                
                var ffields = from ff in db.FormFieldExport
                              where ff.form_id == FormID && ff.FormFieldParentID == null &&
                              ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                              orderby ff.positionIndex ascending
                              select ff;

                foreach (var ff in ffields)
                {
                    ResponseValue resVal = getResponseValues((int)ff.id, (int)resp.id);
                    if (resVal != null)
                    {
                        if (resVal.RVRepeatCount == -1)
                        {
                            string val = checkRoaster((int)resVal.formFieldId);
                            if (val != "")
                            {
                                sb.Append("<td>" + val + "</td>");
                            }
                            else sb.Append("<td>" + resVal.value + "</td>");
                        }
                        else sb.Append("<td>" + resVal.value + "</td>");
                    }
                    else sb.Append("<td> </td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></div>");

            tableForms.Text = sb.ToString();
        }
    }
    /// <summary>
    /// </summary>
    /// <param name="ffID">The id of the formfield</param>
    /// <param name="FormResponseID">The id of a formResponse</param>
    /// <returns>A list representing all the responseValues for that field</returns>
    protected ResponseValue getResponseValues(int ffID, int FormResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var responseValue = (from rv in db.ResponseValue
                             where rv.FormResponseID == FormResponseID && rv.formFieldId == ffID
                             select rv).FirstOrDefault();

        return responseValue;
    }
    /// <summary>
    /// Checks if a field is a roster or a table
    /// </summary>
    /// <param name="formFieldID">The id of the field</param>
    /// <returns>Empty string if is a roster, the name of the list associated with the table otherwise</returns>
    private string checkRoaster(int formFieldID)
    {
        string res = "";
        if (formFieldID != null)
        {
            int surveyID = FormFieldExport.getSurveyID(formFieldID);
            GRASPEntities db = new GRASPEntities();
            var surElID = (from s in db.Survey
                           where s.id == surveyID
                           select s).FirstOrDefault();
            if (surElID != null)
                res = surElID.name;
        }
        return res;
    }
}