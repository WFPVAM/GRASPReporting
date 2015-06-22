using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for LogUtils
/// </summary>
public class LogUtils
{
    public static Guid WriteErrorLog(string errorInfo)
    {
        Guid errorID = Guid.NewGuid();
        string path = HttpContext.Current.Server.MapPath("~/LogFiles/ErrorLog.txt");
        if (!File.Exists(path))
        {
            // Create a file to write to. 
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("");
            }
        }
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine("\r\n ------ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Error ID: " + errorID.ToString() + " ------- \r\n");
            sw.WriteLine(errorInfo + "\r\n\r\n");
        }

        return errorID;
    }

    public static void WriteFileErrorLog(Exception ex, string fileName, string fileContent)
    {
        string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
        file.WriteLine("____________________________________________________________________________");
        file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error on " + fileName);
        if (ex != null)
        {
            file.WriteLine(ex.Message);
            file.WriteLine(ex.StackTrace);
        }
        file.WriteLine("____________________________________________________________________________");
        file.Close();

        Utility.SaveErrorResponse(fileContent, fileName);
        if (fileName.Length > 0)
        {
            File.Delete(Utility.GetResponseFilesFolderName() + "incoming\\" + fileName);
        }
    }
}