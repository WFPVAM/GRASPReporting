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
/// Used to create the HTML structure of all the FormResponse of a Form
/// </summary>
public partial class Admin_ViewForm : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads a table with the compiled form is shown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        int resID = 0;
        int rosterCount = 0;
        decimal prevID = 0;
        if(Request["id"] != "" && Request["id"] != null)
        {
            resID = Convert.ToInt32(Request["id"]);
        }
        GRASPEntities db = new GRASPEntities();

        var items = from rv in db.ResponseValue
                    join ff in db.FormField on rv.formFieldId equals (int?)ff.id
                    where rv.FormResponseID == resID
                    orderby ff.id, rv.id ascending
                    select new { rv, ff };

        if(items.Count() > 0)
        {
            litFormTitle.Text = items.FirstOrDefault().ff.Form.name;
        }
        
        foreach (var f in items)
        {
            if (f.rv.RVRepeatCount == 0) //E' un campo normale
            {
                if (rosterCount != 0)
                {
                    litTableResult.Text += "</div></div>";
                }
                litTableResult.Text += "<div class=\"left clear\"><label>" + f.ff.label + "</label></div><div class=\"right\">" + ((f.rv.value != "") ? f.rv.value : f.ff.name) + "</div>";
                prevID = f.ff.id;
                rosterCount = 0;
            }
            else if (f.rv.RVRepeatCount == -1) //Intestazione del roster
            {
                if (rosterCount != 0)
                {
                    litTableResult.Text += "</div></div>"; //Chiudo il roster precedente
                }
                litTableResult.Text += "<div class=\" rosterContainer clear\"><div class=\"roasterTitle clear\"><label>" + f.ff.label + "</label></div>";
                prevID = f.ff.id;
            }
            else
            {
                if (prevID != f.ff.id && f.rv.RVRepeatCount == 1) //Iniziano i campi del Roster
                {
                    if (rosterCount != 0)
                    {
                        litTableResult.Text += "</div>"; //Chiudo il campo precedente
                    }
                    litTableResult.Text += "<div class=\"left clear\"><label>" + f.ff.label + "</label></div><div class=\"right overflowTable\"><div class=\"inline\">" + ((f.rv.value != "") ? f.rv.value : f.ff.name) + "</div>";
                    rosterCount = 1;
                }
                else
                {
                    litTableResult.Text += "<div class=\"inline\">" + ((f.rv.value != "") ? f.rv.value : f.ff.name) + "</div>";
                }
                prevID = f.ff.id;
            }
        }
    }
}