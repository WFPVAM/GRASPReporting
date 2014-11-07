/*
 *GRASP(Geo-referential Real-time Acquisition Statistics Platform) Reporting Tool <http://www.brainsen.com>
 * Developed by Brains Engineering s.r.l (marco.giorgi@brainsen.com)
 * This file is part of GRASP Reporting Tool.  
 *  GRASP Reporting Tool is free software: you can redistribute it and/or modify it
 *  under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or (at
 *  your option) any later version.  
 *  GRASP Reporting Tool is distributed in the hope that it will be useful, but
 *  WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser  General Public License for more details.  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GRASP Reporting Tool. 
 *  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// Used to create the HTML structure of all the FormResponse of a Form
/// </summary>
public partial class Admin_ViewForm : System.Web.UI.Page
{
    /// <summary>
    /// When the page loads a table with the compiled form is shown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Utility.VerifyAccess(Request))
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }
        if(Session["isAuthenticated"] != null)
        {
            int resID = 0;
            int selVerID = 0;

            if(Request["id"] != "" && Request["id"] != null)
            {
                resID = Convert.ToInt32(Request["id"]);
                if(!IsPostBack)
                {
                    lblFormResponseStatus.Text = FormResponse.GetStatus(resID);

                }
                if(Request["v"] != null && Request["v"].ToString().Length != 0)
                {
                    selVerID = Convert.ToInt32(Request["v"].ToString());
                }
            }
            GRASPEntities db = new GRASPEntities();

            if(Session["RoleID"] != "3")
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                string userResponseFilter = (from u in db.User_Credential
                                             where u.user_id == userID
                                             select u.UserResponseFilter).FirstOrDefault();
                if(userResponseFilter != null && userResponseFilter.Length != 0)
                {
                    int responseAccess = (from r in db.UserToFormResponses
                                          where r.formResponseID == resID && r.userID == userID
                                          select r).Count();
                    if(responseAccess > 0)
                    {
                        if(selVerID != 0)
                        {
                            ShowResponse(resID, selVerID);
                        }
                        else
                        {
                            ShowResponse(resID);
                        }
                    }
                    else
                    {
                        //User cannot see the Response:
                        Response.Write("<h1>Authorization Denied</h1>");
                        Response.End();
                    }
                }
                else
                {
                    if(selVerID != 0)
                    {
                        ShowResponse(resID, selVerID);
                    }
                    else
                    {
                        ShowResponse(resID);
                    }
                }
            }
            else
            {
                if(selVerID != 0)
                {
                    ShowResponse(resID, selVerID);
                }
                else
                {
                    ShowResponse(resID);
                }
            }
        }
        else
        {
            Response.Write("Session Expired");
            Response.End();
        }
    }

    //protected void ShowVersions(int resID, int selVerID)
    //{
    //    string cssClass = "";

    //    using(GRASPEntities db = new GRASPEntities())
    //    {
    //        var vers = (from frr in db.FormResponseReviews
    //                    from rvr in db.ResponseValueReviews
    //                    where frr.FormResponseReviewID == rvr.FormResponseReviewID
    //                         && frr.FormResponseID == resID
    //                    select new { frr.FormResponseReviewID, frr.FormResponseReviewSeqNo, frr.FormResponseReviewDate, frr.FRRUserName }).Distinct().ToList();
    //        if(vers.Count()!=0)
    //        {
    //            litVersions.Text = "Data Edit Versions: ";
    //        }
    //        foreach(var ver in vers)
    //        {
    //            if(ver.FormResponseReviewID == selVerID)
    //            {
    //                cssClass = "class=\"selected\"";
    //            }
    //            else
    //            {
    //                cssClass = "";
    //            }
    //            litVersions.Text = litVersions.Text +
    //                "<a " + cssClass + " href=\"viewform.aspx?id=" + resID + "&v=" + ver.FormResponseReviewID +
    //                "\" title=\"" + ver.FormResponseReviewDate + " by " + ver.FRRUserName + "\">" +
    //                ver.FormResponseReviewSeqNo + "</a> | ";
    //        }
    //    }
    //}
    protected void ShowResponse(int resID)
    {
        int rosterCount = 0;
        decimal prevID = 0;

        using(GRASPEntities db = new GRASPEntities())
        {
            var items = from rv in db.FormFieldResponses
                        where rv.FormResponseID == resID
                        orderby rv.positionIndex ascending
                        select rv;

            if(items.Count() > 0)
            {
                litFormTitle.Text = db.Form.Where(w => w.id == items.FirstOrDefault().parentForm_id).Select(s => s.name).FirstOrDefault();
                int formID = (int)items.FirstOrDefault().parentForm_id;
                litEditLink.Text = "<a href=\"DataEditWebForm.aspx?formID=" + formID.ToString() + "&RID=" + resID.ToString() +
                    "\">Edit Data</a>";
            }

            foreach(var f in items)
            {
                string respValue = "";
                if(f.RVRepeatCount == 0) //It's a normal field
                {

                    if(rosterCount != 0)
                    {
                        litTableResult.Text += "</div></div>";
                    }
                    if(f.value != "")
                    {
                        respValue = f.value;
                        if((f.type == "IMAGE") && (respValue != "GRASPImage\\"))
                        {
                            respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                        }
                        if(respValue != "GRASPImage\\")
                        {
                            litTableResult.Text += "<div class=\"left clear\"><label>" + f.label + "</label></div>" +
                                "<div class=\"right\">" + respValue + "</div>";
                        }
                    }
                    prevID = f.formFieldId.Value;
                    rosterCount = 0;

                }
                else if(f.RVRepeatCount == -1) //Repeatable Header
                {
                    if(rosterCount != 0)
                    {
                        litTableResult.Text += "</div></div>"; //Close previous repeatable
                    }
                    litTableResult.Text += "<div class=\" rosterContainer clear\"><div class=\"roasterTitle clear\"><label>" + f.label + "</label></div>";
                    prevID = f.formFieldId.Value;
                }
                else
                {
                    if(prevID != f.formFieldId.Value && f.RVRepeatCount == 1) //Repetable start
                    {

                        if(rosterCount != 0)
                        {
                            litTableResult.Text += "</div>"; //Close previous field
                        }
                        if(f.value != "")
                        {
                            respValue = f.value;

                            if(f.type == "IMAGE")
                            {
                                respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                            }
                            litTableResult.Text += "<div class=\"left clear\"><label>" + f.label + "</label></div>" +
                            "<div class=\"right overflowTable\"><div class=\"inline\">" + respValue + "</div>";
                        }
                        rosterCount = 1;
                    }
                    else
                    {
                        if(f.value != "")
                        {
                            if(f.type == "IMAGE")
                            {
                                respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                            }
                            litTableResult.Text += "<div class=\"inline\">" + respValue + "</div>";
                        }
                    }
                    prevID = f.formFieldId.Value;
                }
            }
        }
    }

    protected void ShowResponse(int resID, int selVerID)
    {
        int rosterCount = 0;
        decimal prevID = 0;

        using(GRASPEntities db = new GRASPEntities())
        {
            int? revisionNo = (from rev in db.ResponseValueReviews
                               where rev.positionIndex == 1 && rev.FormResponseReviewID <= selVerID
                               orderby rev.FormResponseReviewID descending
                               select rev.FormResponseReviewID).FirstOrDefault();

            var items = from rv in db.ResponseValueReviews
                        join ff in db.FormField on rv.formFieldId equals (int?)ff.id
                        where rv.FormResponseID == resID && rv.FormResponseReviewID == revisionNo
                        orderby ff.id, rv.id ascending
                        select new { rv, ff };

            if(items.Count() > 0)
            {
                litFormTitle.Text = items.FirstOrDefault().ff.Form.name;
                int formID = (int)items.FirstOrDefault().ff.form_id;
                litEditLink.Text = "<a href=\"DataEditWebForm.aspx?formID=" + formID.ToString() + "&RID=" + resID.ToString() +
                    "\">Edit Data</a>";
            }
            else
            {
                litTableResult.Text = "<h3>No edit has been done in this revision.</h3>";
            }

            if(revisionNo != selVerID)
            {
                litTableResult.Text = "<h4>No edit has been done in this revision.</h4>";
            }
            litViewLatest.Text = "<a href=\"viewform.aspx?id=" + resID.ToString() + "\">View Latest Revision</a>";

            foreach(var f in items)
            {
                string respValue = "";
                if(f.rv.RVRepeatCount == 0) //E' un campo normale
                {
                    if(rosterCount != 0)
                    {
                        litTableResult.Text += "</div></div>";
                    }

                    respValue = ((f.rv.value != "") ? f.rv.value : f.ff.name);
                    if((f.ff.type == "IMAGE") && (respValue != "GRASPImage\\"))
                    {
                        respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                    }
                    if(respValue != "GRASPImage\\")
                    {
                        litTableResult.Text += "<div class=\"left clear\"><label>" + f.ff.label + "</label></div>" +
                            "<div class=\"right\">" + respValue + "</div>";
                    }
                    prevID = f.ff.id;
                    rosterCount = 0;
                }
                else if(f.rv.RVRepeatCount == -1) //Intestazione del roster
                {
                    if(rosterCount != 0)
                    {
                        litTableResult.Text += "</div></div>"; //Chiudo il roster precedente
                    }
                    litTableResult.Text += "<div class=\" rosterContainer clear\"><div class=\"roasterTitle clear\"><label>" + f.ff.label + "</label></div>";
                    prevID = f.ff.id;
                }
                else
                {
                    if(prevID != f.ff.id && f.rv.RVRepeatCount == 1) //Iniziano i campi del Roster
                    {
                        if(rosterCount != 0)
                        {
                            litTableResult.Text += "</div>"; //Chiudo il campo precedente
                        }
                        respValue = ((f.rv.value != "") ? f.rv.value : f.ff.name);
                        if(f.ff.type == "IMAGE")
                        {
                            respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                        }
                        litTableResult.Text += "<div class=\"left clear\"><label>" + f.ff.label + "</label></div>" +
                        "<div class=\"right overflowTable\"><div class=\"inline\">" + respValue + "</div>";
                        rosterCount = 1;
                    }
                    else
                    {
                        respValue = ((f.rv.value != "") ? f.rv.value : f.ff.name);
                        if(f.ff.type == "IMAGE")
                        {
                            respValue = "<a href=\"/" + respValue + "\" target=\"_blank\"><img src=\"/" + respValue + "\" /></a>";
                        }
                        litTableResult.Text += "<div class=\"inline\">" + respValue + "</div>";
                    }
                    prevID = f.ff.id;
                }
            }
        }
    }
    protected void btnChangeStatus_Click(object sender, EventArgs e)
    {
        pnlChangeStatus.Visible = true;

        int responseID = 0;
        if(Request["id"] != "" && Request["id"] != null)
        {
            responseID = Convert.ToInt32(Request["id"]);

            using(GRASPEntities db = new GRASPEntities())
            {
                var res = from s in db.FormResponseStatus
                          from fr in db.FormResponse
                          where (s.ResponseStatusDependency) <= fr.ResponseStatusID && fr.id == responseID
                          select s;
                ddlFormResponseStatus.DataSource = res.ToList();
                ddlFormResponseStatus.Items.Clear();
                ddlFormResponseStatus.DataBind();
            }
        }
        pnlChangeStatus.Visible = true;
        pnlHistory.Visible = false;
        litTableResult.Visible = false;
        LitMessage.Text = "";
        pnlResponseStatus.Height = 260;
    }
    protected void btnSaveStatusChange_Click(object sender, EventArgs e)
    {
        if(ddlFormResponseStatus.SelectedValue != "" && Request["id"] != "" && Request["id"] != null)
        {
            if(ddlFormResponseStatus.SelectedValue == "3" && txtDetails.Text.Length == 0)
            {
                LitMessage.Text = "<div>Please insert a reason in order to reject a form response.</div>";
            }
            else
            {
                int formResponseStatusID = Convert.ToInt32(ddlFormResponseStatus.SelectedValue);
                int responseID = Convert.ToInt32(Request["id"]);
                int prevStatusID = FormResponse.UpdateStatus(responseID, formResponseStatusID);
                string userName = HttpContext.Current.User.Identity.Name.ToString();
                FormResponseReviews.Insert(responseID, userName, prevStatusID, formResponseStatusID, txtDetails.Text);
                pnlChangeStatus.Visible = false;
                pnlHistory.Visible = true;
                litTableResult.Visible = true;
                grdHistory.Rebind();
                lblFormResponseStatus.Text = FormResponse.GetStatus(responseID);
                pnlResponseStatus.Height = Unit.Empty;
            }
        }
    }
    protected void grdHistory_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if(Request["id"] != "" && Request["id"] != null)
        {
            int responseID = Convert.ToInt32(Request["id"]);
            using(GRASPEntities db = new GRASPEntities())
            {
                var history = from rr in FormResponseReviews.GetByFormResponse(db, responseID)
                              from rs1 in db.FormResponseStatus
                              from rs2 in db.FormResponseStatus
                              where rr.FormResponsePreviousStatusID == rs1.ResponseStatusID &&
                                rr.FormResponseCurrentStatusID == rs2.ResponseStatusID
                              select new
                              {
                                  rr.FormResponseID,
                                  rr.FormResponseReviewID,
                                  rr.FormResponseReviewDate,
                                  rr.FormResponseReviewDetail,
                                  rr.FormResponseReviewSeqNo,
                                  rr.FRRUserName,
                                  prevStatusName = rs1.ResponseStatusName,
                                  currStatusName = rs2.ResponseStatusName
                              };

                grdHistory.DataSource = history.OrderByDescending(o => o.FormResponseReviewDate).ToList();
            }
        }
    }
}