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
using System.Linq;
using System.Web;

public enum FormFieldType
{
    TRUNCATED_TEXT,
    WRAPPED_TEXT,
    TEXT_AREA,
    TEXT_FIELD,
    NUMERIC_TEXT_FIELD,
    DROP_DOWN_LIST,
    CHECK_BOX,
    RADIO_BUTTON,
    CURRENCY_FIELD,
    DATE_FIELD,
    REPEATABLES,
    REPEATABLES_BASIC,
    GEOLOCATION,
    BARCODE,
    IMAGE,
    EMAIL_FIELD,
    PHONE_NUMBER_FIELD,
    SEPARATOR
}

public enum BindType
{
    NULL,
    NOT_NULL,
    NOT_ACTIVE,
    GREATER_THAN,
    LESS_THAN,
    EQUALS,
    NOT_EQUALS,
    IS_CHECKED,
    LESS_EQUALS_THAN,
    GREATER_EQUALS_THAN

}

public enum ConstraintNumber
{
    MAXIMUM_NUMBER_OF_CHARACTER,
    MINIMUM_NUMBER_OF_CHARACTER,
    GREATER_THAN,
    LESS_THAN,
    EQUALS,
    NOT_EQUALS,
    LESS_EQUALS_THAN,
    GREATER_EQUALS_THAN
}
/// <summary>
/// Summary description for ImportingElement
/// </summary>
public class ImportingElement
{
    public ImportBinding bindReference { get; set; }
    public FormFieldType fieldType { get; set; }
    public string type = "";
    public string typeAttribute = "";
    public string name = "";
    public string label = "";
    public string reference = "";
    public string attributeType = "";
    public string select1Appearance = "";
    public List<string> select1Labels = new List<string>();
    public string refListName = "";
    public Dictionary<ImportingElement, List<ImportingBindingContainer>> elementsToBindings = new Dictionary<ImportingElement, List<ImportingBindingContainer>>();
    public List<ImportConstraintContainer> constraints = new List<ImportConstraintContainer>();
    public FormField generatedFormField { get; set; }
    public bool isRepContainer = false;
    public List<ImportingElement> repElements = new List<ImportingElement>();
    public int numberOfReps = 0;
    public bool isRepItem = false;
    public List<string> surveyValues = new List<string>();
    public ImportingElement repContainer { get; set; }
    public string bindingsPolicy = "All";
    public string constraintPolicy = "All";
    public int positionIndex;
    public string calculated { get; set; }

    public static string AND_POLICY = "All";
    public static string OR_POLICY = "Any";
    public static List<string> bindSymbols =
        new List<string>(new string[] { " = ", " != ", " > ", " < ", " >= ", " <= " });

    public string generateName()
    {
        name = reference;
        while (name.Contains("/"))
        {
            name = cutReferenceName(name);
        }
        return name;
    }

    public string cutReferenceName(string referenceName)
    {
        int beginCut = referenceName.IndexOf("/", 1);
        referenceName = referenceName.Substring(beginCut + 1);
        return referenceName;
    }


