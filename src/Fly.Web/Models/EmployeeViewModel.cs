using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fly.Core.Models;

namespace Fly.Web.Models
{
    public class EmployeeView
    {
        /// <summary>
        ///     网点员工。
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///     登录名。
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        ///     真实姓名。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     性别。
        /// </summary>
        public Sex Sex { get; set; }
        /// <summary>
        ///     职务。
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        ///     联系电话。
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        ///     邮编。
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        ///     住址。
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        ///     邮箱。
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     QQ。
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        ///     员工自定义属性-值： 以 属性`值|属性`值...的方式存储。
        /// </summary>
        public string CustomAttribute { get; set; }
        /// <summary>
        ///     员工状态。
        /// </summary>
        public EmployeeStatus Status { get; set; }
        /// <summary>
        ///     备注。更改状态时，对该员工状态的一种补充说明。
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     角色。
        /// </summary>
        public int? RoleId { get; set; }
        /// <summary>
        ///     所属的内部组织架构，关联到Organization表Id字段。
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        ///     所属平台，关联到Platform表Id字段。
        /// </summary>
        public Guid PlatformId { get; set; }

    }
    public class EmployeeOrOrganizationView
    {
    
    

        /// <summary>
        ///     网点员或机构Id，机构为负数。
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        ///     登录名。
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        ///     真实姓名或机构名。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     性别。
        /// </summary>
        public Sex Sex { get; set; }
        /// <summary>
        ///     职务。
        /// </summary>

     
        public string Job { get; set; }
        /// <summary>
        ///     联系电话。
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        ///     邮编。
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        ///     住址。
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        ///     邮箱。
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     QQ。
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        ///     员工自定义属性-值： 以 属性`值|属性`值...的方式存储。
        /// </summary>
        public string CustomAttribute { get; set; }
        /// <summary>
        ///     员工状态。
        /// </summary>
        public EmployeeStatus Status { get; set; }
        /// <summary>
        ///     备注。更改状态时，对该员工状态的一种补充说明。
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     角色。
        /// </summary>



        public List<Guid> RoleIds { get; set; }
        /// <summary>
        ///     所属的内部组织架构，关联到Organization表Id字段。
        /// </summary>

        public object OrganizationId { get; set; }

        public string OrganizationName { get; set; }
        /// <summary>
        ///     所属平台，关联到Platform表Id字段。
        /// </summary>
        public Guid PlatformId { get; set; }

        public object _parentId { get; set; }

    }
}