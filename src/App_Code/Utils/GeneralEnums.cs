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
        DROP_DOWN_LIST
    }

    public enum ResponseFilesFolderNames
    {
        incoming,
        processed,
        error,
        duplicate
    }
}