using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;

namespace GRASPModel
{
    /// <summary>
    /// Summary description for GraspEntitiesExt
    /// </summary>
    public partial class GRASPEntities
    {


        /// <summary>
        ///     This method exists for use in LINQ queries,
        ///     as a stub that will be converted to a SQL CAST statement.
        /// </summary>
        [EdmFunction("GRASPModel", "ParseDouble")]
        public static double ParseDouble(string stringvalue)
        {
            return Double.Parse(stringvalue);
        }

    }
}