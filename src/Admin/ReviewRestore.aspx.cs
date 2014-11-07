using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class Admin_ReviewRestore : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Utility.VerifyAccess(Request))
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }

        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            if(!IsPostBack)
            {
                using(GRASPEntities db = new GRASPEntities())
                {
                    var users = from u in db.User_Credential.Where(w => w.UserDeleteDate == null && w.roles_id != 3)
                                join f in db.UserFilters on u.user_id equals f.userID into uf
                                from jf in uf.DefaultIfEmpty()
                                where jf == null ? true : jf.formID == formID
                                select new { u.username, u.user_id, u.supervisor, jf.UserFilterDescription };
                    DdlUsers.DataSource = users.ToList();
                    DdlUsers.DataBind();

                }
                RdpStartDate.Visible = false;
                RdpEndDate.Visible = false;
                RdpStartDate.MaxDate = DateTime.Now;
                RdpEndDate.MaxDate = DateTime.Now;
                FillDateFilterDDL();
            }
        }
    }


    protected void FillDateFilterDDL()
    {
        DdlDate.Items.Add(new RadComboBoxItem("Today's data", "0"));
        DdlDate.Items.Add(new RadComboBoxItem("Last seven days", "7"));
        DdlDate.Items.Add(new RadComboBoxItem("Current month", "30"));
        RadComboBoxItem rcbItem = new RadComboBoxItem("All", "-1");
        rcbItem.Selected = true;
        DdlDate.Items.Add(rcbItem);
        DdlDate.Items.Add(new RadComboBoxItem("Custom date...", "-2"));
    }
    protected void ddlDate_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if(DdlDate.SelectedValue == "-2")
        {
            RdpStartDate.Visible = true;
            RdpEndDate.Visible = true;
        }
        else
        {
            RdpStartDate.Visible = false;
            RdpEndDate.Visible = false;
        }
    }

    protected DateTime GetDateFilter()
    {
        DateTime dFilter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        switch(DdlDate.SelectedValue)
        {
            case "0":
                //dFilter = dFilter;
                break;
            case "7":
                dFilter = dFilter.AddDays(-7);
                break;
            case "30":
                dFilter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                break;
            case "-2":

                break;
        }
        return dFilter;

    }


    protected void BtnRestoreReviews_Click(object sender, EventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);

            //If the request comes from CustomFilter, retrieve the filter definition
            string filter = "";
            if(Request["f"] != null && Request["f"] != "")
            {
                filter = Server.UrlDecode(Request["f"].ToString());
            }
            //litResult.Text = "";
            string res = RestoreReviews(formID, filter, false);
            litResult.Text = "<strong>Operation completed</strong>. Time taken: " + res + "";

        }
    }

    protected void BtnTest_Click(object sender, EventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);

            //If the request comes from CustomFilter, retrieve the filter definition
            string filter = "";
            if(Request["f"] != null && Request["f"] != "")
            {
                filter = Server.UrlDecode(Request["f"].ToString());
            }
            //litResult.Text = "";
            string res = RestoreReviews(formID, filter, true);
            litResult.Text = "With selected filters will be restored " + res + " response(s) to the orignal values.<br/>Click on the Restore button to start the restore reviews process." + 
                "<br/>Please note that this operation could take several minutes to complete, depending on CPU power and the number of responses to restore.";
            BtnRestoreReviews.Enabled = true;
        }
    }

    protected string RestoreReviews(int formID, string filter, bool test)
    {
        DateTime? dateFilter = null;
        int responseStatusID = 0;
        StringBuilder sb = new StringBuilder();

        Stopwatch stopWatch = new Stopwatch();

        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        using(GRASPEntities db = new GRASPEntities())
        {

            IQueryable<FormResponse> responses;
            if(filter.Length > 0)
            {
                //If we have a dynamic filter, use the UNION to get extended fields
                int filterCount = Convert.ToInt32(hdnFilterCount.Value);
                var respUnion = (from r in db.ResponseValue
                                 from fr in db.FormResponse
                                 where fr.id == r.FormResponseID && fr.parentForm_id == formID
                                 select new
                                 {
                                     FormResponseID = r.FormResponseID.Value,
                                     Value = r.value,
                                     nvalue = r.nvalue.Value,
                                     formFieldID = r.formFieldId.Value
                                 }).Union(
                                     from re in db.ResponseValueExt
                                     from fr in db.FormResponse
                                     where fr.id == re.FormResponseID && fr.parentForm_id == formID
                                     select new
                                     {
                                         FormResponseID = re.FormResponseID,
                                         Value = "",
                                         nvalue = re.nvalue.Value,
                                         formFieldID = re.FormFieldID.Value
                                     });

                var filteredResponseIDs = (from r in respUnion.Where(filter)
                                           group r by r.FormResponseID into grp
                                           where grp.Count() == filterCount
                                           select grp.Key);

                responses = (from r in db.FormResponse
                             from rf in filteredResponseIDs
                             where r.id == rf
                             orderby r.id descending
                             select r);
            }
            else
            {
                //No filter, straight query..
                responses = (from r in db.FormResponse
                             where r.parentForm_id == formID
                             orderby r.id descending
                             select r);
            }

            responses = from r in responses
                            from rr in db.FormResponseReviews
                            where r.id == rr.FormResponseID
                            select r;

            //If a date has been selected, filter by a starting date
            if(DdlDate.SelectedValue != "-1" && DdlDate.SelectedValue != "-2")
            {
                dateFilter = GetDateFilter();
                responses = responses.Where(w => w.FRCreateDate >= dateFilter);
            }
            else if(DdlDate.SelectedValue == "-2" && RdpStartDate.SelectedDate != null)
            {
                DateTime startDate = RdpStartDate.SelectedDate.Value;
                DateTime endDate = RdpEndDate.SelectedDate.Value;
                responses = from r in responses
                            from rr in db.FormResponseReviews
                            where r.id == rr.FormResponseID
                                && rr.FormResponseReviewDate >= startDate && rr.FormResponseReviewDate <= endDate
                            select r;
            }

            //If not ANY status, filter by Response Status
            if(DdlReviewStatus.SelectedValue != "" && DdlReviewStatus.SelectedValue != "0")
            {
                responseStatusID = Convert.ToInt32(DdlReviewStatus.SelectedValue);
                responses = responses.Where(w => w.ResponseStatusID == responseStatusID);
            }

            if(test)
            {
                return responses.Count().ToString();
            }
            else
            {
                string updResp = "UPDATE FormResponse SET ResponseStatusID=1 WHERE id=";

                var minRespReview = (from rv in db.ResponseValueReviews
                                     from r in responses
                                     where rv.FormResponseID == r.id
                                     select rv).GroupBy(g => g.FormResponseID)
                                     .Select(s => new { FormResponseID = s.Key, FormResponseReviewID = s.Min(m => m.FormResponseReviewID) });


                List<ResponseValueReviews> respValues = (from rv in db.ResponseValueReviews
                                                         from rr in minRespReview
                                                         where rv.FormResponseID == rr.FormResponseID && rv.FormResponseReviewID == rr.FormResponseReviewID
                                                         select rv).ToList();

                var responseList = responses.ToList();

                foreach(var r in responseList)
                {
                    int formResponseID = (int)r.id;
                    //update FormResponses to set to the initial status;
                    sb.AppendLine(updResp + formResponseID);

                    //Update ResponseReview                    

                    string sqlInsert = "INSERT INTO ResponseValue (pushed,formFieldID,FormresponseID,positionIndex,RVRepeatCount,value,nvalue,dvalue) " +
                            " VALUES (0,";
                    List<ResponseValueReviews> respValuesByresponse = respValues.Where(w => w.FormResponseID == formResponseID).ToList();
                    if(respValuesByresponse.Count() > 0)
                    {
                        //Delete the existing response values
                        sb.AppendLine("DELETE ResponseValue WHERE FormResponseID=" + r.id);
                    }

                    //Reinsert all the original values
                    foreach(ResponseValueReviews rv in respValues.Where(w => w.FormResponseID == formResponseID))
                    {
                        string updVal = sqlInsert;
                        updVal += rv.formFieldId.ToString() + ",";
                        updVal += formResponseID.ToString() + ",";
                        updVal += rv.positionIndex.ToString() + ",";
                        updVal += rv.RVRepeatCount.ToString() + ",";
                        updVal += "N'" + rv.value.ToString().Replace("'", "''") + "',";
                        if(rv.nvalue != null)
                        {
                            updVal += rv.nvalue.ToString() + ",";
                        }
                        else
                        {
                            updVal += "NULL,";
                        }
                        if(rv.dvalue != null)
                        {
                            updVal += "'" + rv.dvalue.ToString() + "'";
                        }
                        else
                        {
                            updVal += "NULL";
                        }
                        updVal += ");";
                        sb.AppendLine(updVal);
                    }

                }


                stopWatch.Start();
                //db.Database.ExecuteSqlCommand("USE master;ALTER DATABASE GRASP_UNDP3 SET RECOVERY BULK_LOGGED;");
                db.Database.ExecuteSqlCommand(sb.ToString());
                //litResult.Text += "<br>BULK Insert: " + String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);


                foreach(var r in responseList.AsParallel())
                {
                    int formResponseID = (int)r.id;
                    List<ResponseValueReviews> respValuesByresponse = respValues.Where(w => w.FormResponseID == formResponseID).ToList();
                    //Array responseIDs = respValuesByresponse.Select(s=>s.FormResponseID).ToArray();
                    
                    if(respValuesByresponse.Count() > 0)
                    {                        
                        Index.GenerateIndexesHASH(formID, formResponseID);
                        ServerSideCalculatedField.GenerateSingle(formID, formResponseID);
                        UserToFormResponses.GenerateAssociationForAllUsers(formID, formResponseID);
                    }
                }

                //db.Database.ExecuteSqlCommand("USE master;ALTER DATABASE GRASP_UNDP3 SET RECOVERY FULL;");

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                //litResult.Text += "<br>UPDATE Linq: " + String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            }

        }
    }
}