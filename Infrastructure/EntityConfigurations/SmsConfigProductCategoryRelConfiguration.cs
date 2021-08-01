using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsConfigProductCategoryRelConfiguration : IEntityTypeConfiguration<SmsConfigProductCategoryRel>
    {
        public void Configure(EntityTypeBuilder<SmsConfigProductCategoryRel> builder)
        {
            builder.HasKey(x => new { x.SmsConfigId, x.ProductCategoryId });

            builder.HasOne(x => x.SmsConfig)
                .WithMany(x => x.SmsConfigProductCategoryRels)
                .HasForeignKey(x => x.SmsConfigId);

            builder.HasOne(x => x.ProductCategory)
                .WithMany()
                .HasForeignKey(x => x.ProductCategoryId);
        }
    }
}
