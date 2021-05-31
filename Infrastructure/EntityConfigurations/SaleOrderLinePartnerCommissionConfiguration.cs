using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLinePartnerCommissionConfiguration : IEntityTypeConfiguration<SaleOrderLinePartnerCommission>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLinePartnerCommission> builder)
        {
            builder.HasOne(x => x.Employee)
              .WithMany()
              .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.SaleOrderLine)
              .WithMany(x => x.PartnerCommissions)
              .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.Commission)
              .WithMany()
              .HasForeignKey(x => x.CommissionId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
              .WithMany()
              .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
              .WithMany()
              .HasForeignKey(x => x.WriteById);
        }
    }
}
