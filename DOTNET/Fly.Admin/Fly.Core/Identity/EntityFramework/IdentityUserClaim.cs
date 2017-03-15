using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity.EntityFramework
{
    public class IdentityUserClaim<TKey>
    {
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
        //public virtual Guid Id { get; set; }
        public virtual TKey EmployeeId { get; set; }
    }
    public class IdentityUserClaim : IdentityUserClaim<string>
    {
    }
}
