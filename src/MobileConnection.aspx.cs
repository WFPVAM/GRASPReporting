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
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
/// <summary>
/// Used to receive HTTP Requests from Mobile, fetch them and then save data on the DB
/// </summary>
public partial class MobileConnection : System.Web.UI.Page
{

    private string FixBase64ForImage(string Image)
    {
        System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);

        sbText.Replace("\r\n", String.Empty);

        sbText.Replace(" ", String.Empty);

        sbText.Replace('-', '+');

        sbText.Replace('_', '/');

        sbText.Replace(@"\/", "/");

        return sbText.ToString();
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        //string xml = "<?xml version=\"1.0\" encoding=\"UTF-\"?></forms>";
        //Console.Write(xml.Length.ToString());

        //string senderF = "+391111";
        //if (xml != "<?xml version=\"1.0\" encoding=\"UTF-\"?></forms>")
        //{
        //    XmlDocument xmlData = TryParseXml(xml);
        //    if (xmlData != null)
        //    {
        //        readXML(xmlData, senderF);
        //    }
        //}
        //else
        //{
        //    readXML(senderF);
        //}
        //file.WriteLine(text);

        string senderF = "";
        string data = "";
        string imei = "";

        System.Collections.Specialized.NameValueCollection postedValues = Request.Form;
        senderF = postedValues[0];
        data = Server.HtmlDecode(postedValues[1]);
        try
        {
            imei = Server.HtmlDecode(postedValues[2]);
        }
        catch(Exception ex)
        {
        }

