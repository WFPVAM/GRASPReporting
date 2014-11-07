using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CalcEngine;

public partial class Admin_CalculatedFieldInsert : System.Web.UI.Page
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
            int formID = 0;
            if(Request["FormID"] != null && Request["FormID"] != "")
            {
                formID = Convert.ToInt32(Request["FormID"]);

                lnkCalcFieldList.Attributes["href"] = "CalculatedFields.aspx?FormID=" + formID.ToString();

                GRASPEntities db = new GRASPEntities();
                var respValues = (from ff in db.FormField
                                  where ff.form_id == formID && ff.type == "NUMERIC_TEXT_FIELD"
                                  orderby ff.id
                                  select ff
                                 );
                litFields.Text = "<table>";
                foreach(var rv in respValues)
                {
                    litFields.Text = litFields.Text + ("<tr><td class=\"f\">" + rv.name + "</td><td>" + rv.label + "</td></tr>");
                }
                var fieldExt = from ffe in db.FormFieldExt
                               where ffe.FormID == formID
                               orderby ffe.PositionIndex
                               select ffe;
                foreach(var rv in fieldExt)
                {
                    litFields.Text = litFields.Text + ("<tr><td class=\"f\">" + rv.FormFieldExtName + "</td><td>" + rv.FormFieldExtLabel + "</td></tr>");
                }

                litFields.Text = litFields.Text + ("</table>");

                lblMsg.Visible = false;
                pnlSaveOrEdit.Visible = false;
                pnlInsert.Visible = false;
            }
        }
    }
    protected void btnTestFormula_Click(object sender, EventArgs e)
    {
        lblMsg.Text = "";
        lblResult.Text = "";

        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            string formulaResult = CalculateFormulaPL(formID, txtFormula.Text, 10, null, true);
            if(formulaResult.Contains("Error "))
            {
                lblMsg.Text = formulaResult;
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Visible = true;
                pnlSaveOrEdit.Visible = false;
                btnTestFormula.Enabled = true;
                pnlFieldList.Visible = true;
                pnlResult.Visible = false;
            }
            else
            {
                lblResult.Text = formulaResult;
                lblMsg.Text = "Do you want to save the formula?";
                lblMsg.ForeColor = System.Drawing.Color.Black;
                lblMsg.Visible = true;
                pnlSaveOrEdit.Visible = true;
                btnTestFormula.Enabled = false;
                txtFormula.Enabled = false;
                pnlFieldList.Visible = false;
                pnlResult.Visible = true;

            }

        }
    }
    
    protected string CalculateFormulaPL(int formID, string formula, int iteration, int? formFieldExtID, bool test)
    {
        string outResult = "";
        int formFieldID = 0;
        int positionIndex = 0;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("BEGIN TRANSACTION;");

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        GRASPEntities db = new GRASPEntities();

        int i = 1;
        int n = 1;

        if(!test && formFieldExtID != null)
        {
            //Delete all the existing results
            db.Database.ExecuteSqlCommand("DELETE FROM ResponseValueExt WHERE FormFieldExtID = " + formFieldExtID.Value);
            FormFieldExt ffe = (from f in db.FormFieldExt
                                where f.FormFieldExtID == formFieldExtID
                                select f).FirstOrDefault();
            formFieldID = (int)ffe.FormFieldID;
            positionIndex = (int)ffe.PositionIndex;
        }



        var formFields = (from ff in db.FormField
                          where ff.form_id == formID && ff.type == "NUMERIC_TEXT_FIELD"
                          orderby ff.id
                          select new { ff.name }
                         ).Union(
                            from fe in db.FormFieldExt
                            where fe.FormID == formID
                            orderby fe.PositionIndex
                            select new { name = fe.FormFieldExtName });



        var respValues = (from rv in db.FormFieldResponses
                          where rv.parentForm_id == formID && (rv.type == "NUMERIC_TEXT_FIELD" || rv.type == "SERVERSIDE-CALCULATED") && formula.Contains(rv.name)
                          select new { name = rv.name, value = rv.value, formResponseID = rv.FormResponseID }).ToList();

        var frmResponses = db.FormResponse.Where(w => w.parentForm_id == formID).ToList();
        foreach(var r in frmResponses.AsParallel())
        {
            CalcEngine.CalcEngine ce = new CalcEngine.CalcEngine();
            var dct = new CalcDictionary(ce);
            ce.Variables = dct;
            ce.CultureInfo = new System.Globalization.CultureInfo("en-US");
            foreach(var ff in formFields.ToList())
            {
                dct[ff.name] = 0;
            }

            foreach(var rv in respValues.ToList().AsParallel().Where(re => re.formResponseID == r.id))
            {
                if(rv.value.Length > 0)
                {
                    decimal val = 0;
                    Decimal.TryParse(rv.value, out val);
                    object exVal = 0;
                    dct.TryGetValue(rv.name, out exVal);
                    decimal exvald = Convert.ToDecimal(exVal);
                    dct[rv.name] = exvald + val;
                }
            }
            string eval = "";
            try
            {
                eval = ce.Evaluate(formula).ToString();
                if(!test && formFieldExtID != null)
                {
                    //ResponseValueExt.Insert(db, (int)r.id, formFieldExtID.Value, formFieldID, positionIndex, Double.Parse(eval));

                    sb.Append("INSERT INTO ResponseValueExt ([FormResponseID],[FormFieldExtID],[nvalue],[FormFieldID],[PositionIndex]) VALUES " +
                            "(" + r.id.ToString() + "," + formFieldExtID.Value.ToString() + "," + eval + "," + formFieldID.ToString() + "," + positionIndex.ToString() + ");\r\n");

                }

            }
            catch(Exception ex)
            {
                outResult = "Error has occured on formResponse No. " + r.id.ToString() + ", please review the formula and try again.<br/>";
                outResult += ex.Message + "<br>" + ex.StackTrace + "<br>";
                break;
            }
            if(test)
            {
                outResult += "Result on formResponse No. " + r.id.ToString() + " = " + eval + "<br/>";
            }
            i++;
            n++;
            if(iteration != 0 && i == iteration)
                break;
            if(!test && n == 500)
            {
                n = 0;
                //db.SaveChanges();
                sb.AppendLine("COMMIT TRANSACTION;");
                db.Database.ExecuteSqlCommand(sb.ToString());
                sb.Clear();
                sb.AppendLine("BEGIN TRANSACTION;");
            }
        }


        if(!test && !outResult.Contains("Error"))
        {
            //db.SaveChanges();
            sb.AppendLine("COMMIT TRANSACTION;");
            db.Database.ExecuteSqlCommand(sb.ToString());
            sb.Clear();
            outResult = "Formula executed on " + i.ToString() + " record(s)";
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        //Debug.WriteLine("RunTime: " + elapsedTime);
        outResult += "Time taken: [" + elapsedTime + "]";
        return outResult;
    }

    protected void btnGoToSave_Click(object sender, EventArgs e)
    {
        pnlInsert.Visible = true;
        pnlSaveOrEdit.Visible = false;
        lblResult.Text = "";
        lblMsg.Text = "";
        pnlResult.Visible = false;
    }
    protected void btnTestAgain_Click(object sender, EventArgs e)
    {
        pnlSaveOrEdit.Visible = false;
        btnTestFormula.Enabled = true;
        pnlInsert.Visible = false;
        txtFormula.Enabled = true;
        pnlFieldList.Visible = true;
        lblResult.Text = "";
        lblMsg.Text = "";
        pnlResult.Visible = false;
    }
    protected void btnSaveNewField_Click(object sender, EventArgs e)
    {
        int formID = 0;
        int frmRespCount = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            using(GRASPEntities db = new GRASPEntities())
            {
                int positionIndex;
                int formFieldID;

                FormFieldExt lastFFE = (from f in db.FormFieldExt
                                        where f.FormID == formID
                                        select f).OrderByDescending(o => o.FormFieldExtID).FirstOrDefault();
                if(lastFFE != null && lastFFE.PositionIndex > 0)
                {
                    positionIndex = lastFFE.PositionIndex.Value + 1;
                    formFieldID = (int)lastFFE.FormFieldID.Value + 1;
                }
                else
                {
                    positionIndex = ((from f in db.FormField
                                      where f.form_id == formID
                                      select f.positionIndex).Max()) + 1;
                    formFieldID = ((int)(from f in db.FormField
                                         where f.form_id == formID
                                         select f.id).Max()) + 1;
                }
                FormFieldExt ffe = FormFieldExt.Insert(db, formID, formFieldID, positionIndex, txtFormula.Text, txtName.Text, txtLabel.Text);
                db.SaveChanges();
                hdnFormFieldExtID.Value = ffe.FormFieldExtID.ToString();
                frmRespCount = db.FormResponse.Where(r => r.parentForm_id == formID).Count();
            }
            litGenerate.Text = "The formula will be calculated on " + String.Format("{0:0,0}", frmRespCount) + " responses.<br/>" +
                "The operation could take several minutes to finish, depending by the formula complexity and number of responses.<br/>" +
                "<br/>";
            pnlGenerateValues.Visible = true;

            pnlSaveOrEdit.Visible = false;
            pnlFieldList.Visible = false;

        }
    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            int ffeID = Convert.ToInt32(hdnFormFieldExtID.Value);
            string formulaResult = CalculateFormulaPL(formID, txtFormula.Text, 0, ffeID, false);
            if(formulaResult.Contains("Error "))
            {
                lblMsg.Text = formulaResult;
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Visible = true;
            }
            else
            {
                lblResult.Text = formulaResult;
                pnlResult.Visible = true;


                pnlInsert.Visible = false;
                pnlSaveOrEdit.Visible = false;
                pnlFieldList.Visible = false;
                btnGenerate.Enabled = false;
            }
        }
    }

}

