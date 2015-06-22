using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for Notifications
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class Notifications : System.Web.Services.WebService {

    public Notifications () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// Checks for new incoming forms and return string to the ajax javascript timer function.
    /// </summary>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    [WebMethod]
    public string CheckNewIncomingForms()
    {
        try
        {
            if (GlobalVariables.IsNewFormsArrived)
            {
                GlobalVariables.IsNewFormsArrived = false;
                GlobalVariables.IsNotificationButtonActive = true;
                return "true";
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return "false";
    }
    
}
