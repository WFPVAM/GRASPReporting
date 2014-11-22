using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for ServerSideCalculatedField
/// </summary>
public static class ServerSideCalculatedField
{
    public static void GenerateSingle(int formID, int formResponseID)
    {
        using(GRASPEntities db = new GRASPEntities())
        {
            var ffe = (from f in db.FormFieldExt
                      where f.FormID == formID
                      select f).OrderBy(f=>f.FormFieldID);
            foreach(var f in ffe)
            {
                CalculateFormulaPL(formID, f.FormFieldExtFormula, 0, f.FormFieldExtID, false, formResponseID);
            }

        }
    }

    public static string Generate(int formFieldExtID)
    {
        string res = "";
        using(GRASPEntities db = new GRASPEntities())
        {
            var ffe = from f in db.FormFieldExt
                      where f.FormFieldExtID == formFieldExtID
                      select f;
            foreach(var f in ffe)
            {
                res = CalculateFormulaPL((int)f.FormID, f.FormFieldExtFormula, 0, f.FormFieldExtID, false);
            }
        }
        return res;
    }

    public static string CalculateFormulaPL(int formID, string formula, int iteration, int? formFieldExtID, bool test)
    {
        return CalculateFormulaPL(formID, formula, iteration, formFieldExtID, test, 0);
    }

    public static string CalculateFormulaPL(int formID, string formula, int iteration, int? formFieldExtID, bool test, int formResponseID)
    {
        string outResult = "";
        int formFieldID = 0;
        int positionIndex = 0;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("BEGIN TRANSACTION;");

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        GRASPEntities db = new GRASPEntities();

        int i = 1;
        int n = 1;

        if(!test)
        {
            FormFieldExt ffe = (from f in db.FormFieldExt
                                where f.FormFieldExtID == formFieldExtID
                                select f).FirstOrDefault();
            formFieldID = (int)ffe.FormFieldID;
            positionIndex = (int)ffe.PositionIndex;

            if(formFieldExtID != null)
            {
                if(formResponseID == 0) //Delete all the existing results for a particular formField
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM ResponseValueExt WHERE formFieldID = " + formFieldID);
                }
                else //delete only the calculated field for the current responseID
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM ResponseValueExt WHERE FormResponseID = " + formResponseID + " AND formFieldID = " + formFieldID);
                }
            }
            else //Delete all the fields of the form response
            {
                db.Database.ExecuteSqlCommand("DELETE FROM ResponseValueExt WHERE FormResponseID = " + formResponseID);
            }
        }



        var formFields = (from ff in db.FormField
                          where ff.form_id == formID && ff.type == "NUMERIC_TEXT_FIELD"
                          orderby ff.id
                          select new { ff.name }
                         ).Union(
                            from fe in db.FormFieldExt
                            where fe.FormID == formID
                            orderby fe.PositionIndex
                            select new { name = fe.FormFieldExtName });



        var respValues = (from rv in db.FormFieldResponses
                          where rv.parentForm_id == formID && (rv.type == "NUMERIC_TEXT_FIELD" || rv.type == "SERVERSIDE-CALCULATED")
                            && formula.Contains(rv.name) && (formResponseID == 0 || rv.FormResponseID == formResponseID)
                          select new { name = rv.name, value = rv.value, formResponseID = rv.FormResponseID }).ToList();

        var frmResponses = db.FormResponse.Where(w => w.parentForm_id == formID && (w.id == formResponseID || formResponseID == 0)).ToList();
        
        foreach(var r in frmResponses.AsParallel())
        {
            CalcEngine.CalcEngine ce = new CalcEngine.CalcEngine();
            var dct = new CalcDictionary(ce);
            ce.Variables = dct;
            ce.CultureInfo = new System.Globalization.CultureInfo("en-US");
            foreach(var ff in formFields.ToList())
            {
                dct[ff.name] = 0;
            }

            foreach(var rv in respValues.ToList().AsParallel().Where(re => re.formResponseID == r.id))
            {
                if(rv.value.Length > 0)
                {
                    decimal val = 0;
                    Decimal.TryParse(rv.value, out val);
                    object exVal = 0;
                    dct.TryGetValue(rv.name, out exVal);
                    decimal exvald = Convert.ToDecimal(exVal);
                    dct[rv.name] = exvald + val;
                }
            }
            string eval = "";
            try
            {
                eval = ce.Evaluate(formula).ToString();
                if(!test && formFieldExtID != null)
                {
                    //ResponseValueExt.Insert(db, (int)r.id, formFieldExtID.Value, formFieldID, positionIndex, Double.Parse(eval));

                    sb.Append("INSERT INTO ResponseValueExt ([FormResponseID],[FormFieldExtID],[nvalue],[FormFieldID],[PositionIndex]) VALUES " +
                            "(" + r.id.ToString() + "," + formFieldExtID.Value.ToString() + "," + eval + "," + formFieldID.ToString() + "," + positionIndex.ToString() + ");\r\n");

                }

            }
            catch(Exception ex)
            {
                outResult = "Error has occured on formResponse No. " + r.id.ToString() + ", please review the formula and try again.<br/>";
                outResult += ex.Message + "<br>" + ex.StackTrace + "<br>";
                break;
            }
            if(test)
            {
                outResult += "Result on formResponse No. " + r.id.ToString() + " = " + eval + "<br/>";
            }
            i++;
            n++;
            if(iteration != 0 && i == iteration)
                break;
            if(!test && n == 500)
            {
                n = 0;
                //db.SaveChanges();
                sb.AppendLine("COMMIT TRANSACTION;");
                db.Database.ExecuteSqlCommand(sb.ToString());
                sb.Clear();
                sb.AppendLine("BEGIN TRANSACTION;");
            }
        }


        if(!test && !outResult.Contains("Error"))
        {
            //db.SaveChanges();
            sb.AppendLine("COMMIT TRANSACTION;");
            db.Database.ExecuteSqlCommand(sb.ToString());
            sb.Clear();
            outResult = "Formula executed on " + i.ToString() + " record(s)";
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        //Debug.WriteLine("RunTime: " + elapsedTime);
        outResult += "Time taken: [" + elapsedTime + "]";
        return outResult;
    }

}


