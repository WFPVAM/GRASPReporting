using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Permissions
/// </summary>
/// <author>Saad Mansour</author>
public partial class Permissions
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    public static List<Permissions> GetAllPermissionses()
    {
        List<Permissions> permissionses = null;
        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                permissionses = (from p in db.Permissions
                    select p).ToList();
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return permissionses;
    }

    /// <summary>
    /// Gets permissions ids of the given role id.
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    public static List<Role_Permissions> GetRolePermissionsByRoleID(int roleId)
    {
        List<Role_Permissions> rolePermissionsespermissionses = null;

        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                 rolePermissionsespermissionses = (from rp in db.RolePermissions
                    where rp.RoleID == roleId
                    select rp).ToList();
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return rolePermissionsespermissionses;
    }

    /// <summary>
    /// Adds the new role permissions and removes the deleted role permissions.
    /// </summary>
    /// <param name="newRolePermissionses"></param>
    /// <param name="deletedRolePermissionses"></param>
    /// <returns></returns>
    public static bool UpdateRolePermissions(List<Role_Permissions> newRolePermissionses, List<Role_Permissions> deletedRolePermissionses)
    {
        bool isSuccess = false;

        try
        {
            using (GRASPEntities db = new GRASPEntities())
            {
                if (deletedRolePermissionses != null)
                {
                    foreach (Role_Permissions rolePermissions in deletedRolePermissionses)
                    {
                        Role_Permissions rolePermissionsToRemove = db.RolePermissions.Single(o => o.id == rolePermissions.id);
                        db.RolePermissions.Remove(rolePermissionsToRemove);
                    }
                }

                if (newRolePermissionses != null)
                {
                    db.RolePermissions.AddRange(newRolePermissionses);   
                }

                db.SaveChanges();
            }

            isSuccess = true;
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return isSuccess;
    }

    /// <summary>
    /// Checks whether the logged user has the givin permission.
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    /// <author>Saad Mansour</author>
    public static bool IsLoggedUserHasPermission(GeneralEnums.Permissions permission)
    {
        bool isLoggedUserInRole = false;

        try
        {
            string loggedUserName = HttpContext.Current.User.Identity.Name.ToUpper();
            int userRoleID = User_Credential.GetRoleIdByUserName(loggedUserName);

            using (GRASPEntities db = new GRASPEntities())
            {
                var permissionName = from p in db.Permissions
                    join rp in db.RolePermissions on p.id equals rp.PermissionID
                    where rp.RoleID == userRoleID
                    select p.Name;

                if (permissionName.Contains(permission.ToString()))
                {
                    isLoggedUserInRole = true;
                }
            }
        }
        catch (Exception ex)
        {
            LogUtils.WriteErrorLog(ex.ToString());
        }

        return isLoggedUserInRole;
    }
}