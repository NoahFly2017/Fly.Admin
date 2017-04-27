using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    /// <summary>
    ///     网点内部的组织架构。
    /// </summary>
    public class Organization
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     组织架构的友好名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        ///     父级组织架构，顶级为-1/0/NULL。
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        ///     所属平台，关联到Platform表的Id字段。
        /// </summary>
        public Guid PlatformId { get; set; }

        #region 导航属性
        /// <summary>
        ///     导航属性，父级的组织架构。
        /// </summary>
        public virtual Organization Parent { get; set; }
        /// <summary>
        ///     导航属性，所属平台网点。
        /// </summary>        
        public virtual Platform Platform { get; set; }
        /// <summary>
        ///     导航属性，所有子组织架构（嵌套孙组织不包含在内）。
        /// </summary>
        public virtual ICollection<Organization> Children { get; set; }
        /// <summary>
        ///     导航属性，该组织架构下的所有直系员工（不包含子组织架构下的员工）。
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; }        
        #endregion

    }
}
