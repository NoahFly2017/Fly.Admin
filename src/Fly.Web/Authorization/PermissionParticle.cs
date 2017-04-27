using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using Fly.Core.Models;
using Fly.Core.DataAccess;

using System.Web.Mvc;
using Fly.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web.Mvc.Html;



namespace Fly.Web.Authorization
{
    /// <summary>
    /// 权限
    /// </summary>
    public static class PermissionParticle
    {
        #region 数据库中权限保存到Session
        /// <summary>
        /// 获取权限组
        /// </summary>
        /// <param name="cxt"></param>
        /// <returns></returns>
        private static List<ViewPermissionGroup> GetViewPermissionGroup(FlyDbContext cxt, List<Guid> roleIdList, Guid platformId)
        {

            List<PermissionGroup> childrenPermissionGroups = (from t in cxt.PermissionGroups join t1 in cxt.PermissionLines on t.Id equals t1.GroupId join t2 in cxt.RolePermissions on t1.Id equals t2.PermissionLineId where roleIdList.Contains(t2.RoleId) && t.PlatformId == platformId select t).ToList();
            List<PermissionGroup> newChildrenPermissionGroups = childrenPermissionGroups;
            do
            {
                newChildrenPermissionGroups = newChildrenPermissionGroups.Where(m => m.Parent != null).Select(m => m.Parent).ToList();
                childrenPermissionGroups.AddRange(newChildrenPermissionGroups);
            }

            while (newChildrenPermissionGroups.Where(m => m.Parent != null).Select(m => m.Parent).ToList().Count > 0);
            return childrenPermissionGroups.Distinct().Select(m => new ViewPermissionGroup { Id = m.Id, ParentId = m.ParentId, Url = m.Url, Tag = m.Tag, SN = m.SN, Headshot = m.Headshot, DisplayName = m.DisplayName }).ToList();//；拥有的权限组
        }
        /// <summary>
        /// 获取权限项
        /// </summary>
        /// <param name="cxt"></param>
        /// <returns></returns>
        private static List<ViewPermissionLine> GetViewPermissionLine(FlyDbContext cxt, List<Guid> roleIdList)
        {
            return (from t in cxt.PermissionLines join t1 in cxt.RolePermissions on t.Id equals t1.PermissionLineId where roleIdList.Contains(t1.RoleId) select t).Distinct().Select(m => new ViewPermissionLine { Id = m.Id, Url = m.Url, GroupId = m.GroupId, Tag = m.Tag }).ToList();
        }

        /// <summary>
        /// 重新把权限加入session中
        /// </summary>
        public static void SetPermission()
        {
            var permissionGroups = new List<ViewPermissionGroup>();
            var permissionLines = new List<ViewPermissionLine>();
            SetPermission(out permissionGroups, out permissionLines);
        }

        #region private
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="permissionGroups"></param>
        /// <param name="permissionLines"></param>
        private static void GetPermission(out List<ViewPermissionGroup> permissionGroups, out List<ViewPermissionLine> permissionLines)
        {
            permissionGroups = System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>;
            permissionLines = System.Web.HttpContext.Current.Session["PermissionLines"] as List<ViewPermissionLine>;
           // if ((permissionLines == null && permissionGroups == null) || permissionLines.Count == 0 && permissionGroups.Count == 0)
            if (permissionLines == null || permissionGroups == null)
            {
                SetPermission(out permissionGroups, out permissionLines);
            }
        }
        /// <summary>
        /// 把权限加入session中
        /// </summary>
        /// <param name="permissionGroups"></param>
        /// <param name="permissionLines"></param>
        private static void SetPermission(out List<ViewPermissionGroup> permissionGroups, out List<ViewPermissionLine> permissionLines)
        {
            using (FlyDbContext cxt = new FlyDbContext())
            {
                var employeeId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                var employee = cxt.Employees.FirstOrDefault(m => m.Id == employeeId);
                var roleIdList = employee.Roles.Select(m => m.RoleId).ToList();
                #region 临时使用
                Init(roleIdList);
                #endregion

                System.Web.HttpContext.Current.Session["PermissionGroups"] =
                    permissionGroups = GetViewPermissionGroup(cxt, roleIdList, employee.PlatformId);
                System.Web.HttpContext.Current.Session["PermissionLines"] =
                    permissionLines = GetViewPermissionLine(cxt, roleIdList);

            }
        }
        #endregion
        #endregion



