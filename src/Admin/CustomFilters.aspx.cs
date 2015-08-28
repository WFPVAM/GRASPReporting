using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class Admin_CustomFilter : System.Web.UI.Page
{
    public string url { get; set; }
    public string ReportName { get; set; }

    private enum FieldValueControls
    {
        txtFilterVal,
        ddlSurveyValues,
        dateFieldValue
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Utility.VerifyAccess(Request))
        {
            Response.Write("<h3>Access Denied</h3>");
            Response.End();
        }

        btnRemoveLastEntry.Enabled = false;

        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            if(!IsPostBack)
            {
                InitializeFilterVariables();
                ddlSQLOperator.Items.Add(new RadComboBoxItem("AND", "AND"));
                ddlSQLOperator.Items.Add(new RadComboBoxItem("OR", "OR"));

                ddlOperator.Items.Add(new RadComboBoxItem("=", "=="));
                ddlOperator.Items.Add(new RadComboBoxItem(">", ">"));
                ddlOperator.Items.Add(new RadComboBoxItem(">=", ">="));
                ddlOperator.Items.Add(new RadComboBoxItem("<", "<"));
                ddlOperator.Items.Add(new RadComboBoxItem("=<", "=<"));
                ddlOperator.Items.Add(new RadComboBoxItem("!=", "!="));

                GRASPEntities db = new GRASPEntities();
                var ffs = (from f in db.FormField
                           where f.form_id == formID && f.type != "SEPARATOR" && f.type != "TRUNCATED_TEXT" &&
                           f.type != "WRAPPED_TEXT" && f.type != "REPEATABLES_BASIC" && f.type != "REPEATABLES"
                           && f.type != "IMAGE"
                           orderby f.positionIndex
                           select new { name = f.name, label = f.label, type = f.type, id = f.id }).Union(
                                       from fe in db.FormFieldExt
                                       where fe.FormID == formID
                                       orderby fe.PositionIndex
                                       select new { name = fe.FormFieldExtName, label = fe.FormFieldExtLabel, type = "SERVERSIDE_CALCULATED", id = fe.FormFieldID.Value });

                ddlFormFields.DataSource = ffs.ToList();
                ddlFormFields.DataBind();

                SetFieldValueControlVisibility(FieldValueControls.txtFilterVal);
                dateFieldValue.Visible = false;
            }
        }
    }

    protected void btnAddFilter_Click(object sender, EventArgs e)
    {
        string fieldID = ddlFormFields.SelectedValue.ToString();
        string op = ddlOperator.SelectedValue.ToString();
        string filterVal;

        RadComboBoxItem selItem = ddlFormFields.SelectedItem;
        Label lbltype = (Label)selItem.FindControl("lblType");
        string fieldType = lbltype.Text;

        //Fill the summary for readbale query:
        if(ddlSurveyValues.Visible)
        {
            filterVal = ddlSurveyValues.Text;
        }
        else if (dateFieldValue.Visible)
        {
            filterVal = dateFieldValue.SelectedDate.Value.ToString("yyyy-MM-dd");
        }
        else
        {
            filterVal = txtFilterVal.Text;       
        }

        //Label fieldLabel = (Label)selItem.FindControl("lblFieldLabel");
        lblFilterSummary.Text += ddlFormFields.Text + " " + ddlOperator.Text + " " + filterVal + "<br/>";

        //ddlSQLOperator.Visible = true;
        ddlSQLOperator.Visible = false;
        ddlFormFields.Width = 210;
        txtFilterVal.Width = 120;
        if(lblFilter.Text.Length != 0)
        {
            //lblFilter.Text += " " + ddlSQLOperator.SelectedValue + " ";
            lblFilter.Text += " OR ";
        }

        switch(fieldType)
        {
            case "NUMERIC_TEXT_FIELD":
                lblFilter.Text += "(formFieldID==" + fieldID + " AND nvalue" + op + filterVal + ")";
                break;
            case "DATE_FIELD":
            case "TEXT_FIELD":
            case "RADIO_BUTTON":
            case "DROP_DOWN_LIST":
                //if(!filterVal.Contains("\""))
                //{
                //    filterVal = filterVal.Replace("\"", "\\\"");
                //}
                if((filterVal.Substring(0) != "\""))
                {
                    filterVal = "\"" + filterVal;
                }

                if((filterVal.Substring(filterVal.Length - 2) != "\""))
                {
                    filterVal = filterVal + "\"";
                }
                lblFilter.Text += "(formFieldID==" + fieldID + " AND value" + op + filterVal + ")";
                break;
            case "SERVERSIDE_CALCULATED":
                lblFilter.Text += "(formFieldID==" + fieldID + " AND nvalue" + op + filterVal + ")";
                break;
            //case "DATE_FIELD":
            //    lblFilter.Text += "(formFieldID==" + fieldID + " AND value" + op + filterVal + ")";
            //    break;
        }
        hdnFilterCount.Value = (Convert.ToInt32(hdnFilterCount.Value) + 1).ToString(); //s3 count
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            GRASPEntities db = new GRASPEntities();

            string filter = lblFilter.Text;
            int filterCount = Convert.ToInt32(hdnFilterCount.Value);
            if(filter.Length != 0)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var respUnion = (from r in db.ResponseValue
                                 from fr in db.FormResponse
                                 where fr.id == r.FormResponseID && fr.parentForm_id == formID
                                 select new { FormResponseID = r.FormResponseID.Value, Value = r.value, nvalue = r.nvalue.Value, formFieldID = r.formFieldId.Value }).Union(
                                 from re in db.ResponseValueExt
                                 from fr in db.FormResponse
                                 where fr.id == re.FormResponseID && fr.parentForm_id == formID
                                 select new { FormResponseID = re.FormResponseID, Value = "", nvalue = re.nvalue.Value, formFieldID = re.FormFieldID.Value });

                var filteredResponseIDs = (from r in respUnion.Where(filter)
                                           group r by r.FormResponseID into grp
                                           where grp.Count() == filterCount
                                           select grp.Key).ToList();

                int c = filteredResponseIDs.Count();
                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                lblTestResult.Text = "Records satisfying your query: <strong>" + c.ToString() + "</strong> &nbsp;&nbsp;&nbsp;[calculated in " + elapsedTime + "]";
                lblTestResult.Visible = true;
                if(c > 0)
                {
                    SetButtonsPanelVisibility(true);
                }
                else
                {
                    SetButtonsPanelVisibility(false);
                }
            }
        }
    }
    protected void btnClearAll_Click(object sender, EventArgs e)
    {
        lblFilter.Text = "";
        lblFilterSummary.Text = "";
        lblTestResult.Text = "";
        hdnFilterCount.Value = "0";
        ddlSQLOperator.Visible = false;
        SetButtonsPanelVisibility(false);
    }
    protected void btnRemoveLastEntry_Click(object sender, EventArgs e)
    {
        //int lastOR = lblFilter.Text.LastIndexOf(" OR ");

        //hdnFilterCount.Value = (Convert.ToInt32(hdnFilterCount.Value) - 1).ToString();
    }
    protected void btnViewData_Click(object sender, EventArgs e)
    {
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            string filter = lblFilter.Text;
            filter = Server.UrlEncode(filter);
            string filterSummary = Server.UrlEncode(Server.HtmlEncode(lblFilterSummary.Text));
            Response.Redirect("ViewFormResponses.aspx?FormID=" + Request["FormID"].ToString() + "&f=" + filter + "&fc=" + hdnFilterCount.Value + "&fs=" + filterSummary);
        }
    }
    protected void btnExportData_Click(object sender, EventArgs e)
    {
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            string filter = lblFilter.Text;
            filter = Server.UrlEncode(filter);
            string filterSummary = Server.UrlEncode(Server.HtmlEncode(lblFilterSummary.Text));
            string url = "ExportData.aspx?FormID=" + Request["FormID"].ToString() + "&f=" + filter + "&fc=" + hdnFilterCount.Value + "&fs=" + filterSummary;
            Response.Redirect(url);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <author>Saad Mansour</author>
    protected void btnApplyToReport_Click(object sender, EventArgs e)
    {
        if (Request["FormID"] != null 
            && Request["FormID"] != "")
        {
            string filter = lblFilter.Text;
            filter = Server.UrlEncode(filter);
            string filterSummary = Server.UrlEncode(Server.HtmlEncode(lblFilterSummary.Text));
            url = "ViewChart.aspx?FormID=" + Request["FormID"]
                  + "&reportName=" + Request["reportName"]
                  + "&reportID=" + Request["reportID"]
                  + "&customFilter=" + filter
                  + "&filterCount=" + hdnFilterCount.Value
                  + "&filterSummary=" + filterSummary;

            Session["IsFiltersChanged"] = true;

            //Calls the close window js function, and passes the url to refresh the report page. 
            ScriptManager.RegisterStartupScript(this, GetType(), "close", "closeWindow('" + url + "');", true);
        }
    }


    protected void ddlFormFields_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        int formID = 0;
        int count = 0;
        double? max;
        double? min;
        int countNull = 0;
        RadComboBoxItem selItem = ddlFormFields.SelectedItem;
        Label lbltype = (Label)selItem.FindControl("lblType");
        string fieldType = lbltype.Text;
        int formFieldID = Convert.ToInt32(e.Value);

        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
        }

        using(GRASPEntities db = new GRASPEntities())
        {
            switch(fieldType)
            {
                case "NUMERIC_TEXT_FIELD":
                    litFieldInfo.Text ="<strong>" + e.Text + "</strong> is a Numeric field<br/>";
                    count = (from ffr in db.FormFieldResponses
                                     where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID
                                     select ffr.nvalue).Count();
                    max = (from ffr in db.FormFieldResponses
                                   where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue != null
                                   select ffr.nvalue).Max();
                    min = (from ffr in db.FormFieldResponses
                                   where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue != null
                                   select ffr.nvalue).Min();
                    countNull = (from ffr in db.FormFieldResponses
                                         where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue == null
                                         select ffr.nvalue).Count();
                    litFieldInfo.Text += "<br/><label>Total Values: </label>" + count.ToString();
                    litFieldInfo.Text += "<br/><label>Max Value: </label>" + max.Value.ToString();
                    litFieldInfo.Text += "<br/><label>Min Value: </label>" + min.Value.ToString();
                    litFieldInfo.Text += "<br/><label>Null Values: </label>" + countNull.ToString();

                    SetFieldValueControlVisibility(FieldValueControls.txtFilterVal);
                    break;
                case "DATE_FIELD": //<author>Saad Mansour</author>
                    //Set the right yellow label information.
                    litFieldInfo.Text = "<strong>" + e.Text + "</strong> is a Date field. The Date Format is yyyy-MM-dd For Example: 2015-01-20<br/>";

                    //Finds the occurrences of the date field.
                    count = FormFieldResponses.GetFieldCount(formID, formFieldID);
                    string max2 = FormFieldResponses.GetFieldMaxValue(formID, formFieldID);
                    string min2 = FormFieldResponses.GetFieldMinValue(formID, formFieldID);
                    countNull = FormFieldResponses.GetFieldNullValuesCount(formID, formFieldID);

                    litFieldInfo.Text += "<br/><label>Total Values: </label>" + count.ToString();
                    litFieldInfo.Text += "<br/><label>Max Value: </label>" + (max2 != null ? max2 : "");
                    litFieldInfo.Text += "<br/><label>Min Value: </label>" + (min2 != null ? min2 : "");
                    litFieldInfo.Text += "<br/><label>Null Values: </label>" + countNull.ToString();

                    SetFieldValueControlVisibility(FieldValueControls.dateFieldValue);
                    break;
                case "TEXT_FIELD":
                    litFieldInfo.Text = "<strong>" + e.Text + "</strong> is associated to a free text field<br/>";
                    count = (from ffr in db.FormFieldResponses
                                     where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID
                                     select ffr).Count();

                    litFieldInfo.Text += "<br/>Top Five Responses:<ul>";
                    var topFive = (from ffr in db.FormFieldResponses
                                   where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID
                                   select ffr).GroupBy(g => g.value).Select(group => new
                                   {
                                       value = group.Key,
                                       count = group.Count()
                                   }).OrderByDescending(o => o.count).Take(5);
                    foreach(var tf in topFive.AsParallel())
                    {
                        litFieldInfo.Text += "<li><label>" + tf.value + " </label>(" + tf.count.ToString() + ")</li>";
                    }
                    litFieldInfo.Text += "</ul>";

                    SetFieldValueControlVisibility(FieldValueControls.txtFilterVal);
                    break;
                case "RADIO_BUTTON":
                case "DROP_DOWN_LIST":

                    litFieldInfo.Text = "<strong>" + e.Text + "</strong> is associated to a survey<br/>";
                    count = (from ffr in db.FormFieldResponses
                                     where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID
                                     select ffr.nvalue).Count();
                    litFieldInfo.Text += "<br/><label>Total Values: </label>" + count.ToString();
                    litFieldInfo.Text += "<br/>Survey List:<ul>";
                    var surveyList = from sl in db.SurveyListAPI
                                     from ff in db.FormField
                                     where ff.survey_id == sl.id && ff.id == formFieldID
                                     select new { sl.value };
                    foreach(var s in surveyList.AsParallel())
                    {
                        count = (from ffr in db.FormFieldResponses
                                 where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.value== s.value
                                 select ffr).Count();
                        litFieldInfo.Text += "<li><label>" + s.value + " </label>(" + count.ToString() + ")</li>";
                    }
                    litFieldInfo.Text += "</ul>";
                    ddlSurveyValues.DataSource = surveyList.ToList();
                    ddlSurveyValues.DataBind();

                    SetFieldValueControlVisibility(FieldValueControls.ddlSurveyValues);
                    break;
                case "SERVERSIDE_CALCULATED":
                                        litFieldInfo.Text ="<strong>" + e.Text + "</strong> is a Numeric field<br/>";
                    count = (from ffr in db.FormFieldResponses
                                     where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID
                                     select ffr.nvalue).Count();
                    max = (from ffr in db.FormFieldResponses
                                   where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue != null
                                   select ffr.nvalue).Max();
                    min = (from ffr in db.FormFieldResponses
                                   where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue != null
                                   select ffr.nvalue).Min();
                    countNull = (from ffr in db.FormFieldResponses
                                         where ffr.parentForm_id == formID && ffr.formFieldId == formFieldID && ffr.nvalue == null
                                         select ffr.nvalue).Count();

                    litFieldInfo.Text += "<br/><label>Total Values: </label>" + count.ToString();
                    litFieldInfo.Text += "<br/><label>Max Value: </label>" + max.Value.ToString();
                    litFieldInfo.Text += "<br/><label>Min Value: </label>" + min.Value.ToString();
                    litFieldInfo.Text += "<br/><label>Null Values: </label>" + countNull.ToString();

                    SetFieldValueControlVisibility(FieldValueControls.txtFilterVal);
                    break;
            }

        }

    }

    /// <summary>
    /// Sets the visibility of the field value control, and hide the other controls.
    /// </summary>
    /// <param name="visibileControl"></param>
    /// <author>Saad Mansour</author>
    private void SetFieldValueControlVisibility(FieldValueControls visibileControl)
    {
        if (visibileControl == FieldValueControls.txtFilterVal)
        {
            txtFilterVal.Visible = true;
            reqTextField.Visible = true;
            ddlSurveyValues.Visible = false;
            dateFieldValue.Visible = false;
            reqDate.Visible = false;
        }
        else if (visibileControl == FieldValueControls.ddlSurveyValues)
        {
            txtFilterVal.Visible = false;
            reqTextField.Visible = false;
            ddlSurveyValues.Visible = true;
            dateFieldValue.Visible = false;
            reqDate.Visible = false;
        }
        else //dateFieldValue
        {
            txtFilterVal.Visible = false;
            reqTextField.Visible = false;
            ddlSurveyValues.Visible = false;
            dateFieldValue.Visible = true;
            reqDate.Visible = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isVisible"></param>
    /// <author>Saad Mansour</author>
    private void SetButtonsPanelVisibility(bool isVisible)
    {
        pnlExportView.Visible = isVisible;

        //Hide the ApplyToReport button if the form isn't called from the report page.
        if (isVisible
            && Request["reportID"] == null)
            btnApplyToReport.Visible = false;
    }

    private void InitializeFilterVariables()
    {
        Report objReport = (Report)Session["ReportObject"];
        if (Request["reportID"] != null
            && objReport != null) //Called from report, and report object is not null.
        {
            hdnFilterCount.Value = !string.IsNullOrEmpty(objReport.FiltersCount.ToString()) ? 
                objReport.FiltersCount.ToString() : "0";
            lblFilterSummary.Text = Server.HtmlDecode(Server.UrlDecode(objReport.FiltersSummary));
            lblFilter.Text = objReport.Filters;
        }
        else
        {
            hdnFilterCount.Value = "0";
            lblFilterSummary.Text = "";
            lblFilter.Text = "";
        }
    }
}