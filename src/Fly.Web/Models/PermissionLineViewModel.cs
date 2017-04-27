using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Web.Models
{
    public class PermissionLineViewModel
    {
        /// <summary>
        /// 权限组或权限项/按钮ID，权限组为负数
        /// </summary>
        public object Id { get; set; }


        /// <summary>
        ///     所属权限组/菜单功能，关联到PermissionGroup表Id字段
        /// </summary>
        public object GroupId { get; set; }

        /// <summary>
        ///     权限项/按钮或权限组功能的友好名称。
        /// </summary>

        public string DisplayName { get; set; }
        /// <summary>
        ///     权限项/按钮功能的服务方法/路径。
        /// </summary> 
        public string Url { get; set; }
        /// <summary>
        ///     权限项/按钮功能在HTML中的标识。
        /// </summary>
        public string Tag { get; set; }

        public object _parentId { get; set; }



    }


    public class PermissionLineTreeViewModel
    {
        /// <summary>
        /// Easyui树形id
        /// </summary>
        public object id { get; set; }

        /// <summary>
        /// easyui树形名称
        /// </summary>
        public string text { get; set; }

        private List<PermissionLineTreeViewModel> _children=new List<PermissionLineTreeViewModel>();
        /// <summary>
        /// easyui树形节点的子节点
        /// </summary>
        public List<PermissionLineTreeViewModel> children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}