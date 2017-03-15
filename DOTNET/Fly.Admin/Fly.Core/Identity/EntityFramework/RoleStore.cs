using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Core.Identity.EntityFramework
{
    public class RoleStore<TRole, TKey, TUserRole> : IQueryableRoleStore<TRole, TKey>, IRoleStore<TRole, TKey>, IDisposable
        where TRole : IdentityRole<TKey, TUserRole>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
    {
        private bool _disposed;
        private EntityStore<TRole> _roleStore;
        
        public DbContext Context
        {
            get;
            private set;
        }
        
        public bool DisposeContext
        {
            get;
            set;
        }
        
        public IQueryable<TRole> Roles
        {
            get
            {
                return this._roleStore.EntitySet;
            }
        }
        
        public RoleStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.Context = context;
            this._roleStore = new EntityStore<TRole>(context);
        }
        
        public Task<TRole> FindByIdAsync(TKey roleId)
        {
            this.ThrowIfDisposed();
            return this._roleStore.GetByIdAsync(roleId);
        }
        
        public Task<TRole> FindByNameAsync(string roleName)
        {
            this.ThrowIfDisposed();
            return QueryableExtensions.FirstOrDefaultAsync<TRole>(this._roleStore.EntitySet, (TRole u) => u.Name.ToUpper() == roleName.ToUpper());
        }
        /// <summary>
        ///     Insert an entity
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task CreateAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            this._roleStore.Create(role);
            await this.Context.SaveChangesAsync().WithCurrentCulture<int>();
        }
        
        public virtual async Task DeleteAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            this._roleStore.Delete(role);
            await this.Context.SaveChangesAsync().WithCurrentCulture<int>();
        }
        
        public virtual async Task UpdateAsync(TRole role)
        {
            this.ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            this._roleStore.Update(role);
            await this.Context.SaveChangesAsync().WithCurrentCulture<int>();
        }
      
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.DisposeContext && disposing && this.Context != null)
            {
                this.Context.Dispose();
            }
            this._disposed = true;
            this.Context = null;
            this._roleStore = null;
        }
    }

    public class RoleStore<TRole> : RoleStore<TRole, string, IdentityUserRole>, IQueryableRoleStore<TRole>, IQueryableRoleStore<TRole, string>, IRoleStore<TRole, string>, IDisposable where TRole : IdentityRole, new()
    {
        public RoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public RoleStore(DbContext context)
            : base(context)
        {
        }
    }

 

}
