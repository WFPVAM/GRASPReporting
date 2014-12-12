using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

[ServiceContract(Namespace = "IService/JSONData")]
public interface IService
{
    [OperationContract][WebInvoke(Method = "GET",ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
    UriTemplate = "Surveys/{formId=null}/{columnNames=null}/{formResponseId=null}")]
    Message Surveys(string formId, string columnNames, string formResponseId);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
    UriTemplate = "CreateView/{viewName}/{tableName}/{columnNames}/{whereClause=null}/{orderbyClause=null}")]
    Message CreateView(string viewName, string tableName, string columnNames, string whereClause, string orderbyClause);
}