using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity.EntityFramework
{
    public class IdentityUserRole<TKey>
    {
        public virtual TKey RoleId { get; set; }
        public virtual TKey EmployeeId { get; set; }
    }

    public class IdentityUserRole : IdentityUserRole<string>
    {
    }
}
