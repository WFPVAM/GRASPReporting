using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FormFieldResponses
/// </summary>
/// <author>Saad Mansour</author>
public partial class FormFieldResponses
{
	public FormFieldResponses()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    /// <summary>
    /// Get the count of a given field.
    /// </summary>
    /// <param name="formID"></param>
    /// <param name="formFieldID"></param>
    /// <returns></returns>
    public static int GetFieldCount(int formID, int formFieldID)
    {
        int count = 0;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                //Finds the occurrences of the date field.
                count = (from ffr in db.FormFieldResponses
                         where ffr.parentForm_id == formID
                            && ffr.formFieldId == formFieldID
                         select ffr.nvalue).Count();
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        if (count == null)
        {
            count = 0;
        }

        return count;
    }

    /// <summary>
    /// Gets the max value of the given field.
    /// </summary>
    /// <param name="formID"></param>
    /// <param name="formFieldID"></param>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    public static string GetFieldMaxValue(int formID, int formFieldID, string fieldType)
    {
        //double? max = 0;
        string max = string.Empty;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                switch (fieldType)
                {
                    case "NUMERIC_TEXT_FIELD" :
                    case "SERVERSIDE_CALCULATED":
                        max = (from ffr in db.FormFieldResponses
                               where ffr.parentForm_id == formID
                                && ffr.formFieldId == formFieldID
                                && ffr.nvalue != null
                               select ffr.nvalue).Max().ToString();
                        break;
                    //case "DATE_FIELD":
                    //    max = (from ffr in db.FormFieldResponses
                    //           where ffr.parentForm_id == formID
                    //            && ffr.formFieldId == formFieldID
                    //            && ffr.dvalue != null
                    //           select ffr.nvalue).Max().ToString();
                    //    break;
                    default:
                        max = (from ffr in db.FormFieldResponses
                               where ffr.parentForm_id == formID
                                && ffr.formFieldId == formFieldID
                                && ffr.value != null
                               select ffr.value).Max();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return max;
    }

    /// <summary>
    /// Gets the min value of the given field.
    /// </summary>
    /// <param name="formID"></param>
    /// <param name="formFieldID"></param>
    /// <returns></returns>
    public static string GetFieldMinValue(int formID, int formFieldID, string fieldType)
    {
        //double? min = 0;
        string min = string.Empty;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                switch (fieldType)
                {
                    case "NUMERIC_TEXT_FIELD":
                    case "SERVERSIDE_CALCULATED":
                        min = (from ffr in db.FormFieldResponses
                               where ffr.parentForm_id == formID
                                && ffr.formFieldId == formFieldID
                                && ffr.nvalue != null
                               select ffr.nvalue).Min().ToString();
                        break;
                    //case "DATE_FIELD":
                    //    max = (from ffr in db.FormFieldResponses
                    //           where ffr.parentForm_id == formID
                    //            && ffr.formFieldId == formFieldID
                    //            && ffr.dvalue != null
                    //           select ffr.nvalue).Min().ToString();
                    //    break;
                    default:
                        min = (from ffr in db.FormFieldResponses
                               where ffr.parentForm_id == formID
                                && ffr.formFieldId == formFieldID
                                && ffr.value != null
                               select ffr.value).Min();
                        break;
                }
                min = (from ffr in db.FormFieldResponses
                         where ffr.parentForm_id == formID
                         && ffr.formFieldId == formFieldID
                         && ffr.value != null
                         select ffr.value).Min();
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return min;
    }

    /// <summary>
    /// Gets the count of null values of the given field.
    /// </summary>
    /// <param name="formID"></param>
    /// <param name="formFieldID"></param>
    /// <returns></returns>
    public static int GetFieldNullValuesCount(int formID, int formFieldID)
    {
        int countNull = 0;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                countNull = (from ffr in db.FormFieldResponses
                       where ffr.parentForm_id == formID 
                        && ffr.formFieldId == formFieldID 
                        && ffr.value == null
                             select ffr.value).Count(); //the nvalue field was used.
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        if (countNull == null)
        {
            countNull = 0;
        }

        return countNull;
    }
}