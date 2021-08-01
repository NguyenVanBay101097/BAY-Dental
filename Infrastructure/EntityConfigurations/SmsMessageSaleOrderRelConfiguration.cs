using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessageSaleOrderRelConfiguration : IEntityTypeConfiguration<SmsMessageSaleOrderRel>
    {
        public void Configure(EntityTypeBuilder<SmsMessageSaleOrderRel> builder)
        {
            builder.HasKey(x => new { x.SaleOrderId, x.SmsMessageId });

            builder.HasOne(x => x.SmsMessage)
               .WithMany(x => x.SmsMessageSaleOrderRels)
               .HasForeignKey(x => x.SmsMessageId);

            builder.HasOne(x => x.SaleOrder)
                .WithMany()
                .HasForeignKey(x => x.SaleOrderId);
        }
    }
}
