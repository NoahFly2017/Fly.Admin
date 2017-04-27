using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly.Core.Models.Configuration
{
    public class IPAddressBlackWebConfiguration : EntityTypeConfiguration<IPAddressBlackWeb>
    {
        public IPAddressBlackWebConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.IPv4).HasMaxLength(128);
            Property(t => t.IPv6).HasMaxLength(128);
            Property(t => t.Remark).HasMaxLength(512);

            HasRequired(t => t.Platform).WithMany(t => t.IPAddressBlackWebs).HasForeignKey(t => t.PlatformId).WillCascadeOnDelete(false);
        }
    }
}
