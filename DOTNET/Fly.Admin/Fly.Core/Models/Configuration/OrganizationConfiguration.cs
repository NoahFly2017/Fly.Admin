using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class OrganizationConfiguration : EntityTypeConfiguration<Organization>
    {
        public OrganizationConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.DisplayName).HasMaxLength(128).IsRequired();

            HasOptional(t => t.Parent).WithMany(t => t.Children).HasForeignKey(t => t.ParentId);
            HasRequired(t => t.Platform).WithMany(t => t.Organizations).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
