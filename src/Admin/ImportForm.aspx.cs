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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Used to import a form from a csv file
/// </summary>
public partial class Admin_ImportForm : System.Web.UI.Page
{
    public string formID = "";
    public string name = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        formID = Request["FormID"];
        name = Request["FormName"];
    }
    /// <summary>
    /// Takes the zip file uploaded and parses it in order to create or updates the ResponseValues of a selected form
    /// from the csv files contained in it.
    /// First it checks if there are all the CSV files for the rosters or tables then checks if data will be overwritten.
    /// After these checks it begins to scan file by file and make the appropriate changes to the DB.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnImport_Click(object sender, EventArgs e)
    {
        string fileNameMain = "";
        Literal1.Text = "";
        int FormID = 0;
        string[] header = { };
        char separator = ';';
        Dictionary<string, string> rosterFF = new Dictionary<string, string>(); //associazione tra Roster o Tabelle e il loro Formfield da usare come formfieldparent nei repeatables
        //Dictionary<int, int> rosterComp = new Dictionary<int, int>(); //associazione tra il formresponse e il numero di compilazioni per quel roster
        Dictionary<string, int> mapResID = new Dictionary<string, int>(); //associazione tra il formresponseid indicativo e quello reale creato in seguito all insert
        Dictionary<string, int> mapSurList = new Dictionary<string, int>(); //mantiene le associazioni delle liste in ordine crescente
        try
        {
            FormID = Convert.ToInt32(formID);
            separator = Convert.ToChar(ddlCharacter.SelectedValue);
        }
        catch (Exception ex)
        {
        }

        GRASPEntities db = new GRASPEntities();

        var formFieldsLabels = from ff in db.FormFieldExport
                               where ff.form_id == FormID && ff.FormFieldParentID == null &&
                               ff.type != "SEPARATOR" && ff.type != "TRUNCATED_TEXT" && ff.type != "WRAPPED_TEXT"
                               orderby ff.positionIndex ascending
                               select ff;
        if (FileUpload1.HasFile)
        {
            string ext = Path.GetExtension(FileUpload1.FileName);
            if (ext == ".zip" || ext == ".ZIP")
            {
                using (ZipFile zip = ZipFile.Read(FileUpload1.FileContent))
                {
                    zip.Entries.OrderBy(x => x.FileName);
                    foreach (ZipEntry entry in zip)
                    {
                        if (entry.FileName == name + ".csv")
                        {
                            fileNameMain = entry.FileName;
                            Stream s = entry.OpenReader();
                            StreamReader sr = new StreamReader(s);
                            int count = 0;
                            using (var file = sr)
                            {
                                int asd = 112;
                                string line;
                                while ((line = file.ReadLine()) != null)
                                {
                                    int lengthValue = 0;
                                    int i = 2;
                                    if (count != 0)
                                    {
                                        string[] responses = line.Split(separator);
                                        if (responses[1] != "DELETE")
                                        {
                                            if (!responses[0].Contains("ADD"))
                                            {
                                                ResponseValue.deleteResponsesValues(Convert.ToInt32(responses[0]));
                                            }
                                            if (overwrite.Checked == true)
                                            {
                                                FormResponse.deleteFormResponse(Convert.ToInt32(responses[0]));
                                            }
                                            int formResponseID = 0;
                                            if(responses[0].Contains("ADD"))
                                            {
                                                lengthValue = responses[1].Length;
                                                formResponseID = FormResponse.createFormResponse(FormID, responses[1].Substring(1, lengthValue - 2), "WEBImport");
                                            }
                                            else if (FormResponse.formResponseExists(Convert.ToInt32(responses[0])))
                                            {
                                                FormResponse.updateFormResponseToImport(Convert.ToInt32(responses[0]));
                                                formResponseID = Convert.ToInt32(responses[0]);
                                            }
                                            mapResID.Add(responses[0], formResponseID);
                                            lengthValue = responses[1].Length;

                                            foreach (var ffID in formFieldsLabels)
                                            {
                                                int roster = FormField.isRoster((int)ffID.id);
                                                if (roster == -1)
                                                {
                                                    string[] RepeatableCode = header[i].Split(' ');
                                                    string tmpHeader = RepeatableCode[RepeatableCode.Length - 1].Substring(1, 2);
                                                    if (!rosterFF.ContainsKey(tmpHeader))
                                                        rosterFF.Add(tmpHeader, ffID.id.ToString());
                                                    //mi salvo il formfieldparentID e la i-sima posizione

                                                    if (ffID.survey_id != null)
                                                    {
                                                        int lc = 1;
                                                        IEnumerable<SurveyElement> slist = FormFieldExport.getSurveyList((int)ffID.id);
                                                        foreach (SurveyElement sl in slist)
                                                        {
                                                            if (!mapSurList.ContainsKey(responses[0] + sl.value))
                                                                mapSurList.Add(responses[0] + sl.value, lc++);
                                                        }

                                                    }
                                                    lengthValue = responses[i].Length;
                                                    if (responses[i].Contains('"') && lengthValue > 0)
                                                        ResponseValue.createResponseValue(responses[i++].Substring(1, lengthValue - 2), formResponseID, (int)ffID.id, -1);
                                                    else
                                                        ResponseValue.createResponseValue(responses[i++], formResponseID, (int)ffID.id, -1);
                                                }
                                                else
                                                {
                                                    lengthValue = responses[i].Length;
                                                    if (responses[i].Contains('"') && lengthValue > 0)
                                                        ResponseValue.createResponseValue(responses[i++].Substring(1, lengthValue - 2), formResponseID, (int)ffID.id, 0);
                                                    else
                                                        ResponseValue.createResponseValue(responses[i++], formResponseID, (int)ffID.id, 0);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ResponseValue.deleteResponsesValues(Convert.ToInt32(responses[0]));
                                            FormResponse.deleteFormResponse(Convert.ToInt32(responses[0]));
                                        }
                                    }
                                    else
                                    {
                                        header = line.Split(separator);
                                        foreach (var ffID in formFieldsLabels)
                                        {
                                            int roster = FormField.isRoster((int)ffID.id);
                                            if (roster == -1)
                                            {
                                                string[] RepeatableCode = header[i].Split(' ');
                                                string tmpHeader = RepeatableCode[RepeatableCode.Length - 1].Substring(1, 2);
                                                string fileInZip = Path.GetFileNameWithoutExtension(fileNameMain) + "_" + tmpHeader;
                                                if (!checkIfExistsInZip(fileInZip, zip))
                                                {
                                                    Literal1.Text += fileInZip + " missing<br />";
                                                }
                                            }
                                            i++;
                                        }
                                        if (Literal1.Text != "")
                                        {
                                            Literal1.Text += "Import could not be done!";
                                            break;
                                        }
                                    }
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            if (Literal1.Text != "")
                                break;
                            Dictionary<string, int> mapRosterCount = new Dictionary<string, int>();
                            string[] value = entry.FileName.Split('_');
                            int repeatableLength = value[value.Length - 1].Length;
                            string roster = value[value.Length - 1].Substring(0, repeatableLength - 1);
                            string formFieldParentID = "";
                            int ffParentID = 0;
                            rosterFF.TryGetValue(roster, out formFieldParentID);

                            if (formFieldParentID != "" && formFieldParentID != null)
                            {
                                ffParentID = Convert.ToInt32(formFieldParentID);
                            }

                            var rosterFields = from i in db.FormFieldExport
                                               where i.FormFieldParentID == ffParentID
                                               orderby i.positionIndex ascending
                                               select i.id;

                            Stream s = entry.OpenReader();
                            StreamReader sr = new StreamReader(s);
                            int countR = 0;
                            bool isRoster = false;
                            using (var file = sr)
                            {
                                int j = 0;

                                if (roster.Contains('R'))
                                {
                                    j = 1;
                                    isRoster = true;
                                }
                                else
                                {
                                    j = 2;
                                    isRoster = false;
                                }

                                int rc = 1;
                                string line;
                                while ((line = file.ReadLine()) != null)
                                {
                                    int lengthValue = 0;
                                    if (isRoster)
                                        j = 1;
                                    else j = 2;

                                    if (countR != 0)
                                    {
                                        string[] responsesR = line.Split(separator);
                                        int fResID = 0;
                                        mapResID.TryGetValue(responsesR[0], out fResID);
                                        if (!mapRosterCount.ContainsKey(responsesR[0]))
                                        {
                                            mapRosterCount.Add(responsesR[0], 1);
                                            rc = 1;
                                        }
                                        else
                                        {
                                            mapRosterCount[responsesR[0]] = mapRosterCount[responsesR[0]] + 1;
                                            rc = mapRosterCount[responsesR[0]];
                                        }

                                        foreach (var ffID in rosterFields)
                                        {
                                            if (isRoster)
                                            {
                                                lengthValue = responsesR[j].Length;
                                                if (responsesR[j].Contains('"') && lengthValue > 0)
                                                    ResponseValue.createResponseValue(responsesR[j++].Substring(1, lengthValue - 2), fResID, (int)ffID, rc);
                                                else
                                                    ResponseValue.createResponseValue(responsesR[j++], fResID, (int)ffID, rc);
                                            }
                                            else
                                            {
                                                int tc = 0;
                                                mapSurList.TryGetValue(responsesR[0] + responsesR[1], out tc);
                                                lengthValue = responsesR[j].Length;
                                                if (responsesR[j].Contains('"') && lengthValue > 0)
                                                    ResponseValue.createResponseValue(responsesR[j++].Substring(1, lengthValue - 2), fResID, (int)ffID, tc);
                                                else
                                                    ResponseValue.createResponseValue(responsesR[j++], fResID, (int)ffID, tc);
                                            }
                                        }
                                        ResponseValue.setPositionIndex(fResID);
                                    }
                                    countR++;
                                }
                            }
                        }
                    }
                    if (Literal1.Text == "")
                        Literal1.Text = "Import correctly done.";
                }
            }
        }
    }
    /// <summary>
    /// Checks if there is an entry in the zip File with that name
    /// </summary>
    /// <param name="fileInZip">A string representing the filename</param>
    /// <param name="zip">The ZIP file</param>
    /// <returns>true if the file exists, false by default</returns>
    private bool checkIfExistsInZip(string fileInZip, ZipFile zip)
    {
        bool exists = false;
        foreach (ZipEntry entry in zip)
        {
            if (Path.GetFileNameWithoutExtension(entry.FileName).Equals(fileInZip))
            {
                exists = true;
            }
        }
        return exists;
    }
}