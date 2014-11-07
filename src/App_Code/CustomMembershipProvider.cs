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

using System.Web.Security;
using System.Configuration;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

/// <summary>
/// Contains auxiliary functions for user control
/// </summary>
    public sealed class CustomMembershipProvider : MembershipProvider
    {
        private int newPasswordLength = 8;
        private string eventSource = "CustomMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred in membership. Please check the Event Log.";
        private string connectionString;
        private MachineKeySection machineKey;
        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }
        public static CustomMembershipProvider Instance
        {
            get
            {
                return ((CustomMembershipProvider)System.Web.Security.Membership.Provider);
            }
        }
        public CustomMembershipProvider()
            : base()
        {
        }
        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //
            if (config == null)
                throw new ArgumentNullException("config");
            if (name == null || name.Length == 0)
                name = "CustomMembershipProvider";
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "CustomMembershipProvider - Membership Provider");
            }
            // Initialize the abstract base class.
            base.Initialize(name, config);
            pApplicationName = GetConfigValue(config["applicationName"],
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));
            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }
            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
            //
            // Initialize Connection.
            //
            /*
            ConnectionStringSettings ConnectionStringSettings =
                ConfigurationManager.ConnectionStrings[config["ManPro_Memeberships_Roles"]];
            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }
            connectionString = ConnectionStringSettings.ConnectionString;
            */
            //connectionString = ConfigurationManager.AppSettings["ManPro_Memeberships_Roles"];
            connectionString = ConfigurationManager.ConnectionStrings["GRASP_MemberShip"].ConnectionString;
            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
                WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                                            "are not supported with auto-generated keys.");
        }


        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }

        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }
        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }
        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }
        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }
        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }
        private int pMinRequiredNonAlphanumericCharacters;
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }
        private int pMinRequiredPasswordLength;
        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }
        private string pPasswordStrengthRegularExpression;
        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
                return false;

            ValidatePasswordEventArgs args =
                new ValidatePasswordEventArgs(username, newPwd, true);
            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("UPDATE T_Users " +
                            " SET User_Password = @Password " +
                            " WHERE User_Email = @Username", conn);
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = EncodePassword(newPwd);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public override bool ChangePasswordQuestionAndAnswer(string username,
                                    string password,
                                    string newPwdQuestion,
                                    string newPwdAnswer)
        {
            throw new NotSupportedException();
        }

        //*******************************************************************  CREATE USER OVERRIDE
        public override MembershipUser CreateUser(string username,
                                                     string password,
                                                     string email,
                                                     string passwordQuestion,
                                                     string passwordAnswer,
                                                     bool isApproved,
                                                     object providerUserKey,
                                                     out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (RequiresUniqueEmail && GetUserNameByEmail(email).Length > 0)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            MembershipUser u = GetUser(username, false);
            if (u == null)
            {
                DateTime createDate = DateTime.Now;
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("INSERT INTO Users " +
                            " (UserEmail, userPwd, roleID, clientID, userCreateDate)" +
                            " Values (@UserEmail, @userPwd, @roleID, @clientID, @userCreateDate)", conn);

                cmd.Parameters.Add("@UserEmail", SqlDbType.NVarChar, 50).Value = email;                
                cmd.Parameters.Add("@userPwd", SqlDbType.NVarChar, 15).Value = EncodePassword(password);
                cmd.Parameters.Add("@userCreateDate", SqlDbType.DateTime).Value = createDate;
                cmd.Parameters.Add("@roleID", SqlDbType.Int,4).Value = HttpContext.Current.Session["R_User_roleID"].ToString();
                cmd.Parameters.Add("@clientID", SqlDbType.Int, 4).Value = HttpContext.Current.Session["R_User_clientID"].ToString();

                try
                {
                    conn.Open();
                    int recAdded = cmd.ExecuteNonQuery();
                    if (recAdded > 0)
                    {
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (SqlException e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateUser");
                    }
                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }
                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return null;
        }
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("DELETE FROM Users " +
                            " WHERE UserEmail = @UserEmail", conn);
            cmd.Parameters.Add("@UserEmail", SqlDbType.NVarChar, 50).Value = username;
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
                return true;
            return false;
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Users", conn);
            MembershipUserCollection users = new MembershipUserCollection();
            SqlDataReader reader = null;
            totalRecords = 0;
            try
            {
                conn.Open();
                totalRecords = (int)cmd.ExecuteScalar();
                if (totalRecords <= 0)
                {
                    return users;
                }
                cmd.CommandText = "SELECT User_Id, User_Name, User_Family, User_Email, User_Phone, User_Password, User_CreateDate, User_LastLogInDate, User_Area " +
                                 " FROM T_Users " +
                                 " ORDER BY Username Asc";
                reader = cmd.ExecuteReader();
                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;
                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }
                    if (counter >= endIndex) { cmd.Cancel(); }
                    counter++;
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers ");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return users;
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }
        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT User_Password FROM T_Users " +
                        " WHERE User_Email = @User_Email", conn);
            cmd.Parameters.Add("@User_Email", SqlDbType.NVarChar, 50).Value = username;
            string password = "";
            string passwordAnswer = "";
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    password = reader.GetString(0);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                throw new MembershipPasswordException("Incorrect password answer.");
            }
            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }
            return password;
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM User_Credential " +
                            " WHERE username = @username", conn);
            cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
            MembershipUser u = null;
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return u;
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT User_Id, User_Name, User_Family, User_Email, User_Phone, User_Password, User_CreateDate, User_LastLogInDate, User_Area" +
                        " FROM T_Users WHERE User_Id = @User_Id", conn);
            cmd.Parameters.Add("@User_Id", SqlDbType.BigInt).Value = providerUserKey;
            MembershipUser u = null;
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return u;
        }
        private MembershipUser GetUserFromReader(SqlDataReader reader)
        {
            int providerUserKey = reader.GetInt32(0);
            string username = reader.GetString(9);
            string email = reader.GetString(1);
            string passwordQuestion = "";
            string comment = reader.GetString(5);
            bool isApproved = true;
            bool isLockedOut = false;
            DateTime creationDate = new DateTime();
            DateTime lastLoginDate = new DateTime();
            DateTime lastActivityDate = lastLoginDate;
            MembershipUser u = new MembershipUser(this.Name,
                                                                                        username,
                                                                                        providerUserKey,
                                                                                        email,
                                                                                        passwordQuestion,
                                                                                        comment,
                                                                                        isApproved,
                                                                                        isLockedOut,
                                                                                        creationDate,
                                                                                        lastLoginDate,
                                                                                        lastActivityDate,
                                                                                        DateTime.MinValue,
                                                                                        DateTime.MinValue);
            return u;
        }
        public override bool UnlockUser(string username)
        {
            throw new NotSupportedException();
        }
        public override string GetUserNameByEmail(string email)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT User_Email" +
                        " FROM T_Users WHERE User_Email = @User_Email", conn);
            cmd.Parameters.Add("@User_Email", SqlDbType.NVarChar, 50).Value = email;
            string username = "";
            try
            {
                conn.Open();
                username = (string)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (username == null)
                username = "";
            return username;
        }
        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }
            if (answer == null && RequiresQuestionAndAnswer)
            {
                throw new ProviderException("Password answer required for password reset.");
            }
            string newPassword =
                System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args =
                new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT User_Password FROM T_Users " +
                         " WHERE User_Email = @User_Email", conn);
            cmd.Parameters.Add("@User_Email", SqlDbType.VarChar, 50).Value = username;
            int rowsAffected = 0;
            string passwordAnswer = "";
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    throw new MembershipPasswordException("Incorrect password answer.");
                }
                SqlCommand updateCmd = new SqlCommand("UPDATE T_Users " +
                        " SET User_Password = @User_Password" +
                        " WHERE User_Email = @User_Email", conn);
                updateCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("@User_Email", SqlDbType.NVarChar, 50).Value = username;
                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM User_Credential " +
                            " WHERE username = @username AND UserDeleteDate IS NULL", conn);
            cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
            SqlDataReader reader = null;
            string pwd = "";
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(4);
                }
                else
                {
                    return false;
                }
                isValid = CheckPassword(password, pwd);
                if (isValid)
                {
                    //Imposto le eventuali variabili di sessione:
                    HttpContext.Current.Session["userID"] = reader.GetInt32(0).ToString();
                    HttpContext.Current.Session["roleID"] = reader.GetInt32(10).ToString();
                    //non verra' piu usata in quanto un utente puo esser associato a piu clienti
                        //if(reader.GetInt32(2)>1)
                        //    HttpContext.Current.Session["clientID"] = reader.GetInt32(3).ToString();
                }
                reader.Close();
                

            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); }
                conn.Close();
            }
            return isValid;
        }
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }
            if (pass1 == pass2)
            {
                return true;
            }
            return false;
        }
        private string EncodePassword(string password)
        {
            string encodedPassword = password;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                        Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(machineKey.ValidationKey);
                    encodedPassword =
                        Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                        Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return password;
        }
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM T_Users " +
                                "WHERE User_Email LIKE @User_Email", conn);
            cmd.Parameters.Add("@User_Email", SqlDbType.NVarChar, 50).Value = usernameToMatch;
            MembershipUserCollection users = new MembershipUserCollection();
            SqlDataReader reader = null;
            try
            {
                conn.Open();
                totalRecords = (int)cmd.ExecuteScalar();
                if (totalRecords <= 0)
                {
                    return users;
                }
                cmd.CommandText = "SELECT User_Id, User_Name, User_Family, User_Email, User_Phone, User_Password, User_CreateDate, User_LastLogInDate, User_Area " +
                    " FROM T_Users " +
                    " WHERE User_Email LIKE @User_Email" +
                    " ORDER BY User_Email Asc";
                reader = cmd.ExecuteReader();
                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;
                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }
                    if (counter >= endIndex) { cmd.Cancel(); }
                    counter++;
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return users;
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return FindUsersByName(emailToMatch, pageIndex, pageSize, out totalRecords);
        }
        
        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;
            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();
            log.WriteEntry(message);
        }
    }
