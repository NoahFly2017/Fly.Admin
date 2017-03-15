using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class PermissionGroupConfiguration : EntityTypeConfiguration<PermissionGroup>
    {
        public PermissionGroupConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.DisplayName).HasMaxLength(128).IsRequired();
            Property(t => t.Url).HasMaxLength(256);
            Property(t => t.Tag).HasMaxLength(128);
            Property(t => t.Headshot).HasMaxLength(256);
            HasOptional(t => t.Parent).WithMany(t => t.Children).HasForeignKey(t => t.ParentId);
            HasRequired(t => t.Platform).WithMany(t => t.PermissionGroups).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
