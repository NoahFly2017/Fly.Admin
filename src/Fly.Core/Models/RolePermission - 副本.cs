using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fly.Core.Models
{
    /// <summary>
    ///     角色权限表。
    /// </summary>
    public class RolePermission
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     所属角色，关联到Role表的Id字段。
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        ///     权限项ID，关联到PermissionLine表的Id字段。
        /// </summary>
        public Guid PermissionLineId { get; set; }

        /// <summary>
        ///     导航属性，所属角色。
        /// </summary>
        public virtual Role Role { get; set; }
        /// <summary>
        ///     导航属性，包含的权限项/按钮功能。
        /// </summary>
        public virtual PermissionLine PermissionLine { get; set; }
    }
}
