using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.TenantData
{
    public class TenantDbContext : IdentityDbContext<ApplicationAdminUser>, IDbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }

        public DbSet<AppTenant> Tenants { get; set; }

        public IDbContextTransaction BeginTransaction()
        {
            return this.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return this.Database.BeginTransactionAsync(cancellationToken: cancellationToken);
        }

        public Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync();
        }
    }
}
