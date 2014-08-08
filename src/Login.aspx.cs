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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Login Page
/// </summary>
public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblVersion.Text = Utility.getVersion();
    }
    protected void login_LoggedIn(object sender, EventArgs e)
    {
        Response.Redirect("Admin/Home_Page.aspx", true);
        string user = User_Credential.getNameForUser(login.UserName);
        Session["role"] = User_Credential.getRoleForUser(login.UserName);
        //string userRoleName = sRes[0];

        //Session["auth"] = "1";
        //Session["uLevel"] = sRes[0];

        afterLogin(login.UserName, user);
    }

    private void afterLogin(string username, string user)
    {
        Debug.WriteLine("user name: " + user);
        //UsersRolesDSTableAdapters.UsersTableAdapter userTA =
        //       new UsersRolesDSTableAdapters.UsersTableAdapter();
        //UsersRolesDS.UsersDataTable userDT =
        //    new UsersRolesDS.UsersDataTable();

        //userDT = userTA.GetDataByUserEmail(username);
        //DataTableReader objDTReader = userDT.CreateDataReader();

        //objDTReader.Read();

        //if (objDTReader.HasRows)
        //{
        //    Session["uEmail"] = objDTReader["userEmail"].ToString();
        //    Session["uClient"] = objDTReader["clientRagSoc"].ToString();
        //    Session["uLastAccess"] = objDTReader["userLastAccess"].ToString();
        //    Session["uRoleName"] = objDTReader["roleName"].ToString();
        //    Session["uID"] = objDTReader["userID"].ToString();

        //    Application["OnlineUsersName"] = Application["OnlineUsersName"] + " - " + Session["uEmail"];
        //}

        //userTA.UpdateAccess(DateTime.Now, username);

        //string roleID = Session["roleID"].ToString();
        //if (userRoleName == "Cliente")
        //{
        //    if (Request.Url.ToString().ToLower().Contains("manutenzione"))
        //        Response.Redirect("Customer/ManPro", true);
        //    else
        //        Response.Redirect("Customer/", true);
        //    //Response.Redirect("~/Customer", true);
        //}
        //if (userRoleName == "OperatoreGRIweb") Response.Redirect("~/Operator/?r=gw", true);
        //if (userRoleName == "OperatoreManPro") Response.Redirect("~/Operator/?r=mp", true);
        //if (userRoleName == "Amministratore")
        //{
        //    if (Request.Url.ToString().ToLower().Contains("manutenzione"))
        //        Response.Redirect("Admin/ManPro", true);
        //    else
        //        Response.Redirect("Admin/", true);
        //}
    }
}