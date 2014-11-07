using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// FormFieldExport class contains auxiliary functions to query FormFieldExport view on Grasp DB
/// </summary>
public partial class FormFieldExport
{
    /// <summary>
    /// Queries the DB to obtain the surveyList associated with the formfield
    /// </summary>
    /// <param name="formFieldID">The id of the formfield</param>
    /// <returns>A string representing the id of the SurveyList</returns>
    public static int getSurveyID(int formFieldID)
    {
        int res = 0;
        GRASPEntities db = new GRASPEntities();

        var item = (from ffe in db.FormFieldExport
                    where ffe.id == formFieldID && ffe.type == "REPEATABLES"
                    select ffe.survey_id).FirstOrDefault();
        if (item != null)
            res = (int)item;
        return res;
    }
    /// <summary>
    /// Queries the DB to obtain the number of the elements of a surveylist
    /// </summary>
    /// <param name="surveyID">The id of the surveylist</param>
    /// <returns>The surveylist elements count</returns>
    public static int getSurveyListCount(int surveyID)
    {
        GRASPEntities db = new GRASPEntities();

        var list = from s in db.SurveyListAPI
                   where s.id == surveyID
                   select s;

        return list.Count();
    }
    /// <summary>
    /// Queries the DB to obtain the surveylist associated with the formfield id
    /// </summary>
    /// <param name="formFieldID">The id of the formfield</param>
    /// <returns>The elements of the surveylist</returns>
    public static IEnumerable<SurveyElement> getSurveyList(int formFieldID)
    {
        int surveyID = getSurveyID(formFieldID);
        GRASPEntities db = new GRASPEntities();

        IEnumerable<SurveyElement> list = from s in db.Survey
                                          join sse in db.Survey_SurveyElement on s.id equals sse.Survey_id
                                          join se in db.SurveyElement on sse.values_id equals se.id
                                          where s.id == surveyID
                                          select se;

        return list;
    }

    internal static int getSurveyList(decimal p)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<SurveyElement> list = from s in db.Survey
                                          join sse in db.Survey_SurveyElement on s.id equals sse.Survey_id
                                          join se in db.SurveyElement on sse.values_id equals se.id
                                          where s.id == p
                                          select se;

        return list.Count();
    }

    public static IEnumerable<SurveyElement> getSurveyListElements(int p)
    {
        GRASPEntities db = new GRASPEntities();

        IEnumerable<SurveyElement> list = from s in db.Survey
                                          join sse in db.Survey_SurveyElement on s.id equals sse.Survey_id
                                          join se in db.SurveyElement on sse.values_id equals se.id
                                          where s.id == p
                                          select se;

        return list;
    }
}