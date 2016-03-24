using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FormResponseStatus
/// </summary>
public partial class FormResponseStatus
{

    public static IQueryable<FormResponseStatus> GetAllStatus(GRASPEntities db)
    {
            IQueryable<FormResponseStatus> frs = from f in db.FormResponseStatus
                                                 select f;
            return frs;
    }
}