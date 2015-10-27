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

    public enum UserRoles
    {
        SuperAdministrator,
        Supervisor
    }

    /// <summary>
    /// Represents permissions names as in "[GRASP].[dbo].Permissions" table
    /// </summary>
    public enum Permissions
    {
        DeleteFormResponse,
        EditFormResponse
    }

    public enum FormStatuses
    {
        None = 0,
        Deleted = 101,
        NotExisted = 102,
        NotFinalized = 103,
        NewPublishedVersion = 104,
        Finalized = 105       
    }

    /// <summary>
    /// List of aggregate functions using in Reports Charts.
    /// </summary>
    public enum AggregateFuns
    {
        count,
        average,
        sum,
        stdev,
        min,
        max
    }
}