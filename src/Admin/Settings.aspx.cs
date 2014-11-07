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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Corresponds to the menu item Settings
/// </summary>
public partial class Settings : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads the RadEditor is filled with the text saved in the file InfoHP.txt
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Utility.VerifyAccess(Request))
        {
            if(!IsPostBack)
            {
                string pathFileInfo = Server.MapPath("../Public/InfoHP.txt");
                if(File.Exists(pathFileInfo))
                {
                    using(StreamReader sr = new StreamReader(pathFileInfo))
                    {
                        RadEditor1.Content = sr.ReadToEnd();
                    }
                }
            }
        }
        else
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }

    }
    /// <summary>
    /// Saves the content of the RadEditor in the file InfoHP.txt
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void saveInfo_Click(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(RadEditor1.Content);
        using(StreamWriter outfile = new StreamWriter(Server.MapPath("../Public/InfoHP.txt")))
        {
            outfile.Write(sb.ToString());
        }
    }
}