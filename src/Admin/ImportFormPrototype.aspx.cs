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

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
/// <summary>
/// Used to import a form from a csv file
/// </summary>
public partial class Admin_ImportFormPrototype : System.Web.UI.Page
{
    public string formID = "";
    public string name = "";

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        byte[] content;

        if (FileUpload1.HasFile)
        {
            string ext = Path.GetExtension(FileUpload1.FileName);
            if (ext == ".xml")
            {
                content = FileUpload1.FileBytes;
                string result = System.Text.Encoding.UTF8.GetString(content);
                XmlDocument form = TryParseXml(result);
                if (form != null)
                    GetFormData(form);
                else
                {
                    string errore = "XML errato";
                }
            }

        }
    }

    private XmlDocument TryParseXml(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            Debug.WriteLine("xmlContent:" + xml);
            return doc;
        }
        catch (XmlException e)
        {
            return null;
        }
    }
    //seguire il metodo doImport nel file FormPrototypeImportDialogHandler.java dal designer
    private void GetFormData(XmlDocument form)
    {
        //a partire dalla riga 200 
        string formID = "";
        string formName = "";

        formID = form.GetElementsByTagName("id").Item(0).FirstChild.Value.ToString();
        formName = form.GetElementsByTagName("h:title").Item(0).FirstChild.Value.ToString();
        Debug.WriteLine(formID);

        if (!Utility.existingForm(formID))
        {
            List<ImportBinding> bindList = new List<ImportBinding>();
            XmlNodeList bindings = form.GetElementsByTagName("bind");
            Debug.WriteLine("Found " + bindings.Count + " bind section");
            for (int i = 0; i < bindings.Count; i++)
            {
                XmlElement node = (XmlElement)bindings.Item(i);
                ImportBinding bind = checkAttributesBind(node);
                bindList.Add(bind);
                string path = "/" + bind.nodeset;
                Debug.WriteLine(path);

                try
                {
                    string tmpExpr = Utility.generateNameBind(path);
                    string tmpValue = "";
                    if (form.GetElementsByTagName(tmpExpr) != null)
                        tmpValue = form.GetElementsByTagName(tmpExpr).Item(0).FirstChild.Value.ToString();
                    bind.tagContent = tmpValue;
                    Debug.WriteLine("TAG CONTENT " + bind.tagContent);
                }
                catch (Exception ex)
                {
                    bind.tagContent = "";
                    Debug.WriteLine("TAG CONTENT " + bind.tagContent);
                }

            }

            List<ImportingElement> importingElemList = new List<ImportingElement>();
            XmlElement body = (XmlElement)form.GetElementsByTagName("h:body").Item(0);

            XmlNodeList groups = body.GetElementsByTagName("group");
            Debug.WriteLine("Found " + groups.Count + " groups");
            for (int i = 0; i < groups.Count; i++)
            {
                XmlElement g = (XmlElement)groups.Item(i);
                if (g.Attributes.GetNamedItem("appearance") != null)
                {
                    if (g.Attributes.GetNamedItem("appearance").Value == "field-list" && (g.ParentNode.Name == "h:body"))
                    {
                        checkAppearanceFieldList(g, importingElemList);
                    }
                }
                else if (g.Attributes.GetNamedItem("appearance") == null)
                {
                    {
                        if (g.Attributes.GetNamedItem("value") != null)
                        {
                            if (g.Attributes.GetNamedItem("value").Value == "repeatable-survey" && (g.ParentNode.Name == "h:body"))
                                checkRepeatableSurvey(g, importingElemList);
                        }
                        else if (g.ParentNode.Name == "h:body")
                        {
                            checkRepeatableBasic(g, importingElemList, form);
                        }
                    }
                }
            }
            pairElementsAndBinding(importingElemList, bindList);

            List<ImportingElement> totalElements = new List<ImportingElement>();
            foreach (ImportingElement elem in importingElemList)
            {
                totalElements.Add(elem);
                if (elem.fieldType == FormFieldType.REPEATABLES || elem.fieldType == FormFieldType.REPEATABLES_BASIC)
                {
                    foreach (ImportingElement rep in elem.repElements)
                    {
                        totalElements.Add(rep);
                    }
                }
            }

            foreach (ImportingElement elem in importingElemList)
            {
                elem.getIndexFormName();
                elem.detectFormFieldType();
                elem.detectBindings(totalElements);
                elem.detectConstraints();
            }

            importingElemList = discoverSeparators(importingElemList);

            //SAVE DATA TO DB
            Form formImported = FormField.createForm(formID, formName);
            foreach (ImportingElement elem in importingElemList)
            {
                FormField genFF = FormField.createFormField(elem, (int)formImported.id);
                elem.generatedFormField = genFF;
            }

            foreach (ImportingElement elem in importingElemList)
            {
                generateFormFieldAndBindings(elem);
            }

        }
        else
        {
            //form con questo id già presente sul DB
        }
    }

    private void generateFormFieldAndBindings(ImportingElement elem)
    {
        foreach (ImportingElement el in elem.elementsToBindings.Keys)
        {
            foreach (ImportingBindingContainer bc in elem.elementsToBindings[el])
            {
                FormField.createBinding(el.generatedFormField, bc);
            }
        }


        foreach (ImportConstraintContainer constCont in elem.constraints)
        {
            FormField.createConstraints(elem.generatedFormField, constCont);
        }
        if (elem.fieldType == FormFieldType.REPEATABLES || elem.fieldType == FormFieldType.REPEATABLES_BASIC)
        {
            foreach (ImportingElement rep in elem.repElements)
            {
                generateFormFieldAndBindings(rep);
            }

        }
    }

    private List<ImportingElement> discoverSeparators(List<ImportingElement> elements)
    {
        List<ImportingElement> ret = new List<ImportingElement>();
        int i = 0;
        foreach (ImportingElement el in elements)
        {
            if (el.positionIndex == i)
                ret.Add(el);
            else
            {
                ImportingElement closeGroupElem = new ImportingElement();
                closeGroupElem.fieldType = FormFieldType.SEPARATOR;
                closeGroupElem.type = "separator";
                closeGroupElem.name = "section" + i;
                closeGroupElem.positionIndex = i;
                ret.Add(closeGroupElem);
                ret.Add(el);
                i++;
            }
            i++;
        }
        return ret;
    }

    private void pairElementsAndBinding(List<ImportingElement> importingElements, List<ImportBinding> importBindings)
    {
        foreach (ImportingElement elem in importingElements)
        {
            foreach (ImportBinding bind in importBindings)
            {
                if ((elem.fieldType != FormFieldType.SEPARATOR) && (elem.reference == bind.nameReference))
                {
                    elem.bindReference = bind;
                }
                if (elem.fieldType == FormFieldType.REPEATABLES)
                {
                    foreach (ImportingElement repItem in elem.repElements)
                    {
                        foreach (ImportBinding bind2 in importBindings)
                        {
                            if (repItem.reference == bind2.nameReference)
                            {
                                repItem.bindReference = bind2;
                                break;
                            }
                        }
                    }
                }
                else if (elem.fieldType == FormFieldType.REPEATABLES_BASIC)
                {
                    foreach (ImportingElement repItem in elem.repElements)
                    {
                        foreach (ImportBinding bind2 in importBindings)
                        {
                            if (repItem.reference == bind2.nameReference)
                            {
                                repItem.bindReference = bind2;
                                break;
                            }
                        }
                    }
                }

            }
        }
    }

    private void checkRepeatableBasic(XmlElement g, List<ImportingElement> importingElemList, XmlDocument form)
    {
        ImportingElement newRepElem = new ImportingElement();
        newRepElem.isRepContainer = true;
        if (g.GetElementsByTagName("label").Item(0).FirstChild != null)
            newRepElem.label = g.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
        else newRepElem.label = "";
        XmlElement repeat = (XmlElement)g.GetElementsByTagName("repeat").Item(0);
        string groupRef = "";
        if (repeat.Attributes.GetNamedItem("nodeset") != null)
        {
            newRepElem.reference = repeat.Attributes.GetNamedItem("nodeset").Value.ToString();
        }
        if (repeat.Attributes.GetNamedItem("jr:count") != null)
        {
            String jrCount = repeat.Attributes.GetNamedItem("jr:count").Value.ToString();
            jrCount = cutData(jrCount);
            if (form.GetElementsByTagName(jrCount) != null)
                newRepElem.numberOfReps = Convert.ToInt32(form.GetElementsByTagName(jrCount).Item(0).FirstChild.Value.ToString().Trim());
        }
        if (g.Attributes.GetNamedItem("ref") != null)
        {
            groupRef = g.Attributes.GetNamedItem("ref").Value.ToString();
            newRepElem.reference = groupRef;
            newRepElem.name = newRepElem.cutReferenceName(newRepElem.reference);
        }
        XmlNodeList repElements = repeat.GetElementsByTagName("group").Item(0).ChildNodes;
        for (int i = 0; i < repElements.Count; i++)
        {
            if (repElements.Item(i).Name == "input")
            {
                XmlElement node = (XmlElement)repElements.Item(i);
                Debug.WriteLine("Found an input node");
                ImportingElement importingElem = checkAttributesRefInput(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                newRepElem.repElements.Add(importingElem);
            }
            if (repElements.Item(i).Name == "select")
            {
                XmlElement node = (XmlElement)repElements.Item(i);
                Debug.WriteLine("Found a select node");
                ImportingElement importingElem = checkAttributesRefSelect(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                newRepElem.repElements.Add(importingElem);
            }
            if (repElements.Item(i).Name == "select1")
            {
                XmlElement node = (XmlElement)repElements.Item(i);
                Debug.WriteLine("Found a select1 node");
                ImportingElement importingElem = checkAttributesRefSelect1(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                if (importingElem.typeAttribute != "currency")
                    newRepElem.repElements.Add(importingElem);
            }
        }
        newRepElem.type = "repeatables";
        newRepElem.fieldType = FormFieldType.REPEATABLES_BASIC;
        Debug.WriteLine(newRepElem.repElements.Count);
        importingElemList.Add(newRepElem);
    }

    private string cutData(string jrCount)
    {
        string ret = "";
        int beginCut = jrCount.IndexOf("/", 1);
        ret = jrCount.Substring(beginCut + 1);
        return ret;
    }
    /// <summary>
    /// repeatable group tied with a survey
    /// </summary>
    /// <param name="g"></param>
    /// <param name="importingElemList"></param>
    private void checkRepeatableSurvey(XmlElement g, List<ImportingElement> importingElemList)
    {
        ImportingElement newRepElem = new ImportingElement();
        string groupRef = "";
        newRepElem.isRepContainer = true;
        if (g.GetElementsByTagName("label").Item(0).FirstChild != null)
            newRepElem.label = g.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
        else newRepElem.label = "";
        if (g.Attributes.GetNamedItem("ref") != null)
        {
            groupRef = g.Attributes.GetNamedItem("ref").Value.ToString();
            newRepElem.reference = groupRef;
            newRepElem.name = groupRef;
        }
        if (g.Attributes.GetNamedItem("reflist") != null)
            newRepElem.refListName = g.Attributes.GetNamedItem("reflist").Value.ToString();

        //retrieves a list of survey elements examining the repetitions
        checkSurvey(g, newRepElem, groupRef);
        Debug.WriteLine("Element in repeatable section: " + newRepElem.repElements.Count);
        newRepElem.type = "repeatables";
        newRepElem.fieldType = FormFieldType.REPEATABLES;
        Debug.WriteLine(newRepElem.repElements.Count);
        importingElemList.Add(newRepElem);
    }

    private void checkSurvey(XmlElement g, ImportingElement newRepElem, string groupRef)
    {
        XmlNodeList surveyElements = g.GetElementsByTagName("group");
        for (int i = 0; i < surveyElements.Count; i++)
        {
            XmlElement el = (XmlElement)surveyElements.Item(i);
            if (el.Attributes.GetNamedItem("appearance") != null)
            {
                if (el.Attributes.GetNamedItem("appearance").Value == "field-list")
                {
                    if (el.GetElementsByTagName("label").Item(0).FirstChild != null)
                        newRepElem.surveyValues.Add(el.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString());
                }
            }
        }
        //cycle in the first repetition
        checkFirstRepetition(surveyElements, newRepElem, groupRef);

    }

    private void checkFirstRepetition(XmlNodeList surveyElements, ImportingElement newRepElem, string groupRef)
    {
        XmlNodeList firstRep = surveyElements.Item(0).ChildNodes;
        for (int j = 0; j < firstRep.Count; j++)
        {
            if (firstRep.Item(j).Name == "input")
            {
                XmlElement node = (XmlElement)firstRep.Item(j);
                Debug.WriteLine("Found an input node");
                ImportingElement importingElem = checkAttributesRefInput(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                newRepElem.repElements.Add(importingElem);
            }
            if (firstRep.Item(j).Name == "select")
            {
                XmlElement node = (XmlElement)firstRep.Item(j);
                Debug.WriteLine("Found a select node");
                ImportingElement importingElem = checkAttributesRefSelect(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                newRepElem.repElements.Add(importingElem);
            }
            if (firstRep.Item(j).Name == "select1")
            {
                XmlElement node = (XmlElement)firstRep.Item(j);
                Debug.WriteLine("Found a select1 node");
                ImportingElement importingElem = checkAttributesRefSelect1(node);
                if (node.Attributes.GetNamedItem("ref") != null)
                {
                    importingElem.reference = groupRef + "/" + node.Attributes.GetNamedItem("ref").Value.ToString();
                    importingElem.generateName();
                }
                importingElem.isRepItem = true;
                importingElem.repContainer = newRepElem;
                if (importingElem.typeAttribute != "currency")
                    newRepElem.repElements.Add(importingElem);
            }
        }
    }
    /// <summary>
    /// cycle for normal items
    /// </summary>
    /// <param name="g"></param>
    /// <param name="importingElemList"></param>
    private void checkAppearanceFieldList(XmlElement g, List<ImportingElement> importingElemList)
    {
        XmlNodeList groupElements = g.ChildNodes;
        Debug.WriteLine("Found " + groupElements.Count + " elements in this group");
        for (int j = 0; j < groupElements.Count; j++)
        {
            if (groupElements.Item(j).Name == "input")
            {
                Debug.WriteLine("Found an input node");
                XmlElement node = (XmlElement)groupElements.Item(j);
                ImportingElement importingElem = checkAttributesRefInput(node);
                importingElemList.Add(importingElem);
            }
            if (groupElements.Item(j).Name == "select")
            {
                Debug.WriteLine("Found a select node");
                XmlElement node = (XmlElement)groupElements.Item(j);
                ImportingElement importingElem = checkAttributesRefSelect(node);
                importingElemList.Add(importingElem);
            }
            if (groupElements.Item(j).Name == "select1")
            {
                Debug.WriteLine("Found a select1 node");
                XmlElement node = (XmlElement)groupElements.Item(j);
                ImportingElement importingElem = checkAttributesRefSelect1(node);
                if (importingElem.typeAttribute != "currency")
                    importingElemList.Add(importingElem);
            }
        }
    }

    private ImportingElement checkAttributesRefSelect1(XmlElement node)
    {
        ImportingElement importingElem = new ImportingElement();

        if (node.Attributes.GetNamedItem("appearance") != null)
            importingElem.select1Appearance = node.Attributes.GetNamedItem("appearance").Value.ToString();
        if (node.GetElementsByTagName("label").Item(0).FirstChild != null)
            importingElem.label = node.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
        else importingElem.label = "";
        if (node.Attributes.GetNamedItem("ref") != null)
        {
            importingElem.reference = node.Attributes.GetNamedItem("ref").Value.ToString();
            importingElem.generateName();
        }
        if (node.Attributes.GetNamedItem("reflist") != null)
            importingElem.refListName = node.Attributes.GetNamedItem("reflist").Value.ToString();

        XmlNodeList itemList = node.GetElementsByTagName("item");
        Debug.WriteLine("Found " + itemList.Count + " items in select1 node");
        for (int i = 0; i < itemList.Count; i++)
        {
            XmlElement item = (XmlElement)itemList.Item(i);
            string labelValue = "";
            string itemValue = "";

            labelValue = item.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
            itemValue = item.GetElementsByTagName("value").Item(0).FirstChild.Value.ToString();
            if (labelValue != "Select" && itemValue == "0")
                importingElem.select1Labels.Add(labelValue);
        }
        importingElem.type = "select1";
        if (node.Attributes.GetNamedItem("type") != null)
            importingElem.typeAttribute = node.Attributes.GetNamedItem("type").Value.ToString();

        return importingElem;
    }

    private ImportingElement checkAttributesRefSelect(XmlElement node)
    {
        ImportingElement importingElem = new ImportingElement();

        XmlNodeList items = node.GetElementsByTagName("item");
        XmlElement uniqueItem = (XmlElement)items.Item(0);
        if (uniqueItem.GetElementsByTagName("label").Item(0).FirstChild != null)
            importingElem.label = uniqueItem.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
        else importingElem.label = "";
        if (node.Attributes.GetNamedItem("ref") != null)
        {
            importingElem.reference = node.Attributes.GetNamedItem("ref").Value.ToString();
            importingElem.generateName();
        }
        importingElem.type = "select";
        if (node.Attributes.GetNamedItem("type") != null)
            importingElem.typeAttribute = node.Attributes.GetNamedItem("type").Value.ToString();

        return importingElem;
    }

    private ImportingElement checkAttributesRefInput(XmlElement node)
    {
        ImportingElement importingElem = new ImportingElement();

        if (node.Attributes.GetNamedItem("ref") != null)
        {
            importingElem.reference = node.Attributes.GetNamedItem("ref").Value.ToString();
            importingElem.generateName();
        }
        importingElem.type = "input";
        if (node.Attributes.GetNamedItem("type") != null)
        {
            importingElem.typeAttribute = node.Attributes.GetNamedItem("type").Value.ToString();
        }
        else importingElem.typeAttribute = "";
        if (!importingElem.typeAttribute.Contains("label"))
        {
            if (node.GetElementsByTagName("label").Item(0).FirstChild != null)
                importingElem.label = node.GetElementsByTagName("label").Item(0).FirstChild.Value.ToString();
            else importingElem.label = "";

        }
        else if (node.Attributes.GetNamedItem("labelvalue") != null)
        {
            importingElem.label = node.Attributes.GetNamedItem("labelvalue").Value.ToString();
        }
        else importingElem.label = "";
        if (node.Attributes.GetNamedItem("calc") != null)
        {
            importingElem.calculated = node.Attributes.GetNamedItem("calc").Value.ToString();
        }

        return importingElem;
    }

    private ImportBinding checkAttributesBind(XmlNode node)
    {
        ImportBinding bind = new ImportBinding();

        if (node.Attributes.GetNamedItem("nodeset") != null)
            bind.nodeset = node.Attributes.GetNamedItem("nodeset").Value.ToString();
        if (node.Attributes.GetNamedItem("type") != null)
            bind.type = node.Attributes.GetNamedItem("type").Value.ToString();
        if (node.Attributes.GetNamedItem("required") != null)
            bind.required = (node.Attributes.GetNamedItem("required").Value.ToString() == "true()") ? true : false;
        if (node.Attributes.GetNamedItem("readonly") != null)
            bind.readOnly = (node.Attributes.GetNamedItem("readonly").Value == "true()") ? true : false;
        if (node.Attributes.GetNamedItem("constraint") != null)
            bind.constraint = node.Attributes.GetNamedItem("constraint").Value;
        if (node.Attributes.GetNamedItem("jr:constraintMsg") != null)
            bind.constraintMessage = node.Attributes.GetNamedItem("jr:constraintMsg").Value;
        if (node.Attributes.GetNamedItem("relevant") != null)
            bind.relevant = node.Attributes.GetNamedItem("relevant").Value;
        bind.generateNameReference();

        return bind;
    }
}