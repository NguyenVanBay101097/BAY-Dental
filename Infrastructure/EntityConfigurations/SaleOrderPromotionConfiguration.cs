using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPromotionConfiguration : IEntityTypeConfiguration<SaleOrderPromotion>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPromotion> builder)
        {
            builder.HasOne(x => x.SaleCouponProgram)
            .WithMany(x => x.Promotions)
            .HasForeignKey(x => x.SaleCouponProgramId);

            builder.HasOne(x => x.SaleOrder)
            .WithMany(x => x.Promotions)
            .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.SaleOrderLine)
              .WithMany(x => x.Promotions)
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
