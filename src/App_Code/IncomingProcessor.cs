using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Xml;

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
        int repCount = 0;
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
                WriteErrorLog(null, fileName, text);
                return "ko";
            }
        }
        catch(Exception ex)
        {
            WriteErrorLog(ex, fileName, text);
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
                                case "REPEATABLES_BASIC":
                                case "REPEATABLES":
                                    if(prevFFID == fIDX)
                                    {
                                        repCount++;
                                        foreach(XmlNode rChild in child.ChildNodes)
                                        {
                                            ffIdRoster = -1;
                                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                                            {
                                                if(rChild.Name == fieldTypeMapping[i, 0])
                                                {
                                                    ffIdRoster = i;
                                                    break;
                                                }
                                            }
                                            if(ffIdRoster != -1)
                                            {
                                                if(fieldTypeMapping[fIDX, 2] == "NUMERIC_TEXT_FIELD")
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount, "NUMERIC_TEXT_FIELD");

                                                }
                                                else
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        repCount = 1;
                                        foreach(XmlNode rChild in child.ChildNodes)
                                        {
                                            ffIdRoster = -1;
                                            for(int i = 0; i < fieldTypeMapping.GetLength(0); i++)
                                            {
                                                if(rChild.Name == fieldTypeMapping[i, 0])
                                                {
                                                    ffIdRoster = i;
                                                    break;
                                                }
                                            }
                                            if(ffIdRoster != -1)
                                            {
                                                if(fieldTypeMapping[fIDX, 2] == "NUMERIC_TEXT_FIELD")
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount, "NUMERIC_TEXT_FIELD");

                                                }
                                                else
                                                {
                                                    ResponseValue.createResponseValue(db, rChild.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[ffIdRoster, 1]), Convert.ToInt32(fieldTypeMapping[ffIdRoster, 3]), repCount);
                                                }
                                            }
                                        }
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

                                    string imagePthValue = Utility.GetImageFolderName() + "\\" + child.InnerText;
                                    ResponseValue.createResponseValue(db, imagePthValue, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                    break;

                                case "GEOLOCATION":
                                    if(!string.IsNullOrEmpty(child.InnerText))
                                    {
                                        ResponseValue.createResponseValue(db, child.InnerText, formResponseID, Convert.ToInt32(fieldTypeMapping[fIDX, 1]), Convert.ToInt32(fieldTypeMapping[fIDX, 3]), 0);
                                        FormResponseCoord.createFormResponseCoord(child.InnerText, formResponseID);
                                    }
                                    break;
                                case "NUMERIC_TEXT_FIELD":
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
                    if(previousRoster)
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
                    SaveErrorResponse(fileContent, fileName);
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

                SaveProcessedResponse(fileContent, formResponseID.ToString().PadLeft(9, '0'));
                if(fileName.Length > 0)
                {
                    File.Delete(Utility.GetResponseFilesFolderName() + "incoming\\" + fileName);
                }
                return "ok";
            }
        }
        catch(Exception ex)
        {
            WriteErrorLog(ex, fileName, fileContent);
            return "ko";
        }
        finally
        {
            //file.Close();
        }
    }

    public string SaveFileResponse(string data, string sender)
    {
        try
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff_") + sender.Replace("+", "");
            string folderPath = Utility.GetResponseFilesFolderName() + "incoming\\";
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using(StreamWriter file = new StreamWriter(folderPath + fileName, true))
            {
                file.Write(data);
                file.Close();
            }
            return "ok";
        }
        catch(Exception ex)
        {
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

    public void SaveErrorResponse(string data, string fileName)
    {
        string folderPath = Utility.GetResponseFilesFolderName() + "error\\";
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

    public void WriteErrorLog(Exception ex, string fileName, string fileContent)
    {
        string folderPath = HttpContext.Current.Server.MapPath("~/LogFiles/");
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        StreamWriter file = new StreamWriter(folderPath + "\\MobileConnection.txt", true);
        file.WriteLine("____________________________________________________________________________");
        file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error on " + fileName);
        if(ex != null)
        {
            file.WriteLine(ex.Message);
            file.WriteLine(ex.StackTrace);
        }
        file.WriteLine("____________________________________________________________________________");
        file.Close();

        SaveErrorResponse(fileContent, fileName);
        if(fileName.Length > 0)
        {
            File.Delete(Utility.GetResponseFilesFolderName() + "incoming\\" + fileName);
        }
    }

}