using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLinePaymentRelConfiguration : IEntityTypeConfiguration<SaleOrderLinePaymentRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLinePaymentRel> builder)
        {
           // builder.HasKey(x => new { x.PaymentId, x.SaleOrderLineId });

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.SaleOrderLinePaymentRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.SaleOrderLine)
                .WithMany(x => x.SaleOrderLinePaymentRels)
                .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.CreatedBy)
           .WithMany()
           .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
