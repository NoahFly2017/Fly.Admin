using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Remark).HasMaxLength(512);
            Property(t => t.PlatformId).IsRequired();

            HasRequired(t => t.Platform).WithMany(t => t.Roles).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
