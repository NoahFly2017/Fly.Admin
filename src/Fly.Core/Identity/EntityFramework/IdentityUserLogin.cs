using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity.EntityFramework
{
    public class IdentityUserLogin<TKey>
    {
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual TKey EmployeeId { get; set; }
    }

    public class IdentityUserLogin : IdentityUserLogin<string>
    {
    }
}
