using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity.EntityFramework
{
    public class IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : DbContext
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
    {

        public IdentityDbContext()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            EntityTypeConfiguration<TUser> configuration = modelBuilder.Entity<TUser>().ToTable("Employees");
            configuration.HasMany<TUserRole>(u => u.Roles).WithRequired().HasForeignKey<TKey>(ur => ur.EmployeeId);
            configuration.HasMany<TUserClaim>(u => u.Claims).WithRequired().HasForeignKey<TKey>(uc => uc.EmployeeId);
            configuration.HasMany<TUserLogin>(u => u.Logins).WithRequired().HasForeignKey<TKey>(ul => ul.EmployeeId);
            IndexAttribute indexAttribute = new IndexAttribute("UserNameIndex")
            {
                IsUnique = true
            };
            configuration.Property((Expression<Func<TUser, string>>)(u => u.UserName)).IsRequired().HasMaxLength(0x100).HasColumnAnnotation("Index", new IndexAnnotation(indexAttribute));
            configuration.Property((Expression<Func<TUser, string>>)(u => u.Email)).HasMaxLength(0x100);
            modelBuilder.Entity<TUserRole>().HasKey(r => new { EmployeeId = r.EmployeeId, RoleId = r.RoleId }).ToTable("EmployeeRoles");
            modelBuilder.Entity<TUserLogin>().HasKey(l => new { LoginProvider = l.LoginProvider, ProviderKey = l.ProviderKey, EmployeeId = l.EmployeeId }).ToTable("EmployeeLogins");
            modelBuilder.Entity<TUserClaim>().HasKey(c => new { ClaimType = c.ClaimType, ClaimValue = c.ClaimValue, EmployeeId = c.EmployeeId }).ToTable("EmployeeClaims");
            EntityTypeConfiguration<TRole> configuration2 = modelBuilder.Entity<TRole>().ToTable("Roles");
            IndexAttribute attribute2 = new IndexAttribute("RoleNameIndex")
            {
                IsUnique = true
            };
            configuration2.Property((Expression<Func<TRole, string>>)(r => r.Name)).IsRequired().HasMaxLength(0x100).HasColumnAnnotation("Index", new IndexAnnotation(attribute2));
            configuration2.HasMany<TUserRole>(r => r.Users).WithRequired().HasForeignKey<TKey>(ur => ur.RoleId);
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if ((entityEntry != null) && (entityEntry.State == EntityState.Added))
            {
                List<DbValidationError> source = new List<DbValidationError>();
                TUser user = entityEntry.Entity as TUser;
                if (user != null)
                {
                    if (this.Employees.Any<TUser>(u => string.Equals(u.UserName, user.UserName)))
                    {
                        source.Add(new DbValidationError("User", string.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateUserName, new object[] { user.UserName })));
                    }
                    if (this.RequireUniqueEmail && this.Employees.Any<TUser>(u => string.Equals(u.Email, user.Email)))
                    {
                        source.Add(new DbValidationError("User", string.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateEmail, new object[] { user.Email })));
                    }
                }
                else
                {
                    TRole role = entityEntry.Entity as TRole;
                    if ((role != null) && this.Roles.Any<TRole>(r => string.Equals(r.Name, role.Name)))
                    {
                        source.Add(new DbValidationError("Role", string.Format(CultureInfo.CurrentCulture, IdentityResources.RoleAlreadyExists, new object[] { role.Name })));
                    }
                }
                if (source.Any<DbValidationError>())
                {
                    return new DbEntityValidationResult(entityEntry, source);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }


        public bool RequireUniqueEmail { get; set; }
        public virtual IDbSet<TRole> Roles { get; set; }
        public virtual IDbSet<TUser> Employees { get; set; }
    }

    public class IdentityDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> where TUser : IdentityUser
    {
        public IdentityDbContext()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContext(string nameOrConnectionString)
            : this(nameOrConnectionString, true)
        {
        }

        public IdentityDbContext(string nameOrConnectionString, bool throwIfV1Schema)
            : base(nameOrConnectionString)
        {
            if (throwIfV1Schema && IdentityDbContext<TUser>.IsIdentityV1Schema(this))
            {
                throw new InvalidOperationException(IdentityResources.IdentityV1SchemaError);
            }
        }

        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        internal static bool IsIdentityV1Schema(DbContext db)
        {
            SqlConnection connection = db.Database.Connection as SqlConnection;
            if ((connection != null) && db.Database.Exists())
            {
                using (SqlConnection connection2 = new SqlConnection(connection.ConnectionString))
                {
                    connection2.Open();
                    return (((IdentityDbContext<TUser>.VerifyColumns(connection2, "Employees", new string[] { "Id", "UserName", "PasswordHash", "SecurityStamp", "Discriminator" }) && IdentityDbContext<TUser>.VerifyColumns(connection2, "Roles", new string[] { "Id", "Name" })) && (IdentityDbContext<TUser>.VerifyColumns(connection2, "EmployeeRoles", new string[] { "EmployeeId", "RoleId" }) && IdentityDbContext<TUser>.VerifyColumns(connection2, "EmployeeClaims", new string[] { "Id", "ClaimType", "ClaimValue", "EmployeeId" }))) && IdentityDbContext<TUser>.VerifyColumns(connection2, "EmployeeLogins", new string[] { "EmployeeId", "ProviderKey", "LoginProvider" }));
                }
            }
            return false;
        }

        internal static bool VerifyColumns(SqlConnection conn, string table, params string[] columns)
        {
            List<string> list = new List<string>();
            using (SqlCommand command = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@Table", conn))
            {
                command.Parameters.Add(new SqlParameter("Table", table));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                }
            }
            return columns.All<string>(new Func<string, bool>(list.Contains));
        }
    }

    public class IdentityDbContext : IdentityDbContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IdentityDbContext()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }
    }


}
