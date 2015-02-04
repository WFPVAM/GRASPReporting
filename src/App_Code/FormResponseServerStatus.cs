using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FormResponseServerStatus
/// </summary>
public partial class FormResponseServerStatus
{
    public FormResponseServerStatus()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static bool IsFormSaved(string fileName)
    {
        bool isSaved = false;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                FormResponseServerStatus formResponseServerStatus = (from fr in db.FormResponseServerStatus
                                                                     where fr.InstanceUniqueIdentifier == fileName
                                                                     select fr).FirstOrDefault();
                if (formResponseServerStatus != null)
                {
                    isSaved = formResponseServerStatus.IsSavedToServer;
                }
            }
        }
        catch (Exception)
        {
        }

        return isSaved;
    }

    public static bool InsertOrUpdateStatus(string fileName, bool isSaved)
    {
        bool isSuccess = false;

        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                var formResponseServerStatus = (from fr in db.FormResponseServerStatus
                                                                     where fr.InstanceUniqueIdentifier == fileName
                                                                     select fr).FirstOrDefault();
                if (formResponseServerStatus != null)
                {
                    formResponseServerStatus.IsSavedToServer = isSaved;
                }
                else
                {
                    formResponseServerStatus = new FormResponseServerStatus();
                    formResponseServerStatus.InstanceUniqueIdentifier = fileName;
                    formResponseServerStatus.IsSavedToServer = isSaved;
                    db.FormResponseServerStatus.Add(formResponseServerStatus);
                }

                db.SaveChanges();
                isSuccess = true;
            }
        }
        catch (Exception)
        {
        }

        return isSuccess;
    }

}