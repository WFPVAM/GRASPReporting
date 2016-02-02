using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class ProcessIncomingResponses : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Check whether it is called from the local Server and from Sceduler Task to process incoming forms.
        if (HttpContext.Current.Request.IsLocal 
            && Request["method"] != null
            && Request.QueryString["method"].Equals("pif"))
        {
            IncomingProcessor.ProcessIncommingForms(null, null);
        }
    }
}