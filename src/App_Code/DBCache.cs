using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DBCache
/// </summary>
public static class DBCache
{


    //public static readonly Func<ObjectContext, List<HASHDuplicates>, IQueryable<FormResponse>> CompiledDuplicatedResponses =
    //    CompiledQuery.Compile<ObjectContext, List<HASHDuplicates>, IQueryable<FormResponse>>(
    //    (db, hashes) => from r in db.FormResponse
    //                    from h in db.IndexHASHes
    //                    from d in hashes
    //                    where r.id == h.FormResponseID && h.IndexHASHString == d.hash
    //                    select r);
    


}


