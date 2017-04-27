using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Fly.Core.Identity.EntityFramework;

namespace Fly.Core.Models
{
    public class Employee : IdentityUser<Guid, EmployeeLogin, EmployeeRole, EmployeeClaim>
    {
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
        ///     邮编。
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        ///     住址。
        /// </summary>
        public string Address { get; set; }
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
        ///     所属的内部组织架构，关联到Organization表Id字段。
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        ///     所属平台，关联到Platform表Id字段。
        /// </summary>
        public Guid PlatformId { get; set; }

        #region 导航属性
        /// <summary>
        ///     导航属性,所属的内部组织架构。
        /// </summary>
        public virtual Organization Organization { get; set; }

        /// <summary>
        ///     导航属性,所属平台网点。
        /// </summary>
        public virtual Platform Platform { get; set; }
        #endregion

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Employee,Guid> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    /// <summary>
    ///     性别。
    /// </summary>
    public enum Sex
    {
        /// <summary>
        ///     未知性别/保密。
        /// </summary>
        Unknown,
        /// <summary>
        ///     男。
        /// </summary>
        Male,
        /// <summary>
        ///     女。
        /// </summary>
        Female
    }

    /// <summary>
    ///     员工状态。
    /// </summary>
    public enum EmployeeStatus
    {
        /// <summary>
        ///     未赋予任何状态，不允许登陆系统。
        /// </summary>
        None,
        /// <summary>
        ///     已离职，不允许登陆系统。
        /// </summary>
        Leave,
        /// <summary>
        ///     在职，允许登陆系统。
        /// </summary>
        Normal
    }
}