        #region 判断权限
        /// <summary>
        /// 是否有访问指定controller的action权限
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static bool UrlPermission(string actionName, string controllerName)
        {
            List<ViewPermissionGroup> permissionGroups;
            List<ViewPermissionLine> permissionLines;


            GetPermission(out permissionGroups, out permissionLines);

            UrlHelper url = new System.Web.Mvc.UrlHelper();
            if (actionName == null && controllerName == null)
            {
                return false;
            }
            var path = "/" + controllerName + "/" + actionName; //url.Action(actionName, controllerName);

            var i = permissionGroups != null ? permissionGroups.Where(m => m.Url.ToLower().Contains(path.ToLower())).Count() : 0;
            i += permissionLines != null ? permissionLines.Where(m => m.Url.ToLower().Contains(path.ToLower())).Count() : 0;
            return i > 0;
            //return true;
        }
        /// <summary>
        /// 指定标识是否包含在权限
        /// </summary>
        /// <param name="htmltag"></param>
        /// <returns></returns>
        public static bool TagPermissionLine(string htmltag)
        {
            List<ViewPermissionGroup> permissionGroups;
            List<ViewPermissionLine> permissionLines;
            GetPermission(out permissionGroups, out permissionLines);
            var i = permissionGroups != null ? permissionGroups.Where(m => m.Tag.ToLower() == htmltag.ToLower()).Count() : 0;
            i += permissionLines != null ? permissionLines.Where(m => m.Tag.ToLower() == htmltag.ToLower()).Count() : 0;
            return i > 0;
           // return true;
        }
        #endregion

        #region 临时使用
        /// <summary>
        /// 把所有权限项加到当前用户的角色
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="roleIdList"></param>
        public static void Init(List<Guid> roleIdList)
        {
            if (roleIdList.Count == 0)
            {
                using (FlyDbContext cxt = new FlyDbContext())
                {
                    var employeeId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                    var role = cxt.Roles.FirstOrDefault();
                    var Permissions = cxt.PermissionLines.ToList();
                    var Employee = cxt.Employees.Where(m => m.Id == employeeId).FirstOrDefault();
                    foreach (var permissionLine in Permissions)
                    {
                        var rolePermission = new RolePermission();
                        rolePermission.PermissionLineId = permissionLine.Id;
                        rolePermission.RoleId = role.Id;
                        cxt.RolePermissions.Add(rolePermission);
                    }
                    Employee.Roles.Add(new EmployeeRole { RoleId = role.Id });
                    cxt.SaveChanges();
                }
            }
        }
        #endregion

        #region


        #endregion
    }
    /// <summary>
    ///     权限组/功能菜单。
    /// </summary>
    public class ViewPermissionGroup
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     父级权限组Id，顶级为NULL。
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        ///     权限组/功能菜单的页面路径。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///     功能菜单的HTML标识。
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        ///     权限组序号
        /// </summary>
        public int SN { get; set; }

        public string DisplayName { get; set; }
        public string Headshot { get; set; }

    }
    /// <summary>
    ///     权限项/按钮功能。
    /// </summary>
    public class ViewPermissionLine
    {
        public Guid Id { get; set; }

        /// <summary>
        ///     所属权限组/菜单功能，关联到PermissionGroup表Id字段
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        ///     权限项/按钮功能的服务方法/路径。
        /// </summary> 
        public string Url { get; set; }

        /// <summary>
        ///     权限项/按钮功能在HTML中的标识。
        /// </summary>
        public string Tag { get; set; }
    }
}
