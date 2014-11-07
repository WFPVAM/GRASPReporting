using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for UserToFormResponses
/// </summary>
public partial class UserToFormResponses
{
    public static string GenerateAssociation(int userID, int formID, bool refreshAll)
    {
        return GenerateAssociation(userID, formID, refreshAll, 0);
    }
    public static string GenerateAssociation(int userID, int formID, bool refreshAll,int formResponseID)
    {
        StringBuilder sb = new StringBuilder();
        string filter;

        using(GRASPEntities db = new GRASPEntities())
        {
            if(refreshAll)
            {
                db.Database.ExecuteSqlCommand("DELETE UserToFormResponses WHERE userID=@userID AND formID=@formID",
                    new SqlParameter("userID", userID), new SqlParameter("formID", formID));
            }

            filter = (from uf in db.UserFilters
                             where uf.userID == userID && formID == formID && uf.UserFilterIsEnabled == 1
                             select uf.UserFilterString).FirstOrDefault();

            var respUnion = (from r in db.ResponseValue
                             from fr in db.FormResponse
                             where fr.id == r.FormResponseID && fr.parentForm_id == formID
                                && (formResponseID==0 || fr.id==formResponseID)
                             select new { FormResponseID = r.FormResponseID.Value, Value = r.value, nvalue = r.nvalue.Value, formFieldID = r.formFieldId.Value }).Union(
                 from re in db.ResponseValueExt
                 from fr in db.FormResponse
                 where fr.id == re.FormResponseID && fr.parentForm_id == formID
                                && (formResponseID == 0 || fr.id == formResponseID)
                 select new { FormResponseID = re.FormResponseID, Value = "", nvalue = re.nvalue.Value, formFieldID = re.FormFieldID.Value });

            var filteredResponseIDs = (from r in respUnion.Where(filter)
                                       group r by r.FormResponseID into grp
                                       where grp.Count() == 1
                                       select grp.Key).ToList();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string insertSQLcmd = "INSERT INTO UserToFormResponses (UserID,FormID,FormResponseID) VALUES(" + userID + "," + formID + ",";
            foreach(var fr in filteredResponseIDs)
            {
                sb.AppendLine(insertSQLcmd + fr.ToString() + ");");
            }
            if(sb.Length != 0)
            {
                db.Database.ExecuteSqlCommand(sb.ToString());
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);



            return elapsedTime;
        }
    }

    public static void GenerateAssociationForAllUsers(int formID, int formResponseID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            List<UserFilters> userToFilter = (from uf in db.UserFilters
                      where formID == formID && uf.UserFilterIsEnabled == 1
                      select uf).ToList();

            foreach(UserFilters u in userToFilter)
            {
                GenerateAssociation(u.userID, formID, false, formResponseID);
            }
        }
    }
}