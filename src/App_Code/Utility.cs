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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;

/// <summary>
/// Utility class contains auxiliary functions
/// </summary>
public class Utility
{
    /// <summary>
    /// Checks if a string is a numeric field
    /// </summary>
    /// <param name="input">A string representing an hypothetical number</param>
    /// <returns>true if is numeric, false otherwise</returns>
    public static bool isNumeric(string input)
    {
        bool res = false;
        int v;
        double w;
        if (input != null)
        {
            res = Int32.TryParse(input, out v);
            if (!res)
                res = Double.TryParse(input, out w);
        }
        return res;
    }

    public static string getVersion()
    {
        return "1.0.6";
    }

    public static string getFormForMobileByPhoneNumber(string phone, List<string> downloadedForms)
    {
        //XmlDocument sv = new XmlDocument();
        //XmlElement rootNode = sv.CreateElement("forms");
        string xmlForms = "";
        //XmlDeclaration xmldecl;
        //xmldecl = sv.CreateXmlDeclaration("1.0", "UTF-8", "no");
        
        StringBuilder sb = new StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><forms>");

        GRASPEntities db = new GRASPEntities();

        var groupMembership = (from gm in db.GroupMembership
                               where gm.contact_contact_id == (from c in db.Contact
                                                               where c.phoneNumber == phone
                                                               select c.contact_id).FirstOrDefault()
                               select gm.group_path).FirstOrDefault();

        if (groupMembership != null)
        {
            Dictionary<string, decimal> forms = (from f in db.Form
                                                 where f.permittedGroup_path == groupMembership && f.finalised == 1
                                                 select new { f.id_flsmsId, f.id }).ToDictionary(p => p.id_flsmsId, p => p.id);

            if (forms != null)
            {
                foreach (string dF in downloadedForms)
                {
                    if (forms.ContainsKey(dF))
                    {
                        forms.Remove(dF);
                    }
                }

                foreach (var rF in forms)
                {
                    //XmlElement formNode = sv.CreateElement("form");
                    
                    sb.Append(FormField.getX_Form((int)rF.Value));
                    //formNode.InnerText = sb.ToString().Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                    //rootNode.AppendChild(formNode);
                    
                }
            }
        }
        //sv.AppendChild(rootNode);
        //sv.InsertBefore(xmldecl, rootNode);
        //using (var stringWriter = new StringWriter())
        //using (var xmlTextWriter = XmlWriter.Create(stringWriter))
        //{
        //    sv.WriteTo(xmlTextWriter);
        //    xmlTextWriter.Flush();
        //    xmlForms = stringWriter.GetStringBuilder().ToString();
        //}
        sb.Append("</forms>");
        xmlForms = sb.ToString();
        return xmlForms;

    }

    public static bool existingForm(string id)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.Form
                    where f.id_flsmsId == id
                    select f).FirstOrDefault();

        if (item != null)
            return true;
        else return false;
    }

    public static string generateNameBind(string expr)
    {
        int beginCut = expr.LastIndexOf("/");
        string name = expr.Substring(beginCut + 1);
        return name;
    }

    public static void writeErrorLog(string errorInfo)
    {
        string path = HttpContext.Current.Server.MapPath("~/ErrorLog.txt");
        if(!File.Exists(path))
        {
            // Create a file to write to. 
            using(StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("");
            }
        }
        using(StreamWriter sw = File.AppendText(path))
        {

            sw.WriteLine("\r\n ------ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : Error ------- \r\n");
            sw.WriteLine(errorInfo + "\r\n\r\n");
        }
        
    }


}