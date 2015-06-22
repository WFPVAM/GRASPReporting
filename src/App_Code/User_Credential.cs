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
using System.Web;

/// <summary>
/// User_Credential class contains auxiliary functions to query User_Credential table on Grasp DB
/// </summary>
public partial class User_Credential
{

    public static string checkUserFromNumber(string number)
    {
        if (number == null || number == "")
        {
            return "ERROR:Client phone number not received";
        }
        else
        {
            GRASPEntities db = new GRASPEntities();

            var user = (from u in db.Contact
                        where u.phoneNumber == number
                        select u).FirstOrDefault();
            if (user != null)
                return "OK";
            else return "ERROR:Phone number \"" + number + "\" not in the server's contact list";
        }
    }
    /// <summary>
    /// Queries the DB to get information about username
    /// </summary>
    /// <param name="username">A string representing the username</param>
    /// <returns>The name of the current user</returns>
    public static string getNameForUser(string username)
    {
        GRASPEntities db = new GRASPEntities();

        string user = (from u in db.User_Credential
                      where u.username == username
                      select u.name).FirstOrDefault();
        if (user != null)
            return user;
        else return "";
    }
    /// <summary>
    /// Queries the DB to get information about user's role
    /// </summary>
    /// <param name="username">A string representing the username</param>
    /// <returns>A string representing the role for that user</returns>
    public static string getRoleForUser(string username)
    {
        GRASPEntities db = new GRASPEntities();

        string role = (from u in db.User_Credential
                       where u.username == username
                       select u.supervisor).FirstOrDefault();
        if (role != null)
            return role;
        else return "";
    }
    /// <summary>
    /// Queries the DB to obtain all the system's users
    /// </summary>
    /// <returns>A list of users</returns>
    public static IQueryable<User_Credential> getUsers()
    {
        GRASPEntities db = new GRASPEntities();

        var users = from u in db.User_Credential
                    where u.UserDeleteDate == null
                    orderby u.username
                    select u;

        return users;
    }
    /// <summary>
    /// Update all the informations about a user in the DB
    /// </summary>
    /// <param name="email">A string representing the user email</param>
    /// <param name="name"></param>
    /// <param name="surname"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="phone_number"></param>
    /// <param name="roles_id">the id of the role of the user</param>
    /// <param name="user_id">the id of the user</param>
    /// <returns>The updated user</returns>
    public static User_Credential updateUser(string email, string name, string surname, string username, string password, string phone_number, int roles_id, int user_id)
    {
        GRASPEntities db = new GRASPEntities();

        var user = (from u in db.User_Credential
                   where u.user_id == user_id
                    select u).FirstOrDefault();
        user.email = email;
        user.name = name;
        user.surname = surname;
        user.username = username;
        user.password = password;
        user.phone_number = phone_number;
        user.roles_id = roles_id;
        user.supervisor = Role.getRole(roles_id);
        db.SaveChanges();

        return user;
    }
    /// <summary>
    /// Adds a user on the DB
    /// </summary>
    /// <param name="email"></param>
    /// <param name="name"></param>
    /// <param name="surname"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="phone_number"></param>
    /// <param name="roles_id"></param>
    /// <returns>the user added on the DB</returns>
    public static User_Credential addUser(string email, string name, string surname, string username, string password, string phone_number, int roles_id)
    {
        GRASPEntities db = new GRASPEntities();

        var user = new User_Credential();

        user.email = email ?? string.Empty;
        user.name = name ?? string.Empty;
        user.surname = surname ?? string.Empty;
        user.username = username;
        user.password = password;
        user.phone_number = phone_number ?? string.Empty;
        user.roles_id = roles_id;
        user.supervisor = Role.getRole(roles_id);

        db.User_Credential.Add(user);
        db.SaveChanges();

        return user;
    }
    /// <summary>
    /// Deletes the user from the DB
    /// </summary>
    /// <param name="user_id">the id of the user</param>
    /// <returns>the deleted user</returns>
    public static User_Credential deleteUser(int user_id)
    {
        GRASPEntities db = new GRASPEntities();

        var user = (from u in db.User_Credential
                    where u.user_id == user_id
                    select u).FirstOrDefault();

        user.UserDeleteDate = DateTime.Now;
        db.SaveChanges();

        return user;
    }

    public static User_Credential UpdateUserResponseFilter(int user_id, string filter)
    {
        GRASPEntities db = new GRASPEntities();

        var user = (from u in db.User_Credential
                    where u.user_id == user_id
                    select u).FirstOrDefault();
        user.UserResponseFilter = filter;
        db.SaveChanges();

        return user;
    }

}