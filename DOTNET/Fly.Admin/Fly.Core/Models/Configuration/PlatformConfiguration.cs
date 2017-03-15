using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Models.Configuration
{
    public class PlatformConfiguration : EntityTypeConfiguration<Platform>
    {
        public PlatformConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.PlatformName).HasMaxLength(128).IsRequired();
            Property(t => t.Domain).HasMaxLength(256);
            Property(t => t.Contact).HasMaxLength(128);
            Property(t => t.PhoneNumber).HasMaxLength(128);
            Property(t => t.Address).HasMaxLength(256);
            Property(t => t.ZipCode).HasMaxLength(32);
            Property(t => t.Email).HasMaxLength(128);
            Property(t => t.Remark).HasMaxLength(512);
        }
    }
}
