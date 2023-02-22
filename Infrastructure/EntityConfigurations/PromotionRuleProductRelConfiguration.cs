using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PromotionRuleProductRelConfiguration : IEntityTypeConfiguration<PromotionRuleProductRel>
    {
        public void Configure(EntityTypeBuilder<PromotionRuleProductRel> builder)
        {
            builder.HasKey(x => new { x.RuleId, x.ProductId });

            builder.HasOne(x => x.Rule)
              .WithMany(x => x.RuleProductRels)
              .HasForeignKey(x => x.RuleId);

            builder.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.DiscountLineProduct)
           .WithMany()
           .HasForeignKey(x => x.DiscountLineProductId);
        }
    }
}
