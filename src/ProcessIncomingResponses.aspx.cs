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

    //private bool ProcessIncomingForms()
    //{
    //    try
    //    {
    //        //if (GlobalVariables.IsProcessIncomingRunning)
    //        //{
    //        //    return;
    //        //}

    //        //GlobalVariables.IsProcessIncomingRunning = true;
    //        string[] files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
    //        IncomingProcessor incomProc = new IncomingProcessor();

    //        for(int i = 0; i < files.Length; i++)
    //        {
    //            string filePath = files[i];
    //            string fileName = Path.GetFileNameWithoutExtension(filePath);
    //            string senderNo = fileName.GetSubstringAfterLastChar('_'); //"+" + fileName.Substring(18);
    //            using(StreamReader sr = File.OpenText(files[i]))
    //            {
    //                string s = sr.ReadToEnd();
    //                sr.Close();
    //                incomProc.ProcessResponse(s, senderNo, fileName);
    //            }
    //        }

    //        return true;
    //    }
    //    catch(Exception ex)
    //    {
    //        LogUtils.WriteErrorLog(ex.Message);//Response.Write(ex.Message + "<br/><hr/>" + ex.StackTrace);
    //        return false;
    //    }
    //}

    //private void CreateTask()
    //{
    //    // Get the service on the local machine
    //    using (TaskService ts = new TaskService())
    //    {
    //        // Create a new task definition and assign properties
    //        TaskDefinition td = ts.NewTask();
    //        td.RegistrationInfo.Description = "Does something";

    //        // Create a trigger that will fire the task at this time every other day
    //        td.Triggers.Add(new Microsoft.Win32.TaskScheduler.TimeTrigger(new DateTime(2015, 3, 9, 10, 30, 52)));

    //        // Create an action that will launch Notepad whenever the trigger fires
    //        td.Actions.Add(new ExecAction("powershell.exe", "-command (new-object system.net.webclient).downloadstring('http://localhost/GRASPReporting/ProcessIncomingResponses.aspx/?method=pif')", null));

    //        // Register the task in the root folder
    //        ts.RootFolder.RegisterTaskDefinition(@"WPD", td);

    //        // Remove the task we just created
    //        //ts.RootFolder.DeleteTask("Test");
    //    }
    //}
}