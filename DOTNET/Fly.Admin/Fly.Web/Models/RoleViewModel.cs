using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Web.Models
{
    public class RoleComboItemViewModel
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class RolePermissionViewModel
    {
        public Guid PermissionLineId { get; set; }
    }
}