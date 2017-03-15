using System;
using System.Collections.Generic;
using System.Web.Mvc.Properties;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using Fly.Core.Models;
using Fly.Web.Authorization;


namespace System.Web.Mvc.Html
{

    public static class HtmlExtensions 
    {
        #region ActionLinkPermission
        /// <summary>
        ///返回指定的具有权限的a标签（a 元素）。
        /// </summary>
        /// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="linkText">定位点元素的内部文本</param>
        /// <param name="permissionKey">权限标标识</param>
        /// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
        /// <returns> 一个定位点元素（a 元素）。</returns>
        public static MvcHtmlString ActionLinkPermission(this HtmlHelper htmlHelper, string linkText, string permissionKey, object htmlAttributes)
        {
            return htmlHelper.ActionLinkPermission(linkText, permissionKey, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            
        }
        /// <summary>
        /// easyUI 扩展方法
        /// </summary>
        /// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="linkText">定位点元素的内部文本</param>
        /// <param name="permissionKey">权限标标识</param>
        /// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
        /// <returns> 一个定位点元素（a 元素）。</returns>
        public static MvcHtmlString ActionLinkPermission(this HtmlHelper htmlHelper, string linkText, string htmlTag, IDictionary<string, object> htmlAttributes)
        {
            return PermissionParticle.TagPermissionLine(htmlTag) ? MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, linkText, htmlAttributes)) : MvcHtmlString.Empty;
        }
        public static MvcHtmlString HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName, string contentString)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? new MvcHtmlString(contentString) :MvcHtmlString.Empty;
        }
        public static MvcHtmlString NoActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName, string contentString)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ?  MvcHtmlString.Empty:new MvcHtmlString(contentString);
        }
        public static Boolean HasActionPermission( string actionName, string controllerName)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? true : false;
        }

        public static Boolean HasTagPermission(string htmlTag)
        {
            //需啊一个根据tag判断是否显示的方法
            return PermissionParticle.TagPermissionLine( htmlTag) ? true : false;

        }

        /// <summary>
        /// 创建a标签
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="linkText"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        private static string GenerateLinkInternal(RequestContext requestContext, string linkText, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = (!string.IsNullOrEmpty(linkText)) ? HttpUtility.HtmlEncode(linkText) : string.Empty
            };
            tagBuilder.MergeAttributes<string, object>(htmlAttributes);
            return tagBuilder.ToString(TagRenderMode.Normal);
        }
        #endregion




        #region URL.Action
        public static string ActionPermissionEmpty(this UrlHelper Url, string actionName, string controllerName)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName) : string.Empty;
        }
        
        public static string ActionPermission(this UrlHelper Url)
        {
            var controllerName = Url.RequestContext.RouteData.Values["controller"].ToString();
            var actionName = Url.RequestContext.RouteData.Values["action"].ToString();
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action() : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName)
        {
            var controllerName = Url.RequestContext.RouteData.Values["controller"].ToString();
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName) : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName, object routeValues)
        {
            var controllerName = Url.RequestContext.RouteData.Values["controller"].ToString();
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, routeValues) : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName, RouteValueDictionary routeValues)
        {
            var controllerName = Url.RequestContext.RouteData.Values["controller"].ToString();
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, routeValues) : "/Permission/UnPermissionMessage";
        }

        public static string ActionPermission(this UrlHelper Url, string actionName, string controllerName)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName) : "/Permission/UnPermissionMessage";
        }
      
        public static string ActionPermission(this UrlHelper Url, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName, routeValues) : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName, string controllerName, RouteValueDictionary routeValues, string protocol)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName, routeValues, protocol) : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName, string controllerName, object routeValues, string protocol)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName, routeValues, protocol) : "/Permission/UnPermissionMessage";
        }
        public static string ActionPermission(this UrlHelper Url, string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return PermissionParticle.UrlPermission(actionName, controllerName) ? Url.Action(actionName, controllerName, routeValues, protocol, hostName) : "/Permission/UnPermissionMessage";
        }

        #endregion


    }

}