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
using System.Web.Security;

/// <summary>
/// FormResponse class contains auxiliary functions to query FormResponse table on Grasp DB
/// </summary>
public partial class FormResponse
{
    /// <summary>
    /// Adds a FormResponse for a form by setting the sender as the user currently logged in.
    /// </summary>
    /// <param name="parentFormID">The id of the form</param>
    /// <returns>The id of the created record</returns>
    public static int createFormResponse(int parentFormID)
    {
        MembershipUser m = Membership.GetUser(HttpContext.Current.User.Identity.Name.ToString(), false);
        GRASPEntities db = new GRASPEntities();

        var response = new FormResponse();
        response.clientVersion = "WEB";
        response.parentForm_id = parentFormID;
        response.fromDataEntry = 1;
        response.Code_Form = Form.getFormName(parentFormID);
        response.senderMsisdn = m.Comment;
        response.FRCreateDate = DateTime.Now;
        response.pushed = 0;

        db.FormResponse.Add(response);
        db.SaveChanges();

        return (int)response.id;
    }
    /// <summary>
    /// Adds a FormResponse for a form
    /// </summary>
    /// <param name="parentFormID">the id of the form</param>
    /// <param name="sender">the sender of the response</param>
    /// <param name="clientVersion">the client version of the response</param>
    /// <returns>The id of the created record</returns>
    public static int createFormResponse(int parentFormID, string sender, string clientVersion)
    {
        GRASPEntities db = new GRASPEntities();
        string fName = Form.getFormName(parentFormID);
        if (fName != "")
        {
            var response = new FormResponse();
            response.clientVersion = clientVersion;
            response.parentForm_id = parentFormID;
            response.fromDataEntry = 0;
            response.Code_Form = fName;
            response.senderMsisdn = sender;
            response.FRCreateDate = DateTime.Now;
            response.pushed = 0;

            db.FormResponse.Add(response);
            db.SaveChanges();

            return (int)response.id;
        }
        else return 0;
    }
    /// <summary>
    /// Queries the DB to obtain the id of a form by its name
    /// </summary>
    /// <param name="formName">name of the form</param>
    /// <returns>The id of the form, 0 if it not exists</returns>
    public static int getFormID(string formName)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.Form
                    where f.id_flsmsId == formName
                    select f).FirstOrDefault();
        if (item != null)
            return Convert.ToInt32(item.id);
        else return 0;
    }
    /// <summary>
    /// Queries the DB to obtain all the formResponse of a form
    /// </summary>
    /// <param name="id">The id of the form</param>
    /// <returns>A list of FormResponse</returns>
    public static IEnumerable<FormResponse> getFormResponse(int id)
    {
        GRASPEntities db = new GRASPEntities();

        var items = from fr in db.FormResponse
                    where fr.parentForm_id == id
                    select fr;

        return items;
    }
    /// <summary>
    /// Queries the DB to get a FormResponse to delete it
    /// </summary>
    /// <param name="formResponseID">The id of the formResponse</param>
    public static void deleteFormResponse(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from rv in db.FormResponse
                    where rv.id == formResponseID
                    select rv).FirstOrDefault();

        if (item != null)
        {
            db.FormResponse.Remove(item);
            db.SaveChanges();
        }
    }
    /// <summary>
    /// Queries the DB to check if a formResponse with that id exists
    /// </summary>
    /// <param name="formResponseID">The id of a formresponse</param>
    /// <returns>True if exists, false otherwise</returns>
    public static bool formResponseExists(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.FormResponse
                    where (int)f.id == formResponseID
                    select f).FirstOrDefault();
        if (item != null)
            return true;
        else return false;
    }
    /// <summary>
    /// Update the clientversion of a formResponse appending "_CSVImport" when it is imported from CSV file
    /// </summary>
    /// <param name="formResponseID">The id of the formresponse</param>
    public static void updateFormResponseToImport(int formResponseID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.FormResponse
                    where (int)f.id == formResponseID
                    select f).FirstOrDefault();
        if (item != null)
        {
            if (!item.clientVersion.Contains("_CSVImport"))
            {
                item.clientVersion += "_CSVImport";
            }
            db.SaveChanges();
        }
    }

    public static void updateClientVersion(int formResponseID, string clientVersion)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.FormResponse
                    where (int)f.id == formResponseID
                    select f).FirstOrDefault();
        if (item != null)
        {
            item.clientVersion = clientVersion;
            db.SaveChanges();
        }
    }
}