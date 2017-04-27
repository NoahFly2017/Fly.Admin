using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Fly.Core.Models;
namespace Fly.Core.Models.Configuration
{
    public class LogConfiguration : EntityTypeConfiguration<Log>
    {
        public LogConfiguration()
        {
            HasKey(t => t.Id).Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Message).HasMaxLength(256);
            //HasRequired(t => t.Member).WithMany().HasForeignKey(t => t.MemberId).WillCascadeOnDelete(false);
        }
    }
}
