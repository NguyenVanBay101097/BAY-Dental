using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessageSaleOrderLineRelConfiguration : IEntityTypeConfiguration<SmsMessageSaleOrderLineRel>
    {
        public void Configure(EntityTypeBuilder<SmsMessageSaleOrderLineRel> builder)
        {
            builder.HasKey(x => new { x.SaleOrderLineId, x.SmsMessageId });

            builder.HasOne(x => x.SmsMessage)
               .WithMany(x => x.SmsMessageSaleOrderLineRels)
               .HasForeignKey(x => x.SmsMessageId);

            builder.HasOne(x => x.SaleOrderLine)
                .WithMany()
                .HasForeignKey(x => x.SaleOrderLineId);
        }
    }
}
