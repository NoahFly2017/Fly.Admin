using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fly.Core.Identity.EntityFramework;
using Fly.Core.Models;
using Fly.Core.Models.Configuration;

namespace Fly.Core.DataAccess
{
    public class FlyDbContext : IdentityDbContext<Employee, Role, Guid, EmployeeLogin, EmployeeRole, EmployeeClaim>
    {
        public static FlyDbContext Create()
        {
            return new FlyDbContext();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new AnnouncementConfiguration());
            modelBuilder.Configurations.Add(new LogConfiguration());
            modelBuilder.Configurations.Add(new OrganizationConfiguration());;
            modelBuilder.Configurations.Add(new PermissionGroupConfiguration());
            modelBuilder.Configurations.Add(new PermissionLineConfiguration());
            modelBuilder.Configurations.Add(new PlatformConfiguration());
            modelBuilder.Configurations.Add(new IPAddressBlackWebConfiguration());
            modelBuilder.Configurations.Add(new IPAddressWhiteOAConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new RolePermissionConfiguration());

        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<PermissionLine> PermissionLines { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<IPAddressBlackWeb> IPAddressBlackWebs { get; set; }
        public DbSet<IPAddressWhiteOA> IPAddressWhiteOAs { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
