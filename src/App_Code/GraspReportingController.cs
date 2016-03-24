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
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Class used for GRASP API
/// </summary>
public class GraspReportingController : ApiController
{
    /// <summary>
    /// </summary>
    /// <returns>all records in FormResponse</returns>
    [Authorize]
    public IEnumerable<ResponseFormList> GetResponseFormList()
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<ResponseFormList> RFList = from fr in db.FormResponse
                                               select new ResponseFormList
                                               {
                                                   FR_id = (int)fr.id,
                                                   FR_clientVersion = fr.clientVersion,
                                                   FR_sender = fr.senderMsisdn,
                                                   FR_FParentID = (int)fr.parentForm_id,
                                                   FR_codeForm = fr.Code_Form,
                                                   FR_createDate = (DateTime)fr.FRCreateDate
                                               };

        return RFList;
    }

    /// <summary>
    /// </summary>
    /// <param name="id">The id of a FormResponse</param>
    /// <returns>all records in FormResponse whose id is greater than the id passed as a parameter</returns>
    [Authorize]
    public IEnumerable<ResponseFormList> GetResponseFormListFromID(int id)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<ResponseFormList> RFList = from fr in db.FormResponse
                                               where fr.id > id
                                               select new ResponseFormList
                                               {
                                                   FR_id = (int)fr.id,
                                                   FR_clientVersion = fr.clientVersion,
                                                   FR_sender = fr.senderMsisdn,
                                                   FR_FParentID = (int)fr.parentForm_id,
                                                   FR_codeForm = fr.Code_Form,
                                                   FR_createDate = (DateTime)fr.FRCreateDate
                                               };

        return RFList;
    }

    /// <summary>
    /// </summary>
    /// <param name="id">The id of the formResponse</param>
    /// <returns>all records in ResponseValue whose FormResponseID is equals to the parameter passed as input</returns>
    [Authorize]
    public IEnumerable<ResponseValueList> GetValuesByFormResponse(int id)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<ResponseValueList> RVList = from rv in db.ResponseValue
                                                where rv.FormResponseID == id
                                                select new ResponseValueList
                                                {
                                                    RV_id = (int)rv.id,
                                                    RV_value = rv.value,
                                                    RV_ffID = (int)rv.formFieldId,
                                                    RV_frID = (int)rv.FormResponseID,
                                                    RV_createDate = (DateTime)rv.RVCreateDate,
                                                    RV_repeatCount = (int)rv.RVRepeatCount
                                                };

        return RVList;
    }

    /// <summary>
    /// </summary>
    /// <param name="id">The id of the responsevalue</param>
    /// <returns>all records in ResponseValue whose id is greathen than the parameter passed as input</returns>
    [Authorize]
    public IEnumerable<ResponseValueList> GetValuesFromID(int id)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<ResponseValueList> RVList = from rv in db.ResponseValue
                                                where rv.id > id
                                                select new ResponseValueList
                                                {
                                                    RV_id = (int)rv.id,
                                                    RV_value = rv.value,
                                                    RV_ffID = (int)rv.formFieldId,
                                                    RV_frID = (int)rv.FormResponseID,
                                                    RV_createDate = (DateTime)rv.RVCreateDate,
                                                    RV_repeatCount = (int)rv.RVRepeatCount
                                                };

        return RVList;
    }

    /// <summary>
    /// </summary>
    /// <returns>all records in Form</returns>
    [Authorize]
    public IEnumerable<FormList> GetFormList()
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<FormList> FList = from f in db.Form
                                      select new FormList
                                      {
                                          F_id = (int)f.id,
                                          F_designerVersion = f.designerVersion,
                                          F_finalised = f.finalised,
                                          F_name = f.name,
                                          F_owner = f.owner,
                                          F_permittedGroup_path = f.permittedGroup_path,
                                          F_createDate = (DateTime)f.FormCreateDate
                                      };

        return FList;
    }

    /// <summary>
    /// </summary>
    /// <param name="id">The if of the form</param>
    /// <returns>all records in FormField whose form_id is equal to the parameter passed as input</returns>
    [Authorize]
    public IEnumerable<FormFieldList> GetFormFieldByFormID(int id)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<FormFieldList> FFList = from ff in db.FormField
                                            where ff.form_id == id
                                            select new FormFieldList
                                            {
                                                FF_id = (int)ff.id,
                                                FF_label = ff.label,
                                                FF_name = ff.name,
                                                FF_positionIndex = ff.positionIndex,
                                                FF_required = (int)ff.required,
                                                FF_type = ff.type,
                                                FF_formID = (int)ff.form_id,
                                                FF_surveyID = ff.survey_id,
                                                FF_formula = ff.formula,
                                                FF_isReadOnly = (int)ff.isReadOnly,
                                                FF_numberOfRep = (int)ff.numberOfRep,
                                                FF_createDate = (DateTime)ff.FFCreateDate
                                            };

        return FFList;
    }

    /// <summary>
    /// </summary>
    /// <param name="id">The id of the surveylist</param>
    /// <returns>all records in Survey,Survey_SurveyElement,SurveyElement, whose Survey.id is equal to the parameter passed as input</returns>
    [Authorize]
    public IEnumerable<SurveyList> GetSurveyList(int id)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<SurveyList> SList = from s in db.SurveyListAPI
                                        where s.id == id
                                        select new SurveyList
                                        {
                                            S_name = s.name,
                                            S_value = s.value
                                        };

        return SList;
    }

}

