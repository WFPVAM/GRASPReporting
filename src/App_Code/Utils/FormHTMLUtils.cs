using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for FormHTMLUtils
/// </summary>
public class FormHTMLUtils
{
    public static void WriteGPSHTMLField(FormFieldExport FormF, bool isEditMode, List<ResponseValue> responseValues,
        ref string fieldLabel,
        List<string> ngModelName,
        ref string filedBody,
        ref string constr,
        Dictionary<int, string> constraint,
        ref string outVal,
        Literal ltlScript,
        ref string repVal,
        Dictionary<int, string> roaster,
        Dictionary<int, string> table,
        Dictionary<string, string> ngModelNameSubForm,
        ref string model
        )
    {
        try
        {
            if (FormF.FormFieldParentID == null)
            {
                fieldLabel = FormF.label;
                ngModelName.Add(FormF.name);
                filedBody +=
                    " <input type=\"number\" class=\"col-xs-5 col-sm-4\" min=\"-90\" max=\"90\" step=\"any\" " +
                            "ng-model=\"" + model + FormF.name + "LatDE\"";
                constr = "";
                if (constraint.TryGetValue((int)FormF.id, out constr))
                {
                    filedBody += constr;
                }
                filedBody += " name=\"r_" + FormF.name + "LatDE\"";

                var fieldVal = "";
                if (isEditMode)
                {
                    fieldVal = (from v in responseValues
                                where v.formFieldId == FormF.id
                                select v.value).FirstOrDefault();
                    if (fieldVal != null)
                    {
                        filedBody += " ng-init=\"" + model + FormF.name + "LatDE=" +
                                     fieldVal.ToString().Split(' ')[1] + "\" ";
                    }
                }

                if (FormF.required == 1)
                {
                    if (!string.IsNullOrEmpty(outVal))
                    {
                        ltlScript.Text += "$scope." + model + FormF.name + "LatDE = 0;";
                        filedBody += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value +
                                     "\" />\n";
                    }
                    else filedBody += " required />\n";
                    filedBody += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name +
                                 "LatDE.$error.required || mainForm.r_" + FormF.name +
                                 "LatDE.$error.number\"><i class=\"fa fa-warning\"></i></span> \n";
                }
                else filedBody += "/>";

                filedBody +=
                    " <input type=\"number\" class=\"col-xs-5 col-sm-4\" step=\"any\" min=\"-180\" max=\"180\" ng-model=\"" +
                    model + FormF.name + "LongDE\"";
                constr = "";
                if (constraint.TryGetValue((int)FormF.id, out constr))
                {
                    filedBody += constr;
                }
                filedBody += " name=\"r_" + FormF.name + "LongDE\"";

                if (isEditMode
                    && fieldVal != null)
                {
                    filedBody += " ng-init=\"" + model + FormF.name + "LongDE=" + fieldVal.ToString().Split(' ')[0] + "\" ";
                }

                if (FormF.required == 1)
                {
                    if (outVal != null && outVal != "")
                    {
                        ltlScript.Text += "$scope." + model + FormF.name + "LongDE = 0;";
                        filedBody += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value +
                                     "\" />\n";
                    }
                    else filedBody += " required />\n";
                    filedBody += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name +
                                 "LongDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                    filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name +
                                 "LongDE.$error.number\"><i class=\"fa fa-warning\"></i></span> \n";
                }
                else filedBody += "/>";
                filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name +
                             "LatDE.$error.min || mainForm.r_" + FormF.name +
                             "LatDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid Coords!</span>\n";
                filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name +
                             "LongDE.$error.min || mainForm.r_" + FormF.name +
                             "LongDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid Coords!</span>\n";
                filedBody += " \n";
            }
            else //s* i think this else is useless
            {
                repVal = "";
                roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                if (repVal == null)
                    table.TryGetValue((int)FormF.FormFieldParentID, out repVal);

                fieldLabel = FormF.label;
                filedBody += "Lat: <input type=\"number\" step=\"any\" min=\"-90\" max=\"90\" ng-model=\"rb_" + repVal + "." +
                             FormF.name + "LatDE\"";
                ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                if (FormF.isReadOnly == 1 ||
                    FormF.calculated == 1)
                {
                    filedBody += " ng-readonly=\"1\" ";
                }
                constr = "";
                if (constraint.TryGetValue((int)FormF.id, out constr))
                {
                    filedBody += constr;
                }
                filedBody += " name=\"r_" + FormF.name + "LatDE\"";
                if (FormF.required == 1)
                {
                    if (!string.IsNullOrEmpty(outVal))
                    {
                        filedBody += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value +
                                     "\" />\n";
                    }
                    else filedBody += " required />\n";
                    filedBody += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" +
                                 FormF.name + "LatDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                    filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" +
                                 FormF.name + "LatDE.$error.number\"><i class=\"fa fa-warning\"></i></span>\n";
                }
                else filedBody += "/>";


                filedBody += " Long: <input type=\"number\" step=\"any\" min=\"-180\" max=\"180\" ng-model=\"rb_" + repVal + "." +
                             FormF.name + "LongDE\"";
                ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                if (FormF.isReadOnly == 1 || FormF.calculated == 1)
                {
                    filedBody += " ng-readonly=\"1\" ";
                }
                constr = "";
                if (constraint.TryGetValue((int)FormF.id, out constr))
                {
                    filedBody += constr;
                }
                filedBody += " name=\"r_" + FormF.name + "LongDE\"";
                if (FormF.required == 1)
                {
                    if (outVal != null && outVal != "")
                    {
                        filedBody += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value +
                                     "\" />\n";
                    }
                    else filedBody += " required />\n";

                    filedBody += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" +
                                 FormF.name + "LongDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                    filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" +
                                 FormF.name + "LongDE.$error.number\"><i class=\"fa fa-warning\"></i></span>\n";
                }
                else filedBody += "/>";
                filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name +
                             "LatDE.$error.min || subForm" + repVal + ".r_" + FormF.name +
                             "LatDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid coords!</span>\n";
                filedBody += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name +
                             "LongDE.$error.min || subForm" + repVal + ".r_" + FormF.name +
                             "LongDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid coords!</span>\n";
                filedBody += "\n";
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }
    }

}