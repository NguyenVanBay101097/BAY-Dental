using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineConfiguration : IEntityTypeConfiguration<SaleOrderLine>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLine> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ProductUOM)
              .WithMany()
              .HasForeignKey(x => x.ProductUOMId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Salesman)
                .WithMany()
                .HasForeignKey(x => x.SalesmanId);

            builder.HasOne(x => x.OrderPartner)
              .WithMany()
              .HasForeignKey(x => x.OrderPartnerId);

            builder.HasOne(x => x.PromotionProgram)
         .WithMany(x => x.SaleLines)
         .HasForeignKey(x => x.PromotionProgramId);

            builder.HasOne(x => x.Promotion)
              .WithMany(x => x.SaleLines)
              .HasForeignKey(x => x.PromotionId);

            builder.HasOne(x => x.Coupon)
                .WithMany()
                .HasForeignKey(x => x.CouponId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
