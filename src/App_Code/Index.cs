using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Index
/// </summary>
public partial class Index
{
    public static List<Index> GetActiveIndexes(int formID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            List<Index> idx = (from i in db.Indexes
                               where i.formID == formID && i.IndexLastUpdateDate != null
                               select i).ToList();
            return idx;
        }
    }

    public static Index GetIndex(int indexID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            Index idx = (from i in db.Indexes
                         where i.IndexID == indexID
                         select i).FirstOrDefault();
            return idx;
        }
    }
    public static int GenerateIndexesHASH(int formID, int formResponseID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            List<Index> idxs = (from i in db.Indexes
                                where i.formID == formID
                                select i).ToList();
            foreach(Index i in idxs)
            {
                GenerateIndexHASHes(i.IndexID, formResponseID);
            }
            return idxs.Count;
        }

    }

    public static int GenerateIndexesHASH(int formID, Array formResponseIDs)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            List<Index> idxs = (from i in db.Indexes
                                where i.formID == formID
                                select i).ToList();
            foreach(Index i in idxs)
            {
                for(int n = 0; n < formResponseIDs.GetLength(0); n++)
                {
                    int formResponseID = (int)formResponseIDs.GetValue(n);
                    GenerateIndexHASHes(i.IndexID, formResponseID);
                }
            }
            return idxs.Count;
        }

    }
    public static int GenerateIndexHASHes(int indexID)
    {
        return GenerateIndexHASHes(indexID, 0);
    }
    public static int GenerateIndexHASHes(int indexID, int formResponseID)
    {
        StringBuilder sb = new StringBuilder();
        string filter = "";
        string values = "";
        int frID = 0;

        using(GRASPEntities db = new GRASPEntities())
        {
            if(formResponseID == 0)
            {
                //Delete the previously created HASHes            
                IndexHASH.DeleteHASHes(indexID);
            }

            //Build the list of FormFields on which create the HASHes
            List<IndexField> idxFields = (from i in db.IndexFields
                                          where i.IndexID == indexID
                                          select i).ToList();
            foreach(IndexField i in idxFields)
            {
                filter = filter + (" OR formFieldId==" + i.FormFieldID);
            }
            if(filter.Length > 0)
            {
                filter = filter.Substring(4); //Remove the first OR

                //Get all the response values based on the fields of the index and eventually only the formResponseID in the parameter
                var filteredResponseIDs = (from r in db.ResponseValue.Where(filter)
                                           where (r.FormResponseID == formResponseID || formResponseID == 0)
                                           orderby r.FormResponseID, r.formFieldId
                                           select new { r.value, r.FormResponseID });

                string insertSQLcmd = "INSERT INTO IndexHASHes (IndexID,FormResponseID,IndexHASHString) VALUES(" + indexID + ",";
                //Generate the HASHes for each response value
                foreach(var rv in filteredResponseIDs)
                {
                    if(values.Length > 0 && frID != (int)rv.FormResponseID)
                    {
                        //create&insert the HASH
                        //IndexHASH.InsertHashString(db, frID, indexID, values);
                        sb.AppendLine(insertSQLcmd + (frID.ToString() + ",'" + IndexHASH.GetHashString(values) + "');"));
                        values = "";
                    }
                    values += rv.value;
                    frID = (int)rv.FormResponseID;
                }
                if(values.Length > 0)
                {
                    //create&insert the HASH
                    //IndexHASH.InsertHashString(db, frID, indexID, values);
                    sb.AppendLine(insertSQLcmd + (frID.ToString() + ",'" + IndexHASH.GetHashString(values) + "');"));
                    values = "";
                }


                //Save all
                //db.SaveChanges();
                if(sb.Length != 0)
                {
                    db.Database.ExecuteSqlCommand(sb.ToString());
                }
            }
        }

        return 0;
    }
    public static int UpdateIndexLastUpdate(int indexID, DateTime date, string userName)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            Index idx = (from i in db.Indexes
                         where i.IndexID == indexID
                         select i).FirstOrDefault();
            idx.IndexLastUpdateDate = date;
            idx.IndexLastUpdateUserName = userName;
            db.SaveChanges();
            return idx.IndexID;
        }
    }
}