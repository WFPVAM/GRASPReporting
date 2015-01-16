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
        string senderF = "";
        string data = "";
        string imei = "";
        string formName = string.Empty;

        System.Collections.Specialized.NameValueCollection postedValues = Request.Form;
        senderF = postedValues[0];
        data = Server.HtmlDecode(postedValues[1]);

        try
        {
            imei = Server.HtmlDecode(postedValues[3]);
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
                    formName = postedValues[2];
                    if(senderF == null || senderF == "")
                    {
                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.Write("ERROR:Client phone number not received");
                        break;
                    }
                    handleResponseRequest(data, senderF, formName);
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

    private void handleResponseRequest(string data, string senderF, string formName)
    {
        Response.Clear();
        Response.ContentType = "text/plain";
        IncomingProcessor incomProc = new IncomingProcessor();
        //Response.Write(incomProc.ProcessResponse(data, senderF));
        Response.Write(incomProc.SaveFileResponse(data, senderF, formName));
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


}