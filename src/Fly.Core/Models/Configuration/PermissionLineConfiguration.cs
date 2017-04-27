using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class PermissionLineConfiguration : EntityTypeConfiguration<PermissionLine>
    {
        public PermissionLineConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.DisplayName).HasMaxLength(128).IsRequired();
            Property(t => t.Url).HasMaxLength(256);
            Property(t => t.Tag).HasMaxLength(128);
            HasRequired(t => t.Group).WithMany(t => t.PermissionLines).HasForeignKey(t => t.GroupId).WillCascadeOnDelete(false);
        }
    }
}
