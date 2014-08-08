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

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Used for the exporting of a form when CSV method is selected
/// </summary>
public partial class CSVExport : System.Web.UI.Page
{
    int FormID;
    string formName;
    string SeparatorChar = ",";
    bool linearTable = false;
    List<string> filesToZip;
    string csvPath = "";

    /// <summary>
    /// When the page loads the method ExportCSV is called
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {


        csvPath = HttpContext.Current.Server.MapPath("~/Temp/");
        if(!Directory.Exists(csvPath))
        {
            Directory.CreateDirectory(csvPath);
        }
        
        if (Request["FormID"] != null && Request["FormName"] != null)
        {
            if (Request["separator"] != "")
                SeparatorChar = Request["separator"];
            if (Request["linear"] == "on")
                linearTable = true;
            FormID = Convert.ToInt32(Request["FormID"]);
            formName = Request["FormName"];
            try
            {
                ExportCSV();
            }
            catch (Exception ex)
            {
                Utility.writeErrorLog(ex.Message);
                System.IO.DirectoryInfo tempDir = new DirectoryInfo(Server.MapPath("~/Temp/"));

                foreach (FileInfo file in tempDir.GetFiles())
                {
                    file.Delete();
                }
            }
        }

    }
    /// <summary>
    /// Populates the list which forms the header of the main csv file.
    /// When it encounters a roster or a table creates its csv files.
    /// </summary>
    /// <returns>A list of strings representing the header</returns>
    protected List<string> getLabelForCSVHeader()
    {
        List<string> labels = new List<string>();
        List<string> response;
        string roasterCsv = "";
        int r = 0;
        int t = 0;

        labels.Add("\"ResponseID\"");
        labels.Add("\"Sender\"");

        GRASPEntities db = new GRASPEntities();

        var formFieldsLabels = from ff in db.FormFieldExport
                               where ff.form_id == FormID && ff.FormFieldParentID == null &&
                               ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                               orderby ff.positionIndex ascending
                               select ff;

        foreach (FormFieldExport lbl in formFieldsLabels)
        {
            if (lbl.type == "REPEATABLES_BASIC" || lbl.type == "REPEATABLES")
            {
                if (lbl.type == "REPEATABLES_BASIC")
                {
                    labels.Add("\"" + lbl.label + " (R" + ++r + ")\"");
                    csvPath = HttpContext.Current.Server.MapPath("~/Temp/");
                    roasterCsv = csvPath + formName + "_R" + r + ".csv";

                    if (File.Exists(roasterCsv))
                        File.Delete(roasterCsv);
                    using (var wr = new StreamWriter(roasterCsv, true))
                    {
                        wr.WriteLine(formatCSVLine(getLabelForRoasterHeader((int)lbl.id)));

                        foreach (FormResponse res in getCompiledForms())
                        {
                            response = new List<string>();
                            response.Add(res.id.ToString());
                            int formResID = Convert.ToInt32(res.id);
                            int repeatCount = 1;
                            IEnumerable<ResponseValue> results = getResponseValuesForRoasters(formResID, (int)lbl.id);
                            foreach (ResponseValue resVal in results)
                            {
                                if (repeatCount == resVal.RVRepeatCount)
                                {
                                    if (!Utility.isNumeric(resVal.value))
                                        response.Add("\"" + resVal.value + "\"");
                                    else response.Add(resVal.value);
                                    repeatCount = (int)resVal.RVRepeatCount;
                                }
                                else
                                {
                                    wr.WriteLine(formatCSVLine(response));
                                    Debug.WriteLine(formatCSVLine(response));
                                    response = new List<string>();
                                    response.Add(res.id.ToString());
                                    if (!Utility.isNumeric(resVal.value))
                                        response.Add("\"" + resVal.value + "\"");
                                    else response.Add(resVal.value);
                                    repeatCount = (int)resVal.RVRepeatCount;
                                }
                            }
                            //write a single ResponseValue to file
                            if (results.Count() > 0)
                            {
                                wr.WriteLine(formatCSVLine(response));
                                Debug.WriteLine(formatCSVLine(response));
                            }
                        }
                        filesToZip.Add(roasterCsv);
                    }
                }
                if (lbl.type == "REPEATABLES")
                {
                    labels.Add("\"" + lbl.label + " (T" + ++t + ")\"");
                    csvPath = HttpContext.Current.Server.MapPath("~/Temp/");
                    roasterCsv = csvPath + formName + "_T" + t + ".csv";

                    if (File.Exists(roasterCsv))
                        File.Delete(roasterCsv);
                    using (var wr = new StreamWriter(roasterCsv, true))
                    {
                        wr.WriteLine(formatCSVLine(getLabelForTableHeader((int)lbl.id)));

                        if (linearTable == true)
                        {
                            foreach (FormResponse res in getCompiledForms())
                            {
                                response = new List<string>();
                                response.Add(res.id.ToString());
                                int formResID = Convert.ToInt32(res.id);
                                int repeatCount = 1;

                                List<string> surveys = new List<string>();
                                var surEl = (from s in db.Survey
                                             join ff in db.FormField on s.id equals ff.survey_id
                                             join rv in db.ResponseValue on ff.id equals (int)rv.formFieldId
                                             where s.id == (int)lbl.survey_id
                                             select s).FirstOrDefault();
                                foreach (var el in FormFieldExport.getSurveyListElements((int)surEl.id))
                                {
                                    surveys.Add(el.value);
                                }
                                int surveysCount = 0;
                                response.Add(surveys[surveysCount++]);

                                IEnumerable<ResponseValue> results = getResponseValuesForRoasters(formResID, (int)lbl.id);
                                foreach (ResponseValue resVal in results)
                                {
                                    if (repeatCount == resVal.RVRepeatCount)
                                    {
                                        if (!Utility.isNumeric(resVal.value))
                                            response.Add("\"" + resVal.value + "\"");
                                        else response.Add(resVal.value);
                                        repeatCount = (int)resVal.RVRepeatCount;
                                    }
                                    else
                                    {
                                        response.Add(surveys[surveysCount++]);
                                        if (!Utility.isNumeric(resVal.value))
                                            response.Add("\"" + resVal.value + "\"");
                                        else response.Add(resVal.value);
                                        repeatCount = (int)resVal.RVRepeatCount;
                                    }
                                }
                                //wr.WriteLine(formatCSVLine(response));
                                //write a single ResponseValue to file
                                if (results.Count() > 0)
                                {
                                    wr.WriteLine(formatCSVLine(response));
                                    Debug.WriteLine(formatCSVLine(response));
                                }
                            }
                        }
                        else
                        {
                            foreach (FormResponse res in getCompiledForms())
                            {
                                response = new List<string>();
                                response.Add(res.id.ToString());
                                int formResID = Convert.ToInt32(res.id);
                                int repeatCount = 1;

                                List<string> surveys = new List<string>();
                                var surEl = (from s in db.Survey
                                             join ff in db.FormField on s.id equals ff.survey_id
                                             join rv in db.ResponseValue on ff.id equals (int)rv.formFieldId
                                             where s.id == (int)lbl.survey_id
                                             select s).FirstOrDefault();
                                foreach (var el in FormFieldExport.getSurveyListElements((int)surEl.id))
                                {
                                    surveys.Add(el.value);
                                }
                                int surveysCount = 0;
                                response.Add(surveys[surveysCount++]);

                                IEnumerable<ResponseValue> results = getResponseValuesForRoasters(formResID, (int)lbl.id);
                                foreach (ResponseValue resVal in results)
                                {
                                    if (repeatCount == resVal.RVRepeatCount)
                                    {
                                        if (!Utility.isNumeric(resVal.value))
                                            response.Add("\"" + resVal.value + "\"");
                                        else response.Add(resVal.value);
                                        repeatCount = (int)resVal.RVRepeatCount;
                                    }
                                    else
                                    {
                                        wr.WriteLine(formatCSVLine(response));
                                        Debug.WriteLine(formatCSVLine(response));
                                        response = new List<string>();
                                        response.Add(res.id.ToString());
                                        response.Add(surveys[surveysCount++]);
                                        if (!Utility.isNumeric(resVal.value))
                                            response.Add("\"" + resVal.value + "\"");
                                        else response.Add(resVal.value);
                                        repeatCount = (int)resVal.RVRepeatCount;
                                    }
                                }
                                //write a single ResponseValue to file
                                if (results.Count() > 0)
                                {
                                    wr.WriteLine(formatCSVLine(response));
                                    Debug.WriteLine(formatCSVLine(response));
                                }
                            }
                        }
                        filesToZip.Add(roasterCsv);
                    }
                }
            }
            else labels.Add("\"" + lbl.label + "\"");
        }

        return labels;
    }
    /// <summary>
    /// Populates the list which forms the header of a csv file with all the children
    /// of the table whose id is FormFieldParentID.
    /// </summary>
    /// <param name="FormFieldParentID">The id of the table field</param>
    /// <returns>A list of strings representing the header</returns>
    private List<string> getLabelForTableHeader(int FormFieldParentID)
    {
        int surveyID = 0;
        List<string> labels = new List<string>();
        labels.Add("\"ResponseID\"");


        GRASPEntities db = new GRASPEntities();

        var roasters = from ff in db.FormFieldExport
                       where ff.FormFieldParentID == FormFieldParentID
                       orderby ff.positionIndex ascending
                       select ff;

        var surveyid = (from ffe in db.FormFieldExport
                        where ffe.id == FormFieldParentID
                        select ffe.survey_id).FirstOrDefault();



        if (linearTable == true)
        {
            try
            {
                surveyID = Convert.ToInt32(surveyid);
            }
            catch (Exception ex)
            {
                //surveyID == 0 if query is null
            }
            var count = (from s in db.SurveyListAPI
                         where s.id == surveyID
                         select s).Count();
            for (int i = 0; i < count; i++)
            {
                labels.Add("\"ListValue\"");
                foreach (FormFieldExport lbl in roasters)
                {
                    labels.Add("\"" + lbl.label + "\"");
                }
            }

        }
        else
        {
            labels.Add("\"ListValue\"");
            foreach (FormFieldExport lbl in roasters)
            {
                labels.Add("\"" + lbl.label + "\"");
            }
        }
        return labels;
    }
    /// <summary>
    /// Populates the list which forms the header of a csv file with all the children
    /// of the roster whose id is FormFieldParentID.
    /// </summary>
    /// <param name="FormFieldParentID">The id of the roster field</param>
    /// <returns>A list of strings representing the header</returns>
    protected List<string> getLabelForRoasterHeader(int FormFieldParentID)
    {
        List<string> labels = new List<string>();
        labels.Add("\"ResponseID\"");

        GRASPEntities db = new GRASPEntities();

        var roasters = from ff in db.FormFieldExport
                       where ff.FormFieldParentID == FormFieldParentID
                       orderby ff.positionIndex ascending
                       select ff;

        foreach (FormFieldExport lbl in roasters)
        {
            labels.Add("\"" + lbl.label + "\"");
        }

        return labels;
    }
    /// <summary>
    /// </summary>
    /// <returns>A list representing all the FormResponse for the choosen form</returns>
    protected IEnumerable<FormResponse> getCompiledForms()
    {
        GRASPEntities db = new GRASPEntities();

        var formResponse = from fr in db.FormResponse
                           where fr.parentForm_id == FormID
                           select fr;
        return formResponse;
    }
    /// <summary>
    /// </summary>
    /// <param name="ffID">The id representing a field of the choosen form</param>
    /// <param name="FormResponseID">The id representing a FormResponse of the choosen form</param>
    /// <returns>The ResponseValue for that field</returns>
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
    /// <param name="FormResponseID">The id representing a FormResponse of the choosen form</param>
    /// <param name="formFieldParentID">The id representing the roster field</param>
    /// <returns>A list representing all the ResponseValues for that roster</returns>
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
    /// Creates the main CSV file with all the FormResponses for the choosen form
    /// </summary>
    protected void ExportCSV()
    {
        GRASPEntities db = new GRASPEntities();
        filesToZip = new List<string>();
        string mainCSV = csvPath + formName + ".csv";
        List<string> response;

        if (File.Exists(mainCSV))
            File.Delete(mainCSV);
        //Initialise stream object with file
        using (var wr = new StreamWriter(mainCSV, true))
        {
            //write header to file
            List<string> headerLabels = getLabelForCSVHeader();
            wr.WriteLine(formatCSVLine(headerLabels));

            foreach (FormResponse res in getCompiledForms())
            {
                response = new List<string>();
                response.Add(res.id.ToString());
                response.Add("\"" + res.senderMsisdn + "\"");
                int formResID = Convert.ToInt32(res.id);

                var ffields = from ff in db.FormFieldExport
                              where ff.form_id == FormID && ff.FormFieldParentID == null &&
                              ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                              orderby ff.positionIndex ascending
                              select ff;
                //foreach (ResponseValue resVal in getResponseValues(formResID))
                //{
                foreach (var ff in ffields)
                {
                    ResponseValue resVal = getResponseValues((int)ff.id, formResID);
                    if (resVal != null)
                    {
                        if (resVal.RVRepeatCount == -1)
                        {
                            string val = checkRoaster((int)resVal.formFieldId);
                            if (val != "")
                            {
                                response.Add("\"" + val + "\"");
                            }
                            else response.Add(resVal.value);
                        }
                        else if (!Utility.isNumeric(resVal.value))
                            response.Add("\"" + resVal.value + "\"");
                        else response.Add(resVal.value);
                    }
                    else response.Add("");

                }
                //write a single ResponseValue to file
                wr.WriteLine(formatCSVLine(response));
            }
            filesToZip.Add(mainCSV);
        }
        createZip();
        foreach (string f in filesToZip)
        {
            File.Delete(f);
        }

    }
    /// <summary>
    /// Checks if a field is a roster
    /// </summary>
    /// <param name="formFieldID">The id representing the roster field</param>
    /// <returns>A string with the roster name, or an empty string if the field is not a roster</returns>
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
    /// <summary>
    /// Formats a line with the specified separator
    /// </summary>
    /// <param name="row">A list of values that will form the line</param>
    /// <returns>The line well formatted</returns>
    protected string formatCSVLine(List<string> row)
    {
        var sb = new StringBuilder();

        foreach (string value in row)
        {
            if (sb.Length > 0)
                sb.Append(SeparatorChar);
            if (value == null)
                sb.Append("");
            else sb.Append(value.Replace((string)SeparatorChar, " ").Replace("\r\n", " ").Replace("\n", " "));
        }

        return sb.ToString();
    }
    /// <summary>
    /// Creates the zip file with all the CSV files and then downloads it.
    /// </summary>
    protected void createZip()
    {
        Response.ContentType = "application/zip";
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename = \"{0}\"", System.IO.Path.GetFileName(formName + ".zip")));
        //Response.AppendHeader("content-disposition", "attachment; filename=" + formName + ".zip");
        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(csvPath);
            zip.Save(Response.OutputStream);
        }
    }
}