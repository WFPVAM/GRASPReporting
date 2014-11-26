using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

[ServiceContract(Namespace = "IService/JSONData")]
public interface IService
{
    [OperationContract][WebInvoke(Method = "GET",ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
    UriTemplate = "Surveys/{formId=null}/{formResponseId=null}")]
    Message Surveys(string formId, string formResponseId);
}