    public void getIndexFormName()
    {
        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (name[i] == '_')
            {
                positionIndex = Convert.ToInt32(name.Substring(i + 1));
                break;
            }
        }
    }

    public void detectFormFieldType()
    {
        if (type == "select")
        {
            fieldType = FormFieldType.CHECK_BOX;
        }
        else if (type == "select1")
        {
            if (select1Appearance == "full")
            {
                fieldType = FormFieldType.RADIO_BUTTON;
            }
            else if (select1Appearance == "minimal")
            {
                fieldType = FormFieldType.DROP_DOWN_LIST;
            }
        }
        else if (type == "input")
        {
            if (bindReference != null)
            {
                if (bindReference.type == "date")
                    fieldType = FormFieldType.DATE_FIELD;
                else if (bindReference.type == "int")
                    fieldType = FormFieldType.NUMERIC_TEXT_FIELD;
                else if (bindReference.type == "decimal" && typeAttribute != "currency")
                    fieldType = FormFieldType.NUMERIC_TEXT_FIELD;
                else if (bindReference.type == "geopoint")
                    fieldType = FormFieldType.GEOLOCATION;
                else if (bindReference.type == "barcode")
                    fieldType = FormFieldType.BARCODE;
                else if (bindReference.type == "image")
                    fieldType = FormFieldType.IMAGE;
                else if (typeAttribute == "label")
                {
                    fieldType = FormFieldType.TRUNCATED_TEXT;
                    label = bindReference.tagContent;
                }
                else if (typeAttribute == "multi-label")
                {
                    fieldType = FormFieldType.WRAPPED_TEXT;
                    label = bindReference.tagContent;
                }
                else if (typeAttribute == "currency")
                {
                    fieldType = FormFieldType.CURRENCY_FIELD;
                }
                else if (bindReference.type == "string" && bindReference.constraintMessage.Contains("email"))
                    fieldType = FormFieldType.EMAIL_FIELD;
                else if (bindReference.type == "string" && bindReference.constraintMessage.Contains("phonenumber"))
                    fieldType = FormFieldType.PHONE_NUMBER_FIELD;
                else if (typeAttribute == "tarea")
                {
                    fieldType = FormFieldType.TEXT_AREA;
                }
                else if (bindReference.type == "string")
                    fieldType = FormFieldType.TEXT_FIELD;
            }
        }
        if (fieldType == FormFieldType.REPEATABLES || fieldType == FormFieldType.REPEATABLES_BASIC)
        {
            foreach (ImportingElement rep in repElements)
                rep.detectFormFieldType();
        }
    }

    public void detectBindings(List<ImportingElement> totalElements)
    {
        if (this.fieldType == FormFieldType.REPEATABLES || this.fieldType == FormFieldType.REPEATABLES_BASIC)
        {
            foreach (ImportingElement repEl in this.repElements)
            {
                repEl.detectBindings(totalElements);
            }
        }
        if (bindReference != null)
        {
            string splitString = " and ";
            if (bindReference.relevant.Contains(" and "))
            {
                splitString = " and ";
                bindingsPolicy = AND_POLICY;
            }
            if (bindReference.relevant.Contains(" or "))
            {
                splitString = " or ";
                bindingsPolicy = OR_POLICY;
            }
            string[] relevantElements = bindReference.relevant.Split(new string[] { splitString }, StringSplitOptions.None);
            foreach (string relevant in relevantElements)
            {
                if (relevant != "" && !relevant.Contains("selected("))
                {
                    foreach (string t in bindSymbols)
                    {
                        if (relevant.Contains(t.ToString()))
                        {
                            string[] splitted = relevant.Split(new string[] { t }, StringSplitOptions.None);
                            string referenceName = "";
                            if (splitted[0].Contains("./../"))
                            {
                                referenceName = splitted[0].Substring(splitted[0].LastIndexOf("/") + 1);
                                string prefix = splitReferenceName(this.reference)[0];
                                referenceName = prefix + "/" + referenceName;
                            }
                            else
                            {
                                referenceName = cutReferenceName(splitted[0].Trim());
                            }

                            string bindValue = "";
                            if (splitted.Length == 1)
                                bindValue = splitted[0];
                            else
                            {
                                bindValue = splitted[1];

                                for (int i = 2; i < splitted.Length; i++)
                                {
                                    bindValue = bindValue + " = " + splitted[i];
                                }
                            }

                            ImportingBindingContainer newBinding = new ImportingBindingContainer();
                            if ((bindValue[0] == '\'') && (bindValue[bindValue.Length - 1] == '\''))
                                bindValue = bindValue.Substring(1, bindValue.Length - 1);
                            if (t == " = " && bindValue == "")
                                newBinding.bType = BindType.NULL;
                            else if(t == " != " && bindValue == "")
                                newBinding.bType = BindType.NOT_NULL;
                            else if (t == " = " && bindValue != "")
                                newBinding.bType = BindType.EQUALS;
                            else if (t == " != " && bindValue != "")
                                newBinding.bType = BindType.NOT_EQUALS;
                            else if (t == " > ")
                                newBinding.bType = BindType.GREATER_THAN;
                            else if (t == " < ")
                                newBinding.bType = BindType.LESS_THAN;
                            else if (t == " >= ")
                                newBinding.bType = BindType.GREATER_EQUALS_THAN;
                            else if (t == " <= ")
                                newBinding.bType = BindType.LESS_EQUALS_THAN;

                            newBinding.value = bindValue;

                            foreach (ImportingElement el in totalElements)
                            {
                                if (el.fieldType != FormFieldType.SEPARATOR)
                                {
                                    Debug.WriteLine("element ref: " + el.reference);
                                    Debug.WriteLine("binding referencename: " + referenceName);
                                    if (el.reference.Trim() == referenceName.Trim())
                                    {
                                        Debug.WriteLine("adding relevant binding");
                                        if (el.fieldType == FormFieldType.CHECK_BOX)
                                        {
                                            if (bindValue == "true")
                                            {
                                                newBinding.bType = BindType.IS_CHECKED;
                                                newBinding.value = "";
                                            }
                                        }
                                        if (elementsToBindings.ContainsKey(el))
                                        {
                                            elementsToBindings[el].Add(newBinding);
                                        }
                                        else
                                        {
                                            elementsToBindings.Add(el, new List<ImportingBindingContainer>());
                                            elementsToBindings[el].Add(newBinding);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (relevant.Contains("selected("))
                {
                    int beginCut = relevant.IndexOf(relevant) + 9;
                    int endCut = relevant.IndexOf(")", beginCut + 1);
                    string selected = relevant.Substring(beginCut, endCut);
                    string[] splitted = selected.Split(new string[] { " , " }, StringSplitOptions.None);
                    string referenceName = splitted[0];
                    referenceName = cutReferenceName(referenceName);

                    ImportingBindingContainer newBinding = new ImportingBindingContainer();
                    newBinding.bType = BindType.IS_CHECKED;
                    foreach(ImportingElement el in totalElements)
                    {
                        if (el.reference.Trim() == referenceName.Trim())
                        {
                            if (elementsToBindings.ContainsKey(el))
                            {
                                elementsToBindings[el].Add(newBinding);
                            }
                            else
                            {
                                elementsToBindings.Add(el, new List<ImportingBindingContainer>());
                                elementsToBindings[el].Add(newBinding);
                            }
                        }
                    }
                }
            }

        }
    }

    private string[] splitReferenceName(string referenceName)
    {
        int cutIndex = referenceName.LastIndexOf("/");
        string[] ret = new string[2];
        ret[0] = referenceName.Substring(0, cutIndex);
        ret[1] = referenceName.Substring(cutIndex + 1);
        return ret;
    }

    public void detectConstraints()
    {
        Debug.WriteLine(this.fieldType.ToString());
        if (this.fieldType == FormFieldType.REPEATABLES || this.fieldType == FormFieldType.REPEATABLES_BASIC)
        {
            foreach (ImportingElement repEl in this.repElements)
            {
                repEl.detectConstraints();
            }
        }

        if (bindReference != null)
        {
            string splitString = " and ";
            if (bindReference.constraint.Contains(" and "))
            {
                splitString = " and ";
                constraintPolicy = AND_POLICY;
            }
            if (bindReference.constraint.Contains(" or "))
            {
                splitString = " or ";
                constraintPolicy = OR_POLICY;
            }

            string[] constraintElements = bindReference.constraint.Split(new string[] { splitString }, StringSplitOptions.None);
            foreach (string constraint in constraintElements)
            {
                if (constraint != "" && !constraint.Contains("regex"))
                {
                    foreach (string t in bindSymbols)
                    {
                        if (constraint.Contains(t))
                        {
                            string[] splitted = constraint.Split(new string[] { t }, StringSplitOptions.None);
                            string constraintValue = splitted[1];
                            if ((constraintValue[0] == '\'' && constraintValue[constraintValue.Length - 1] == '\''))
                            {
                                constraintValue = constraintValue.Substring(1, constraintValue.Length - 1);
                            }

                            ImportConstraintContainer constCont = new ImportConstraintContainer();
                            constCont.value = constraintValue;

                            if (t == " = ")
                                constCont.cNumber = ConstraintNumber.EQUALS;
                            else if (t == " != ")
                                constCont.cNumber = ConstraintNumber.NOT_EQUALS;
                            else if (t == " > ")
                                constCont.cNumber = ConstraintNumber.GREATER_THAN;
                            else if (t == " < ")
                                constCont.cNumber = ConstraintNumber.LESS_THAN;
                            else if (t == " >= ")
                                constCont.cNumber = ConstraintNumber.GREATER_EQUALS_THAN;
                            else if (t == " <= ")
                                constCont.cNumber = ConstraintNumber.LESS_EQUALS_THAN;

                            this.constraints.Add(constCont);
                            break;
                        }
                    }
                }
            }
        }
    }

    private static string BindToString(BindType value)
    {
        string ret = "";
        switch (value.ToString())
        {
            case "NULL":
                break;
            case "NOT_NULL":
                break;
            case "NOT_ACTIVE":
                break;
            case "GREATER_THAN":
                break;
            case "LESS_THAN":
                break;
            case "EQUALS":
                break;
            case "NOT_EQUALS":
                break;
            case "IS_CHECKED":
                break;
            case "LESS_EQUALS_THAN":
                break;
            case "GREATER_EQUALS_THAN":
                break;
        }
        return ret;
    }

}