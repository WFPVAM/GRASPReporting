using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_CompareResponses : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if(!Utility.VerifyAccess(Request))
        //{
        //    Response.Write("<h3>Access Denied</h3>");
        //    Response.End();
        //}

        if(Request["id"] != null && Request["id"] != "")
        {
            string hash = Request["h"];
            string frToGet = "";
            StringBuilder sb = new StringBuilder();

            GRASPEntities db = new GRASPEntities();

            var frIDs = from h in db.IndexHASHes
                        where h.IndexHASHString == hash
                        select h.FormResponseID;
            foreach(var frID in frIDs)
            {
                frToGet = frToGet + (" OR FormResponseID=" + frID.ToString() + " ");
            }
            frToGet = frToGet.Substring(4);

            string sqlCmd = "";
            sqlCmd = "SELECT STUFF((SELECT distinct ',' + QUOTENAME( cast( (1000 + positionIndex) as varchar(10) ) + '_' + name " +
                        " + CAST(ROW_NUMBER() OVER(PARTITION BY FormResponseID, formFieldId, value ORDER BY (1000 + positionIndex)) AS VARCHAR(5))) " +
                        "from FormFieldResponses " +
                        "WHERE type!='SEPARATOR' AND type!='TRUNCATED_TEXT' AND type!='WRAPPED_TEXT' AND " +
                        "      RVRepeatCount=0 AND (" + frToGet + ") " +
                        " order by 1 " +
                        "FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') ,1,1,'')";
            string columnList = db.Database.SqlQuery<string>(sqlCmd).FirstOrDefault();
            db.Dispose();

            //Build ColumnHeader to use in the CSV file.
            string columnHeader = "";

            //columnHeader = columnHeader.Substring(0, columnHeader.Length - 1);
            string[] colList = columnList.Split(',');
            for(int i = 0; i < colList.Length; i++)
            {
                columnHeader = columnHeader + ("<th>" + colList[i].Substring(6, colList[i].Length - 8) + "</th>\r\n");
            }
            columnHeader = "<tr>\r\n<th>FormResponseID</th>" + (columnHeader + "</tr>\r\n");



            sqlCmd = "SELECT * FROM (SELECT FormResponseID," + columnList + " from " +
                 "(select FormResponseID,value, ( (cast( (1000 + positionIndex) as varchar(10) ) + '_' + name " +
                 "+ CAST( ROW_NUMBER() OVER(PARTITION BY FormResponseID, formFieldId, value ORDER BY (1000 + positionIndex)) AS VARCHAR(5)))" +
                 ") fn from FormFieldResponses " +
                 "WHERE type != 'SEPARATOR' AND type != 'TRUNCATED_TEXT' AND type!='WRAPPED_TEXT' AND " +
                 "      RVRepeatCount=0 AND (" + frToGet + ") " +
                 ") x pivot ( max(value) for fn in (" + columnList + ") ) p) AS X";


            SqlConnection sqlConnection1 = new SqlConnection(System.Configuration.ConfigurationManager.
                                                            ConnectionStrings["GRASP_MemberShip"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = sqlCmd;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                IDataRecord rec = (IDataRecord)reader;
                sb.Append("<tr>");
                for(int i = 0; i < rec.FieldCount; i++)
                {
                    sb.Append("<td>" + rec[i].ToString() + "</td>\r\n");
                }

                sb.Append("</tr>\r\n");
            }

            //Explicit obj close&destroy
            reader.Close();
            reader.Dispose();
            sqlConnection1.Close();
            sqlConnection1.Dispose();
            litTable.Text = "<table>" + columnHeader + sb.ToString() + "</table>";
        }
    }
}