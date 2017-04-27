using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    /// <summary>
    ///     系统公告表
    /// </summary>
    public class Announcement
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     文章标题。
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        ///     文章内容。
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        ///     是否发布，默认否。
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        ///     发布时间。
        /// </summary>
        public DateTime? PublishedTime { get; set; }
        /// <summary>
        ///     创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///     创建人。
        /// </summary>
        public Guid EmployeeId { get; set; }
        /// <summary>
        ///     所属平台。
        /// </summary>
        public Guid PlatformId { get; set; }

        #region 导航属性
        /// <summary>
        ///     导航属性，创建人。
        /// </summary>
        public virtual Employee Employee { get; set; }
        /// <summary>
        ///     导航属性，所属平台。
        /// </summary>
        public virtual Platform Platform { get; set; }
        #endregion 
    }
}
