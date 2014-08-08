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
/// Corresponds to the menu item Surveys; it lists all the forms in order to import, export or view its compiling
/// </summary>
public partial class Admin_Surveys : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// Fills the grid with all the forms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ldsForm_Selecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        var forms = from f in db.Form
                    join fr in db.FormResponse on f.id equals fr.parentForm_id into j1
                    from j2 in j1.DefaultIfEmpty()
                    where f.finalised == 1
                    group j2 by new { f.id, f.name, f.FormCreateDate, f.owner, f.permittedGroup_path} into g
                    select new
                    {
                        Name = g.Key.name,
                        CreateDate = g.Key.FormCreateDate,
                        Owner = g.Key.owner,
                        Group = g.Key.permittedGroup_path,
                        Count = g.Count(t=>t.id !=null),
                        Id = g.Key.id
                    };

        e.Result = forms.AsEnumerable().Select(x => new
        {
            Name = x.Name,
            CreateDate = x.CreateDate.Value.ToString("dd MMM yyyy"),
            Owner = x.Owner,
            Group =x.Group,
            Responses = x.Count,
            Id = x.Id
            //Actions = (x.Count>0) ?"<a style=\"color:#0058B1\" href=\"javascript:ImportForm('" + x.Id.ToString() + "','" + x.Name + "');void(0);\"><i class=\"fa fa-upload fa-2\"></i>Import</a>"+ 
            //" <a style=\"margin-left: 5px;color:#0058B1\" href=\"javascript:ExportSettings('" + x.Id.ToString() + "','" + x.Name + "');void(0);\"><i class=\"fa fa-download fa-2\"></i>Export</a>"+
            //" <a style=\"margin-left: 5px;color:#0058B1\" href=\"javascript:ViewForm('" + x.Id.ToString() + "','" + x.Name + "');void(0);\"><i class=\"fa fa-eye fa-2\"></i>View</a>" : "<a style=\"color:#0058B1\" href=\"javascript:ImportForm('" + x.Id.ToString() + "','" + x.Name + "');void(0);\"><i class=\"fa fa-upload fa-2\"></i>Import</a>"
        });
    }
    /// <summary>
    /// Create the HTML structure to call the javascript function to Export or View a Form if a form Response exists at least
    /// </summary>
    /// <param name="responses">A string representing the number of responses for a form</param>
    /// <param name="id">The id of the form</param>
    /// <param name="name">The name of the form</param>
    /// <returns></returns>
    protected string getLinks(string responses, string id, string name)
    {
        if (!string.Equals(responses, "0"))
        {
            return " <a style=\"margin-left: 5px;color:#0058B1\" href=\"javascript:ExportSettings('" + id + "','" + name + "');void(0);\"><i class=\"fa fa-download fa-2\"></i>Export</a>" +
                "<a style=\"margin-left: 5px;color:#0058B1\" href=\"javascript:ViewForm('" + id + "','" + name + "');void(0);\"><i class=\"fa fa-eye fa-2\"></i>View</a>";
        }
        else return "";
    }
}