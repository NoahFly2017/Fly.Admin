using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Web.Models
{
    public class PermissionGroupViewModel
    {
        public object Id { get; set; }

        /// <summary>
        ///     权限组/功能菜单的友好名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        ///     父级权限组Id，顶级为-1。
        /// </summary>
        public object ParentId { get; set; }

        /// <summary>
        ///     权限组/功能菜单的页面路径。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///     功能菜单的HTML标识。
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// EasyUI用于构建树形的父节点标记，属性名固定且区分大小写，勿改动
        /// </summary>
        public object _parentId { get; set; }

        public string Headshot { get; set; }
        public int SN { get; set; }

    }
}