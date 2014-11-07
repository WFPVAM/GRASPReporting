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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
/// <summary>
/// Used to show the model of the selected form in Data_Entry
/// </summary>
public partial class DataEntryWebFormEditOLD : System.Web.UI.Page
{
    List<FieldConstraintRel2> fieldConstraintRel = new List<FieldConstraintRel2>();
    IQueryable<ResponseValue> responseValues;
    string formula = "";
    string model = "currForm.";
    Dictionary<int, string> relevant;
    Dictionary<int, string> constraint;
    Dictionary<int, string> roaster;
    Dictionary<int, string> table;
    List<string> ngModelName;
    Dictionary<string, string> ngModelNameSubForm;
    Dictionary<string, string> ngModelCalculatedOutofModel;
    public int formID = 0;
    public int FormResponseID = 0;
    int countForRB = 0;
    int closeRosterRelevant = 0;
    public static string isSaved = "true";
    int prevFieldID = 0;
    string outVal = "";
    string script = "";
    string repVal = "";
    string constr = "";
    /// <summary>
    /// Creates the structure of a form, initializing the script to allow AngularJS
    /// to control the data users enters.
    /// When the page loads, system queries the DB to obtain all the fields of a form,
    /// and for each field calls the switchDataEntry method.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        if(Request["save"] == "true")
        {
            success.Visible = true;
            error.Visible = false;
        }
        else if(Request["save"] == "false")
        {
            success.Visible = false;
            error.Visible = true;
        }
        relevant = new Dictionary<int, string>();
        constraint = new Dictionary<int, string>();
        roaster = new Dictionary<int, string>();
        table = new Dictionary<int, string>();
        ngModelName = new List<string>();
        ngModelNameSubForm = new Dictionary<string, string>();
        ngModelCalculatedOutofModel = new Dictionary<string, string>();



        if(!string.IsNullOrEmpty(Request["formID"]) && !string.IsNullOrEmpty(Request["RID"]))
        {
            formID = Convert.ToInt32(Request["formID"]);
            FormResponseID = Convert.ToInt32(Request["RID"]);

            ltlScript.Text = "<script>var app = angular.module('angularjs-starter', []);" +
            "\r\n app.directive('jqdatepicker', function () {return {restrict: 'A',require: 'ngModel', link: function (scope, element, attrs, MainCtrl)" +
        "{element.datepicker({dateFormat: 'yy-mm-dd', onSelect: function (date) { var modName = this.getAttribute('ng-model').split(\".\"); \r\n" +
        "var evalCmd = \"scope.\" + modName[0] + \"['\" + modName[1] + \"']  = '\" + date +\"'\";\r\n " +
        "if(modName[0] == \"currForm\") scope.currForm[modName[1]] = date; else\r\n eval(evalCmd); \r\nscope.$apply(); }});}};});\r\n" +
            "\r\n\r\n app.controller('MainCtrl', function ($scope, $http) {\r\n" + 
            "" +
            " $scope.master = {};$scope.update = function (currForm) {\r\n" +
            "$scope.master = angular.copy(currForm); sendJson(); unsaved = false;}; $scope.reset = function () {$scope.currForm = angular.copy($scope.master); };" +
            " $scope.isUnchanged = function (currForm) { return angular.equals(currForm, $scope.master); }; \r\n" +
            "$scope.reset(); $scope.currForm.date = \"" + DateTime.UtcNow.Date.ToString("yyyy-MM-dd") + "\"; $scope.currForm.des_version = \"WEB\"; $scope.currForm.client_version = \"WEB\";\r\n";
            


            angJSForm.Text = "<br />";
            GRASPEntities db = new GRASPEntities();
            IEnumerable<FormFieldExport> fields = from ff in db.FormFieldExport
                                                  where ff.form_id == formID && ff.FormFieldParentID == null
                                                  orderby ff.positionIndex ascending
                                                  select ff;

            createRelevant(FormResponseID);
            createConstraint();

            IEnumerable<FormFieldExport> rosterFields = from i in db.FormFieldExport
                                                        where i.FormFieldParentID != null
                                                        orderby i.positionIndex ascending
                                                        select i;

            responseValues = from rv in db.ResponseValue
                             where rv.FormResponseID == FormResponseID
                             select rv;



            foreach(FormFieldExport i in fields)
            {
                if(i.type == "REPEATABLES_BASIC" || i.type == "REPEATABLES")
                {
                    switchDataEntry(i);
                    foreach(FormFieldExport f in rosterFields.Where(x => x.FormFieldParentID == i.id))
                    {
                        switchDataEntry(f);
                    }
                }
                else
                {
                    switchDataEntry(i);
                }
            }
        }
        if(prevFieldID != 0)
        {
            angJSForm.Text += "</li></ul></div>\n";
            if(closeRosterRelevant == 1)
            {
                angJSForm.Text += "</div>\n";
                closeRosterRelevant = 0;
            }
        }
        ltlScript.Text += Literal1.Text;

