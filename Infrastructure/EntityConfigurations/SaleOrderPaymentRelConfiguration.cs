using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPaymentRelConfiguration : IEntityTypeConfiguration<SaleOrderPaymentRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPaymentRel> builder)
        {
            builder.HasKey(x => new { x.PaymentId, x.SaleOrderId });

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.SaleOrderPaymentRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.SaleOrder)
                .WithMany(x => x.SaleOrderPaymentRels)
                .HasForeignKey(x => x.SaleOrderId);
        }
    }
}
