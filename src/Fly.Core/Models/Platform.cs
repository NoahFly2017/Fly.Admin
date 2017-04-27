using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    /// <summary>
    ///     营业网点。
    /// </summary>
    public class Platform
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     网点名称。
        /// </summary>
        public string PlatformName { get; set; }
        /// <summary>
        ///     网点子域名，唯一。
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        ///     法人/联系人。
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        ///     联系电话。
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        ///     地址。
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        ///     邮编。
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        ///     邮箱。
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     平台描述。
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     添加时间。
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 原最新资金记录
        /// </summary>
        public int oldId { get; set; }
   
        

        #region 导航属性
        /// <summary>
        ///     导航属性，该平台网点下所有的组织架构。
        /// </summary>
        public virtual ICollection<Organization> Organizations { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下所有的员工。
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下所有的理财/信贷会员。
        /// </summary>
       // public virtual ICollection<Member> Members { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下所有的角色。
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下所有的权限组/功能菜单。
        /// </summary>
        public virtual ICollection<PermissionGroup> PermissionGroups { get; set; }

        /// <summary>
        ///     导航属性，该平台网点下所有的必收文件类型。
        /// </summary>
        //public virtual ICollection<FileType> FileTypes { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下所有的预定义表单。
        /// </summary>
        //public virtual ICollection<Form> Forms { get; set; }

        /// <summary>
        ///     导航属性，该平台网点下所有的订单。
        /// </summary>
       // public virtual ICollection<Order> Orders { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下的所有仓库。
        /// </summary>
       // public virtual ICollection<Repository> Repositories { get; set; }
 
     
        /// <summary>
        ///     导航属性，该平台网点下的所有公告。
        /// </summary>
        public virtual ICollection<Announcement> Announcements { get; set; }
       
        /// <summary>
        ///     导航属性，该平台网点下的所有支付方式。
        /// </summary>
       // public virtual ICollection<PaymentType> PaymentTypes { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下的所有Web访问IP黑名单。
        /// </summary>
        public virtual ICollection<IPAddressBlackWeb> IPAddressBlackWebs { get; set; }
        /// <summary>
        ///     导航属性，该平台网点下的所有OA端访问IP白名单。
        /// </summary>
        public virtual ICollection<IPAddressWhiteOA> IPAddressWhiteOAs { get; set; }
        /// <summary>
        ///     导航属性,该平台网点下的所有积分等级列表。
        /// </summary>
        //public virtual ICollection<ScoreLevel> ScoreLevels { get; set; }
        #endregion
    }
}
