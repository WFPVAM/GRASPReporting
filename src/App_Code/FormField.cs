using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// FormField  class contains auxiliary functions to query FormField table on Grasp DB
/// </summary>
public partial class FormField
{
    /// <summary>
    /// Queries the DB to obtain all the formField of a form
    /// </summary>
    /// <param name="formID">The id representing the form</param>
    /// <returns>A dictionary with all the formfield of the form (Key = formfieldname, value = id)</returns>
    public static Dictionary<string, int> getFormFieldsID(int formID)
    {
        GRASPEntities db = new GRASPEntities();

        Dictionary<string, int> ffields = new Dictionary<string, int>();

        var items = from ff in db.FormField
                    where ff.form_id == formID
                    select ff;

        foreach(var i in items)
        {
            if(i.type != "WRAPPED_TEXT" && i.type != "TRUNCATED_TEXT")
                ffields.Add(i.name, (int)i.id);
        }

        return ffields;
    }

    public static string[,] getFormFieldTypeMap(int formID)
    {
        GRASPEntities db = new GRASPEntities();

        List<FormField> ffs = (from ff in db.FormField
                    where ff.form_id == formID && ff.type != "WRAPPED_TEXT" && ff.type != "TRUNCATED_TEXT"
                    select ff).ToList();
        if(ffs.Count() > 0)
        {
            string[,] items = new string[ffs.Count(), 4];
            int i = 0;
            foreach(FormField ff in ffs)
            {
                items[i, 0] = ff.name;
                items[i, 1] = ff.id.ToString();
                items[i, 2] = ff.type;
                items[i, 3] = ff.positionIndex.ToString();
                i++;
            }
            db.Dispose();
            return items;
        }
        else
        {
            db.Dispose();
            return null;
        }
        
    }
    /// <summary>
    /// Queries the DB to obtain the type of the formfield
    /// </summary>
    /// <param name="formFieldID">The id representing the formfield</param>
    /// <returns>-1 if the formfield is a roster or table, 0 otherwise</returns>
    public static int isRoster(int formFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from ff in db.FormFieldExport
                    where ff.id == formFieldID
                    select ff).FirstOrDefault();

