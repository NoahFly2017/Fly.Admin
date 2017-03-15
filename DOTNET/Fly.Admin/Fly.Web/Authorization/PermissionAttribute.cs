using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.DataAccess;
using Fly.Core.Models;
using System.Xml;
using System.Xml.Linq;
using System.Security.Principal;
using System.Web.Mvc.Properties;
using System.Text;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Fly.Web.Infrastructure;

namespace Fly.Web.Authorization
{
    /// <summary>
    /// 权限筛子
    ///匿名权限 使用[AllowAnonymous]
    ///其他和Authorize使用一样，如：[Permission]只需要登录
    ///数据验证[Permission(IsData=True)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                // 如果发现在权限项中没有添加的权限项
                // 就添加进数据库
                // 并标识为 "未初始化的权限,"
                string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
                //FindUrl(action, controller);
                // return PermissionParticle.UrlPermission(action, controller);
                return true;//只进行系统自带登陆验证，测试环境使用
            }
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            //var userHostAddress = IPHelper.GetClientIP();
            //if (!string.IsNullOrEmpty(userHostAddress))
            //{
            //    var ips = IPHelper.GetWhiteIPs();
            //    if (ips.IPv4.Count(s => s == userHostAddress) == 0 && ips.IPv6.Count(s => s == userHostAddress) == 0)
            //    {
            //        if (HttpContext.Current.Request.RequestType.ToLower() != "post")
            //        {
            //            HttpContext.Current.Response.Write("<!DOCTYPE html><html><head><title>IP未获得授权</title><link href=\"/Content/style/error.css\" rel=\"stylesheet\" /></head><body id=\"IP\"><div><i></i><p>您的IP(<b>" + IPHelper.GetClientIP() + "</b>)不在系统许可范围内，请与管理员联系添加IP。</p><p>添加后如依旧显示此提示，请稍后再试。</p></div></body></html>");
            //        }
            //        else
            //        {
            //            HttpContext.Current.Response.Write("{\"resultCode\":-1,\"message\":\"IP: " + IPHelper.GetClientIP() + " 不在许可范围\"}");
            //        }

            //        filterContext.Result = new EmptyResult();
            //    }
            //}
            //else
            //{
            //    HttpContext.Current.Response.Write("<!DOCTYPE html><html><head><title>IP未获得授权</title><link href=\"/Content/style/error.css\" rel=\"stylesheet\" /></head><body id=\"IP\"><div><i></i><p>您的IP不在系统许可范围内，请与管理员联系添加IP。</p><p>添加后如依旧显示此提示，请稍后再试。</p></div></body></html>");
            //    filterContext.Result = new EmptyResult();
            //}
            base.OnAuthorization(filterContext);
        }


        #region 发现没有添加到数据库中的请求，添加到数据库中做记录,测试环境保留,生产环境删除
        private void FindUrl(string action, string controller)
        {
            using (FlyDbContext cxt = new FlyDbContext())
            {
                var path = "/" + controller + "/" + action;
                var i = cxt.PermissionLines.Where(m => m.Url.ToLower().Contains(path.ToLower())).Count();
                i += cxt.PermissionGroups.Where(m => m.Url.ToLower().Contains(path.ToLower()) && m.PlatformId == cxt.Platforms.FirstOrDefault().Id).Count();
                if (i == 0)
                {
                    PermissionGroup permissionGroup = cxt.PermissionGroups.Where(m => m.DisplayName == "未初始化的权限组").FirstOrDefault();
                    if (permissionGroup == null)
                    {
                        permissionGroup = new PermissionGroup
                        {
                            DisplayName = "未初始化的权限组",
                            Tag = "#",
                            Url = "#",
                            SN = 0,
                            PermissionLines = new List<PermissionLine>(),
                            PlatformId = cxt.Platforms.FirstOrDefault().Id
                        };
                        cxt.PermissionGroups.Add(permissionGroup);
                    }
                    permissionGroup.PermissionLines.Add(
                        new PermissionLine
                        {
                            DisplayName = "未初始化的权限项",
                            Url = path,
                            Tag = "#"
                        });
                    cxt.SaveChanges();
                }
            }
        }

        #endregion
    }
}
