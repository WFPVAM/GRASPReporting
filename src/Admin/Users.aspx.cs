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
using Telerik.Web.UI;
/// <summary>
/// Corresponds to the menu item Users
/// </summary>
public partial class Admin_Users : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if(Utility.VerifyAccess(Request))
        {
        }
        else
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }
    }
    /// <summary>
    /// Fills the grid with the users with specified roles: SuperAdministrator, Supervisor, DataEntryOperator, Analyst
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ldsRoles_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var roles = from r in db.Roles
                    where r.id == 3 || r.id == 4 || r.id == 7 || r.id >= 8
                    select r;

        e.Result = roles;
    }
    protected void rgUser_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        foreach(GridColumn col in rgUser.Columns)
        {
            if(col.UniqueName == "email" || col.UniqueName == "password" || col.UniqueName == "phone_number")
            {
                if(e.CommandName == RadGrid.EditCommandName)
                {
                    col.Visible = true;
                }
                else
                {
                    col.Visible = false;
                }
            }
        }  
    }
}