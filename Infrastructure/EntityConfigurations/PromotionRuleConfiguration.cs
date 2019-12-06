using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PromotionRuleConfiguration : IEntityTypeConfiguration<PromotionRule>
    {
        public void Configure(EntityTypeBuilder<PromotionRule> builder)
        {
            builder.HasOne(x => x.Program)
               .WithMany(x => x.Rules)
               .HasForeignKey(x => x.ProgramId);

            builder.HasOne(x => x.DiscountLineProduct)
                .WithMany()
                .HasForeignKey(x => x.DiscountLineProductId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
