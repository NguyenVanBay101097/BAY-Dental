using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CommissionSettlementConfiguration : IEntityTypeConfiguration<CommissionSettlement>
    {

        public void Configure(EntityTypeBuilder<CommissionSettlement> builder)
        {
            builder.HasOne(x => x.Partner)
            .WithMany()
            .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.SaleOrderLine)
            .WithMany()
            .HasForeignKey(x => x.SaleOrderLineId);

          

            builder.HasOne(x => x.Payment)
           .WithMany()
           .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
            .WithMany()
            .HasForeignKey(x => x.WriteById);
        }
    }
}
