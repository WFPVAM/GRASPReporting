<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Http" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        GlobalConfiguration.Configuration.MessageHandlers.Add(
    new BasicAuthenticationMessageHandler()
);
        System.Web.Routing.RouteTable.Routes.MapHttpRoute(
             name: "DefaultLogin",
             routeTemplate: "api/{controller}"
        );
        System.Web.Routing.RouteTable.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/{controller}/{action}/{id}",
             defaults: new { id = System.Web.Http.RouteParameter.Optional }
        );
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
        //Server.Transfer("~/errors/default.aspx", true);

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        // I take the url referer host. (manipulating the query string this value is null or your local address)
        string strRefererHost = Request.UrlReferrer == null ? string.Empty : Request.UrlReferrer.Host;

        // This is the host name of your application 
        string strUrlHost = Request.Url.Host;

        // I read the query string parameters
        string strQSPars = Request.Url.Query ?? string.Empty;

        // If the referer is not the application host (... someone manipulated the qs)...  
        // and    there is a query string parameter (be sure of this otherwise nobody can access the default page of your site
        // because this page has always a local referer...)
        if(strRefererHost != strUrlHost && strQSPars != string.Empty
            && Request.Url.LocalPath.Contains("ViewForm.aspx"))
            Response.Redirect("~/WrongReferer.aspx"); // your error page

    }
       
</script>
