using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;
using GRASPModel;
using Telerik.Web.UI;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

public partial class Admin_ViewFormResponses : System.Web.UI.Page
{
    public bool HasUserDeleteResponsePermission { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if(Utility.VerifyAccess(Request))
        {
            int formID = 0;
            if(Request["FormID"] != null && Request["FormID"] != "")
            {
                HasUserDeleteResponsePermission = Permissions.IsLoggedUserHasPermission(GeneralEnums.Permissions.DeleteFormResponse);
                formID = Convert.ToInt32(Request["FormID"]);
                if(!IsPostBack)
                {
                    if(Request["fc"] != null && Request["fc"] != "")
                    {
                        hdnFilterCount.Value = Request["fc"].ToString();
                    }
                    if(Request["fs"] != null && Request["fs"] != "")
                    {
                        lblFilterSummary.Text = Server.HtmlDecode(Server.UrlDecode(Request["fs"].ToString()));
                        filterSummary.Visible = true;
                    }
                    FillDateFilterDDL();
                    rdpCustomDate.Visible = false;
                }
            }
        }
        else
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = (GridDataItem)e.Item;
            LinkButton deleteResponseButton = (LinkButton)dataItem.FindControl("linkBtnDeleteResponse");
            deleteResponseButton.Visible = HasUserDeleteResponsePermission;
        }
    }

    protected void RadGrid1_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            int startRowIndex = RadGrid1.CurrentPageIndex * RadGrid1.PageSize;
            int maximumRows = RadGrid1.PageSize;
            int responseStatusID = 0;
            DateTime? dateFilter = null;
            string loggedUser = HttpContext.Current.User.Identity.Name.ToString().ToUpper();
            string roleUser = User_Credential.getRoleForUser(loggedUser);

            //If the request comes from CustomFilter, retrieve the filter definition
            string filter = "";
            if(Request["f"] != null && Request["f"] != "")
            {
                filter = Server.UrlDecode(Request["f"].ToString());
            }

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


                if(roleUser != "SuperAdministrator")
                {
                    //TODO: put filter in a session var
                    int userID = Convert.ToInt32(Session["UserID"]);
                    string userResponseFilter = (from u in db.UserFilters
                                                 where u.userID == userID && u.formID==formID
                                                 select u.UserFilterDescription).FirstOrDefault();
                    if(userResponseFilter!= null && userResponseFilter.Length != 0)
                    {
                        responses = from r in responses
                                    from fr in db.UserToFormResponses
                                    where r.id == fr.formResponseID && fr.userID == userID
                                    select r;
                        if(!IsPostBack)
                        {
                            if(lblFilterSummary.Text.Length > 0)
                            {
                                lblFilterSummary.Text += "<br/>";
                            }
                            lblFilterSummary.Text += "Predefined User Filter: " + userResponseFilter;
                            filterSummary.Visible = true;
                        }
                    }
                }

                //If a date has been selected, filter by a starting date
                if(ddlDate.SelectedValue != "-1")
                {
                    dateFilter = GetDateFilter();
                    responses = responses.Where(w => w.FRCreateDate >= dateFilter);
                }

                //If not ANY status, filter by Response Status
                if(ddlReviewStatus.SelectedValue != "" && ddlReviewStatus.SelectedValue != "0")
                {
                    responseStatusID = Convert.ToInt32(ddlReviewStatus.SelectedValue);
                    responses = responses.Where(w => w.ResponseStatusID == responseStatusID);
                }

                //If sender has been specified, filter by sender field
                if(txtSender.Text.Length != 0)
                {
                    responses = responses.Where(r => r.senderMsisdn == txtSender.Text);
                }

                //RadGrid1.VirtualItemCount = responses.Count();
                //IEnumerable<FormResponse> responseList = responses.Skip(startRowIndex).Take(maximumRows*2).ToList().Skip(startRowIndex).Take(maximumRows);
                //responses = responses.Skip(startRowIndex).Take(maximumRows);

                //Retrieve the two formFieldID for the selected form (Enumerator & Create Date)
                int ffidCompileDate = 0;
                int ffidEnumerator = 0;
                IQueryable<FormField> ffs = from f in db.FormField
                                            where f.form_id == formID && f.positionIndex < 2
                                            select f;
                ffidCompileDate = (int)ffs.Where(f => f.positionIndex == 0).FirstOrDefault().id;
                ffidEnumerator = (int)ffs.Where(f => f.positionIndex == 1).FirstOrDefault().id;

                //Final Join, for the two fields
                var res = (from r in responses
                           from rv1 in db.ResponseValue
                           from rv2 in db.ResponseValue
                           from rs in db.FormResponseStatus
                           where r.id == rv1.FormResponseID && r.id == rv2.FormResponseID && r.ResponseStatusID == rs.ResponseStatusID &&
                                   rv1.formFieldId == ffidCompileDate && rv2.formFieldId == ffidEnumerator && r.parentForm_id == formID
                           orderby r.id descending
                           select new { r.id, r.clientVersion, r.senderMsisdn, r.FRCreateDate, r.LastUpdatedDate, CompileDate = rv1.value, Enumerator = rv2.value, ResponseStatus = rs.ResponseStatusName });


                //var res = (from r in responses
                //           from rv2 in db.ResponseValue
                //           from rs in db.FormResponseStatus
                //           where r.id == rv2.FormResponseID && r.ResponseStatusID == rs.ResponseStatusID &&
                //                   rv2.formFieldId == ffidEnumerator && r.parentForm_id == formID
                //           orderby r.id descending
                //           select new { r.id, r.clientVersion, r.senderMsisdn, r.FRCreateDate, Enumerator = rv2.value });

                ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 180;
                //Build the Custom Paging values for better performance
                var recCount = res;
                RadGrid1.VirtualItemCount = recCount.Count();
                RadGrid1.DataSource = res.Skip(startRowIndex).Take(maximumRows).ToList();
            }
        }
    }


    protected void ddlReviewStatus_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        //RadGrid1.Rebind();
    }

    protected void FillDateFilterDDL()
    {
        ddlDate.Items.Add(new RadComboBoxItem("Today's data", "0"));
        ddlDate.Items.Add(new RadComboBoxItem("Last seven days", "7"));
        ddlDate.Items.Add(new RadComboBoxItem("Current month", "30"));
        RadComboBoxItem rcbItem = new RadComboBoxItem("All", "-1");
        rcbItem.Selected = true;
        ddlDate.Items.Add(rcbItem);
        ddlDate.Items.Add(new RadComboBoxItem("Custom date...", "-2"));
    }
    protected void ddlDate_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if(ddlDate.SelectedValue == "-2")
        {
            rdpCustomDate.Visible = true;
        }
        else
        {
            rdpCustomDate.Visible = false;
        }
    }

    protected DateTime GetDateFilter()
    {
        DateTime dFilter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        switch(ddlDate.SelectedValue)
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
                dFilter = rdpCustomDate.SelectedDate.Value;
                break;
        }
        return dFilter;

    }
    protected void btnApplyQuickFilter_Click(object sender, EventArgs e)
    {
        RadGrid1.Rebind();
    }

    /// <summary>
    /// The delete command associated with the grid that allows users to delete a chart for that report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <author>Saad Mansour</author>
    protected void RadGrid1_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        int selectedFormResponseID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["id"].ToString());
        if (FormResponse.DeleteWithAllDependences(selectedFormResponseID)) //If deletes success, then refresh the grid.
        {
            RadGrid1.DataBind();
        }
    }
}