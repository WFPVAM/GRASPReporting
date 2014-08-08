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
/// ResponseValue class contains auxiliary functions to query ResponseValue table on Grasp DB
/// </summary>
public partial class ResponseValue
{
    /// <summary>
    /// Create a single response for a formfield
    /// </summary>
    /// <param name="value">A string representing the response value</param>
    /// <param name="formResponseID">The id of the formresponse</param>
    /// <param name="formFieldID">The id of the formfield</param>
    /// <param name="rCount">An int representing the repeatcount (-1 for roster/table, > 0 for roster children, 0 otherwise</param>
    public static void createResponseValue(string value, int formResponseID, int formFieldID, int rCount)
    {
        GRASPEntities db = new GRASPEntities();

        var response = new ResponseValue();
        response.value = value;
        response.FormResponseID = formResponseID;
        response.formFieldId = formFieldID;
        response.RVCreateDate = DateTime.Now;
        response.RVRepeatCount = rCount;

        db.ResponseValue.Add(response);
        db.SaveChanges();

    }
    /// <summary>
    /// Updates all the position index of a Formresponse
    /// </summary>
    /// <param name="formResponseID">the id of the formresponse</param>
    public static void setPositionIndex(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var items = from rv in db.ResponseValue
                    where rv.FormResponseID == formResponseID
                    select rv;

        foreach (var i in items)
        {
            i.positionIndex = getPositionIndex((int)i.formFieldId);
        }

        db.SaveChanges();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="formfieldID">the id of the formfield</param>
    /// <returns>The position index of a formfield</returns>
    public static int getPositionIndex(int formfieldID)
    {
        int pi = 0;

        GRASPEntities db = new GRASPEntities();

        var items = (from ff in db.FormField
                    where ff.id == formfieldID
                    select ff).FirstOrDefault();

        if (items != null)
            pi = items.positionIndex;

        return pi;
    }
    /// <summary>
    /// Queries the DB to obtain all the responsevalue of a formresponse
    /// </summary>
    /// <param name="formResponseID">the id of a formresponse</param>
    /// <returns>A list representing all the responsevalue for a formresponse</returns>
    public static IEnumerable<ResponseValue> getResponseValue(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var items = from rv in db.ResponseValue
                    where rv.FormResponseID == formResponseID
                    select rv;

        return items;
    }
    /// <summary>
    /// Deletes all the response value of a formresponse
    /// </summary>
    /// <param name="formResponseID">the id of the formresponse</param>
    public static void deleteResponsesValues(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        db.Database.ExecuteSqlCommand("DELETE FROM FormResponse_ResponseValue WHERE FormResponse_id = {0}", formResponseID);

        var items = from rv in db.ResponseValue
                    where rv.FormResponseID == formResponseID
                    select rv;

        foreach (var i in items)
        {
            db.ResponseValue.Remove(i);
        }

        db.SaveChanges();
    }
}