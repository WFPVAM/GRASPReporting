using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        int counter = 0;
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


            Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();


            IEnumerable<FormResponse> formResponse = FormResponse.getFormResponse(FormID);


                List<FormFieldExport> ffields = (from ff in db.FormFieldExport
                              where ff.form_id == FormID && ff.FormFieldParentID == null &&
                              ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                              orderby ff.positionIndex ascending
                              select ff).ToList();

                List<ResponseValue> responseValues = (from rv in db.ResponseValue
                                      from fr in db.FormResponse
                                      where rv.FormResponseID == fr.id && fr.parentForm_id == FormID
                                      //where rv.FormResponseID == resp.id //&& rv.formFieldId == ffID
                                      select rv).ToList();


            foreach (FormResponse resp in formResponse)
            {
                
                sb.Append("<tr>");
                sb.Append("<td><a href=\"ViewForm.aspx?id=" + resp.id.ToString() + "\" target=\"_blank\">" + resp.id.ToString() + "</a></td>");
                sb.Append("<td>" + resp.senderMsisdn + "</td>");
                

                foreach (var ff in ffields)
                {

                    stopWatch.Start();


                    ResponseValue resVal;// = getResponseValues((int)ff.id, (int)resp.id);

                    resVal = (from rv in responseValues
                              where rv.FormResponseID == resp.id && rv.formFieldId == (int)ff.id 
                              select rv).FirstOrDefault();

                    if (resVal != null)
                    {
                        if (resVal.RVRepeatCount == -1)
                        {
                            string val = ""; // checkRoaster((int)resVal.formFieldId);
                            if (val != "")
                            {
                                sb.Append("<td>" + val + "</td>");
                            }
                            else if(ff.type == "IMAGE")  //if(FormField.isImage((int)resVal.formFieldId) == -1)
                            {
                                if (resVal.value.ToString() != "")
                                {

                                    //string folderPath = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.RawUrl, "/") + resVal.value;
                                    //string folderPath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + resVal.FormResponseID) + "//" + resVal.value;

                                   // sb.Append("<td><img height=\"30px\"   src=\"~\" >" + folderPath + "</td>");
                                    sb.Append("<td></td>");
                                }
                                else sb.Append("<td> </td>");
                            }
                            else sb.Append("<td>" + resVal.value + "</td>");
                        }
                        else
                        {
                            //if (FormField.isImage((int)resVal.formFieldId) == -1)
                            if(ff.type=="IMAGE")
                            {
                                string FilePath = Utility.GetWEBDAVRoot() + resVal.value;
                                bool isExists = System.IO.File.Exists(FilePath);
                                if (isExists)
                                {

                                    if (resVal.value != null && resVal.value.ToString() != "")
                                    {
                                       // string imagepath = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.RawUrl, "/") + resVal.value;
                                      //  sb.Append("<td><img height=\"80px\" width=\"80px\" src=" + imagepath + "></td>");
                                        sb.Append("<td></td>");
                                    }
                                    else sb.Append("<td> </td>");
                                }
                                else sb.Append("<td> </td>");
                            }
                            else
                                sb.Append("<td>" + resVal.value + "</td>");
                        }
                    }
                    else sb.Append("<td> </td>");



                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds);
                    Debug.WriteLine(elapsedTime);
                    stopWatch.Reset();
                }
                sb.Append("</tr>");

                counter++;
                if(counter == 1)
                {
                    break;
                }
            }
            sb.Append("</tbody></table></div>");




            





            //tableForms.Text = "<h1>" + elapsedTime + "</h1>" + sb.ToString();
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