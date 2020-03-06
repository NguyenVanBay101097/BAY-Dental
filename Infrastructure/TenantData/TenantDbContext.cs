using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.AdminEntityConfigurations;
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
        public DbSet<TenantFacebookPage> TenantFacebookPages { get; set; }

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

        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlRawAsync(sql, parameters);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AppTenantConfiguration());
            builder.ApplyConfiguration(new TenantFacebookPageConfiguration());

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
