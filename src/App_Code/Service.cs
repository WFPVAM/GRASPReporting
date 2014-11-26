using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

public class Service : IService
{
    private List<object> Surveys()
    {
        List<Form> listForm = null;
        List<object> listReturn = null;
        List<object> listReturnTmp = null;

        using (GRASPEntities db = new GRASPEntities())
        {
            var item = (from f in db.Form
                        select f);

            if (item != null)
                listForm = (List<Form>)item.ToList();
        }

        if (listForm == null || listForm.Count == 0)
            RESTException("No form in DB", HttpStatusCode.NotFound);

        foreach (Form f in listForm)
        {
            listReturnTmp = getListFormResponse(f.id.ToString(), null);

            if (listReturn == null) listReturn = new List<object>();

            if (listReturnTmp != null && listReturnTmp.Count > 0) listReturn.AddRange(listReturnTmp);
        }

        return listReturn;
    }

    private List<object> getListFormResponse(string idParentForm, string formResponseId)
    {
        int idParentForm_INT;
        decimal formResponseId_DECIMAL = 0;
        List<object> listReturn = null;
        string nameType = string.Empty;

        if (!int.TryParse(idParentForm, out idParentForm_INT))
            RESTException(string.Format("The ID Parent Form submitted is invalid: '{0}'", idParentForm), HttpStatusCode.BadRequest);

        if (!string.IsNullOrEmpty(formResponseId) && !decimal.TryParse(formResponseId, out formResponseId_DECIMAL))
            RESTException(string.Format("The ID Form Response submitted is invalid: '{0}'", formResponseId), HttpStatusCode.BadRequest);

        nameType = Form.getFormName(idParentForm_INT);

        if (string.IsNullOrEmpty(nameType))
            RESTException(string.Format("No form with ID = '{0}'", idParentForm), HttpStatusCode.NotFound);

        nameType = nameType.Substring(0, 1).ToUpper() + nameType.Substring(1);

        AssemblyName aName = new AssemblyName(nameType);
        AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

        TypeBuilder tb = mb.DefineType(nameType, TypeAttributes.Public);

        using (GRASPEntities db = new GRASPEntities())
        {
            System.Linq.IOrderedQueryable<FormResponse> items = null;

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

                foreach (FormResponse response in listFormResponse)
                {
                    object objectFormResponse = Activator.CreateInstance(t);

                    responses = from r in db.FormFieldResponses
                                where r.RVRepeatCount == 0                 //Exclude repeatables vals
                                    && r.type != "SERVERSIDE-CALCULATED"   //Exclude server-side calculated fields
                                    && r.FormResponseID == fr.id
                                select new { r.id, r.name, r.type, r.value, r.nvalue };

                    string nameProperty = "ID";
                    PropertyInfo pi = t.GetProperty(nameProperty);

                    pi.SetValue(objectFormResponse, int.Parse(fr.id.ToString()), null);

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

    Message IService.Surveys(string formId, string formResponseId)
    {
        List<object> listReturn = null;

        if (string.IsNullOrEmpty(formId))
            listReturn = Surveys();
        else
            listReturn = getListFormResponse(formId, formResponseId);

        if (listReturn == null || listReturn.Count == 0)
            throw new WebFaultException<string>("No data", HttpStatusCode.NotFound);

        return WebOperationContext.Current.CreateTextResponse(JsonConvert.SerializeObject(listReturn),
            "application/json; charset=utf-8", Encoding.UTF8);
    }

    private void RESTException(string errorMessage, System.Net.HttpStatusCode httpStatusCode)
    {
        throw new WebFaultException<string>(errorMessage, httpStatusCode);
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

        PropertyBuilder pb = tb.DefineProperty(nameProperty, PropertyAttributes.HasDefault, type, null);

        pb.SetCustomAttribute(attr);

        return pb;
    }

    private static FieldBuilder AddField(TypeBuilder tb, string nameField, Type type)
    {
        FieldBuilder fb = tb.DefineField(nameField, type, FieldAttributes.Private);
        return fb;
    }
}