using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GlobalVariables
/// </summary>
public static class GlobalVariables
{
    //Whether the processing is already running.
    public static bool IsProcessIncomingRunning { get; set; }

    //Whether is there are new forms arrived in the incoming folder. Uses for notification.
    public static bool IsNewFormsArrived { get; set; }

    /// <summary>
    /// Uses to show the last Process Form Responses Time in Home page.
    /// </summary>
    public static string LastProcessFormResponsesTime { get; set; }

    /// <summary>
    /// Wether notification button for new incoming forms is active. Uses to change the button style.
    /// </summary>
    public static bool IsNotificationButtonActive { get; set; }
}