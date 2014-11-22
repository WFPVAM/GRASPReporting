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
using Telerik.Web.UI;

public partial class Admin_CheckDuplicates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Utility.VerifyAccess(Request))
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }

        if(!IsPostBack)
        {
            if(Request["FormID"] != null && Request["FormID"] != "")
            {
                int formID = Convert.ToInt32(Request["FormID"]);
                litFormName.Text = Request["FormName"].ToString();
                List<Index> idxs = Index.GetActiveIndexes(formID).ToList();

                if(idxs != null && idxs.Count() != 0)
                {

                    ddlIndexes.EmptyMessage = "Please select an Index...";
                    ddlIndexes.DataTextField = "IndexName";
                    ddlIndexes.DataValueField = "IndexID";
                    ddlIndexes.DataSource = idxs;
                    ddlIndexes.DataBind();

                    FillDateFilterDDL();
                    rdpCustomDate.Visible = false;
                }
                else
                {
                    Response.Write("The Check Duplicates function works on Indices. You must first create an index for each test you want to make.");
                    Response.End();
                }
            }
        }
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
    protected void grdDuplicatedResponses_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {

        if(Request["FormID"] != null && Request["FormID"] != "" && ddlIndexes.SelectedValue != "" &&
            (ddlDate.SelectedValue != "-2" || rdpCustomDate.SelectedDate != null))
        {
            int formID = Convert.ToInt32(Request["FormID"]);
            int indexID = Convert.ToInt32(ddlIndexes.SelectedValue);
            int startRowIndex = grdDuplicatedResponses.CurrentPageIndex * grdDuplicatedResponses.PageSize;
            int maximumRows = grdDuplicatedResponses.PageSize;
            DateTime? dateFilter = null;

            if(ddlDate.SelectedValue != "-1")
            {
                dateFilter = GetDateFilter();
            }

            GRASPEntities db = new GRASPEntities();

            //example: (formFieldId==17 and value==\"Unsafe\") OR (formFieldId==3 and value==\"0.0.30\")
            string filter = "";
            int filterCount = 0;

            IEnumerable<HASHDuplicates> duplicates = (from h in db.IndexHASHes
                                                      from fr in db.FormResponse
                                                      where h.IndexID == indexID && (dateFilter == null || fr.FRCreateDate >= dateFilter) &&
                                                      h.FormResponseID == fr.id && fr.parentForm_id == formID
                                                      group h by new { h.IndexHASHString } into hGrp
                                                      where hGrp.Count() > 1
                                                      select new HASHDuplicates { hash = hGrp.Key.IndexHASHString, formResponseID = hGrp.Min(r => r.FormResponseID) });


            IQueryable<FormResponse> responses;
            if(filter.Length > 0)
            {
                var filteredResponseIDs = (from r in db.ResponseValue.Where(filter)
                                           group r by r.FormResponseID into grp
                                           where grp.Count() == filterCount
                                           select grp.Key.Value);
                responses = (from r in db.FormResponse
                             from rf in filteredResponseIDs
                             where r.id == rf
                             orderby r.id
                             select r);
            }
            else
            {

                responses = (from r in db.FormResponse
                             from d in duplicates
                             where r.id == d.formResponseID
                             select r);
            }


            grdDuplicatedResponses.VirtualItemCount = responses.Count();
            responses = responses.OrderByDescending(o => o.id).Skip(startRowIndex).Take(maximumRows);

            int ffidCompileDate = 0;
            int ffidEnumerator = 0;
            IQueryable<FormField> ffs = from f in db.FormField
                                        where f.form_id == formID && f.positionIndex < 2
                                        select f;
            ffidCompileDate = (int)ffs.Where(f => f.positionIndex == 0).FirstOrDefault().id;
            ffidEnumerator = (int)ffs.Where(f => f.positionIndex == 1).FirstOrDefault().id;

            var res = (from r in responses
                       from rv1 in db.ResponseValue
                       from rv2 in db.ResponseValue
                       from h in db.IndexHASHes
                       where r.id == rv1.FormResponseID && r.id == rv2.FormResponseID && r.id==h.FormResponseID && h.IndexID==indexID &&
                       rv1.formFieldId == ffidCompileDate && rv2.formFieldId == ffidEnumerator && r.parentForm_id == formID
                       select new { r.id, r.clientVersion, r.senderMsisdn, r.FRCreateDate, CompileDate = rv1.value, Enumerator = rv2.value, hash = h.IndexHASHString });


            grdDuplicatedResponses.DataSource = res.ToList().OrderByDescending(o=>o.id);
        }
    }
    protected void grdDuplicatedResponses_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            switch(e.DetailTableView.Name)
            {
                case "DuplicatedResponses":
                    {
                        int formResponseID = Convert.ToInt32(dataItem.GetDataKeyValue("id").ToString());

                        using(GRASPEntities db = new GRASPEntities())
                        {                            
                            int indexID = Convert.ToInt32(ddlIndexes.SelectedValue);

                            string hash = (from h in db.IndexHASHes
                                           where h.FormResponseID == formResponseID && h.IndexID==indexID
                                           select h.IndexHASHString).FirstOrDefault();

                            int ffidCompileDate = 0;
                            int ffidEnumerator = 0;
                            IQueryable<FormField> ffs = from f in db.FormField
                                                        where f.form_id == formID && f.positionIndex < 2
                                                        select f;
                            ffidCompileDate = (int)ffs.Where(f => f.positionIndex == 0).FirstOrDefault().id;
                            ffidEnumerator = (int)ffs.Where(f => f.positionIndex == 1).FirstOrDefault().id;

                            var dupResponses = (from r in db.FormResponse
                                                from rv1 in db.ResponseValue
                                                from rv2 in db.ResponseValue
                                                from h in db.IndexHASHes
                                                where r.id == rv1.FormResponseID && r.id == rv2.FormResponseID && 
                                                       rv1.formFieldId == ffidCompileDate && rv2.formFieldId == ffidEnumerator &&
                                                       r.id == h.FormResponseID && h.FormResponseID != formResponseID && h.IndexHASHString == hash &&
                                                       h.IndexID==indexID
                                                select new { r.id, r.clientVersion, r.senderMsisdn, r.FRCreateDate, CompileDate = rv1.value, Enumerator = rv2.value });

                            e.DetailTableView.DataSource = dupResponses.ToList();
                        }

                        break;
                    }
            }
        }
    }
    protected void ddlIndexes_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        grdDuplicatedResponses.Rebind();
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
    protected void grdDuplicatedResponses_DeleteCommand(object sender, GridCommandEventArgs e)
    {
        int formResponseID = (int)((GridDataItem)e.Item).GetDataKeyValue("id");
        FormResponse.UpdateStatus(formResponseID, 3); //set to rejected
    }
}


/*

 * http://demos.telerik.com/aspnet-ajax/grid/examples/hierarchy/declarative-relations/defaultcs.aspx
http://demos.telerik.com/aspnet-ajax/grid/examples/data-binding/programmatic-hierarchy/defaultcs.aspx

 Query for debugging purposes:

SELECT rv.id,value,formresponseid FRID,formFieldID FROM ResponseValue rv,(
select FR.id FROM FormResponse  FR,
IndexHASHes  IA,
(
SELECT [IndexHASHString]
  FROM [IndexHASHes]
  group by [IndexHASHString]
  having count(*)>1)  H
  WHERE FR.id=IA.FormResponseID AND IA.IndexHASHString=H.IndexHASHString) FR1
WHERE
	rv.FormResponseID=FR1.id
	AND (rv.formfieldid=3 OR rv.formfieldid=17 OR rv.formfieldid=19 OR rv.formfieldid=137 )
ORDER BY formResponseID,FormFieldID








*/