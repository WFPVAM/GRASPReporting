using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_CalculatedFields : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void GrdCalcFields_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int formID = 0;
        if(Request["FormID"] != null && Request["FormID"] != "")
        {
            formID = Convert.ToInt32(Request["FormID"]);
            using(GRASPEntities db = new GRASPEntities())
            {
                var formFieldExt = (from ffe in db.FormFieldExt
                                    where ffe.FormID == formID
                                    select ffe).OrderBy(o => o.PositionIndex);
                GrdCalcFields.DataSource = formFieldExt.ToList();
            }

        }
    }

    protected void BtnRecalculate_Command(object sender, CommandEventArgs e)
    {
        Telerik.Web.UI.ButtonCommandEventArgs arg = e as Telerik.Web.UI.ButtonCommandEventArgs;
        int fieldExtID = Convert.ToInt32(arg.CommandArgument.ToString());
        LitMessage.Text = ServerSideCalculatedField.Generate(fieldExtID);

    }

    protected void BtnDelete_Command(object sender, CommandEventArgs e)
    {
        Telerik.Web.UI.ButtonCommandEventArgs arg = e as Telerik.Web.UI.ButtonCommandEventArgs;
        int fieldExtID = Convert.ToInt32(arg.CommandArgument.ToString());

        using(GRASPEntities db = new GRASPEntities())
        {
            db.Database.ExecuteSqlCommand("DELETE FROM FormFieldExt WHERE FormFieldExtID=" + fieldExtID);
            db.Database.ExecuteSqlCommand("DELETE FROM ResponseValueExt WHERE FormFieldExtID=" + fieldExtID);
        }
        GrdCalcFields.Rebind();
    }
}