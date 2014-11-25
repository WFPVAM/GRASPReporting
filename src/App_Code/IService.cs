using System;
using System.ServiceModel;
using System.ServiceModel.Web;

[ServiceContract(Namespace = "IService/JSONData")]
public interface IService
{
    [OperationContract]
    [WebInvoke(Method = "GET",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.Wrapped,
    UriTemplate = "json/{id}")]
    object json(String id);
}