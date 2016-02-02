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
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;

/// <summary>
/// Utility class contains auxiliary functions
/// </summary>
public class Utility
{
    public static string GetVideoFileNameSeparator()
    {
        return "%";
    }

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
        if(input != null)
        {
            res = Int32.TryParse(input, out v);
            if(!res)
                res = Double.TryParse(input, out w);
        }
        return res;
    }

    public static string getVersion()
    {
        return "1.3.0";
    }

    private static bool IsGRASPImagesFolderNotUnderReportingFolder()
    {
        if (ConfigurationManager.AppSettings["IsGRASPImagesFolderNotUnderReportingFolder"].ToLower().Equals("no"))
            return false;
        else
            return true;
    }

    //public static bool SetWebConfigProperty(string propertyKey, string propertyValue)
    //{
    //    try
    //    {
    //        System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

    //        System.Configuration.KeyValueConfigurationElement setting = config.AppSettings.Settings["MyValue"];

    //        config.AppSettings.Settings. ["MyValue"].Value 
    //        if (null != setting)
    //        {
    //            config.AppSettings.Settings["MyValue"].Value = textboxValue.Text;
    //        }
    //        else
    //        {
    //            config.AppSettings.Settings.Add("MyValue", textboxValue.Text);
    //        }

    //        config.Save();
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}

    /// <summary>
    /// Gets the path of the images folder.
    /// </summary>
    /// <returns></returns>
    public static string GetImagesFolderPath()
    {
        if (!IsGRASPImagesFolderNotUnderReportingFolder())
        {
            return ConfigurationManager.AppSettings["GRASPReportingPath"];
        }
        else
            return ConfigurationManager.AppSettings["GRASPImagesFolderPath"];
    }

    /// <summary>
    /// Gets the image's folder name.
    /// </summary>
    /// <returns></returns>
    public static string GetImagesFolderName()
    {
        if (!IsGRASPImagesFolderNotUnderReportingFolder())
        {
            return "GRASPImage";
        }
        else 
            return ConfigurationManager.AppSettings["ImageFolderName"];
    }

    public static string GetGRASPImagesVirtualDirectory()
    {
        if (!IsGRASPImagesFolderNotUnderReportingFolder())
            return ConfigurationManager.AppSettings["GRASPReportingVirtualDirectoryName"] + "\\";
        else //GRASPImages has it's own virtual directory.
            return string.Empty; 
    }

    public static string GetGRASPReportingPath()
    {
        return ConfigurationManager.AppSettings["GRASPReportingPath"];
    }

    public static string GetGRASPReportingVirtualDirectoryName()
    {
        return ConfigurationManager.AppSettings["GRASPReportingVirtualDirectoryName"];
    }

    public static string GetResponseFilesFolderName()
    {
        return ConfigurationManager.AppSettings["ResponseFilesFolderPath"];
    }

    public static string GetImagesFolderFullPath()
    {
        return GetImagesFolderPath() + GetImagesFolderName();
    }

    public static string GetFilePathSeparator()
    {
        return "\\";
    }

    public static string GetImageFileType()
    {
        try
        {
            string imageFileType = ConfigurationManager.AppSettings["ImagesFileType"];
            if (!string.IsNullOrEmpty(imageFileType))
            {
                if (imageFileType.StartsWith("."))
                {
                    return imageFileType;
                }
                else
                    return "." + imageFileType;
            }
            else //Default type.
                return ".jpg";
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.Message);
            return null;
        }

    }

    /// <summary>
    /// Gets sender number without +
    /// </summary>
    /// <returns></returns>
    public static string GetSenderNumber(string senderNumber)
    {
        return senderNumber.Replace("+", "");
    }

    public static bool existingForm(string id)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.Form
                    where f.id_flsmsId == id
                    select f).FirstOrDefault();

        if(item != null)
            return true;
        else return false;
    }

    public static string generateNameBind(string expr)
    {
        int beginCut = expr.LastIndexOf("/");
        string name = expr.Substring(beginCut + 1);
        return name;
    }

    public static void SaveErrorResponse(string data, string fileName)
    {
        string folderPath = Utility.GetResponseFilesFolderName() + "error\\";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        using (StreamWriter file = new StreamWriter(folderPath + fileName, true))
        {
            file.Write(data);
            file.Close();
        }
    }

    /// <summary>
    /// 
    /// Example of usage: <a href='Page1.aspx?UserID=<%= HttpUtility.UrlEncode(TamperProofStringEncode("5","F44fggjj")) %>'> Click Here</a>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string TamperProofStringEncode(string value, string key)
    {
        System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)) + "-" + Convert.ToBase64String(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
    }

    public string TamperProofStringDecode(string value, string key)
    {
        string dataValue = "";
        string calcHash = "";
        string storedHash = "";

        System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));

        try
        {
            dataValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split('-')[0]));
            storedHash = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split('-')[1]));
            calcHash = System.Text.Encoding.UTF8.GetString(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataValue)));

            if(storedHash != calcHash)
            {
                //'Data was corrupted
                throw new ArgumentException("Hash value does not match");
                //  'This error is immediately caught below

            }
        }
        catch(Exception ex)
        {
            throw new ArgumentException("Invalid TamperProofString");
        }

        return dataValue;

    }

    public static bool VerifyAccess(HttpRequest request)
    {

        string loggedUser = HttpContext.Current.User.Identity.Name.ToString().ToUpper();
        string roleName = User_Credential.getRoleForUser(loggedUser);
        bool canAccess = false;

        string p = request.Path;
        int startIdx = p.LastIndexOf('/', p.LastIndexOf('/') - 1);
        if(startIdx > 0)
        {
            p = p.Substring(startIdx);
        }
        switch(p.ToLower())
        {
            case "/admin/viewformresponses.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/calculatedfieldinsert.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/checkduplicates.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/compareresponse.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/customfilters.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/data_entry.aspx":
                if((!roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/dataeditwebform.aspx":
                if((!roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/exportdata.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/userfilter.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/users.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/settings.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/surveys.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/indexmanagement.aspx":
                if((roleName != "DataEntryOperator" && !roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/viewform.aspx":
                if((roleName != "DataEntryOperator"))
                    canAccess = true;
                break;
            case "/admin/dataentrywebform.aspx":
                if((!roleName.StartsWith("Reviewer") && roleName != "Reader"))
                    canAccess = true;
                break;
            case "/admin/reviewrestore.aspx":
                if((!roleName.StartsWith("Reviewer") && roleName != "Reader" && roleName != "DataEntryOperator"))
                    canAccess = true;
                break;

        }

        
        return canAccess;

    }

    /// <summary>
    /// Checks whether the given string represents a decimal number or integer, for example if the string is 1.0 it returns 1 (without .0) but if it is 1.5 it returns 1.5 . 
    /// </summary>
    /// <param name="stringNumber">A string represents a number.</param>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    public static string GetIntegerNumberFromString(string stringNumber)
    {
        try
        {
            string decimalNumber = stringNumber.GetSubstringAfterLastChar('.'); //"1287543.0" will return false for a long 
            if (!string.IsNullOrEmpty(decimalNumber))
            {
                int numberAfterPoint = 0;
                bool canConvert = int.TryParse(decimalNumber, out numberAfterPoint);
                if (canConvert)
                    if (numberAfterPoint == 0)
                        stringNumber = stringNumber.GetSubstringBeforeLastChar('.');
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return stringNumber;
    }

    public static string getres()
    {
        object obj = HttpContext.GetLocalResourceObject("~/Admin/ViewChart.aspx" ,"Nofilters.Text");
        //ResourceManager rm = new ResourceManager("ViewChart.aspx.resx",
        //    Assembly.GetExecutingAssembly());


        // Retrieve the value of the string resource named "welcome".
        // The resource manager will retrieve the value of the  
        // localized resource using the caller's current culture setting.
        //String str = rm.GetString("Nofilters");
        return obj != null ? obj.ToString() : "";
    }

    public static XmlDocument TryParseXml(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        catch (XmlException e)
        {
            LogUtils.WriteErrorLog(e.ToString());
            return null;
        }
    }
}