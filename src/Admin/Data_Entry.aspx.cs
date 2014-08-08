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
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Corresponds to the menu item Data Entry; it lists all the form in order to create new compiling
/// </summary>
public partial class Admin_Data_Entry : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    /// <summary>
    /// Populates the grid with all the forms and their properties
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ldsForm_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var forms = from f in db.Form
                    where f.finalised == 1
                    select new
                    {
                        Name = f.name,
                        CreateDate = f.FormCreateDate,
                        Owner = f.owner,
                        Group = f.permittedGroup_path,
                        Id = f.id
                        //Actions = "<a style=\"color:#0058B1\" href=\"/Admin/Data_Entry/DataEntry.aspx?formID=" + SqlFunctions.StringConvert(f.id).TrimStart() + "\"><i class=\"fa fa-pencil-square-o\"></i> New Data</a>"
                    };

        e.Result = forms.AsEnumerable().Select(x => new
        {
            Name = x.Name,
            CreateDate = x.CreateDate,
            Owner = x.Owner,
            Group = x.Group,
            Actions = "<a style=\"color:#0058B1\" href=\"DataEntryWebForm.aspx?formID=" + x.Id.ToString() + "\"><i class=\"fa fa-pencil-square-o\"></i> New Data</a>"
        });
    }
}