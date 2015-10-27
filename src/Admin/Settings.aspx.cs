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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
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
                FillDdlRoles();
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
    protected void TsSettingsMenu_TabClick(object sender, Telerik.Web.UI.RadTabStripEventArgs e)
    {
        int tabIdx = e.Tab.Index;
        switch(tabIdx)
        {
            case 0:
                //editor
                break;
            case 1:
                FillDdlRoles();
                break;
        }
    }

    protected void FillDdlRoles()
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            var roles = (from r in db.Roles
                        select r).OrderBy(o=>o.description);
            DdlRoles.DataSource = roles.ToList();
            DdlRoles.DataBind();
        }
    }

    protected void FillCblReviewStatus(int roleID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            var status = (from r in db.FormResponseStatus
                         select r).OrderBy(o => o.ResponseStatusName);

            CblSelectableStatus.DataSource = status.ToList();
            CblSelectableStatus.DataBind();
            CblReviewableStatus.DataSource = status.ToList();
            CblReviewableStatus.DataBind();
            cblPermissions.DataSource = Permissions.GetAllPermissionses();
            cblPermissions.DataBind();

            var rolesToStatus = (from rs in db.RolesToResponseStatus
                                where rs.RoleID == roleID
                                select rs).ToList();

            foreach(ListItem li in CblSelectableStatus.Items)
            {
                int rsID = Convert.ToInt32(li.Value);
                if(rolesToStatus.Where(w => w.RoleToRespStatusTypeID==1 && w.ResponseStatusID == rsID).Count() > 0)
                {
                    li.Selected = true;
                }
            }

            foreach(ListItem li in CblReviewableStatus.Items)
            {
                int rsID = Convert.ToInt32(li.Value);
                if(rolesToStatus.Where(w => w.RoleToRespStatusTypeID == 2 && w.ResponseStatusID == rsID).Count() > 0)
                {
                    li.Selected = true;
                }
            }

            //Select the permission check boxes of the selected role.
            List<Role_Permissions> rolePermissions = Permissions.GetRolePermissionsByRoleID(roleID);
            foreach (ListItem li in cblPermissions.Items)
            {
                int permissionID = Convert.ToInt32(li.Value);
                if (rolePermissions.Exists(p => p.PermissionID == permissionID))
                {
                    li.Selected = true;
                }
            }
        }
    }

    protected void DdlRoles_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        int roleID = Convert.ToInt32(DdlRoles.SelectedValue);
        FillCblReviewStatus(roleID);
        PnlRoleReviewAssociation.Visible = true;
        LblMessage.Text = "";
    }

    protected void BtnSaveRoleChanges_Click(object sender, EventArgs e)
    {
        LblMessage.Text = "";
        int roleID = Convert.ToInt32(DdlRoles.SelectedValue);
        using(GRASPEntities db = new GRASPEntities())
        {
            db.Database.ExecuteSqlCommand("DELETE FROM RolesToResponseStatus WHERE RoleID=@roleID", new SqlParameter("@roleID", roleID));

            foreach(ListItem li in CblSelectableStatus.Items)
            {
                if(li.Selected)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO RolesToResponseStatus (RoleID,ResponseStatusID,RoleToRespStatusTypeID) VALUES (@roleID,@respStatusID,1)",
                        new SqlParameter("@roleID", roleID), new SqlParameter("@respStatusID", li.Value));
                }
            }

            foreach(ListItem li in CblReviewableStatus.Items)
            {
                if(li.Selected)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO RolesToResponseStatus (RoleID,ResponseStatusID,RoleToRespStatusTypeID) VALUES (@roleID,@respStatusID,2)",
                        new SqlParameter("@roleID", roleID), new SqlParameter("@respStatusID", li.Value));
                }
            }

            //Save the new added permissions and remove the removed permissions.
            List<Role_Permissions> previousRolePermissionses = Permissions.GetRolePermissionsByRoleID(roleID);            
            List<Role_Permissions> newRolePermissionses = new List<Role_Permissions>();
            foreach (ListItem li in cblPermissions.Items)
            {
                if (li.Selected)
                {
                    Role_Permissions rolePermission = new Role_Permissions() {RoleID=roleID, PermissionID = Convert.ToInt32(li.Value)};
                    if (previousRolePermissionses.Exists(rp => rp.PermissionID == rolePermission.PermissionID))
                    {
                        int deletedItemIndex =
                            previousRolePermissionses.FindIndex(rp => rp.PermissionID == rolePermission.PermissionID);
                        previousRolePermissionses.RemoveAt(deletedItemIndex);
                    }
                    else
                    {
                        newRolePermissionses.Add(rolePermission);   
                    }
                }
            }

            bool isSaved = Permissions.UpdateRolePermissions(newRolePermissionses, previousRolePermissionses);

            if (isSaved)
            {
                LblMessage.ForeColor = Color.Green;
                LblMessage.Text = "Changes have been saved successfully.";
            }
            else
            {
                LblMessage.ForeColor = Color.Red;
                LblMessage.Text = "Error happened, changes have not saved.";
            }

        }
    }
}