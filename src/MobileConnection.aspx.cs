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
        try
        {
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
            catch (Exception ex)
            {
            }

            if (data == "test")
            {
                handleTestRequest(senderF);
            }
            else if (Request.QueryString["call"] != null)
            {
                string parameter = Request.QueryString["call"].ToString();
                switch (parameter)
                {
                    case "test":
                        handleTestRequest(senderF);
                        break;
                    case "response":
                        if (senderF == null || senderF == "")
                        {
                            Response.Clear();
                            Response.ContentType = "text/plain";
                            Response.Write("ERROR:Client phone number not received");
                            break;
                        }
                        handleResponseRequest(data, senderF);
                        break;
                    case "sync":
                        if (senderF == null || senderF == "")
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
        }
        catch (Exception ex)
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
        if (xmlData != null)
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
        catch (XmlException e)
        {
            return null;
        }
    }

    private string readXML(XmlDocument doc, string phone)
    {
        List<string> downloadedForms = new List<string>();
        XmlNodeList elemList = doc.GetElementsByTagName("form");
        for (int i = 0; i < elemList.Count; i++)
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
        //Response.Write("Entrato in createZip: " + text + "\n");
        //string path = Server.MapPath("mobile.txt");
        //StreamWriter file = new StreamWriter(path);
        //file.WriteLine(text);
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
        Dictionary<string, int> ffields = new Dictionary<string, int>();
        int formParentID = 0;
        int formResponseID = 0;
        string tmpNm = "";
        string clientVersion = "";
        int ffID = 0;
        int prevFFID = 0;
        int ffIdRoster = 0;
        int i = 0;
        bool previousRoster = false;

        foreach (XmlNode childrenNode in nodes)
        {
            foreach (XmlNode child in childrenNode.ChildNodes)
            {
                if (child.Name == "id")
                {
                    formParentID = FormResponse.getFormID(child.InnerText);
                    ffields = FormField.getFormFieldsID(formParentID);
                    formResponseID = FormResponse.createFormResponse(formParentID, sender, "");
                    if (formResponseID == 0)
                        return false;
                }
                else if (child.Name.Contains('_'))
                {
                    string[] tmpSplit = child.Name.Split('_');
                    tmpNm = tmpSplit[0];
                    if (tmpSplit.Length > 2)
                    {
                        for (int k = 1; k < tmpSplit.Length - 1; k++)
                            tmpNm += "_" + tmpSplit[k];
                    }
                    ffields.TryGetValue(tmpNm, out ffID);

                }
                else
                {
                    ffields.TryGetValue(child.Name, out ffID);
                }

                if (ffID != 0 && ffID != null)
                {
                    int count = FormField.isRoster(ffID);
                    if (count == -1)
                    {

                        if (prevFFID == ffID)
                        {
                            i++;
                            foreach (XmlNode rChild in child.ChildNodes)
                            {
                                ffields.TryGetValue(rChild.Name, out ffIdRoster);
                                if (ffIdRoster != 0 && ffIdRoster != null)
                                {
                                    //file.WriteLine(rChild.InnerText + "," + formResponseID + "," + ffIdRoster + "," + i.ToString());
                                    ResponseValue.createResponseValue(rChild.InnerText, formResponseID, ffIdRoster, i);
                                }
                            }
                        }
                        else
                        {
                            i = 1;
                            foreach (XmlNode rChild in child.ChildNodes)
                            {
                                ffields.TryGetValue(rChild.Name, out ffIdRoster);
                                if (ffIdRoster != 0 && ffIdRoster != null)
                                {
                                    //file.WriteLine(rChild.InnerText + "," + formResponseID + "," + ffIdRoster + "," + i.ToString());
                                    ResponseValue.createResponseValue(rChild.InnerText, formResponseID, ffIdRoster, i);
                                }
                            }
                        }

                        prevFFID = ffID;
                        previousRoster = true;

                    }
                    else
                    {
                        if (previousRoster)
                        {
                            //file.WriteLine(i.ToString() + "," + formResponseID + "," + prevFFID + ", -1");
                            ResponseValue.createResponseValue(i.ToString(), formResponseID, prevFFID, -1);
                        }
                        //file.WriteLine(child.InnerText + "," + formResponseID + "," + ffID + ", 0");
                        ResponseValue.createResponseValue(child.InnerText, formResponseID, ffID, 0);
                        if (tmpNm == "client_version")
                            clientVersion = child.InnerText;
                        prevFFID = ffID;
                        previousRoster = false;

                    }
                }
            }
            if (previousRoster)
            {
                //file.WriteLine(i.ToString() + "," + formResponseID + "," + prevFFID + ", -1");
                ResponseValue.createResponseValue(i.ToString(), formResponseID, prevFFID, -1);
            }
        }
        //file.Close();
        ResponseValue.setPositionIndex(formResponseID);
        FormResponse.updateClientVersion(formResponseID, clientVersion);
        return true;

    }
}