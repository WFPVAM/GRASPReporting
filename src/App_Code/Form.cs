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
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Form class contains auxiliary functions to query Form table on Grasp DB
/// </summary>
public partial class Form
{
    /// <summary>
    /// Queries the DB to check if the formID exists
    /// </summary>
    /// <param name="formID">The id representing the form</param>
    /// <returns>A string representing the form name</returns>
    public static string getFormName(int formID)
    {
        GRASPEntities db = new GRASPEntities();

        var item = (from f in db.Form
                   where f.id == formID
                   select f).FirstOrDefault();

        if (item != null)
            return item.name;
        else return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="downloadedForms"></param>
    /// <returns></returns>
    /// <editedBy>Saad Mansour</editedBy>
    public static string GetAllXFormsByPhoneNumber(string phone, List<string> downloadedForms)
    {
        //XForms of all forms together.
        StringBuilder xmlForms = new StringBuilder();
        //sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><forms>"); //s3
        //sb.Append("<forms>"); //s3

        using (GRASPEntities db = new GRASPEntities())
        {
            var groupsMemberships = (from gm in db.GroupMembership
                                     where gm.contact_contact_id == (from c in db.Contact
                                                                     where c.phoneNumber == phone
                                                                     select c.contact_id).FirstOrDefault()
                                     select gm.group_path);

            if (groupsMemberships != null)
            {
                foreach (var groupMembership in groupsMemberships) //Get user mobile forms of all its groups.
                {
                    Dictionary<string, decimal> forms = (from f in db.Form
                                                         where f.permittedGroup_path == groupMembership && f.finalised == 1
                                                         select new { f.id_flsmsId, f.id }).ToDictionary(p => p.id_flsmsId, p => p.id);

                    if (forms != null)
                    {
                        //Remove the already downloaded forms by mobile user.
                        foreach (string dF in downloadedForms)
                        {
                            if (forms.ContainsKey(dF))
                            {
                                forms.Remove(dF);
                            }
                        }

                        //Appends the xForm for all forms together.
                        foreach (var rF in forms)
                        {
                            //XmlElement formNode = sv.CreateElement("form");
                            xmlForms.Append(global::FormField.GetX_Form((int)rF.Value));
                            //formNode.InnerText = sb.ToString().Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                            //rootNode.AppendChild(formNode);

                        }
                    }
                }
            }
        }

        //sb.Append("</forms>"); //s3
        return xmlForms.ToString();
    }
}