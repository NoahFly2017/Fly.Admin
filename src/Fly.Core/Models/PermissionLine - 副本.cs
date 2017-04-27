using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    /// <summary>
    ///     权限项/按钮功能。
    /// </summary>
    public class PermissionLine
    {
        public Guid Id { get; set; }

        /// <summary>
        ///     所属权限组/菜单功能，关联到PermissionGroup表Id字段
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        ///     权限项/按钮功能的友好名称。
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

        /// <summary>
        ///     导航属性，所属权限组/菜单功能。
        /// </summary>
        public virtual PermissionGroup Group { get; set; }
    } 
}
