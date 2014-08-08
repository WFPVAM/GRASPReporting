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
using System.Data.Spatial;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// FormResponseCoord class contains auxiliary functions to query FormResponseCoord table on Grasp DB
/// </summary>
public partial class FormResponseCoord
{
    /// <summary>
    /// Inserts a record in FormResponseCoord table using Geometry and Geography class to obtain spatial coordinates for latitude and longitude
    /// </summary>
    /// <param name="coordText">Latitude and longitude</param>
    /// <param name="formResponseID">the id of the formresponse</param>
    public static void createFormResponseCoord(string coordText, int formResponseID)
    {
        SqlConnection connection = new SqlConnection("Data Source=NEURON002\\SQLEXPRESS;Initial Catalog=GRASP1;Integrated Security=false;user id=utentesviluppo;password=beuser11");
        string qry = "INSERT INTO FormResponseCoords (FormResponseID,FRCoordText, FRCoordGeo, frXCoordText, FRCoordGeometry) VALUES (" + formResponseID.ToString() + ",'" + coordText + " 0 10',geography::STPointFromText('POINT(" + coordText + ")', 4326), null ,geometry::STPointFromText('POINT(" + coordText + ")', 0));";
        connection.Open();
        SqlCommand commandScript = new SqlCommand(qry, connection);
        commandScript.ExecuteNonQuery();
        connection.Close();
    }
}