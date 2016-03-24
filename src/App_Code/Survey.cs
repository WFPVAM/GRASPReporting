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
/// Summary description for Survey
/// </summary>
public partial class Survey
{
    public static Survey createSurvey(ImportingElement element, int id, string flag)
    {
        GRASPEntities db = new GRASPEntities();

        Survey survey = new Survey();

        survey.name = element.refListName;
        survey.owner_id = id;
        
        db.Survey.Add(survey);
        db.SaveChanges();

        if (flag == "select1")
        {
            foreach (string label in element.select1Labels)
            {                
                int size = FormFieldExport.getSurveyList(survey.id);
                SurveyElement se = createSurveyElement(label, size);
                createSurveyAssociation(survey.id, se.id);
            }
        }
        if (flag == "survey")
        {
            foreach (string label in element.surveyValues)
            {
                int size = FormFieldExport.getSurveyList(survey.id);
                SurveyElement se = createSurveyElement(label, size);
                createSurveyAssociation(survey.id, se.id);
            }
        }

        

        return survey;
    }

    private static void createSurveyAssociation(decimal p1, decimal p2)
    {
        GRASPEntities db = new GRASPEntities();

        Survey_SurveyElement surveyel = new Survey_SurveyElement();

        surveyel.Survey_id = p1;
        surveyel.values_id = p2;

        db.Survey_SurveyElement.Add(surveyel);
        db.SaveChanges();
    }

    public static SurveyElement createSurveyElement(string value, int positionIndex)
    {
        GRASPEntities db = new GRASPEntities();

        SurveyElement surveyel = new SurveyElement();

        surveyel.value = value;
        surveyel.positionIndex = positionIndex;
        surveyel.defaultValue = 0;

        db.SurveyElement.Add(surveyel);
        db.SaveChanges();

        return surveyel;
    }

    public static IEnumerable<SurveyElement> GetSurveyListElements(int p)
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