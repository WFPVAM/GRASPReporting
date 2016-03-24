using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class Admin_IndexManagement : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if(Utility.VerifyAccess(Request))
        {
            if(!IsPostBack)
            {
                grdIndex.Visible = false;
                grdIndexFields.Visible = false;
                pnlIndexDetails.Visible = false;
            }
        }
        else
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }
    }

    #region Grid CRUD Command

    protected void grdIndex_InsertCommand(object source, GridCommandEventArgs e)
    {
        var editableItem = ((GridEditableItem)e.Item);
        //create new entity
        Index idx = new Index();
        //populate its properties
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        idx.IndexName = (string)values["IndexName"];
        idx.IndexCreateDate = DateTime.Now;
        idx.formID = Convert.ToInt32(ddlForms.SelectedValue);

        GRASPEntities db = new GRASPEntities();
        db.Indexes.Add(idx);
        try
        {
            //submit chanages to Db
            db.SaveChanges();
        }
        catch(System.Exception ex)
        {
            SetMessage(ex.Message);
        }
    }

    protected void grdIndex_DeleteCommand(object source, GridCommandEventArgs e)
    {
        var indexID = (int)((GridDataItem)e.Item).GetDataKeyValue("IndexID");

        //retrive entity form the Db
        GRASPEntities db = new GRASPEntities();
        Index idx = (from i in db.Indexes
                     where i.IndexID == indexID
                     select i).FirstOrDefault();

        if(idx != null)
        {
            //add the category for deletion
            db.Indexes.Remove(idx);
            try
            {
                //submit chanages to Db
                db.SaveChanges();
            }
            catch(System.Exception ex)
            {
                SetMessage(ex.Message);
            }
        }
    }
    protected void grdIndex_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if(e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            if(!(e.Item is GridEditFormInsertItem))
            {
                GridEditableItem item = e.Item as GridEditableItem;
                GridEditManager manager = item.EditManager;
                GridTextBoxColumnEditor editor = manager.GetColumnEditor("IndexID") as GridTextBoxColumnEditor;
                editor.TextBoxControl.Enabled = false;
            }
        }
    }
    protected void grdIndex_ItemInserted(object source, GridInsertedEventArgs e)
    {
        if(e.Exception != null)
        {

            e.ExceptionHandled = true;
            SetMessage("Index cannot be inserted. Reason: " + e.Exception.Message);

        }
        else
        {
            SetMessage("New index is inserted!");
        }
    }
    #endregion

    private void DisplayMessage(string text)
    {
        grdIndex.Controls.Add(new LiteralControl(string.Format("<span style='color:red'>{0}</span>", text)));
    }

    private void SetMessage(string message)
    {
        gridMessage = message;
    }

    private string gridMessage = null;

    protected void grdIndex_PreRender(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(gridMessage))
        {
            DisplayMessage(gridMessage);
        }
    }
    protected void grdIndex_SelectedIndexChanged(object sender, EventArgs e)
    {
        int indexID = Convert.ToInt32((grdIndex.SelectedItems[0] as GridDataItem).GetDataKeyValue("IndexID"));
        edsIndexFields.WhereParameters.Clear();
        edsIndexFields.AutoGenerateWhereClause = true;
        //alternatively
        //edsIndexFields.Where = "it.[IndexID] = @IndexID";
        edsIndexFields.WhereParameters.Add("IndexID", TypeCode.Int32, indexID.ToString());
        grdIndexFields.Rebind();
        grdIndexFields.Visible = true;
        pnlGridFields.Visible = true;

        //refresh Update Details Panel
        pnlIndexDetails.Visible = true;
        Index idx = Index.GetIndex(indexID);
        if(idx.IndexLastUpdateDate.HasValue)
        {
            litLastUpdate.Text = "Last update date: " + idx.IndexLastUpdateDate.Value.ToString("yyyy-MM-dd HH:mm") +
                " by " + idx.IndexLastUpdateUserName;
            btnGenerateHASH.Text = "Refresh Index";
        }
        else
        {
            litLastUpdate.Text = "The selected index has not been generated yet.";
            btnGenerateHASH.Text = "Generate Index";
        }
    }
    protected void ddlForms_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        grdIndexFields.Visible = false;
        pnlIndexDetails.Visible = false;
        grdIndex.Visible = true;
        pnlNewIndexHelp.Visible = true;
        grdIndex.Rebind();
    }
    protected void grdIndexFields_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        //if(indexID > 0)
        //{
        //    GRASPEntities db = new GRASPEntities();
        //    var items = from ffidx in db.IndexFields
        //                from ff in db.FormField
        //                where ffidx.FormFieldID == ff.id && ffidx.IndexID == indexID
        //                select new { ffidx.IndexFieldID, ffidx.FormFieldID, ff.name, ff.label };
        //    grdIndexFields.DataSource = items.ToList();
        //    grdIndex.DataBind();
        //}
    }
    protected void grdIndexFields_InsertCommand(object sender, GridCommandEventArgs e)
    {
        var editableItem = ((GridEditableItem)e.Item);
        //populate its properties
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);

        RadComboBox combo = editableItem.FindControl("ddlFormFields") as RadComboBox;
        string ffID = combo.SelectedValue;

        GRASPEntities db = new GRASPEntities();

        //create new entity
        IndexField idx = new IndexField();
        idx.FormFieldID = Convert.ToDecimal(ffID);
        idx.IndexID = Convert.ToInt32(grdIndex.SelectedValue.ToString());

        db.IndexFields.Add(idx);
        try
        {
            //submit chanages to Db
            db.SaveChanges();
        }
        catch(System.Exception ex)
        {
            SetMessage(ex.Message);
        }
    }
    protected void grdIndexFields_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if(e.Item is GridEditFormItem && e.Item.IsInEditMode)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            // access/modify the edit item template settings here
            RadComboBox list = item.FindControl("ddlFormFields") as RadComboBox;
            //decimal ffID = (item.DataItem as IndexField).FormFieldID;

            int formID = Convert.ToInt32(ddlForms.SelectedValue);
            GRASPEntities db = new GRASPEntities();
            List<FormField> ffs = (from f in db.FormField
                                   where f.form_id == formID && f.type != "SEPARATOR" && f.type != "TRUNCATED_TEXT" &&
                                   f.type != "WRAPPED_TEXT" && f.type != "REPEATABLES_BASIC" && f.type != "REPEATABLES"
                                   orderby f.positionIndex
                                   select f).ToList();

            list.DataSource = ffs;
            list.DataBind();

            //if(Session["updatedValue"] != null)
            //{
            //    list.SelectedValue = Session["updatedValue"].ToString();
            //}
        }
        else if(e.Item is GridDataItem && !e.Item.IsInEditMode && Page.IsPostBack)
        {
            //GridDataItem item = e.Item as GridDataItem;
            //Label label = item.FindControl("lblFormFieldName") as Label;
            //// update the label value
            //label.Text = Session["updatedValue"].ToString();
        }
    }
    protected void grdIndexFields_UpdateCommand(object sender, GridCommandEventArgs e)
    {
        GridEditableItem editedItem = e.Item as GridEditableItem;
        RadComboBox list = editedItem.FindControl("ddlFormFields") as RadComboBox;
        Session["updatedValue"] = list.SelectedValue;
    }
    protected void btnGenerateHASH_Click(object sender, EventArgs e)
    {
        string loggedUser = HttpContext.Current.User.Identity.Name.ToString().ToUpper();
        int indexID = Convert.ToInt32(grdIndex.SelectedValue.ToString());
        if(indexID > 0)
        {
            Index.GenerateIndexHASHes(indexID);
            Index.UpdateIndexLastUpdate(indexID, DateTime.Now, loggedUser);
            //TODO: Update lastupdate field, execution time ecc..
            litLastUpdate.Text = "Last update date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " by " + loggedUser;
            btnGenerateHASH.Text = "Refresh Index";
        }
    }
}