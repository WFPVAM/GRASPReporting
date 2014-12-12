using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

public class Service : IService
{
    #region "Interface Methods"
    Message IService.Surveys(string formId, string columnNames, string formResponseId)
    {
        List<object> listReturn = null;
        List<string> listColumnNames = null;

        if (string.IsNullOrEmpty(formId))
        {
            throw new WebFaultException<string>("The ID Parent Form is missing.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrEmpty(columnNames))
        {
            throw new WebFaultException<string>("Column names are missing.", HttpStatusCode.BadRequest);
        }

        listColumnNames = columnNames.Split(new char[] { ',' }).ToList();

        foreach (string s in listColumnNames)
            if (string.IsNullOrEmpty(s.Trim())) listColumnNames.Remove(s);

        if (listColumnNames.Count == 0)
        {
            throw new WebFaultException<string>("Column names are missing.", HttpStatusCode.BadRequest);
        }

        listReturn = getListFormResponse(formId, listColumnNames, formResponseId);

        if (listReturn == null || listReturn.Count == 0)
        {
            throw new WebFaultException<string>("No data", HttpStatusCode.NotFound);
        }

        return WebOperationContext.Current.CreateTextResponse(JsonConvert.SerializeObject(listReturn),
            "application/json; charset=utf-8", Encoding.UTF8);
    }

    Message IService.CreateView(string viewName, string tableName, string columnNames, string whereClause, string orderbyClause)
    {
        string connectionString = getConnectionString("GRASP_MemberShip");
        string sqltestExistingView = "select count(*) FROM sys.all_objects where name = '" + viewName + "'";
        string sqlCreateView = "CREATE VIEW " + viewName + " AS ";

        try
        {
            //check if there's an object with the same name
            int count = (int)ExecuteScalar(connectionString, CommandType.Text, sqltestExistingView, null, null);
            if (count > 0)
            {
                throw new WebFaultException<string>("The view '" + viewName + "' wasn't created. In the DB there's an object with the same name. Try another name.", HttpStatusCode.BadRequest);
            }

            List<string> listColumnNames = columnNames.Split(new char[] { ',' }).ToList();

            //build the SELECT statement
            string sqlSelect = "SELECT ";
            for (int i = 0; i < listColumnNames.Count; i++)
            {
                if (i > 0)
                {
                    sqlSelect += ",";
                }

                sqlSelect += listColumnNames[i];
            }

            sqlSelect += " FROM " + tableName;

            if (!string.IsNullOrEmpty(whereClause))
            {
                sqlSelect += " WHERE ";

                List<string> listWhereClauses = whereClause.Split(new char[] { ',' }).ToList();

                string columnFilter = string.Empty;
                string valueFilter = string.Empty;

                for (int i = 0; i < listWhereClauses.Count; i++)
                {
                    columnFilter = listWhereClauses[i].Split(new char[] { '=' })[0];
                    valueFilter = listWhereClauses[i].Split(new char[] { '=' })[1];

                    if (i > 0)
                    {
                        sqlSelect += " AND ";
                    }

                    switch (Type.GetTypeCode(getColumnType(connectionString, tableName, columnFilter)))
                    {
                        case TypeCode.DateTime:
                        case TypeCode.Char:
                        case TypeCode.String:
                            sqlSelect += columnFilter + " = '" + valueFilter + "'";
                            break;

                        case TypeCode.Boolean:
                        case TypeCode.Byte:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                            sqlSelect += columnFilter + " = " + valueFilter;
                            break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderbyClause))
            {
                sqlSelect += " ORDER BY " + orderbyClause;
            }

            //create view
            ExecuteNonQuery(connectionString, CommandType.Text, sqlCreateView + sqlSelect, null, null);

            return WebOperationContext.Current.CreateTextResponse("View created correctly",
                "application/json; charset=utf-8", Encoding.UTF8);
        }
        catch (SqlException ex)
        {
            throw new WebFaultException<string>(ex.Message, HttpStatusCode.BadRequest);
        }
    }
    #endregion

    #region "SQL Methods"
    private System.Type getColumnType(string connectionString, string table, string column)
    {
        System.Type type = null;

        using (SqlDataReader rdr = ExecuteReader(connectionString, CommandType.Text, "SELECT " + column + " FROM " + table, null, null))
        {
            if (rdr.Read())
            {
                type = rdr.GetFieldType(0);
            }
        }

        return type;
    }

    private string getConnectionString(string nameConnectionString)
    {
        string connectionString = string.Empty;

        foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
            if (cs.Name.Equals(nameConnectionString))
            {
                connectionString = cs.ConnectionString;
                break;
            }

        return connectionString;
    }

    private object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
        SqlParameter[] commandParameters, int? cmd_Timeout)
    {
        SqlCommand cmd = new SqlCommand();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters, cmd_Timeout);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
    }

    private int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
        SqlParameter[] commandParameters, int? cmd_Timeout)
    {
        SqlCommand cmd = new SqlCommand();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters, cmd_Timeout);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
    }

    private SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
        SqlParameter[] commandParameters, int? cmd_Timeout)
    {
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(connectionString);

        try
        {
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters, cmd_Timeout);

            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }
        catch (Exception ex)
        {
            conn.Close();
            throw ex;
        }
    }

    private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType,
        string cmdText, SqlParameter[] cmdParms, int? cmd_Timeout)
    {
        if (cmd_Timeout != null && cmd_Timeout.HasValue)
            cmd.CommandTimeout = cmd_Timeout.Value;

        if ((conn.State != ConnectionState.Open)) conn.Open();

        cmd.Connection = conn;
        cmd.CommandText = cmdText;

        if (((trans != null))) cmd.Transaction = trans;

        cmd.CommandType = cmdType;

        if (((cmdParms != null)))
        {
            SqlParameter parm = null;
            foreach (SqlParameter parm_loopVariable in cmdParms)
            {
                parm = parm_loopVariable;
                cmd.Parameters.Add(parm);
            }
        }
    }
    #endregion

    #region "Private Methods"
    private List<object> getListFormResponse(string idParentForm, List<string> listColumnNames, string formResponseId)
    {
        int idParentForm_INT;
        decimal formResponseId_DECIMAL = 0;
        List<object> listReturn = null;
        string nameType = string.Empty;
        System.Linq.IOrderedQueryable<FormResponse> items = null;

        if (!int.TryParse(idParentForm, out idParentForm_INT))
        {
            throw new WebFaultException<string>(string.Format("The ID Parent Form submitted is invalid: '{0}'", idParentForm), HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrEmpty(formResponseId) && !decimal.TryParse(formResponseId, out formResponseId_DECIMAL))
        {
            throw new WebFaultException<string>(string.Format("The ID Form Response submitted is invalid: '{0}'", formResponseId), HttpStatusCode.BadRequest);
        }

        nameType = Form.getFormName(idParentForm_INT);

        if (string.IsNullOrEmpty(nameType))
        {
            throw new WebFaultException<string>(string.Format("No form with ID = '{0}'", idParentForm), HttpStatusCode.NotFound);
        }

        nameType = nameType.Substring(0, 1).ToUpper() + nameType.Substring(1);

        AssemblyName aName = new AssemblyName(nameType);
        AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

        TypeBuilder tb = mb.DefineType(nameType, TypeAttributes.Public);

        using (GRASPEntities db = new GRASPEntities())
        {
            if (!string.IsNullOrEmpty(formResponseId))
            {
                items = from rv in db.FormResponse
                        where rv.id == formResponseId_DECIMAL
                        orderby rv.id ascending
                        select rv;
            }
            else
            {
                items = from rv in db.FormResponse
                        where rv.parentForm_id == idParentForm_INT
                        orderby rv.id ascending
                        select rv;
            }

            List<FormResponse> listFormResponse = (List<FormResponse>)items.ToList();
            if (listFormResponse != null && listFormResponse.Count > 0)
            {
                FormResponse fr = listFormResponse[0];

                var responses = from r in db.FormFieldResponses
                                where r.RVRepeatCount == 0                 //Exclude repeatables vals
                                    && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                                    && r.FormResponseID == fr.id
                                    && listColumnNames.Contains(r.name)
                                select new { r.id, r.name, r.type, r.value, r.nvalue };

                AddFieldProperty(tb, "ID", typeof(int));

                foreach (var r in responses)
                {
                    switch (r.type)
                    {
                        case "TEXT_FIELD":
                        case "TEXT_AREA":
                        case "RADIO_BUTTON":
                        case "PHONE_NUMBER_FIELD":
                        case "CHECK_BOX":
                        case "GEOLOCATION":
                        case "EMAIL_FIELD":
                        case "DROP_DOWN_LIST":
                            AddFieldProperty(tb, r.name, typeof(string));
                            break;
                        case "NUMERIC_TEXT_FIELD":
                            AddFieldProperty(tb, r.name, typeof(double));
                            break;
                        case "CURRENCY_FIELD":
                            AddFieldProperty(tb, r.name, typeof(decimal));
                            break;
                        case "DATE_FIELD":
                            AddFieldProperty(tb, r.name, typeof(int));
                            break;
                        default:
                            break;
                    }
                }

                //TODO repeatableResponses 

                Type attrType = typeof(DataContractAttribute);
                tb.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { }));

                Type t = tb.CreateType();

                listReturn = new List<object>();

                if (!string.IsNullOrEmpty(formResponseId))
                {
                    responses = from r in db.FormFieldResponses
                                where r.RVRepeatCount == 0                 //Exclude repeatables vals
                                    && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                                    && r.FormResponseID == formResponseId_DECIMAL
                                    && listColumnNames.Contains(r.name)
                                select new { r.id, r.name, r.type, r.value, r.nvalue };
                }
                else
                {
                    responses = from r in db.FormFieldResponses
                                where r.RVRepeatCount == 0                 //Exclude repeatables vals
                                    && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                                    && r.parentForm_id == idParentForm_INT
                                    && listColumnNames.Contains(r.name)
                                select new { r.id, r.name, r.type, r.value, r.nvalue };
                }

                foreach (FormResponse response in listFormResponse)
                {
                    object objectFormResponse = Activator.CreateInstance(t);

                    //responses = from r in db.FormFieldResponses
                    //            where r.RVRepeatCount == 0                 //Exclude repeatables vals
                    //                && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                    //                && r.FormResponseID == response.id
                    //                && listColumnNames.Contains(r.name)
                    //            select new { r.id, r.name, r.type, r.value, r.nvalue };

                    string nameProperty = "ID";
                    PropertyInfo pi = t.GetProperty(nameProperty);

                    pi.SetValue(objectFormResponse, int.Parse(response.id.ToString()), null);

                    foreach (var r in responses)
                    {
                        nameProperty = r.name.Substring(0, 1).ToUpper() + r.name.Substring(1);

                        pi = t.GetProperty(nameProperty);

                        switch (r.type)
                        {
                            case "DATE_FIELD":
                                TimeSpan t1 = DateTime.Parse(r.value) - new DateTime(1970, 1, 1);
                                int secondsSinceEpoch = (int)t1.TotalSeconds;

                                pi.SetValue(objectFormResponse, secondsSinceEpoch, null);
                                break;
                            case "TEXT_FIELD":
                            case "TEXT_AREA":
                            case "RADIO_BUTTON":
                            case "PHONE_NUMBER_FIELD":
                            case "CHECK_BOX":
                            case "GEOLOCATION":
                            case "EMAIL_FIELD":
                            case "CURRENCY_FIELD":
                            case "DROP_DOWN_LIST":
                                pi.SetValue(objectFormResponse, r.value, null);
                                break;
                            case "NUMERIC_TEXT_FIELD":
                                pi.SetValue(objectFormResponse, r.nvalue, null);
                                break;
                        }
                    }

                    listReturn.Add(objectFormResponse);
                }
            }
        }

        return listReturn;
    }

    private static void AddFieldProperty(TypeBuilder tb, string nameField, Type type)
    {
        string nameProperty = nameField.Substring(0, 1).ToUpper() + nameField.Substring(1);

        FieldBuilder fb = AddField(tb, "_" + nameField, type);

        PropertyBuilder pb = AddProperty(tb, nameProperty, type);

        DefineGetSetMethods(tb, fb, pb, type);
    }

    private static void DefineGetSetMethods(TypeBuilder tb, FieldBuilder fb, PropertyBuilder pb, Type type)
    {
        MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        // get
        MethodBuilder mbGetAccessor = tb.DefineMethod("get" + pb.Name, getSetAttr, type, Type.EmptyTypes);

        ILGenerator getIL = mbGetAccessor.GetILGenerator();
        getIL.Emit(OpCodes.Ldarg_0);
        getIL.Emit(OpCodes.Ldfld, fb);
        getIL.Emit(OpCodes.Ret);

        // set
        MethodBuilder mbSetAccessor = tb.DefineMethod("set" + pb.Name, getSetAttr, null, new Type[] { type });

        ILGenerator setIL = mbSetAccessor.GetILGenerator();
        setIL.Emit(OpCodes.Ldarg_0);
        setIL.Emit(OpCodes.Ldarg_1);
        setIL.Emit(OpCodes.Stfld, fb);
        setIL.Emit(OpCodes.Ret);

        pb.SetGetMethod(mbGetAccessor);
        pb.SetSetMethod(mbSetAccessor);
    }

    private static PropertyBuilder AddProperty(TypeBuilder tb, string nameProperty, Type type)
    {
        Type attrType = typeof(DataMemberAttribute);

        CustomAttributeBuilder attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });

        PropertyBuilder pb = tb.DefineProperty(nameProperty, System.Reflection.PropertyAttributes.HasDefault, type, null);

        pb.SetCustomAttribute(attr);

        return pb;
    }

    private static FieldBuilder AddField(TypeBuilder tb, string nameField, Type type)
    {
        FieldBuilder fb = tb.DefineField(nameField, type, FieldAttributes.Private);
        return fb;
    }
    #endregion
}