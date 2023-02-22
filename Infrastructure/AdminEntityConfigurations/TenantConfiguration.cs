using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AdminEntityConfigurations
{
    public class AppTenantConfiguration : IEntityTypeConfiguration<AppTenant>
    {
        public void Configure(EntityTypeBuilder<AppTenant> builder)
        {
            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.EmployeeAdmin)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);
        }
    }
}
