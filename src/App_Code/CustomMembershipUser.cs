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
using System.Web.Security;

/// <summary>
/// Used to manage users
/// </summary>
public class CustomMembershipUser : MembershipUser
{

    private bool _IsSubscriber;
    private string _CustomerID;

    public bool IsSubscriber
    {
        get { return _IsSubscriber; }
        set { _IsSubscriber = value; }
    }

    public string CustomerID
    {
        get { return _CustomerID; }
        set { _CustomerID = value; }
    }

    public CustomMembershipUser(string providername,
                              string username,
                              object providerUserKey,
                              string email,
                              string passwordQuestion,
                              string comment,
                              bool isApproved,
                              bool isLockedOut,
                              DateTime creationDate,
                              DateTime lastLoginDate,
                              DateTime lastActivityDate,
                              DateTime lastPasswordChangedDate,
                              DateTime lastLockedOutDate,
                              bool isSubscriber,
                              string customerID)
        :
                              base(providername,
                                   username,
                                   providerUserKey,
                                   email,
                                   passwordQuestion,
                                   comment,
                                   isApproved,
                                   isLockedOut,
                                   creationDate,
                                   lastLoginDate,
                                   lastActivityDate,
                                   lastPasswordChangedDate,
                                   lastLockedOutDate)
    {
        this.IsSubscriber = isSubscriber;
        this.CustomerID = customerID;
    }
}
