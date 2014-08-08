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
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Home Page
/// </summary>
public partial class Admin_Dashboard : System.Web.UI.Page
{
    /// <summary>
    /// When HomePage loads APK informations are taken to display in the panel.
    /// Other informations are taken from the file created in the page settings.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string path = Server.MapPath("~/public/GraspMobile.apk");
        FileInfo fi1 = new FileInfo(path);

        if (fi1.Exists)
        {
            ltrlInfo.Text = "Last Update: <i>" + fi1.LastWriteTime.ToString("dd MMMM yyyy") + "</i>";
            long Size = fi1.Length;
            double dim = (Convert.ToDouble(Size) / 1024f) / 1024f;
            ltrlInfo.Text += "<br />Dimension: <i>" + dim.ToString("N2") + " MB</i>";

        }

        string pathFileInfo = Server.MapPath("../Public/InfoHP.txt");
        if (File.Exists(pathFileInfo))
        {
            using (StreamReader sr = new StreamReader(pathFileInfo))
            {
                Literal1.Text = sr.ReadToEnd();
            }
        }
    }
}