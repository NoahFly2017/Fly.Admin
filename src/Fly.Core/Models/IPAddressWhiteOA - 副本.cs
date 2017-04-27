using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    /// <summary>
    ///     OA 端访问白名单，默认情况下OA访问都是黑名单。
    /// </summary>
    public class IPAddressWhiteOA
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     IPV4 地址。
        /// </summary>
        public string IPv4 { get; set; }
        /// <summary>
        ///     IPV6 地址。
        /// </summary>
        public string IPv6 { get; set; }
        /// <summary>
        ///     备注。
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        ///     创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///     所属营业网点。
        /// </summary>
        public Guid PlatformId { get; set; }

        #region 导航属性
        /// <summary>
        ///     导航属性，所属网点。
        /// </summary>
        public virtual Platform Platform { get; set; }

        #endregion
    }
}
