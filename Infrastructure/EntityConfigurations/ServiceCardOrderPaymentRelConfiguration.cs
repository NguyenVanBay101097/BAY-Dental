using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ServiceCardOrderPaymentRelConfiguration : IEntityTypeConfiguration<ServiceCardOrderPaymentRel>
    {
        public void Configure(EntityTypeBuilder<ServiceCardOrderPaymentRel> builder)
        {
            builder.HasKey(x => new { x.PaymentId, x.CardOrderId });

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.CardOrderPaymentRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.CardOrder)
                .WithMany()
                .HasForeignKey(x => x.CardOrderId);
        }
    }
}
