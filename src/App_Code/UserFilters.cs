using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserFilters
/// </summary>
public partial class UserFilters
{

    public static UserFilters Insert(int formID, int userID, string filter, string filterDescription)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            UserFilters newUF = new UserFilters();
            newUF.UserFilterCreateDate = DateTime.Now;
            newUF.UserFilterIsEnabled = 1;
            newUF.UserFilterString = filter;
            newUF.UserFilterDescription = filterDescription;
            newUF.formID = formID;
            newUF.userID = userID;
            db.UserFilters.Add(newUF);
            db.SaveChanges();
            return newUF;
        }
    }

    public static void RemoveFilter(int formID, int userID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            db.Database.ExecuteSqlCommand("DELETE UserFilters WHERE formID=@formID AND userID=@userID", new SqlParameter("@formID", formID), new SqlParameter("@userID", userID));
        }
    }
}