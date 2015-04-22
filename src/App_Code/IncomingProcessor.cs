using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Telerik.Web.UI;

/// <summary>
/// Summary description for IncomingProcessor
/// </summary>
public class IncomingProcessor
{
    public int firstFormResponseID;

    public IncomingProcessor()
    {
        firstFormResponseID = 0;
    }

    public string ProcessResponse(string text, string sender)
    {
        return ProcessResponse(text, sender, "");
    }

    public string ProcessResponse(string text, string sender, string fileName)
    {

        string[,] fieldTypeMapping = null;
        int fIDX = -1;
        int formParentID = 0;
        int formResponseID = 0;
        string tmpNm = "";
        string clientVersion = "";
        int prevFFID = 0;
        int ffIdRoster = 0;
        int repCount = 0; //Uses to count the records of a Repeatable control (Roster or Table). Uses: 1- In export to write each record in a new row.
        bool previousRoster = false;
        string fileContent="";

        try
        {
            if(text.Length != 0)
            {
                byte[] encodedText = Convert.FromBase64String(text);
                Stream stream = new MemoryStream(encodedText);
                GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);

                stream.Position = 0;
                StreamReader sr = new StreamReader(zipStream);
                string tmp = sr.ReadToEnd();
                int index = tmp.IndexOf("</data>?");
                fileContent = tmp.Substring(0, index + 7);
            }
            else
            {
                LogUtils.WriteFileErrorLog(null, fileName, text);
                return "ko";
            }
        }
        catch(Exception ex)
        {
            LogUtils.WriteFileErrorLog(ex, fileName, text);
            return "ko";
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(fileContent);
        var nodes = xmlDoc.SelectNodes("data");

        try
        {
            using(GRASPEntities db = new GRASPEntities())
            {
                foreach(XmlNode childrenNode in nodes)
                {
                    foreach(XmlNode child in childrenNode.ChildNodes)
                    {
                        if(child.Name == "id")
                        {
                            formParentID = FormResponse.getFormID(child.InnerText);
                            formResponseID = FormResponse.createFormResponse(formParentID, sender, "");

                            fieldTypeMapping = FormField.getFormFieldTypeMap(formParentID); //idx= 0:name; 1:id; 2:type; 3:positionIndex

                            if(formResponseID == 0)
                            {
                                return "ko";
                            }
                            if(firstFormResponseID == 0)
                            {
                                firstFormResponseID = formResponseID;
                            }
                        }
                        else if(child.Name.Contains('_'))
                        {
                            string[] tmpSplit = child.Name.Split('_');
                            tmpNm = tmpSplit[0];
                            if(tmpSplit.Length > 2)
                            {
                                for(int k = 1; k < tmpSplit.Length - 1; k++)
                                    tmpNm += "_" + tmpSplit[k];
                            }
                            fIDX = -1;
                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                            {
                                if(tmpNm == fieldTypeMapping[i, 0])
                                {
                                    fIDX = i;
                                    break;
                                }
                            }
                        }
                        else
                        {

                            fIDX = -1;
                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                            {
                                if(tmpNm == fieldTypeMapping[i, 0])
                                {
                                    fIDX = i;
                                    break;
                                }
                            }
                        }

                        if(fIDX != -1)
                        {
                            switch(fieldTypeMapping[fIDX, 2])
                            {
                                case "REPEATABLES_BASIC": //Roster
                                    if (prevFFID == fIDX)
                                        repCount++;
                                    else
                                        repCount = 1;
                                    
                                    InsertRepeatableData(db, child, fieldTypeMapping, formResponseID, repCount);

                                    prevFFID = fIDX;
                                    previousRoster = true;
                                    break;
                                case "REPEATABLES": //Table
                                    repCount = 0;
                                    foreach(XmlNode rChild in child.ChildNodes)
                                    {
                                        InsertRepeatableData(db, rChild, fieldTypeMapping, formResponseID, ++repCount);
                                    }
                                    
                                    prevFFID = fIDX;
                                    previousRoster = true;
                                    break;
                                case "IMAGE":
                                    //Added by Rushdi on 30-SEP-2014
                                    if(child.InnerText.Contains("/instances"))
                                    {
                                        child.InnerText = sender.Replace('+', ' ').Trim() + "\\" + child.InnerText.Split('/').Last();
                                    }
                                    //------------------------------
                                    string imageFilePath = string.Empty;
                                    if (child.InnerText.Contains("\\"))
                                    {
                                        string imageFileName = Path.GetFileNameWithoutExtension(child.InnerText.Split('\\')[1]);
                                        //fileName PartialDmgGaza_2014-09-18_10-59-04_101
                                        imageFilePath = Utility.GetGRASPImagesVirtualDirectory() + GetImageFileFullPath(fileName, imageFileName);
                                    }
                                    else
                                        imageFilePath = Utility.GetGRASPImagesVirtualDirectory() + Utility.GetImagesFolderName() + "\\" + child.InnerText;

                                    ResponseValue.createResponseValue(db, imageFilePath, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                    break;

                                case "GEOLOCATION":
                                    if(!string.IsNullOrEmpty(child.InnerText))
                                    {
                                        ResponseValue.createResponseValue(db, child.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                        FormResponseCoord.createFormResponseCoord(child.InnerText, formResponseID);
                                    }
                                    break;
                                case "NUMERIC_TEXT_FIELD":
                                    child.InnerText = Utility.GetIntegerNumberFromString(child.InnerText);
                                    ResponseValue.createResponseValue(db, child.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0, "NUMERIC_TEXT_FIELD");
                                    break;
                                default:
                                    if(previousRoster)
                                    {
                                        //file.WriteLine(repCount.ToString() + "," + formResponseID + "," + prevFFID + ", -1");
                                        ResponseValue.createResponseValue(db, repCount.ToString(), formResponseID, Convert.ToInt32(fieldTypeMapping[prevFFID, 1]), Convert.ToInt32(fieldTypeMapping[prevFFID, 3]), -1);
                                    }
                                    //file.WriteLine(child.InnerText + "," + formResponseID + "," + Convert.ToInt32(fieldTypeMapping[fIDX, 1]) + ", 0");
                                    string valueToInsert = "";
                                    if(child.InnerText != null && child.InnerText.Length > 4000)
                                    {
                                        valueToInsert = child.InnerText.Substring(0, 3999);
                                    }
                                    else
                                    {
                                        valueToInsert = child.InnerText;
                                    }
                                    ResponseValue.createResponseValue(db, valueToInsert, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                    if(tmpNm == "client_version")
                                        clientVersion = child.InnerText;
                                    prevFFID = fIDX;
                                    previousRoster = false;
                                    break;
                            }
                        }
                    }
                    if(previousRoster) //Add a record for the roster or table root field.
                    {
                        ResponseValue.createResponseValue(db, repCount.ToString(), formResponseID, Convert.ToInt32(fieldTypeMapping[prevFFID, 1]), Convert.ToInt32(fieldTypeMapping[prevFFID, 3]), -1);
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    string exLog = "";
                    if(ex is DbEntityValidationException)
                    {
                        DbEntityValidationException dbEx = (DbEntityValidationException)ex;
                        foreach(var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach(var validationError in validationErrors.ValidationErrors)
                            {
                                exLog += "Property: " + validationError.PropertyName + "  Error: " + validationError.ErrorMessage + "\r\n";
                            }
                        }
                    }
                    else
                    {
                        exLog = ex.Message + "\r\n" + ex.StackTrace;
                    }
                    string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
                    if(!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
                    //file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //file.WriteLine(text);
                    file.WriteLine("____________________________________________________________________________");
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error on " + fileName);
                    file.WriteLine(exLog);
                    file.WriteLine("____________________________________________________________________________");
                    file.Close();

                    db.Database.ExecuteSqlCommand("DELETE FormResponse WHERE id=" + formResponseID);
                    Utility.SaveErrorResponse(fileContent, fileName);
                    if(fileName.Length > 0)
                    {
                        File.Delete(Utility.GetResponseFilesFolderName() + "incoming\\" + fileName);
                    }
                    return "ko";
                }

                FormResponse.updateClientVersion(formResponseID, clientVersion);
                //Index.GenerateIndexesHASH(formParentID, formResponseID);
                //ServerSideCalculatedField.GenerateSingle(formParentID, formResponseID);
                //UserToFormResponses.GenerateAssociationForAllUsers(formParentID, formResponseID);

                //SaveProcessedResponse(fileContent, formResponseID.ToString().PadLeft(9, '0'));
                SaveProcessedResponse(fileContent, fileName);
                if(fileName.Length > 0)
                {
                    File.Delete(Utility.GetResponseFilesFolderName() + "incoming\\" + fileName);
                }
                return "ok";
            }
        }
        catch(Exception ex)
        {
            LogUtils.WriteFileErrorLog(ex, fileName, fileContent);
            return "ko";
        }
        finally
        {
            //file.Close();
        }
    }

    /// <summary>
    /// Converts the responses in a Dictionary, so for each of its elements a ResponseValue is created.
    /// </summary>
    /// <param name="val">A string representing the responses for a roster or a table</param>
    /// <param name="ffields">The Dictionary representing all the fields for a form (Key = field_name, value = field_id)</param>
    /// <param name="key">The name of the roster field</param>
    /// <param name="formResponseID">The id representing the form Response</param>
    /// <param name="rc">An int representing the repetition count for this roster/table</param>
    public void InsertRepeatableData(GRASPEntities db, XmlNode val, string[,] fieldTypeMapping, int formResponseID, int repCount)
    {
        foreach (XmlNode node in val)
        {
            int fIDX = 0;
            for (int i = 0; i < fieldTypeMapping.GetLength(0); i++)
            {
                if (node.Name == fieldTypeMapping[i, 0])
                {
                    fIDX = i;
                    break;
                }
            }
            if (fIDX != 0)
            {
                if (fieldTypeMapping[fIDX, 2] == GeneralEnums.FieldTypes.NUMERIC_TEXT_FIELD.ToString())
                {
                    node.InnerText = Utility.GetIntegerNumberFromString(node.InnerText);
                    ResponseValue.createResponseValue(db, node.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), repCount, GeneralEnums.FieldTypes.NUMERIC_TEXT_FIELD.ToString());
                }else
                    ResponseValue.createResponseValue(db, node.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), repCount);
            }
        }
    }

    private string GetImageFileFullPath(string formNameWithSender, string imageFileName)
    {
        try
        {
            //PartialDmgGaza_2014-09-18_10-59-04_101
            string senderNumber = formNameWithSender.GetSubstringAfterLastChar('_');
            string formName = formNameWithSender.Split('_')[0];
            string formInstanceName = formNameWithSender.GetSubstringBeforeLastChar('_');
            string imageFilePath = Utility.GetImagesFolderName() + Utility.GetFilePathSeparator()
                + senderNumber + Utility.GetFilePathSeparator()
                + formName + Utility.GetFilePathSeparator()
                + formInstanceName + Utility.GetFilePathSeparator()
                + imageFileName + Utility.GetImageFileType();
            return imageFilePath;
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.Message);
            return null;
        }
    }

    public static void ProcessIncommingForms(RadButton btnProcessIncomingResponse, Literal litIncomingInfo)
    {
        bool concurrentCalls = false; //whether the process called while it is already running.
        try
        {
            if (GlobalVariables.IsProcessIncomingRunning) //whether the processing is already running.
            {
                if (btnProcessIncomingResponse != null)
                {
                    btnProcessIncomingResponse.Enabled = false;
                    litIncomingInfo.Text = "Processing is already Running. Please, wait.";
                }
                concurrentCalls = true;
                return;
            }

            GlobalVariables.IsProcessIncomingRunning = true;
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
            if (formToProc > 10)
            {
                formToProc = 10;
            }
            for (int i = 0; i < files.Length; i++)
            {
                sw.Reset();
                sw.Start();
                string filePath = files[i];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string senderNo = fileName.GetSubstringAfterLastChar('_');//"+" + fileName.Substring(18);
                using (StreamReader sr = File.OpenText(files[i]))
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
                TimeSpan ts2 = TimeSpan.FromMilliseconds((ms / (double)(i + 1)) * (files.Length - (i + 1)));
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
        }
        catch (Exception ex)
        {
            concurrentCalls = false;
            LogUtils.WriteErrorLog(ex.ToString());
        }
        finally
        {
            if (concurrentCalls == false)
            {
                GlobalVariables.IsProcessIncomingRunning = false;
                CheckProcessIncomingFormsStatus(btnProcessIncomingResponse, litIncomingInfo);
            }
        }
    }

    public static void CheckProcessIncomingFormsStatus(RadButton btnProcessIncomingResponse, Literal litIncomingInfo)
    {
        try
        {
            if (GlobalVariables.IsProcessIncomingRunning)
            {
                if (btnProcessIncomingResponse != null)
                {
                    btnProcessIncomingResponse.Enabled = false;
                    litIncomingInfo.Text = "Processing is already Running. Please, wait.";   
                }
            }
            else
            {
                if (btnProcessIncomingResponse != null)
                {
                    string[] files = Directory.GetFiles(Utility.GetResponseFilesFolderName() + "incoming");
                    if (files.Length > 0)
                    {
                        btnProcessIncomingResponse.Enabled = true;
                        litIncomingInfo.Text = "New Form Response(s): " + files.Length;
                    }
                    else
                    {
                        btnProcessIncomingResponse.Enabled = false;
                        litIncomingInfo.Text = "All Responses have been processed.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }
    }

    public string SaveFileResponse(string data, string sender, string formName)
    {
        string formInstanceName = string.Empty;
        
        try
        {
            string fileType = formName.GetSubstringAfterLastChar('_');
            
            //Checks whether the file is image. The image file name structure is [FormName]_[Date]_[Time]_[ImgaeFileName]_[image]
            // ex.: "PartialDmgGaza_2014-09-18_10-59-04_1411027216475.jpg_image"
            if (!string.IsNullOrEmpty(fileType) 
                && fileType.Equals("image"))
            {
                //get file without _image type
                formName = formName.GetSubstringBeforeLastChar('_'); //"PartialDmgGaza_2014-09-18_10-59-04_1411027216475.jpg"
                //without image name
                formInstanceName = formName.GetSubstringBeforeLastChar('_'); //"PartialDmgGaza_2014-09-18_10-59-04"
                string imageFileName = Path.GetFileNameWithoutExtension(formName.GetSubstringAfterLastChar('_')); //"1411027216475"
                bool imageSavedSucceeded = SaveIncommingImage(data, sender, formInstanceName, imageFileName);
                if (imageSavedSucceeded)
                {
                    return "ok"; //s3* ok
                }else
                    return "ko";
            }

            //string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff_") + sender.Replace("+", "");
            formInstanceName = GetFileName(sender, formName);
            bool isFileAlreadySaved = FormResponseServerStatus.IsFormSaved(formInstanceName); //s3* 
            if (isFileAlreadySaved)
            {
                SaveDuplicateForm(data, formInstanceName);
                return "ok";
            }

            string folderPath = Utility.GetResponseFilesFolderName() + "incoming\\";
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (StreamWriter file = new StreamWriter(folderPath + formInstanceName, true))
            {
                file.Write(data);
                file.Close();
            }

            FormResponseServerStatus.InsertOrUpdateStatus(formInstanceName, true);
            return "ok";
        }
        catch(Exception ex)
        {
            FormResponseServerStatus.InsertOrUpdateStatus(formInstanceName, false);
            string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
            file.WriteLine("____________________________________________________________________________");
            file.WriteLine("------ ERROR from " + sender + " on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            file.WriteLine(ex.Message);
            file.WriteLine(ex.StackTrace);
            file.WriteLine("____________________________________________________________________________");
            file.Close();
            return "ko";
        }
    }

    private bool SaveIncommingImage(string data, string sender, string formInstanceName, string imageFileName)
    {
        try
        {
            string imagesFolderFullPath = Utility.GetImagesFolderFullPath();
            if (!Directory.Exists(imagesFolderFullPath))
            {
                Directory.CreateDirectory(imagesFolderFullPath);
            }
            byte[] imageBytes = Convert.FromBase64String(data);
            //byte[] decompressedImage = Decompress(imageBytes);          
            string incommingImagePathWithFile = GetImagePathWithFileName(sender, formInstanceName, imageFileName);
            if (!File.Exists(incommingImagePathWithFile))
            {
                using (FileStream imageFile = new FileStream(incommingImagePathWithFile, FileMode.Create))
                {
                    imageFile.Write(imageBytes, 0, imageBytes.Length);
                    imageFile.Flush();
                }
            }
            else
            {
                //s3 Duplicate img
            }

            //using (StreamWriter file = new StreamWriter(folderPath + fileName, true))
            //{
            //    file.Write(data);
            //    file.Close();
            //}

            //using (var imageFile = new FileStream(folderPath + "\\" + v.Key + ".jpg", FileMode.Create))
            //{
            //    imageFile.Write(bytes, 0, bytes.Length);
            //    imageFile.Flush();
            //}
            return true;
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.Message);
            return false;
        }
    }

    //private byte[] Decompress(byte[] compressedBytes)
    //{
    //    using (var ms = new MemoryStream())
    //    {
    //        using (var bs = new MemoryStream(compressedBytes))
    //        {
    //            //bs.Seek(0, SeekOrigin.Begin);
    //            using (var z = new GZipStream(bs, CompressionMode.Decompress))
    //            {
    //                //z.Seek(0, SeekOrigin.Begin);
    //                z.CopyTo(ms);
    //            }
    //        }
    //        return ms.ToArray();
    //    }
    //}

    //public static string UnZip(string value)
    //{
    //    //Transform string into byte[]
    //    byte[] byteArray = new byte[value.Length];
    //    int indexBA = 0;
    //    foreach (char item in value.ToCharArray())
    //    {
    //        byteArray[indexBA++] = (byte)item;
    //    }

    //    //Prepare for decompress
    //    System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
    //    System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
    //        System.IO.Compression.CompressionMode.Decompress);

    //    //Reset variable to collect uncompressed result
    //    byteArray = new byte[byteArray.Length];

    //    //Decompress
    //    int rByte = sr.Read(byteArray, 0, byteArray.Length);

    //    //Transform byte[] unzip data to string
    //    System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
    //    //Read the number of bytes GZipStream red and do not a for each bytes in
    //    //resultByteArray;
    //    for (int i = 0; i < rByte; i++)
    //    {
    //        sB.Append((char)byteArray[i]);
    //    }
    //    sr.Close();
    //    ms.Close();
    //    sr.Dispose();
    //    ms.Dispose();
    //    return sB.ToString();
    //}

    ///// <span class="code-SummaryComment"><summary></span>
    ///// Gets the uncompressed image. If the image is compressed, 
    ///// it will be uncompressed first.
    ///// <span class="code-SummaryComment"></summary></span>
    //public Image GetDecompressedImage()
    //{
    //    if (decompressed == null)
    //    {
    //        stream.Seek(0, SeekOrigin.Begin);
    //        decompressed = new Bitmap(stream);
    //    }
    //    return decompressed;
    //}

    ///// <span class="code-SummaryComment"><summary></span>
    ///// Clears the uncompressed image, leaving the compressed one in memory.
    ///// <span class="code-SummaryComment"></summary></span>
    //public void ClearDecompressedImage()
    //{
    //    if (decompressed != null)
    //    {
    //        if (stream == null)
    //        {
    //            stream = new MemoryStream();
    //            decompressed.Save(stream, format);
    //        }
    //        decompressed = null;
    //    }
    //}

    private string GetImagePathWithFileName(string sender, string formInstanceName, string imageFileName)
    {
        try
        {
            //Creates image path. Path Example: GRASPImages\[senderNumber]\[FormName]\[FormInstanceName]\imagefile
            string senderPath = Utility.GetImagesFolderFullPath() + Utility.GetFilePathSeparator() + Utility.GetSenderNumber(sender);
            if (!Directory.Exists(senderPath))
            {
                Directory.CreateDirectory(senderPath);
            }

            string formName = formInstanceName.Split('_')[0]; //"PartialDmgGaza"
            string formNamePath = senderPath + Utility.GetFilePathSeparator() + formName; //GRASPImages\\101\\PartialDmgGaza"
            if (!Directory.Exists(formNamePath))
            {
                Directory.CreateDirectory(formNamePath);
            }

            string imageInstanceFormPath = formNamePath + Utility.GetFilePathSeparator() + formInstanceName;//RASPImages\101\PartialDmgGaza\PartialDmgGaza_2014-09-18_10-59-04
            if (!Directory.Exists(imageInstanceFormPath))
            {
                Directory.CreateDirectory(imageInstanceFormPath);
            }

            string incommingImagePathWithFile = imageInstanceFormPath + Utility.GetFilePathSeparator() + imageFileName + Utility.GetImageFileType();
            return incommingImagePathWithFile; //GRASPImages\\101\\PartialDmgGaza\\PartialDmgGaza_2014-09-18_10-59-04\\1411027216475.jpg"
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.Message);
            return null;
        }
    }

    private bool SaveDuplicateForm(string data, string fileName)
    {
        bool isSuccess = false;

        try
        {
            string folderPath = Utility.GetResponseFilesFolderName() + "duplicate\\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            fileName += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            using (StreamWriter file = new StreamWriter(folderPath + fileName, true))
            {
                file.Write(data);
                file.Close();
            }
            isSuccess = true;
        }
        catch (Exception ex)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
            file.WriteLine("____________________________________________________________________________");
            file.WriteLine("------ ERROR in Saving Duplicate Form Instance: '" + fileName + "' on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            file.WriteLine(ex.Message);
            file.WriteLine(ex.StackTrace);
            file.WriteLine("____________________________________________________________________________");
            file.Close();
        }

        return isSuccess;
    }

    private string GetFileName(string sender, string formName)
    {
        string fileName = formName + "_" + sender.Replace("+", "");
        return fileName;
    }

    public void SaveProcessedResponse(string data, string fileName)
    {
        string folderPath = Utility.GetResponseFilesFolderName() + "processed\\";
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        using(StreamWriter file = new StreamWriter(folderPath + fileName, true))
        {
            file.Write(data);
            file.Close();
        }
    }

    public void GenerateIndexesHash()
    {
        if(firstFormResponseID > 0)
        {
            Index.GenerateIndexesHASH(this.firstFormResponseID);
        }
    }

    public void GenerateCalculatedField()
    {
        if(firstFormResponseID > 0)
        {
            ServerSideCalculatedField.GenerateFrom(this.firstFormResponseID);
        }
    }

    public void GenerateUserToFormResponseAssociation()
    {
        if(firstFormResponseID > 0)
        {
            UserToFormResponses.GenerateAssociationForAllUsersFrom(this.firstFormResponseID);
        }
    }
}