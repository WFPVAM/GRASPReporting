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

/// <summary>
/// Role class contains auxiliary functions to query ReportField table on Grasp DB
/// </summary>
public partial class Role
{
    /// <summary>
    /// Checks the Role table to obtain the role name
    /// </summary>
    /// <param name="roleID">the id of the role</param>
    /// <returns>a string representing the role, empty string if the role not exists</returns>
    public static string getRole(int roleID)
    {
        GRASPEntities db = new GRASPEntities();

        string role = (from r in db.Roles
                   where r.id == roleID
                   select r.description).FirstOrDefault();
        if (role != null)
            return role;
        else return "";
    }
}