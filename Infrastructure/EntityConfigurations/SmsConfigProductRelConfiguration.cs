using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsConfigProductRelConfiguration : IEntityTypeConfiguration<SmsConfigProductRel>
    {
        public void Configure(EntityTypeBuilder<SmsConfigProductRel> builder)
        {
            builder.HasKey(x => new { x.SmsConfigId, x.ProductId });

            builder.HasOne(x => x.SmsConfig)
                .WithMany(x => x.SmsConfigProductRels)
                .HasForeignKey(x => x.SmsConfigId);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);
        }
    }
}
