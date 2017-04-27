using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models
{
    public class Log
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     所属会员，关联理财/信贷会员表『TbMember』中的「Id」，外键。
        /// </summary>
        public Guid? MemberId { get; set; }
        /// <summary>
        ///     日志类型，可选值：0-Unknown(未知类型), 1-Login(登陆)，1-Member(个人信息变动)，2-Family(家庭成员变动)，3-Work(工作经历变动)等等。
        /// </summary>
        public LogType LogType { get; set; }
        /// <summary>
        ///     日志信息，如：家庭成员从XX变动到YY,新增工作经历等等。
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        ///     操作的Ip地址。
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        ///     操作时间。
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     导航属性，所属的关联理财/信贷会员。
        /// </summary>
        //public virtual Member Member { get; set; }
    }

    /// <summary>
    ///     日志类型。
    /// </summary>
    public enum LogType
    {
        /// <summary>
        ///     未知类型。
        /// </summary>
        Unknown,
        /// <summary>
        ///     登陆。
        /// </summary>
        Login,
        /// <summary>
        ///     个人基础信息变动。
        /// </summary>
        Member,
        /// <summary>
        ///     个人家庭信息变动。
        /// </summary>
        Family,
        /// <summary>
        ///     工作信息变动。
        /// </summary>
        Work,
        /// <summary>
        ///     订单日志。
        /// </summary>
        Order,
        /// <summary>
        ///     会员资金账户变动。
        /// </summary>
        Account,
        /// <summary>
        ///     征信报告-贷记卡信息变动
        /// </summary>
        CreditReferenceCreditCard,
        /// <summary>
        ///     征信报告-准贷记卡信息变动
        /// </summary>
        CreditReferenceSemiCreditCard,
        /// <summary>
        ///     征信报告-贷款信息变动
        /// </summary>
        CreditReferenceLoan,
        /// <summary>
        ///     异常访问
        /// </summary>
        Abnormal
    }
}
