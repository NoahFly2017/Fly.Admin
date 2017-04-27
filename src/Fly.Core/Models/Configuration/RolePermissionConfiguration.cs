using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class RolePermissionConfiguration : EntityTypeConfiguration<RolePermission>
    {
        public RolePermissionConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(t => t.Role).WithMany(t => t.RolePermissions).HasForeignKey(t => t.RoleId);
            HasRequired(t => t.PermissionLine).WithMany().HasForeignKey(t => t.PermissionLineId);
        }
    }
}
