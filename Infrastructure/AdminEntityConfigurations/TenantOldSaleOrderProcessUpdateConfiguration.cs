using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AdminEntityConfigurations
{
    public class TenantOldSaleOrderProcessUpdateConfiguration : IEntityTypeConfiguration<TenantOldSaleOrderProcessUpdate>
    {
        public void Configure(EntityTypeBuilder<TenantOldSaleOrderProcessUpdate> builder)
        {
            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}

