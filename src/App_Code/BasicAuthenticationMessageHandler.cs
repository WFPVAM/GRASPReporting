/*
 *GRASP(Geo-referential Real-time Acquisition Statistics Platform) Reporting Tool <http://www.brainsen.com>
 * Developed by Brains Engineering s.r.l (marco.giorgi@brainsen.com)
 * This file is part of GRASP Reporting Tool.  
 *  GRASP Reporting Tool is free software: you can redistribute it and/or modify it
 *  under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or (at
 *  your option) any later version.  
 *  GRASP Reporting Tool is distributed in the hope that it will be useful, but
 *  WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser  General Public License for more details.  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with GRASP Reporting Tool. 
 *  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

/// <summary>
/// BasicAuthenticationMessaheHandler class contains a method to check the user authorization for calling Grasp API
/// </summary>
public class BasicAuthenticationMessageHandler : DelegatingHandler
{
    /// <summary>
    /// Parses the request headers to check the permission of the sender.
    /// If it has a Basic Authentication then it makes all the checks on the user
    /// </summary>
    /// <param name="request">The HTTP request message to send</param>
    /// <param name="cancellationToken">Token to send if the request will crash</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization;

        if (authHeader == null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (authHeader.Scheme != "Basic")
        {
            return base.SendAsync(request, cancellationToken);
        }

        var encodedUserPass = authHeader.Parameter.Trim();
        var userPass = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUserPass));
        var parts = userPass.Split(":".ToCharArray());
        var username = parts[0];
        var password = parts[1];

        GRASPEntities db = new GRASPEntities();

        var item = (from u in db.User_Credential
                    join r in db.Roles on u.roles_id equals r.id
                    where u.username == username && u.password == password
                    select new { u, r }).FirstOrDefault();
        if (item == null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        var identity = new GenericIdentity(username, "Basic");
        string[] roles = new string[1];
        roles[0] = item.r.description;
        var principal = new GenericPrincipal(identity, roles);
        Thread.CurrentPrincipal = principal;
        if (HttpContext.Current != null)
        {
            HttpContext.Current.User = principal;
        }

        return base.SendAsync(request, cancellationToken);
    }
}