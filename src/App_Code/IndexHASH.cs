using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for IndexHash
/// </summary>
public partial class IndexHASH
{
	public static void InsertHashString(GRASPEntities db, int formResponseID, int indexID, string stringToHASH)
    {
        IndexHASH h = new IndexHASH();
        h.FormResponseID = formResponseID;
        h.IndexID = indexID;
        h.IndexHASHString = GetHashString(stringToHASH);
        db.IndexHASHes.Add(h);
    }

    private static byte[] GetHash(string inputString)
    {
        HashAlgorithm algorithm = SHA1.Create();  // SHA1.Create()
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach(byte b in GetHash(inputString))
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

    public static int DeleteHASHes(int indexID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            return db.Database.ExecuteSqlCommand("DELETE FROM IndexHASHes WHERE IndexID = @indexID", new SqlParameter("@indexID", indexID));
        }
    }


}