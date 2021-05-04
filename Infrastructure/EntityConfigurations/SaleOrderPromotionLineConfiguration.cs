using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPromotionLineConfiguration : IEntityTypeConfiguration<SaleOrderPromotionLine>

    {
        public void Configure(EntityTypeBuilder<SaleOrderPromotionLine> builder)
        {

            builder.HasOne(x => x.Promotion)
              .WithMany(x => x.Lines)
              .HasForeignKey(x => x.PromotionId);

            builder.HasOne(x => x.SaleOrderLine)
              .WithMany()
              .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