public class CalcDictionary : IDictionary<string, object>
{
    CalcEngine.CalcEngine _ce;
    Dictionary<string, object> _dct;

    public CalcDictionary(CalcEngine.CalcEngine ce)
    {
        _ce = ce;
        _dct = new Dictionary<string, object>();
    }

    //---------------------------------------------------------------
    #region IDictionary<string,object> Members

    public void Add(string key, object value)
    {
        _dct.Add(key, value);
    }
    public bool ContainsKey(string key)
    {
        return _dct.ContainsKey(key);
    }
    public ICollection<string> Keys
    {
        get { return _dct.Keys; }
    }
    public bool Remove(string key)
    {
        return _dct.Remove(key);
    }
    public ICollection<object> Values
    {
        get { return _dct.Values; }
    }
    public bool TryGetValue(string key, out object value)
    {
        if(_dct.TryGetValue(key, out value))
        {
            var expr = value as string;
            if(expr != null && expr.Length > 0 && expr[0] == '=')
            {
                value = _ce.Evaluate(expr.Substring(1));
            }
            return true;
        }
        return false;
    }
    public object this[string key]
    {
        get
        {
            object value;
            if(TryGetValue(key, out value))
            {
                return value;
            }
            throw new Exception("invalid index");
        }
        set
        {
            _dct[key] = value;
        }
    }
    #endregion

    //---------------------------------------------------------------
    #region ICollection<KeyValuePair<string,object>> Members

    public void Add(KeyValuePair<string, object> item)
    {
        var d = _dct as ICollection<KeyValuePair<string, object>>;
        d.Add(item);
    }
    public void Clear()
    {
        _dct.Clear();
    }
    public bool Contains(KeyValuePair<string, object> item)
    {
        var d = _dct as ICollection<KeyValuePair<string, object>>;
        return d.Contains(item);
    }
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        var d = _dct as ICollection<KeyValuePair<string, object>>;
        d.CopyTo(array, arrayIndex);
    }
    public int Count
    {
        get { return _dct.Count; }
    }
    public bool IsReadOnly
    {
        get { return false; }
    }
    public bool Remove(KeyValuePair<string, object> item)
    {
        var d = _dct as ICollection<KeyValuePair<string, object>>;
        return d.Remove(item);
    }
    #endregion

    //---------------------------------------------------------------
    #region IEnumerable<KeyValuePair<string,object>> Members

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _dct.GetEnumerator() as IEnumerator<KeyValuePair<string, object>>;
    }

    #endregion

    //---------------------------------------------------------------
    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _dct.GetEnumerator() as System.Collections.IEnumerator;
    }

    #endregion
}