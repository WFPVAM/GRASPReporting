/*
 *GRASP(Geo-referential Real-time Acquisition Statistics Platform) Reporting Tool <http://www.brainsen.com>
 * Developed by Brains Engineering s.r.l (marco.giorgi@brainsen.com)
 * This file is part of GRASP Reporting Tool.  
 *  GRASP Reporting Tool is free software: you can redistribute it and/or modify it
 *  under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or (at
 *  your option) any later version.  
 *  GRASP Reporting Tool is distributed in the hope that it will be useful, but
 *  WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser  General Public License for more details.  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GRASP Reporting Tool. 
 *  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
/// <summary>
/// Home Page
/// </summary>
public partial class Admin_Dashboard : System.Web.UI.Page
{
    /// <summary>
    /// When HomePage loads APK informations are taken to display in the panel.
    /// Other informations are taken from the file created in the page settings.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string path = Server.MapPath("~/public/GraspMobile.apk");
        FileInfo fi1 = new FileInfo(path);

        if(fi1.Exists)
        {
            ltrlInfo.Text = "Last Update: <i>" + fi1.LastWriteTime.ToString("dd MMMM yyyy") + "</i>";
            long Size = fi1.Length;
            double dim = (Convert.ToDouble(Size) / 1024f) / 1024f;
            ltrlInfo.Text += "<br />Dimension: <i>" + dim.ToString("N2") + " MB</i>";

        }

        string pathFileInfo = Server.MapPath("../Public/InfoHP.txt");
        if(File.Exists(pathFileInfo))
        {
            using(StreamReader sr = new StreamReader(pathFileInfo))
            {
                Literal1.Text = sr.ReadToEnd();
            }
        }

        PnlResponseProcessing.Visible = false;

        string loggedUser = HttpContext.Current.User.Identity.Name.ToString().ToUpper();
        string roleUser = User_Credential.getRoleForUser(loggedUser);
        if(roleUser == "SuperAdministrator")
        {
            PnlResponseProcessing.Visible = true;
        }

        RadProgressArea.Localization.UploadedFiles = "Processed Response: ";
        RadProgressArea.Localization.CurrentFileName = "-";
        RadProgressArea.Localization.TotalFiles = "Total Response: ";
        RadProgressArea.Localization.Uploaded = "";
        RadProgressArea.Localization.Total = "";
        RadProgressArea.Localization.TransferSpeed = "";
        string[] files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
        if(files.Length > 0)
        {
            BtnProcessIncomingResponse.Enabled = true;
            LitIncomingInfo.Text = "New Form Response(s): " + files.Length;
        }
        else
        {
            BtnProcessIncomingResponse.Enabled = false;
            LitIncomingInfo.Text = "All Responses have been processed.";
        }

    }
    protected void BtnProcessIncomingResponse_Click(object sender, EventArgs e)
    {
        string[] files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
        double step = 100.0000 / (double)files.Length;

        RadProgressContext ProgressContex = RadProgressContext.Current;
        ProgressContex.PrimaryTotal = files.Length;
        ProgressContex.PrimaryValue = 0;
        ProgressContex.PrimaryPercent = 0;

        IncomingProcessor incomProc = new IncomingProcessor();

        double ms = 0;
        Stopwatch sw = new Stopwatch();

        int formToProc = files.Length;
        if(formToProc > 10)
        {
            formToProc = 10;
        }
        for(int i = 0; i < files.Length; i++)
        {
            sw.Reset();
            sw.Start();
            string filePath = files[i];
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string senderNo = "+" + fileName.Substring(18);
            using(StreamReader sr = File.OpenText(files[i]))
            {
                string s = sr.ReadToEnd();
                sr.Close();
                incomProc.ProcessResponse(s, senderNo, fileName);
            }

            ProgressContex.CurrentOperationText = "Processing " + fileName;
            ProgressContex.PrimaryValue = (i + 1).ToString();
            ProgressContex.PrimaryPercent = (step * (i + 1)).ToString("00.##");
            sw.Stop();

            TimeSpan ts = sw.Elapsed;
            ms += ts.TotalMilliseconds;
            TimeSpan ts2 = TimeSpan.FromMilliseconds((ms/ (double)(i + 1)) * (files.Length - (i + 1)));
            ts.Add(ts2);

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts2.Hours, ts2.Minutes, ts2.Seconds);
            ProgressContex.TimeEstimated = elapsedTime;
        }
        ProgressContex.CurrentOperationText = "Insert Done. Pleas Wait...";
        Thread.Sleep(2000);
        ProgressContex.CurrentOperationText = "Calculating Indexes. Pleas Wait...";
        Thread.Sleep(1100);
        incomProc.GenerateIndexesHash();
        ProgressContex.CurrentOperationText = "Calculating Server Side Calulated Fields. Please Wait...";
        Thread.Sleep(1100);
        incomProc.GenerateCalculatedField();
        ProgressContex.CurrentOperationText = "Validating User to Response Permission. Please Wait...";
        Thread.Sleep(1100);
        incomProc.GenerateUserToFormResponseAssociation();


        files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
        if(files.Length > 0)
        {
            BtnProcessIncomingResponse.Enabled = true;
            LitIncomingInfo.Text = "New Form Response(s): " + files.Length;
        }
        else
        {
            BtnProcessIncomingResponse.Enabled = false;
            LitIncomingInfo.Text = "All Responses have been processed.";
        }

    }
}