using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly.Core.Models.Configuration
{
    public class AnnouncementConfiguration : EntityTypeConfiguration<Announcement>
    {
        public AnnouncementConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Title).HasMaxLength(128).IsRequired();
            Property(t => t.Content).IsRequired();

            HasRequired(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).WillCascadeOnDelete(false);
            HasRequired(t => t.Platform).WithMany(t => t.Announcements).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
