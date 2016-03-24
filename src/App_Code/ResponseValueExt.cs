using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ResponseValueExt
/// </summary>
public partial class ResponseValueExt
{
    public static ResponseValueExt Insert(GRASPEntities db, int formResponseID, int formFieldExtID, int formFieldID,int positionIndex, double nvalue)
    {
        ResponseValueExt rve = new ResponseValueExt();
        rve.FormResponseID = formResponseID;
        rve.FormFieldExtID = formFieldExtID;
        rve.nvalue = nvalue;
        rve.FormFieldID = formFieldID;
        rve.PositionIndex = positionIndex;
        db.ResponseValueExt.Add(rve);
        return rve;
    }

}