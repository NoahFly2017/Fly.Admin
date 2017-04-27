using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.Core.Identity.EntityFramework;

namespace Fly.Core.Models
{
    public class Role : IdentityRole<Guid, EmployeeRole>
    {
        /// <summary>
        ///     角色自定义属性，以 属性名|属性名...的方式保存。
        /// </summary>
        public string CustomAttribute { get; set; }
        /// <summary>
        ///     备注。
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     所属网点，关联到Platform表的Id字段。
        /// </summary>
        public Guid PlatformId { get; set; }

        #region 导航属性。
        /// <summary>
        ///     导航属性，所属网点。
        /// </summary>
        public virtual Platform Platform { get; set; }
        /// <summary>
        ///     导航属性，该角色下所有的权限。
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        #endregion
    }
}
