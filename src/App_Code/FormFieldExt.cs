using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FormFieldExt
/// </summary>
public partial class FormFieldExt
{

    public static FormFieldExt Insert(GRASPEntities db, int formID, int formFieldID,int positionIndex, string formula, string fieldName, string fieldLabel)
    {
        FormFieldExt ffe = new FormFieldExt();
        ffe.FormID = formID;
        ffe.FormFieldExtFormula = formula;
        ffe.FormFieldExtName = fieldName;
        ffe.FormFieldExtLabel = fieldLabel;
        ffe.FormFieldID = formFieldID;
        ffe.PositionIndex = positionIndex;
        db.FormFieldExt.Add(ffe);
        return ffe;
    }
}