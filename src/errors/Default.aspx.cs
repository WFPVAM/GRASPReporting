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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;
using System.Text;

public partial class errors_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder errorInfo = new StringBuilder();
        //string errorInfo = "";
        //Exception exception = Server.GetLastError();
        Exception objErr = Server.GetLastError().GetBaseException();
        if (objErr != null)
        {
            //litStack.Text = " Stack trace: <pre>" + objErr.StackTrace + "</pre>";

            StackTrace st = new StackTrace(new StackFrame(true));
            StackFrame sf = st.GetFrame(0);

            errorInfo.Append(
               "\r\n<b>URL:</b> \r\n" + Request.Url.ToString() +
               "\r\n\r\n<b>Source:</b> \r\n" + objErr.Source +
               "\r\n\r\n<b>Error Message:</b> \r\n" + objErr.Message +
               "\r\n\r\n<b>Stack Trace:</b> \r\n" + objErr.StackTrace +
                "\r\n\r\nFile Name: " + sf.GetFileName().ToString() + "\r\n" +
                "Method Name: " + sf.GetMethod().Name.ToString() + "\r\n" +
                "Error Line Number: " + (sf.GetFileLineNumber() - 1) + "\r\n" +    // line numbers are offset by 1 for some reason
                "Error Column Number: " + sf.GetFileColumnNumber().ToString() + "\r\n");

            try
            {
                int loop1, loop2;
                NameValueCollection coll;

                // Load ServerVariable collection into NameValueCollection object.
                coll = Request.ServerVariables;
                // Get names of all keys into a string array. 
                String[] arr1 = coll.AllKeys;
                for (loop1 = 0; loop1 < arr1.Length; loop1++)
                {
                    errorInfo.Append("\r\nKey: " + arr1[loop1] + "\r\n");
                    String[] arr2 = coll.GetValues(arr1[loop1]);
                    for (loop2 = 0; loop2 < arr2.Length; loop2++)
                    {
                        errorInfo.Append("Value " + loop2 + ": " + Server.HtmlEncode(arr2[loop2]) + "\r\n");
                    }
                }

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    errorInfo.Append("\r\n User: " + HttpContext.Current.User.Identity.Name);
                }
            }
            catch
            {
            }

            Server.ClearError();

            string path = Server.MapPath("~/ErrorLog.txt");
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
                sw.WriteLine("Datetime: " + DateTime.Now + "\r\n\r\n" + errorInfo.ToString());
                sw.WriteLine("\r\n\r\n ------ New Error ------- \r\n\r\n");
            }
            txtError.Text = errorInfo.Replace("\r\n","<br />").ToString();
        }
        //Response.Redirect("~/");
    }
}