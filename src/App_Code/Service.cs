using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service" nel codice, nel file svc e nel file di configurazione contemporaneamente.
public class Service : IService
{
    object IService.json(string id)
    {
        decimal idForm;
        string result = string.Empty;

        if (decimal.TryParse(id, out idForm))
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                var items = from rv in db.FormResponse
                            where rv.parentForm_id == idForm
                            orderby rv.id ascending
                            select rv;

                List<FormResponse> listFormResponse = (List<FormResponse>)items.ToList();
                foreach (FormResponse response in listFormResponse)
                {
                    result += FormResponse.GetAsJson(int.Parse(response.id.ToString()));
                }

                //return new Students { FirstName = "Emanuele", LastName = "Tramutola", Id = id };
                return result;
            }
        }
        else
        {
            throw new ArgumentException("The ID submitted is invalid.");
        }
    }
}