        if(item.type == "REPEATABLES" || item.type == "REPEATABLES_BASIC")
            return -1;
        else
            return 0;
    }

    public static int isImage(int formFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from ff in db.FormFieldExport
                    where ff.id == formFieldID
                    select ff).FirstOrDefault();

        if(item.type == "IMAGE")
            return -1;
        else
            return 0;
    }

    public static int isGEOLocation(int formFieldID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from ff in db.FormFieldExport
                    where ff.id == formFieldID
                    select ff).FirstOrDefault();

        if(item.type == "GEOLOCATION")
            return -1;
        else
            return 0;
    }

    /// <summary>
    /// Queries the DB to obtain information about a specific formfield
    /// </summary>
    /// <param name="name">A string representing the formfield name</param>
    /// <param name="formID">The id of the form</param>
    /// <returns>The id of the formfield with that name</returns>
    public static int getIdFromName(string name, int formID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from ff in db.FormField
                    where ff.name == name && ff.form_id == formID
                    select ff).FirstOrDefault();

        if(item != null)
            return Convert.ToInt32(item.id);
        else return 0;
    }
    /// <summary>
    /// Queries the DB to obtain all the formfield of a specific type
    /// </summary>
    /// <param name="type">A string representing the formfield type</param>
    /// <returns>A list of formfield id</returns>
    public static IEnumerable<decimal> getFormFieldFromType(string type)
    {
        GRASPEntities db = new GRASPEntities();

        var items = from ff in db.FormField
                    where ff.type == type
                    select ff.id;

        return items;
    }

    public static string getX_Form(int formID)
    {
        StringBuilder sb = new StringBuilder();

        GRASPEntities db = new GRASPEntities();

        var items = from ff in db.FormFieldExport
                    where ff.form_id == formID && ff.FormFieldParentID == null
                    select ff.x_form;

        if(items != null)
        {
            sb.Append("<form>");
            foreach(var item in items)
            {

                if(item.Contains("<des_version_2/>"))
                {
                    sb.Append(item.Replace("<des_version_2/>", "<des_version_2>0.0.32</des_version_2>").Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;"));
                }
                else sb.Append(item.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;"));
            }
            sb.Append("</form>");
        }

        return sb.ToString().Replace("&lt;?xml version=\"1.0\"?&gt;", "&lt;?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?&gt;"); ;
    }

    public static FormField createFormField(ImportingElement element, int id)
    {
        GRASPEntities db = new GRASPEntities();

        var formField = new FormField();

        if(element.fieldType != FormFieldType.SEPARATOR)
        {
            if(!element.isRepItem)
            {
                element = cutIndexFromName(element);
            }
            formField.type = element.fieldType.ToString();
            formField.label = element.label;
            formField.name = element.name;
            formField.required = element.bindReference.required ? (byte)1 : (byte)0;
            formField.bindingsPolicy = element.bindingsPolicy;
            formField.constraintPolicy = element.constraintPolicy;
            formField.isReadOnly = element.bindReference.readOnly ? (byte)1 : (byte)0;

            if(element.calculated != null)
            {
                formField.calculated = 1;
                formField.formula = element.calculated;
            }
            else formField.calculated = 0;
        }
        else
        {
            formField.type = element.fieldType.ToString();
            formField.label = element.label;
            formField.name = element.name;
            formField.required = 0;
        }
        if((element.fieldType == FormFieldType.DROP_DOWN_LIST) || (element.fieldType == FormFieldType.RADIO_BUTTON))
        {
            Survey s = Survey.createSurvey(element, id, "select1");
            formField.survey_id = s.id;
        }
        if(element.fieldType == FormFieldType.REPEATABLES_BASIC)
        {
            formField.type = element.fieldType.ToString();
            formField.label = element.label;
            formField.name = element.name;
            formField.required = 0;
            foreach(ImportingElement reps in element.repElements)
            {
                createFormField(reps, id);
            }
            formField.numberOfRep = element.numberOfReps;
        }
        if(element.fieldType == FormFieldType.REPEATABLES)
        {
            formField.type = element.fieldType.ToString();
            formField.label = element.label;
            formField.name = element.name;
            formField.required = 0;
            foreach(ImportingElement reps in element.repElements)
            {
                createFormField(reps, id);
            }
            Survey s = Survey.createSurvey(element, id, "survey");
            formField.survey_id = s.id;
        }

        formField.pushed = 0;
        formField.form_id = id;
        formField.FFCreateDate = DateTime.Now;
        db.FormField.Add(formField);
        db.SaveChanges();

        return formField;
    }

    private static ImportingElement cutIndexFromName(ImportingElement element)
    {
        for(int i = element.name.Length - 1; i >= 0; i--)
        {
            if(element.name[i] == '_')
            {
                try
                {
                    element.positionIndex = Convert.ToInt32(element.name.Substring(i + 1));
                }
                catch(Exception ex)
                {
                    break;
                }
                element.name = element.name.Substring(0, i);
                break;
            }
        }
        return element;
    }

    public static Form createForm(string formID, string formName)
    {
        GRASPEntities db = new GRASPEntities();

        Form form = new Form();

        form.bindingsPolicy = "All";
        form.designerVersion = "WebImport";
        form.finalised = 0;
        form.id_flsmsId = formID;
        form.name = formName;
        form.owner = "supervisor@WFP.org";
        form.FormCreateDate = DateTime.Now;

        db.Form.Add(form);
        db.SaveChanges();

        return form;

    }

    public static void createBinding(FormField ff, ImportingBindingContainer ibc)
    {
        GRASPEntities db = new GRASPEntities();

        BindingContainer bc = new BindingContainer();

        bc.pushed = 0;
        bc.bType = ibc.bType.ToString();
        bc.maxRange = ibc.maxRange;
        bc.minRange = ibc.minRange;
        bc.value = ibc.value;
        bc.FormFieldAndBinding.Add(createFormFieldAndBinding(ff, bc.id));

        db.BindingContainer.Add(bc);
        db.SaveChanges();


    }

    private static FormFieldAndBinding createFormFieldAndBinding(FormField ff, decimal p)
    {
        GRASPEntities db = new GRASPEntities();

        FormFieldAndBinding ffb = new FormFieldAndBinding();

        ffb.pushed = 0;
        ffb.bContainer_id = p;
        if(ff != null)
            ffb.fField_id = ff.id;

        db.FormFieldAndBinding.Add(ffb);
        db.SaveChanges();

        return ffb;
    }

    public static void createConstraints(FormField ff, ImportConstraintContainer icc)
    {
        GRASPEntities db = new GRASPEntities();

        ConstraintContainer cc = new ConstraintContainer();

        cc.pushed = 0;
        cc.cType = icc.cNumber.ToString();
        cc.maxRange = icc.maxRange;
        cc.minRange = icc.minRange;
        cc.value = icc.value;
        db.ConstraintContainer.Add(cc);
        db.SaveChanges();

        createconstraintAssociation(ff.id, cc.id);
    }

    private static void createconstraintAssociation(decimal p1, decimal p2)
    {
        GRASPEntities db = new GRASPEntities();

        FormField_ConstraintContainer cc = new FormField_ConstraintContainer();

        cc.FormField_id = p1;
        cc.constraints_id = p2;

        db.FormField_ConstraintContainer.Add(cc);
        db.SaveChanges();
    }
}