using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.Models;
using Fly.Web.Authorization;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Fly.Core.DataAccess;
namespace Fly.Web.Controllers
{
    [Permission]
    public class MenuController : Controller
    {
        [ChildActionOnly]
        public PartialViewResult StartMenu()
        {
            List<ViewPermissionGroup> PermissionGroups;
           
            //由于后台Controllers部分没有加上 [Permission]  
            if (System.Web.HttpContext.Current.Session["PermissionGroups"] == null)
            {
                using (FlyDbContext cxt = new FlyDbContext())
                {
                    var employeeId = Guid.Parse(User.Identity.GetUserId());
                    var employee = cxt.Employees.FirstOrDefault(m => m.Id == employeeId);
                    var roleIdList = employee.Roles.Select(m => m.RoleId).ToList();
                    System.Web.HttpContext.Current.Session["PermissionGroups"] =
                        GetViewPermissionGroup(cxt, roleIdList, employee.PlatformId);
                    System.Web.HttpContext.Current.Session["PermissionLines"] =
                       GetViewPermissionLine(cxt, roleIdList);
                }
                PermissionGroups = (System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>).OrderBy(p => p.SN).ToList();
                return PartialView(PermissionGroups);
            }
            else {
                PermissionGroups = (System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>).OrderBy(p => p.SN).ToList();
                return PartialView(PermissionGroups);
            }
        }
        [ChildActionOnly]
        public PartialViewResult SubMenu()
        {
            UrlHelper urlHelper = new UrlHelper(this.Request.RequestContext);
            var controllerName = urlHelper.RequestContext.RouteData.Values["controller"].ToString();
            var actionName = urlHelper.RequestContext.RouteData.Values["action"].ToString();
            List<ViewPermissionGroup> showPermissionGoups = new List<ViewPermissionGroup>();
            if (System.Web.HttpContext.Current.Session["PermissionGroups"] != null)
            {
                var permissonGroups = (System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>).OrderBy(p => p.SN);
                var parentId = permissonGroups.SingleOrDefault(p => p.Url.ToLower() == ("/" + controllerName + "/" + actionName).ToLower()).ParentId;
                ViewBag.subTitle = permissonGroups.SingleOrDefault(p => p.Id == parentId).DisplayName;
                showPermissionGoups = permissonGroups.Where(p => p.ParentId == parentId).ToList();
            }

            return PartialView(showPermissionGoups);
        }
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
    }
}