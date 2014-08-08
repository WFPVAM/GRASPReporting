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
/// <summary>
/// Master Page used to show menu and header
/// </summary>
public partial class _master_Admin : System.Web.UI.MasterPage
{
    public string LoggedUser = "";
    public string RoleUser = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        lblVersion.Text = Utility.getVersion();
        LoggedUser = HttpContext.Current.User.Identity.Name.ToString().ToUpper();
        RoleUser = User_Credential.getRoleForUser(LoggedUser);
        if (RoleUser == "DataEntryOperator")
        {
            settings.Visible = false;
            maps.Visible = false;
            users.Visible = false;
            reports.Visible = false;
        }
        else if (RoleUser == "Supervisor" || RoleUser == "Analyst")
        {
            settings.Visible = false;
            users.Visible = false;
        }
        else
        {
            settings.Visible = true;
            maps.Visible = true;
            users.Visible = true;
            reports.Visible = true;
        }
    }
    protected void exit_Click(object sender, EventArgs e)
    {
        System.Web.Security.FormsAuthentication.SignOut();
    }
}