        ltlScript.Text += "\r\n\r\n $http({\r\n" +
                              "  method: 'GET',\r\n" +
                              "  url: 'http://graspreporting.brainsen.com/formJSON.txt'\r\n" +
                              "}).success(function(data, status) {\r\n" +
                              "   console.log('works!!!   ' + data);\r\n" +
                              "   $scope.currForm = data;"+
                              "  }).error(function(data, status) {\r\n" +
                              "  // Some error occurred\r\n" +
                              "   console.log(status);\r\n" +
                              " $scope.currForm = { " + FormResponse.GetAsJson(FormResponseID) + " }});\r\n" +
                        "  });</script>";
        Literal1.Text = "";

    }
    /// <summary>
    /// Creates the HTML structure of the field passed in input.
    /// Each field is created using AngularJS directives.
    /// </summary>
    /// <param name="FormF">FormField to analyze</param>
    private void switchDataEntry(FormFieldExport FormF)
    {
        switch(FormF.type)
        {
            case "DATE_FIELD":
                if(prevFieldID != 0 && (int)FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"text\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + " jqdatepicker />\n";
                        }
                        else angJSForm.Text += " required jqdatepicker />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"text\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + " jqdatepicker />\n";
                        }
                        else angJSForm.Text += " required jqdatepicker />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }


                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "PHONE_NUMBER_FIELD":
            case "BARCODE":
            case "TEXT_FIELD":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"text\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    var val = (from v in responseValues
                               where v.formFieldId == FormF.id
                               select v.value).FirstOrDefault();
                    if(val != null)
                    {
                        angJSForm.Text += " ng-init=\"" + model + FormF.name + "='" + val + "'\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"text\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "TEXT_AREA":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <textarea rows=\"2\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" ></textarea>\n";
                        }
                        else angJSForm.Text += " required ></textarea>\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "></textarea></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <textarea rows=\"2\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" ></textarea>\n";
                        }
                        else angJSForm.Text += " required ></textarea>\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "></textarea></div>\n";
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "CURRENCY_FIELD":
            case "NUMERIC_TEXT_FIELD":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {

                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"number\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }

                    var val = (from v in responseValues
                               where v.formFieldId == FormF.id
                               select v.value).FirstOrDefault();
                    if(val != null)
                    {
                        angJSForm.Text += " ng-init=\"" + model + FormF.name + "=" + val.Replace(".0", "") + "\" ";
                    }

                    constr = "";
                    List<FieldConstraintRel2> currFieldConstraints = (from fc in fieldConstraintRel
                                                                     where fc.FormFieldID == (int)FormF.id
                                                                     select fc).ToList();
                    foreach(FieldConstraintRel2 fcr in currFieldConstraints)
                    {
                        if(constraint.TryGetValue(fcr.ConstraintID, out constr))
                        {
                            angJSForm.Text += constr;
                        }
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        //constr = "";
                        //if (constraint.TryGetValue((int)FormF.id, out constr))
                        //{
                        //    angJSForm.Text += constr;
                        //}
                        if(outVal != null && outVal != "")
                        {
                            ltlScript.Text += "$scope." + model + FormF.name + " = 0;";
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        if(constr != null && constr != "")
                        {
                            angJSForm.Text += "<span ng-show=\"mainForm.r_" + FormF.name + ".$error.min || mainForm.r_" + FormF.name + ".$error.max\">Out of Bounds!</span>\n";
                        }
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span>";
                        angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else if(constr != null && constr != "")
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        angJSForm.Text += "/>\n<span ng-show=\"mainForm.r_" + FormF.name + ".$error.min || mainForm.r_" + FormF.name + ".$error.max\">Out of Bounds!</span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                    if(FormF.calculated == 1)
                    {
                        if(FormF.formula != null)
                        {
                            bool isInModel = false;
                            foreach(string i in ngModelName)
                            {
                                if(FormF.formula.Contains(i))
                                {
                                    isInModel = true;
                                }
                            }
                            if(isInModel)
                            {
                                ltlScript.Text += "$scope.$watch('" + getFormula(FormF.formula) + "', function (value) {  $scope." + model + FormF.name + "= value;}, true);";
                            }
                            else
                            {
                                ltlScript.Text += getFormula(FormF.formula, "$scope." + model + FormF.name, repVal);
                            }
                        }
                    }
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"number\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    ngModelCalculatedOutofModel.Add(FormF.name, "currForm." + repVal);
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        //constr = "";
                        //if (constraint.TryGetValue((int)FormF.id, out constr))
                        //{
                        //    angJSForm.Text += constr;
                        //}
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        if(constr != null && constr != "")
                        {
                            angJSForm.Text += "<span ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.min || subForm" + repVal + ".r_" + FormF.name + ".$error.max\">Out of Bounds!</span>\n";
                        }
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span>";
                        angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else if(constr != null && constr != "")
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        angJSForm.Text += "/>\n<span ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.min || subForm" + repVal + ".r_" + FormF.name + ".$error.max\">Out of Bounds!</span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                    if(FormF.calculated == 1)
                    {
                        if(FormF.formula != null)
                        {
                            ltlScript.Text += "$scope.$watch('currForm." + repVal + "', function (value) { var i = 0;for(i = 0; i < value.length; i++) value[i]." + FormF.name + " = " + getFormula(FormF.formula, repVal) + ";}, true);";
                        }
                    }
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "DROP_DOWN_LIST":
                if(prevFieldID != 0 && (int)FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                if(!ltlScript.Text.Contains("$scope.options" + (int)FormF.survey_id + "="))
                {
                    script = "";
                    script += "$scope.options" + (int)FormF.survey_id + "= [";
                    foreach(SurveyElement se in getOptions((int)FormF.survey_id))
                    {
                        script += "{value:\"" + se.value.Replace("'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "\"},";
                    }
                    ltlScript.Text += script.Substring(0, script.Length - 1);
                    ltlScript.Text += "];";
                }


                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\">\n";
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<select ng-model=\"" + model + FormF.name + ".value\" ng-options=\"o.value as o.value for o in options" + (int)FormF.survey_id + "\"";
                    //var val = (from v in responseValues
                    //           where v.formFieldId == FormF.id
                    //           select v.value).FirstOrDefault();
                    //if(val != null)
                    //{
                    //    angJSForm.Text += " ng-init=\"" + model + FormF.name + "=options" + (int)FormF.survey_id + "[3]\" ";
                    //}
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" ></select>\n";
                        }
                        else angJSForm.Text += " required ></select>";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "></select></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<select ng-model=\"rb_" + repVal + "." + FormF.name + ".value\" ng-options=\"o.value as o.value for o in options" + (int)FormF.survey_id + "\"";

                    if(!ngModelNameSubForm.ContainsKey(FormF.name))
                        ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" /></select>\n";
                        }
                        else angJSForm.Text += " required /></select>\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div></select>\n";
                }


                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "RADIO_BUTTON":
                if(prevFieldID != 0 && (int)FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                angJSForm.Text += "<div class=\"left clear\">" + FormF.label + "</div>\n";
                foreach(SurveyElement se in getOptions((int)FormF.survey_id))
                {
                    if(FormF.FormFieldParentID == null)
                    {
                        ngModelName.Add(FormF.name);
                        angJSForm.Text += "<div class=\"right\"><label>" + se.value + "</label><input type=\"radio\" ng-model=\"" + model + FormF.name + "\" value=\"" + se.value + "\"";


                        var val = (from v in responseValues
                                   where v.formFieldId == FormF.id
                                   select v.value).FirstOrDefault();
                        if(val != null)
                        {
                            angJSForm.Text += " ng-init=\"" + model + FormF.name + "='" + val + "'\" ";
                        }

                        if(FormF.required == 1)
                        {
                            angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                            if(outVal != null && outVal != "")
                            {
                                angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                            }
                            else angJSForm.Text += " required />\n";
                            angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                        }
                        else angJSForm.Text += "/></div>\n";
                    }
                    else
                    {
                        repVal = "";
                        roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                        if(repVal == null)
                            table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                        angJSForm.Text += "<div class=\"left clear\"><label>" + se.value + "</label></div><div class=\"right\"><input type=\"radio\" ng-model=\"rb_" + repVal + "." + FormF.name + "\" value=\"" + se.value + "\"";
                        if(!ngModelNameSubForm.ContainsKey(FormF.name))
                            ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                        if(FormF.required == 1)
                        {
                            angJSForm.Text += " name=\"r_{{$index}}" + FormF.name + "\"";
                            if(outVal != null && outVal != "")
                            {
                                angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                            }
                            else angJSForm.Text += " required />\n";
                            angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                        }
                        else angJSForm.Text += "/></div>\n";
                    }
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "CHECK_BOX":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    ltlScript.Text += "$scope." + model + FormF.name + " = false;";
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"checkbox\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        //constr = "";
                        //if (constraint.TryGetValue((int)FormF.id, out constr))
                        //{
                        //    angJSForm.Text += constr;
                        //}
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"checkbox\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        //constr = "";
                        //if (constraint.TryGetValue((int)FormF.id, out constr))
                        //{
                        //    angJSForm.Text += constr;
                        //}
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }
                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "REPEATABLES_BASIC":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                closeRosterRelevant = 0;
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                    closeRosterRelevant = 1;
                }
                ltlScript.Text += "$scope." + model + FormF.name + " = [];";
                checkFieldForRoaster((int)FormF.id, FormF.name);
                roaster.Add((int)FormF.id, FormF.name);
                angJSForm.Text += "<div class=\"left clear\"><label>Roster " + FormF.label + "</label><br/>\n";
                angJSForm.Text += "<a ng-click=\"addNew" + FormF.name + "()\"><i class=\"fa fa-plus\"></i> Add New " + FormF.name + "</a></div><div class=\"left clear\"><ul>";
                angJSForm.Text += "<li ng-repeat=\"rb_" + FormF.name + " in " + model + FormF.name + "\" ng-form=\"subForm" + FormF.name + "\">\n<div style=\"overflow: hiddenX;\">";

                //angJSForm.Text += "<div class=\"left clear\"><label>1.0 Month of the data collection: Please select one</label></div><div class=\"right\"><select ng-model=\"currForm.mothreport\" ng-options=\"o.value for o in options14\" ng-init=\"currForm.mothreport=options14[4]\"  name=\"r_mothreport\" required ></select><span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_mothreport.$error.required\"><i class=\"fa fa-warning\"></i></span></div>";
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "REPEATABLES":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                closeRosterRelevant = 0;
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                    closeRosterRelevant = 1;
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                ltlScript.Text += "$scope." + model + FormF.name + " = [";
                checkFieldForTable((int)FormF.survey_id);
                table.Add((int)FormF.id, FormF.name);
                angJSForm.Text += "<div class=\"left clear\"><ul><li ng-repeat=\"rb_" + FormF.name + " in " + model + FormF.name + "\" ng-form=\"subForm" + FormF.name + "\">\n<div style=\"overflow: hidden;\">";
                angJSForm.Text += "<label>{{rb_" + FormF.name + ".value}}</label><br />";
                break;
            case "SEPARATOR":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                angJSForm.Text += "<div class=\"clear\"><hr /></div>\n";
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "TRUNCATED_TEXT":
            case "WRAPPED_TEXT":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "<div class=\"clear left\"><label style=\"font-weight: bold;\" ng-model=\"" + model + FormF.name + "\">" + FormF.label + "</label></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"clear left\"><label style=\"font-weight: bold;\" ng-model=\"rb_" + repVal + "." + FormF.name + "\">" + FormF.label + "</label></div>\n";
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "GEOLOCATION":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> Lat: <input type=\"number\" min=\"-90\" max=\"90\" ng-model=\"" + model + FormF.name + "LatDE\"";
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    angJSForm.Text += " name=\"r_" + FormF.name + "LatDE\"";
                    if(FormF.required == 1)
                    {
                        if(outVal != null && outVal != "")
                        {
                            ltlScript.Text += "$scope." + model + FormF.name + "LatDE = 0;";
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + "LatDE.$error.required || mainForm.r_" + FormF.name + "LatDE.$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/>";

                    angJSForm.Text += " Long: <input type=\"number\" min=\"-90\" max=\"90\" ng-model=\"" + model + FormF.name + "LongDE\"";
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    angJSForm.Text += " name=\"r_" + FormF.name + "LongDE\"";
                    if(FormF.required == 1)
                    {
                        if(outVal != null && outVal != "")
                        {
                            ltlScript.Text += "$scope." + model + FormF.name + "LongDE = 0;";
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + "LongDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                        angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + "LongDE.$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/>";
                    angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + "LatDE.$error.min || mainForm.r_" + FormF.name + "LatDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid Coords!</span>\n";
                    angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + "LongDE.$error.min || mainForm.r_" + FormF.name + "LongDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid Coords!</span>\n";
                    angJSForm.Text += "</div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> Lat: <input type=\"number\" min=\"-90\" max=\"90\" ng-model=\"rb_" + repVal + "." + FormF.name + "LatDE\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    angJSForm.Text += " name=\"r_" + FormF.name + "LatDE\"";
                    if(FormF.required == 1)
                    {
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LatDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                        angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LatDE.$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/>";


                    angJSForm.Text += " Long: <input type=\"number\" min=\"-180\" max=\"180\" ng-model=\"rb_" + repVal + "." + FormF.name + "LongDE\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1 || FormF.calculated == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    constr = "";
                    if(constraint.TryGetValue((int)FormF.id, out constr))
                    {
                        angJSForm.Text += constr;
                    }
                    angJSForm.Text += " name=\"r_" + FormF.name + "LongDE\"";
                    if(FormF.required == 1)
                    {
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";

                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LongDE.$error.required\"><i class=\"fa fa-warning\"></i></span>";
                        angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LongDE.$error.number\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/>";
                    angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LatDE.$error.min || subForm" + repVal + ".r_" + FormF.name + "LatDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid coords!</span>\n";
                    angJSForm.Text += "<span style=\"color: red; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + "LongDE.$error.min || subForm" + repVal + ".r_" + FormF.name + "LongDE.$error.max\"><i class=\"fa fa-warning\"></i>Invalid coords!</span>\n";
                    angJSForm.Text += "</div>\n";
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            case "EMAIL_FIELD":
                if(prevFieldID != 0 && FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                outVal = "";
                if(relevant.TryGetValue((int)FormF.id, out outVal))
                {
                    angJSForm.Text += outVal;
                }
                if(FormF.FormFieldParentID == null)
                {
                    ngModelName.Add(FormF.name);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"email\" ng-model=\"" + model + FormF.name + "\"";
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"mainForm.r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }
                else
                {
                    repVal = "";
                    roaster.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    if(repVal == null)
                        table.TryGetValue((int)FormF.FormFieldParentID, out repVal);
                    angJSForm.Text += "<div class=\"left clear\"><label>" + FormF.label + "</label></div><div class=\"right\"> <input type=\"email\" ng-model=\"rb_" + repVal + "." + FormF.name + "\"";
                    ngModelNameSubForm.Add(FormF.name, "rb_" + repVal + ".");
                    if(FormF.isReadOnly == 1)
                    {
                        angJSForm.Text += " ng-readonly=\"1\" ";
                    }
                    if(FormF.required == 1)
                    {
                        angJSForm.Text += " name=\"r_" + FormF.name + "\"";
                        if(outVal != null && outVal != "")
                        {
                            angJSForm.Text += " ng-required=\"currForm" + Regex.Match(outVal, "currForm(.+?)\">").Groups[1].Value + "\" />\n";
                        }
                        else angJSForm.Text += " required />\n";
                        angJSForm.Text += "<span style=\"color: #f1c409; padding: 5px;\" ng-show=\"subForm" + repVal + ".r_" + FormF.name + ".$error.required\"><i class=\"fa fa-warning\"></i></span></div>\n";
                    }
                    else angJSForm.Text += "/></div>\n";
                }

                if(outVal != null && outVal != "")
                {
                    angJSForm.Text += "</div>\n";
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
            default:
                if(prevFieldID != 0 && (int)FormF.FormFieldParentID == null)
                {
                    angJSForm.Text += "</div></li></ul></div>\n";
                    if(closeRosterRelevant == 1)
                    {
                        angJSForm.Text += "</div>\n";
                        closeRosterRelevant = 0;
                    }
                }
                prevFieldID = (FormF.FormFieldParentID == null) ? 0 : (int)FormF.FormFieldParentID;
                break;
        }
    }
    /// <summary>
    /// Creates the structure for the table in the AngularJS script, that will be used with the ng-repeat directive.
    /// </summary>
    /// <param name="surID">The id representing the list associated with the table</param>
    private void checkFieldForTable(int surID)
    {
        GRASPEntities db = new GRASPEntities();

        string script = "";

        var surveys = (from s in db.Survey
                       where s.id == surID
                       select s).FirstOrDefault();

        foreach(SurveyElement se in FormFieldExport.getSurveyListElements(surID))
        {
            script += "{ value: '" + se.value + "'},";
        }

        ltlScript.Text += script.Substring(0, script.Length - 1);
        ltlScript.Text += " ];";
    }
    /// <summary>
    /// Creates the angularJS function that allow user to add another instance of the roster
    /// </summary>
    /// <param name="formFieldParentID">The id representing the roster field</param>
    /// <param name="name">The name used in the HTML structure to identify an input (used with the ng-model directive)</param>
    private void checkFieldForRoaster(int formFieldParentID, string name)
    {
        GRASPEntities db = new GRASPEntities();

        var roasters = from ff in db.FormFieldExport
                       where ff.FormFieldParentID == formFieldParentID
                       orderby ff.positionIndex ascending
                       select ff;

        ltlScript.Text += "\n$scope.addNew" + name + " = function () {$scope.currForm." + name + ".push({ ";
        string script = "";
        foreach(FormFieldExport lbl in roasters)
        {
            if(lbl.type != "TRUNCATED_TEXT" && lbl.type != "WRAPPED_TEXT")
            {
                if(lbl.type == "NUMERIC_TEXT_FIELD" || lbl.type == "CURRENCY_FIELD")
                {
                    script += lbl.name + ": 2,";
                }
                else script += lbl.name + ": '',";
            }
        }
        ltlScript.Text += script.Substring(0, script.Length - 1);
        ltlScript.Text += " });};\n";

    }
    /// <summary>
    /// Fits the formula to the model of AngularJS for a field
    /// </summary>
    /// <param name="p">A string representing the formula</param>
    /// <returns>The modified formula</returns>
    private string getFormula(string p)
    {
        string formula = p;
        MatchCollection formulaFields = splitFormula(formula);
        foreach(var ff in formulaFields)
        {
            foreach(string i in ngModelName)
            {
                if(i.Contains(ff.ToString()))
                {
                    formula = formula.Replace(i, "currForm." + i);
                }
            }
        }
        return formula;
    }
    /// <summary>
    /// Fits the formula to the model of AngularJS for a roster or table field
    /// </summary>
    /// <param name="p">A string representing the roster</param>
    /// <param name="r">A string representing the roster</param>
    /// <returns>The modified formula</returns>
    private string getFormula(string p, string r)
    {
        string formula = p;
        foreach(var j in ngModelNameSubForm)
        {
            if(formula.Contains(j.Key))
            {
                formula = formula.Replace(j.Key, "value[i]." + j.Key);
            }
        }

        return formula;
    }

    private string getFormula(string p, string r, string q)
    {
        string formula = p;
        //string[] formulaSplitted = 
        MatchCollection formulaFields = splitFormula(formula);
        //string[] formulaFields = formula.Split('+');
        string watchField = "";
        ngModelCalculatedOutofModel.TryGetValue(p, out watchField);
        if(watchField == null)
        {
            foreach(var ff in formulaFields)
            {
                ngModelCalculatedOutofModel.TryGetValue(ff.ToString(), out watchField);
            }
        }
        foreach(var j in ngModelCalculatedOutofModel)
        {
            if(formula.Contains(j.Key))
            {
                formula = formula.Replace(j.Key, "value[i]." + j.Key);
            }
        }

        //return formula;
        string script = "$scope.$watch('" + watchField + "', function (value) { " + r + " = 0; var i = 0; for(i = 0; i < value.length; i++) " + r + " += (" + formula + ");}, true);";
        return script;
    }

    private MatchCollection splitFormula(string formula)
    {
        MatchCollection i = Regex.Matches(formula, @"\w+(\w+)?");
        return i;
    }
    /// <summary>
    /// </summary>
    /// <param name="surveyID">The id representing a SurveyList associated with a field</param>
    /// <returns>The elements of the surveyList</returns>
    protected IEnumerable<SurveyElement> getOptions(int surveyID)
    {
        return FormFieldExport.getSurveyListElements(surveyID);
    }
    /// <summary>
    /// Creates the HTML structure to allow AngularJS to control the visibility of the fields.
    /// For each field of the form it add this structure in a Dictionary, that will be used in the switchDataEntry method.
    /// It also creates the javascript functions that disable the data inserted when a field is hidden.
    /// At the end it calls the createConstraint function.
    /// </summary>
    private void createRelevant(int formResponseID)
    {
        int formFid = 0;
        int key = 0;
        string value = "";
        string scriptFunc = "";
        GRASPEntities db = new GRASPEntities();

        var items = from br in db.BindingRules
                    where br.form_id == formID
                    orderby br.FormField_id ascending
                    select br;
        var values = (from v in db.FormFieldResponses
                     where v.FormResponseID == formResponseID && v.type=="DROP_DOWN_LIST"
                     select new { v.formFieldId, v.RVRepeatCount }).ToList();

        foreach(var i in items)
        {
            if(values.Where(w => w.formFieldId == i.FormField_id).Count() == 0)
            {
                var name = "";
                if(formFid != (int)i.FormField_id)
                {
                    if(formFid != 0)
                    {
                        value = value + ("\">");
                        scriptFunc = scriptFunc + ("\",function(){");
                        createWatchReset(scriptFunc, key);
                        relevant.Add(key, value);
                    }
                    key = (int)i.FormField_id;
                    var tmp = i.value;
                    name = i.name;
                    if(i.type == "DROP_DOWN_LIST")
                    {
                        name = i.name + ".value";
                    }
                    if(i.type == "CHECK_BOX")
                    {
                        tmp = "true";
                    }

                    if(i.value.Contains("/data/"))
                    {
                        tmp = model + Regex.Match(i.value, @"/data/(.+?)_").Groups[1].Value;
                        value = "<div ng-show=\"" + model + name + getTypeBind(i.bType) + " " + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace("\n", " ") + " ";
                        scriptFunc = "$scope.$watch(\"" + model + name + getTypeBindNeg(i.bType) + " " + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + " ";
                    }
                    else
                    {
                        if(tmp == "true")
                        {
                            value = "<div ng-show=\"" + model + name + getTypeBind(i.bType) + " " + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace("\n", " ") + "";
                            scriptFunc = "$scope.$watch(\"" + model + name + getTypeBindNeg(i.bType) + "" + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "";
                        }
                        else
                        {
                            value = "<div ng-show=\"" + model + name + getTypeBind(i.bType) + "'" + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace("\n", " ") + "'";
                            scriptFunc = "$scope.$watch(\"" + model + name + getTypeBindNeg(i.bType) + "'" + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "'";
                        }
                    }
                    formFid = key;
                }
                else
                {
                    name = i.name;
                    string tmp = i.value;
                    if(i.type == "DROP_DOWN_LIST")
                    {
                        name = i.name + ".value";
                    }
                    if(i.type == "CHECK_BOX")
                    {
                        tmp = "true";
                    }
                    if(i.bindingsPolicy == "All")
                    {
                        value = value + (" && " + model + name + getTypeBind(i.bType) + "'" + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace("\n", " ") + "'");
                        scriptFunc = scriptFunc + ("|| " + model + name + getTypeBindNeg(i.bType) + "'" + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "'");
                    }
                    else
                    {
                        if(tmp == "true")
                        {
                            value = "<div ng-show=\"" + model + name + getTypeBind(i.bType) + " " + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace("\n", " ") + "";
                            scriptFunc = "$scope.$watch(\"" + model + name + getTypeBindNeg(i.bType) + "" + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "";
                        }
                        else
                        {
                            value = value + (" || " + model + name + getTypeBind(i.bType) + "'" + tmp.Replace(@"'", @"\'").Replace("\r\n", " ").Replace(@"\n", " ") + "'");
                            scriptFunc = scriptFunc + (" && " + model + name + getTypeBindNeg(i.bType) + "'" + tmp.Replace(@"'", @"\\'").Replace("\r\n", " ").Replace("\n", " ") + "'");
                        }
                    }
                }
            }
        }

        if(formFid != 0)
        {
            value += "\">";
            scriptFunc += "\",function(){";
            createWatchReset(scriptFunc, key);
            relevant.Add(key, value);
        }


    }
    /// <summary>
    /// Creates the HTML structure for the min/max constraints to be included in the numeric fields
    /// </summary>
    private void createConstraint()
    {
        int formFid = 0;
        int key = 0;
        string value = "";

        GRASPEntities db = new GRASPEntities();

        var constr = from c in db.ConstraintContainer
                     join fc in db.FormField_ConstraintContainer on c.id equals fc.constraints_id
                     select new { c, fc };

        foreach(var c in constr)
        {
            FieldConstraintRel2 fcr = new FieldConstraintRel2();
            fcr.ConstraintID = (int)c.fc.id;
            fcr.FormFieldID = (int)c.fc.FormField_id;
            fieldConstraintRel.Add(fcr);

            if(formFid != (int)c.fc.id)
            {
                if(formFid != 0)
                {
                    constraint.Add(key, value);
                }
                key = (int)c.fc.id;
                int v = Convert.ToInt32(c.c.value);
                switch(c.c.cNumber)
                {
                    case "GREATER_THAN":
                        value = "min=\"" + Convert.ToString(v + 1) + "\"";
                        break;
                    case "LESS_THAN":
                        value = "max=\"" + Convert.ToString(v - 1) + "\"";
                        break;
                    case "GREATER_EQUALS_THAN":
                        value = "min=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "LESS_EQUALS_THAN":
                        value = "max=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "EQUALS":
                        value = "max=\"" + Convert.ToString(v) + "\" min=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "NOT_EQUALS":
                        value = "";
                        break;
                }
                formFid = key;
            }
            else
            {
                int v = Convert.ToInt32(c.c.value);
                switch(c.c.cNumber)
                {
                    case "GREATER_THAN":
                        value += "min=\"" + Convert.ToString(v + 1) + "\"";
                        break;
                    case "LESS_THAN":
                        value += "max=\"" + Convert.ToString(v - 1) + "\"";
                        break;
                    case "GREATER_EQUALS_THAN":
                        value += "min=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "LESS_EQUALS_THAN":
                        value += "max=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "EQUALS":
                        value += "max=\"" + Convert.ToString(v) + "\" min=\"" + Convert.ToString(v) + "\"";
                        break;
                    case "NOT_EQUALS":
                        value += "";
                        break;
                }
            }
        }
        if(formFid != 0)
        {
            constraint.Add(key, value);
        }
    }
    /// <summary>
    /// </summary>
    /// <param name="p">A string representing a binding type</param>
    /// <returns>A symbol representing the negative of the binding type</returns>
    private string getTypeBindNeg(string p)
    {
        string signal = "";
        switch(p)
        {
            case "GREATER_THAN":
                signal = "<";
                break;
            case "LESS_THAN":
                signal = ">";
                break;
            case "GREATER_EQUALS_THAN":
                signal = "<=";
                break;
            case "LESS_EQUALS_THAN":
                signal = ">=";
                break;
            case "EQUALS":
            case "NULL":
            case "IS_CHECKED":
                signal = "!=";
                break;
            case "NOT_EQUALS":
            case "NOT_NULL":
            case "NOT_ACTIVE":
                signal = "==";
                break;
        }
        return signal;
    }
    /// <summary>
    /// Adds in the script of AngularJS some controls for a field
    /// </summary>
    /// <param name="scriptFunc">The script</param>
    /// <param name="key">The id representing the field</param>
    private void createWatchReset(string scriptFunc, int key)
    {
        GRASPEntities db = new GRASPEntities();
        var formfield = (from ff in db.FormField
                         where ff.id == key
                         select ff).FirstOrDefault();
        if(formfield != null)
        {
            //if (formfield.type == "NUMERIC_TEXT_FIELD" || formfield.type == "CURRENCY_FIELD")
            //{
            //    scriptFunc += " $scope.currForm." + formfield.name + " = 0;});";
            //}
            //else if (formfield.type == "REPEATABLES_BASIC" || formfield.type == "REPEATABLES")
            //{
            //    scriptFunc += " $scope.currForm." + formfield.name + " = [];});";
            //}
            //else if (formfield.type == "WRAPPED_TEXT" || formfield.type == "TRUNCATED_TEXT")
            //{
            //    scriptFunc += " });";
            //}
            //else scriptFunc += " $scope.currForm." + formfield.name + " = \"\";});";
            if(formfield.type == "REPEATABLES_BASIC" || formfield.type == "REPEATABLES")
            {
                scriptFunc += " $scope.currForm." + formfield.name + " = [];});";
            }
            else scriptFunc += "var key = \"" + formfield.name + "\"; delete $scope.currForm[key];});";
            Literal1.Text += scriptFunc;
        }

    }
    /// <summary>
    /// </summary>
    /// <param name="p">A string representing a binding type</param>
    /// <returns>A symbol representing the binding type</returns>
    private string getTypeBind(string p)
    {
        string signal = "";
        switch(p)
        {
            case "GREATER_THAN":
                signal = ">";
                break;
            case "LESS_THAN":
                signal = "<";
                break;
            case "GREATER_EQUALS_THAN":
                signal = ">=";
                break;
            case "LESS_EQUALS_THAN":
                signal = "<=";
                break;
            case "EQUALS":
            case "NOT_NULL":
            case "IS_CHECKED":
                signal = "==";
                break;
            case "NOT_EQUALS":
            case "NOT_ACTIVE":
            case "NULL":
                signal = "!=";
                break;
        }
        return signal;
    }
    /// <summary>
    /// Saves all the data inserted by the user into the DB.
    /// The result string is converted into a Dictionary, so for each of its elements a ResponseValue is created.
    /// For all the children of a roster or a table is used the createRoster method to save the data.
    /// This function is called by Javascript client-side.
    /// </summary>
    /// <param name="result">A string representing the model of the form (in the form of a json)</param>
    /// <param name="formID">The id of the form</param>
    /// <returns>The formI</returns>
    [WebMethod]
    public static string getJSON(string result, int formID)
    {
        //int formID = Convert.ToInt32(HttpContext.Current.Request["formID"]);
        int formResponseID = FormResponse.createFormResponse(formID);
        Dictionary<string, Object> values = JsonConvert.DeserializeObject<Dictionary<string, Object>>(result);
        Dictionary<string, int> ffields = FormField.getFormFieldsID(formID);


        string coords = "";
        foreach(var v in values)
        {
            int ffID = 0;
            ffields.TryGetValue(v.Key, out ffID);
            if(ffID == 0 || ffID == null)
            {
                int ind = v.Key.IndexOf("LatDE");
                if(ind > 0)
                {
                    coords = v.Value.ToString();
                }
                ind = v.Key.IndexOf("LongDE");
                if(ind > 0)
                {
                    coords += " " + v.Value.ToString();
                    string tmp = v.Key.Substring(0, ind);
                    ffID = FormField.getIdFromName(tmp, formID);
                    coords = coords.Replace(",", ".");
                    ResponseValue.createResponseValue(coords, formResponseID, ffID, 0);
                    FormResponseCoord.createFormResponseCoord(coords, formResponseID);
                    coords = "";
                }
            }
            else
            {
                int count = FormField.isRoster(ffID);
                if(count == -1)
                {
                    bool isEmpty = true;
                    int i = 0;
                    JArray rVal;
                    try
                    {
                        string a = null;
                        rVal = (JArray)values[v.Key];

                        foreach(var r in rVal)
                        {
                            isEmpty = false;
                            createRoster(r.ToString(), ffields, v.Key, formResponseID, ++i);
                        }
                    }
                    catch(Exception ex)
                    {
                        isSaved = "false";
                    }
                    if(!isEmpty)
                        ResponseValue.createResponseValue(i.ToString(), formResponseID, ffID, -1);
                }
                else
                {
                    try
                    {
                        Dictionary<string, Object> ResVal = JsonConvert.DeserializeObject<Dictionary<string, Object>>(v.Value.ToString());
                        ResponseValue.createResponseValue(ResVal.FirstOrDefault().Value.ToString(), formResponseID, ffID, 0);
                    }
                    catch(Exception ex)
                    {
                        ResponseValue.createResponseValue(v.Value.ToString(), formResponseID, ffID, 0);
                    }
                }
            }
        }
        ResponseValue.setPositionIndex(formResponseID);
        return formID.ToString();
    }
    /// <summary>
    /// Converts the responses in a Dictionary, so for each of its elements a ResponseValue is created.
    /// </summary>
    /// <param name="val">A string representing the responses for a roster or a table</param>
    /// <param name="ffields">The Dictionary representing all the fields for a form (Key = field_name, value = field_id)</param>
    /// <param name="key">The name of the roster field</param>
    /// <param name="formResponseID">The id representing the form Response</param>
    /// <param name="rc">An int representing the repetition count for this roster/table</param>
    public static void createRoster(string val, Dictionary<string, int> ffields, string key, int formResponseID, int rc)
    {
        Dictionary<string, Object> rVal = JsonConvert.DeserializeObject<Dictionary<string, Object>>(val);

        foreach(var rv in rVal)
        {
            int ffID = 0;
            ffields.TryGetValue(rv.Key, out ffID);
            if(ffID != 0 && ffID != null)
            {
                try
                {
                    Dictionary<string, Object> ResVal = JsonConvert.DeserializeObject<Dictionary<string, Object>>(rv.Value.ToString());
                    ResponseValue.createResponseValue(ResVal.FirstOrDefault().Value.ToString(), formResponseID, ffID, rc);
                }
                catch(Exception ex)
                {
                    ResponseValue.createResponseValue(rv.Value.ToString(), formResponseID, ffID, rc);
                }
            }

        }
    }
}

public class FieldConstraintRel2
{
    public int FormFieldID { get; set; }
    public int ConstraintID { get; set; }
}