using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _uc_ResultMsgBar : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void ShowResultMsg(bool isSuccess)
    {
        if (isSuccess)
        {
            successMsg.Visible = true;
            errorMsg.Visible = false;
        }
        else
        {
            successMsg.Visible = false;
            errorMsg.Visible = true;
        }
    }

    public void ShowResultMsg(bool isSuccess, string msg)
    {
        if (isSuccess)
        {
            litSuccess.Text = msg;
        }else
            litFail.Text = msg;

        ShowResultMsg(isSuccess);
    }
}