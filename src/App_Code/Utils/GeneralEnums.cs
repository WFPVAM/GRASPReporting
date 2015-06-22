using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GeneralEnums
/// </summary>
public class GeneralEnums
{
    public enum FieldTypes
    {
        NUMERIC_TEXT_FIELD,
        DROP_DOWN_LIST,
        DATE_FIELD,
        CHECK_BOX
    }

    public enum ResponseFilesFolderNames
    {
        incoming,
        processed,
        error,
        duplicate,
        unknownForms
    }

    public enum SaveIncomingInstanceResults
    {
        ok,
        ko
    }
}