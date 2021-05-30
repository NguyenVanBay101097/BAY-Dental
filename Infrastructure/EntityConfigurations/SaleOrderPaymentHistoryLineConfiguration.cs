using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPaymentHistoryLineConfiguration : IEntityTypeConfiguration<SaleOrderPaymentHistoryLine>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPaymentHistoryLine> builder)
        {

            builder.HasOne(x => x.SaleOrderLine)
                   .WithMany(x => x.PaymentHistoryLines)
                   .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.SaleOrderPayment)
                    .WithMany(x => x.Lines)
                    .HasForeignKey(x => x.SaleOrderPaymentId)
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
