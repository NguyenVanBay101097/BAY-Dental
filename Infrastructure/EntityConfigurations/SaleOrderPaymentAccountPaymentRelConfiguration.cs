using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPaymentAccountPaymentRelConfiguration : IEntityTypeConfiguration<SaleOrderPaymentAccountPaymentRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPaymentAccountPaymentRel> builder)
        {
            builder.HasKey(x => new { x.SaleOrderPaymentId, x.PaymentId });
            builder.HasOne(x => x.SaleOrderPayment)
             .WithMany(x => x.PaymentRels)
             .HasForeignKey(x => x.SaleOrderPaymentId);

            builder.HasOne(x => x.Payment)
              .WithMany()
              .HasForeignKey(x => x.PaymentId);
        }
    }
}