        if(data == "test")
        {
            handleTestRequest(senderF);
        }
        else if(Request.QueryString["call"] != null)
        {
            string parameter = Request.QueryString["call"].ToString();
            switch(parameter)
            {
                case "test":
                    handleTestRequest(senderF);
                    break;
                case "response":
                    if(senderF == null || senderF == "")
                    {
                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.Write("ERROR:Client phone number not received");
                        break;
                    }
                    handleResponseRequest(data, senderF);
                    break;
                case "sync":
                    if(senderF == null || senderF == "")
                    {
                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.Write("ERROR:Client phone number not received");
                        break;
                    }
                    handleSyncRequest(data, senderF);
                    break;
                default:
                    Response.Clear();
                    Response.ContentType = "text/plain";
                    Response.Write("Generating a request response generated an error");
                    break;
            }
        }
        try
        { }
        catch(Exception ex)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.Write("Generating a request response generated an error");
        }

    }

    private void handleTestRequest(string senderF)
    {
        Response.Clear();
        Response.ContentType = "text/plain";
        Response.Write(User_Credential.checkUserFromNumber(senderF));
    }

    private void handleResponseRequest(string data, string senderF)
    {
        Response.Clear();
        Response.ContentType = "text/plain";
        Response.Write(CreateZipFromText(data, senderF));
    }

    private void handleSyncRequest(string data, string sender)
    {
        data = data.Replace("\n", "").Replace("\r", "").Replace("  ", "");
        XmlDocument xmlData = TryParseXml(data);
        if(xmlData != null)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.Write(readXML(xmlData, sender));
        }
        else
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.Write("Generating a request response generated an error");
        }

    }

    private XmlDocument TryParseXml(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        catch(XmlException e)
        {
            return null;
        }
    }

    private string readXML(XmlDocument doc, string phone)
    {
        List<string> downloadedForms = new List<string>();
        XmlNodeList elemList = doc.GetElementsByTagName("form");
        for(int i = 0; i < elemList.Count; i++)
        {
            downloadedForms.Add(elemList[i].InnerXml);
        }
        string response = Utility.getFormForMobileByPhoneNumber(phone, downloadedForms);
        return response;
    }

    private string readXML(string phone)
    {
        List<string> downloadedForms = new List<string>();
        string response = Utility.getFormForMobileByPhoneNumber(phone, downloadedForms);
        return response;
    }

    private bool CreateZipFromText(string text, string sender)
    {


        byte[] encodedText = Convert.FromBase64String(text);

        Stream stream = new MemoryStream(encodedText);
        GZipStream a = new GZipStream(stream, CompressionMode.Decompress);
        MemoryStream output = new MemoryStream();

        stream.Position = 0;
        StreamReader sr = new StreamReader(a);
        string tmp = sr.ReadToEnd();
        int index = tmp.IndexOf("</data>?");
        string pat = tmp.Substring(0, index + 7);
        Debug.WriteLine(pat);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(pat);
        string xpath = "data";
        var nodes = xmlDoc.SelectNodes(xpath);
        //Dictionary<string, int> ffields = new Dictionary<string, int>();
        string[,] fieldTypeMapping = null;
        int fIDX = -1;
        int formParentID = 0;
        int formResponseID = 0;
        string tmpNm = "";
        string clientVersion = "";
        //int ffID = 0;
        int prevFFID = 0;
        int ffIdRoster = 0;
        int repCount = 0;
        bool previousRoster = false;
        try
        {
            using(GRASPEntities db = new GRASPEntities())
            {

                foreach(XmlNode childrenNode in nodes)
                {
                    foreach(XmlNode child in childrenNode.ChildNodes)
                    {
                        if(child.Name == "id")
                        {
                            formParentID = FormResponse.getFormID(child.InnerText);
                            formResponseID = FormResponse.createFormResponse(formParentID, sender, "");

                            //ffields = FormField.getFormFieldsID(formParentID);
                            fieldTypeMapping = FormField.getFormFieldTypeMap(formParentID); //idx= 0:name; 1:id; 2:type; 3:positionIndex

                            if(formResponseID == 0)
                                return false;
                        }
                        else if(child.Name.Contains('_'))
                        {
                            string[] tmpSplit = child.Name.Split('_');
                            tmpNm = tmpSplit[0];
                            if(tmpSplit.Length > 2)
                            {
                                for(int k = 1; k < tmpSplit.Length - 1; k++)
                                    tmpNm += "_" + tmpSplit[k];
                            }
                            //ffields.TryGetValue(tmpNm, out ffID);
                            fIDX = -1;
                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                            {
                                if(tmpNm == fieldTypeMapping[i, 0])
                                {
                                    fIDX = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //ffields.TryGetValue(child.Name, out ffID);
                            fIDX = -1;
                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                            {
                                if(tmpNm == fieldTypeMapping[i, 0])
                                {
                                    fIDX = i;
                                    break;
                                }
                            }
                        }

                        if(fIDX != -1)
                        {
                            //file.WriteLine(" --- " + fieldTypeMapping[fIDX, 0] + " | " + fieldTypeMapping[fIDX, 1] + " | " + fieldTypeMapping[fIDX, 2] + " ---");
                            switch(fieldTypeMapping[fIDX, 2])
                            {
                                case "REPEATABLES_BASIC":
                                case "REPEATABLES":

                                    //if(prevFFID == Convert.ToInt32(fieldTypeMapping[fIDX, 1]))
                                    if(prevFFID == fIDX)
                                    {
                                        repCount++;
                                        foreach(XmlNode rChild in child.ChildNodes)
                                        {
                                            //ffields.TryGetValue(rChild.Name, out ffIdRoster);  
                                            ffIdRoster = -1;
                                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                                            {
                                                if(rChild.Name == fieldTypeMapping[i, 0])
                                                {
                                                    ffIdRoster = i;
                                                    break;
                                                }
                                            }
                                            if(ffIdRoster != -1)
                                            {
                                                //file.WriteLine(rChild.InnerText + "," + formResponseID + "," + Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]) + "," + repCount.ToString());
                                                if(fieldTypeMapping[fIDX, 2] == "NUMERIC_TEXT_FIELD")
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount, "NUMERIC_TEXT_FIELD");

                                                }
                                                else
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        repCount = 1;
                                        foreach(XmlNode rChild in child.ChildNodes)
                                        {
                                            //ffields.TryGetValue(rChild.Name, out ffIdRoster);
                                            ffIdRoster = -1;
                                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                                            {
                                                if(rChild.Name == fieldTypeMapping[i, 0])
                                                {
                                                    ffIdRoster = i;
                                                    break;
                                                }
                                            }
                                            if(ffIdRoster != -1)
                                            {
                                                //file.WriteLine(rChild.InnerText + "," + formResponseID + "," + Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]) + "," + repCount.ToString());
                                                if(fieldTypeMapping[fIDX, 2] == "NUMERIC_TEXT_FIELD")
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount, "NUMERIC_TEXT_FIELD");

                                                }
                                                else
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount);
                                                }
                                            }
                                        }
                                    }

                                    //prevFFID = Convert.ToInt32(fieldTypeMapping[fIDX, 1]);
                                    prevFFID = fIDX;
                                    previousRoster = true;
                                    break;
                                case "IMAGE":
                                    //string folderPath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + formResponseID);

                                    //string stringValue = FixBase64ForImage(child.InnerText);
                                    //var bytes = Convert.FromBase64String(stringValue);

                                    //bool isExists = System.IO.Directory.Exists(folderPath);
                                    //if (!isExists)
                                    //    System.IO.Directory.CreateDirectory(folderPath);

                                    //using (var imageFile = new FileStream(folderPath + "\\" + child.Name + ".jpg", FileMode.Create))
                                    //{
                                    //    imageFile.Write(bytes, 0, bytes.Length);
                                    //    imageFile.Flush();
                                    //}

                                    //Added by Rushdi on 30-SEP-2014
                                    if(child.InnerText.Contains("/instances"))
                                    {
                                        child.InnerText = sender.Replace('+', ' ').Trim() + "\\" + child.InnerText.Split('/').Last();
                                    }
                                    //------------------------------

                                    string imagePthValue = Utility.GetImageFolderName() + "\\" + child.InnerText;
                                    ResponseValue.createResponseValue(db, imagePthValue, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                    break;

                                case "GEOLOCATION":
                                    if(!string.IsNullOrEmpty(child.InnerText))
                                    {
                                        ResponseValue.createResponseValue(db, child.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                        FormResponseCoord.createFormResponseCoord(child.InnerText, formResponseID);
                                    }
                                    break;
                                case "NUMERIC_TEXT_FIELD":
                                    ResponseValue.createResponseValue(db, child.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0, "NUMERIC_TEXT_FIELD");
                                    break;
                                default:
                                    if(previousRoster)
                                    {
                                        //file.WriteLine(repCount.ToString() + "," + formResponseID + "," + prevFFID + ", -1");
                                        ResponseValue.createResponseValue(db, repCount.ToString(), formResponseID, Convert.ToInt32(fieldTypeMapping[prevFFID, 1]), Convert.ToInt32(fieldTypeMapping[prevFFID, 3]), -1);
                                    }
                                    //file.WriteLine(child.InnerText + "," + formResponseID + "," + Convert.ToInt32(fieldTypeMapping[fIDX, 1]) + ", 0");
                                    string valueToInsert = "";
                                    if(child.InnerText != null && child.InnerText.Length > 4000)
                                    {
                                        valueToInsert = child.InnerText.Substring(0, 3999);
                                    }
                                    else
                                    {
                                        valueToInsert = child.InnerText;
                                    }
                                    ResponseValue.createResponseValue(db, valueToInsert, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                    if(tmpNm == "client_version")
                                        clientVersion = child.InnerText;
                                    prevFFID = fIDX;
                                    previousRoster = false;
                                    break;
                            }
                        }
                    }
                    if(previousRoster)
                    {
                        //file.WriteLine(repCount.ToString() + "," + formResponseID + "," + prevFFID + ", -1");
                        ResponseValue.createResponseValue(db, repCount.ToString(), formResponseID, Convert.ToInt32(fieldTypeMapping[prevFFID, 1]), Convert.ToInt32(fieldTypeMapping[prevFFID, 3]), -1);
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    string exLog = "";
                    if(ex is DbEntityValidationException)
                    {
                        DbEntityValidationException dbEx = (DbEntityValidationException)ex;
                        foreach(var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach(var validationError in validationErrors.ValidationErrors)
                            {
                                exLog += "Property: " + validationError.PropertyName + "  Error: " + validationError.ErrorMessage + "\r\n";
                            }
                        }
                    }
                    else
                    {
                        exLog = ex.Message + "\r\n" + ex.StackTrace;
                    }
                    string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
                    if(!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    file.WriteLine(text);
                    file.WriteLine("____________________________________________________________________________");
                    file.WriteLine("------ ERROR " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    file.WriteLine(exLog);
                    file.WriteLine("____________________________________________________________________________");
                    file.Close();

                    db.Database.ExecuteSqlCommand("DELETE FormResponse WHERE id=" + formResponseID);

                    return false;
                }

                FormResponse.updateClientVersion(formResponseID, clientVersion);
                Index.GenerateIndexesHASH(formParentID, formResponseID);
                ServerSideCalculatedField.GenerateSingle(formParentID, formResponseID);
                UserToFormResponses.GenerateAssociationForAllUsers(formParentID, formResponseID);
                return true;
            }
        }
        catch(Exception ex)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
            file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            file.WriteLine(text);
            file.WriteLine("____________________________________________________________________________");
            file.WriteLine("------ ERROR " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            file.WriteLine(ex.Message);
            file.WriteLine(ex.StackTrace);
            file.WriteLine("____________________________________________________________________________");
            file.Close();
            return false;
        }
        finally
        {
            //file.Close();
        }
    }
}