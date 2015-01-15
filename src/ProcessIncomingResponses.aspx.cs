using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProcessIncomingResponses : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ProcessIncoming();
    }

    private bool ProcessIncoming()
    {
        try
        {
            string[] files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
            IncomingProcessor incomProc = new IncomingProcessor();

            for(int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string senderNo = "+" + fileName.Substring(18);
                using(StreamReader sr = File.OpenText(files[i]))
                {
                    string s = sr.ReadToEnd();
                    sr.Close();
                    incomProc.ProcessResponse(s, senderNo, fileName);
                }
            }

            return true;
        }
        catch(Exception ex)
        {
            Response.Write(ex.Message + "<br/><hr/>" + ex.StackTrace);
            return false;
        }
    }
}