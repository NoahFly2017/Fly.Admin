using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).HasMaxLength(128).IsRequired();
            Property(t => t.Job).HasMaxLength(128);
            Property(t => t.ZipCode).HasMaxLength(32);
            Property(t => t.Address).HasMaxLength(256);
            Property(t => t.Email).HasMaxLength(128);
            Property(t => t.QQ).HasMaxLength(128);
            Property(t => t.Remark).HasMaxLength(512);

            HasRequired(t => t.Organization).WithMany(t => t.Employees).HasForeignKey(t => t.OrganizationId).WillCascadeOnDelete(false);
            HasRequired(t => t.Platform).WithMany(t => t.Employees).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
