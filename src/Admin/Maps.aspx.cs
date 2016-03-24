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
/// Shows a marker for each geolocalised form
/// </summary>
public partial class Admin_Maps : System.Web.UI.Page
{
    public static string geoCoordinates = "";
    string geoTmp = "";
    /// <summary>
    /// When the page loads it queries the DB to obtain all the formResponse with geolocalization.
    /// All the coordinates are stored in a string that will be used by Google Maps APIs
    /// to create the markers on the map
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        GRASPEntities db = new GRASPEntities();

        foreach (decimal ffID in FormField.getFormFieldFromType("GEOLOCATION"))
        {
            var coordinates = (from rv in db.ResponseValue
                              where rv.formFieldId == ffID
                              orderby rv.FormResponseID descending
                              select rv).Take(100);

            foreach(var coord in coordinates)
            {
                if (!string.IsNullOrEmpty(coord.value))
                {
                    string[] ltnlng = coord.value.Split(' ');
                    if(ltnlng[0] != "0" && ltnlng[1] != "0")
                    {
                        geoTmp += "[" + ltnlng[1] + ", " + ltnlng[0] + ", " + coord.FormResponseID + "],";
                    }
                }
            }
        }
        if(geoTmp.Length > 0)
            geoCoordinates = geoTmp.Substring(0, geoTmp.Length - 1);
    }
}