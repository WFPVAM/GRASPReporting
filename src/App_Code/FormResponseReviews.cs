using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FormResponseReviews
/// </summary>
public partial class FormResponseReviews
{
    public static FormResponseReviews Insert(int formResponseID, string frrUserName, int previousStatusID, int currentStatusID, string reviewDetail)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            if(previousStatusID == 0 || currentStatusID == 0)
            {
                currentStatusID = (from f in db.FormResponse
                                   where f.id == formResponseID
                                   select f.ResponseStatusID).FirstOrDefault();
                previousStatusID = currentStatusID;
            }
            FormResponseReviews frr = new FormResponseReviews();
            frr.FormResponseID = formResponseID;
            frr.FRRUserName = frrUserName;
            frr.FormResponsePreviousStatusID = previousStatusID;
            frr.FormResponseCurrentStatusID = currentStatusID;
            frr.FormResponseReviewDetail = reviewDetail;
            frr.FormResponseReviewDate = DateTime.Now;
            frr.FormResponseReviewSeqNo = GetLastSeqNo(formResponseID) + 1;
            db.FormResponseReviews.Add(frr);
            db.SaveChanges();

            return frr;
        }
    }

    public static IQueryable<FormResponseReviews> GetByFormResponse(GRASPEntities db, int formResponseID)
    {
        IQueryable<FormResponseReviews> frr = from f in db.FormResponseReviews
                                              where f.FormResponseID == formResponseID
                                              select f;
        return frr;
    }

    public static int GetLastSeqNo(int formResponseID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            int? maxVal = (from f in db.FormResponseReviews
                           where f.FormResponseID == formResponseID
                           select f.FormResponseReviewSeqNo).Max();
            if(maxVal != null && maxVal > 0)
            {
                return maxVal.Value;
            }
            else
            {
                return 0;
            }
        }
    }
}