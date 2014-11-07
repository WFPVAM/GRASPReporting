﻿/*
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
using System.Text;
using System.Threading;
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
        response.ResponseStatusID = 1;  //default status ToBeReviewed
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
        if(fName != "")
        {
            var response = new FormResponse();
            response.clientVersion = clientVersion;
            response.parentForm_id = parentFormID;
            response.fromDataEntry = 0;
            response.Code_Form = fName;
            response.senderMsisdn = sender;
            response.FRCreateDate = DateTime.Now;
            response.ResponseStatusID = 1;  //default status ToBeReviewed
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
        if(item != null)
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

        if(item != null)
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
        if(item != null)
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
        if(item != null)
        {
            if(!item.clientVersion.Contains("_CSVImport"))
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
        if(item != null)
        {
            item.clientVersion = clientVersion;
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Update the status of the form response, returning the previous status.
    /// </summary>
    /// <param name="formResponseID"></param>
    /// <param name="statusID"></param>
    /// <returns>The previous formResponseStatusID</returns>
    public static int UpdateStatus(int formResponseID, int statusID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {

            var item = (from f in db.FormResponse
                        where (int)f.id == formResponseID
                        select f).FirstOrDefault();
            if(item != null)
            {
                int prevStatusID = item.ResponseStatusID;
                item.ResponseStatusID = statusID;
                db.SaveChanges();
                return prevStatusID;
            }
            else
            {
                return 0;
            }
        }
    }
    public static string GetStatus(int formResponseID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            string statusName = (from f in db.FormResponse
                                 from fs in db.FormResponseStatus
                                 where (int)f.id == formResponseID && f.ResponseStatusID == fs.ResponseStatusID
                                 select fs.ResponseStatusName).FirstOrDefault();
            if(statusName != null)
            {
                return statusName;
            }
            else
            {
                return "";
            }
        }
    }

    public static string GetAsJson(int formResponseID)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sbRep = new StringBuilder();
        string jsonOutput = "";

        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        using(GRASPEntities db = new GRASPEntities())
        {
            var responses = from r in db.FormFieldResponses
                            where r.RVRepeatCount == 0                 //Exclude repeatables vals
                                && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                                && r.FormResponseID==formResponseID
                            select new { r.name, r.type, r.value,r.nvalue };
            foreach(var r in responses)
            {
                switch(r.type)
                {
                    case "TEXT_FIELD":
                    case "TEXT_AREA":
                    case "DATE_FIELD":
                    case "RADIO_BUTTON":
                    case "PHONE_NUMBER_FIELD":
                    case "CHECK_BOX":
                    case "GEOLOCATION":
                    case "EMAIL_FIELD":
                        if(r.value.Length != 0)
                        {
                            sb.AppendLine("\"" + r.name + "\": \"" + r.value.Replace("\\", "") + "\",");
                        }
                        break;
                    case "DROP_DOWN_LIST":
                        if(r.value.Length != 0)
                        {
                            sb.AppendLine("\"" + r.name + "\": {\"value\":\"" + r.value + "\"},");
                        }
                        break;
                    case "NUMERIC_TEXT_FIELD":
                        if(r.value.Length>0 && r.nvalue != null)
                        {
                            sb.AppendLine("\"" + r.name + "\": " + r.nvalue + ",");
                        }
                        break;
                    case "CURRENCY_FIELD":
                        if(r.value.Length != 0)
                        {
                            sb.AppendLine("\"" + r.name + "\": " + r.value + ",");
                        }
                        break;
                }
            }

            var repeatableResponses = (from r in db.ResponseRepeatable
                                       where r.FormResponseID == formResponseID
                                       select r).ToList();
            
            foreach(var rep in repeatableResponses.Where(w => w.RVRepeatCount == -1))
            {
                string repFields = "";

                if(sbRep.Length != 0)
                {
                    sbRep.Append(",");
                }
                sbRep.AppendLine("\"" + rep.name + "\": [");

                int repCount=0;
                foreach(var r in repeatableResponses.Where(w => w.RVRepeatCount > 0 && w.ParentFormFieldID == rep.formFieldId).OrderBy(o => o.RVRepeatCount))
                {
                    if(r.RVRepeatCount != repCount)
                    {
                        if(repCount == 0)
                        {
                            repFields = "{"; //first cycle, only open
                        }
                        else
                        {
                            //string tmp = sbRep.ToString();
                            //tmp = tmp.Substring(0, tmp.Length - 2);
                            //sbRep.Clear();
                            //sbRep.Append(tmp);
                            repFields = repFields.Substring(0, repFields.Length - 1); //remove last comma
                            repFields = repFields + ("},{"); //close previous and open new one
                        }
                    }
                    else
                    {
                        //sbRep.Append(",");
                    }
                    switch(r.type)
                    {
                        case "TEXT_FIELD":
                        case "TEXT_AREA":
                        case "DATE_FIELD":
                        case "RADIO_BUTTON":
                        case "PHONE_NUMBER_FIELD":
                        case "CHECK_BOX":
                        case "GEOLOCATION":
                        case "EMAIL_FIELD":
                            if(r.value.Length != 0)
                            {
                                repFields = repFields + ("\"" + r.name + "\": \"" + r.value + "\",");
                            }
                            break;
                        case "DROP_DOWN_LIST":
                            if(r.value.Length != 0)
                            {
                                repFields = repFields + ("\"" + r.name + "\": {\"value\":\"" + r.value + "\"},");
                            }
                            break;
                        case "NUMERIC_TEXT_FIELD":
                            if(r.nvalue != null)
                            {
                                repFields = repFields + ("\"" + r.name + "\": " + r.nvalue + ",");
                            }
                            break;
                        case "CURRENCY_FIELD":
                            if(r.nvalue != null)
                            {
                                repFields = repFields + ("\"" + r.name + "\": " + r.nvalue + ",");
                            }
                            break;
                    }
                    repCount = r.RVRepeatCount.Value;
                }
                //cycle on repeatable fields finished. Close the Repeatable.
                repFields = repFields.Substring(0, repFields.Length - 1); //remove comma
                sbRep.AppendLine(repFields + "}]");                
            }
        }
        if(sbRep.Length == 0)
        {
            jsonOutput = sb.ToString(); //There is no repeatable 
            jsonOutput = jsonOutput.Substring(0, jsonOutput.Length - 3);  //remove last comma
        }
        else
        {
            jsonOutput = sb.ToString() + sbRep.ToString();
        }

        return jsonOutput;
    }
}