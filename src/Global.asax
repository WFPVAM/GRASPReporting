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

       
</script>
