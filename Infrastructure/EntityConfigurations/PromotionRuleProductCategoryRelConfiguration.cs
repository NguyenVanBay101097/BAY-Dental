using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PromotionRuleProductCategoryRelConfiguration : IEntityTypeConfiguration<PromotionRuleProductCategoryRel>
    {
        public void Configure(EntityTypeBuilder<PromotionRuleProductCategoryRel> builder)
        {
            builder.HasKey(x => new { x.RuleId, x.CategId });

            builder.HasOne(x => x.Rule)
              .WithMany(x => x.RuleCategoryRels)
              .HasForeignKey(x => x.RuleId);

            builder.HasOne(x => x.Categ)
             .WithMany()
             .HasForeignKey(x => x.CategId);

            builder.HasOne(x => x.DiscountLineProduct)
           .WithMany()
           .HasForeignKey(x => x.DiscountLineProductId);
        }
    }
}
