using System;
using System.Collections.Generic;
using System.Configuration;
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
       
        ConnectionStringSettingsCollection settings =
            ConfigurationManager.ConnectionStrings;

        var ConnectionString = "";
        if (settings != null)
        {
            foreach (ConnectionStringSettings cs in settings)
            {
                if (cs.Name == "GRASP_MemberShip")

                {
                    ConnectionString = cs.ConnectionString;
                }
            }
        }
        
        SqlConnection connection = new SqlConnection(ConnectionString);
        string qry = "INSERT INTO FormResponseCoords (FormResponseID,FRCoordText, FRCoordGeo, frXCoordText, FRCoordGeometry) VALUES (" + formResponseID.ToString() + ",'" + coordText + "',geography::STPointFromText('POINT(" + coordText + ")', 4326), null ,geometry::STPointFromText('POINT(" + coordText + ")', 4326))";
       /// string qry = "INSERT INTO FormResponseCoords (FormResponseID,FRCoordText, FRCoordGeo, frXCoordText, FRCoordGeometry) VALUES (" + formResponseID.ToString() + ",'" + coordText + "',geography::STPointFromText('POINT(" + coordText + ")', 4326), null ,geometry::STPointFromText('POINT(" + coordText + ")', 0));";
        connection.Open();
        SqlCommand commandScript = new SqlCommand(qry, connection);
        commandScript.ExecuteNonQuery();
        connection.Close();
    }
}