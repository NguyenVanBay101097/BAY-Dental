using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponConfiguration : IEntityTypeConfiguration<SaleCoupon>
    {
        public void Configure(EntityTypeBuilder<SaleCoupon> builder)
        {
            builder.Property(x => x.Code).IsRequired();

            builder.HasOne(x => x.SaleOrder)
               .WithMany(x => x.AppliedCoupons)
               .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Order)
              .WithMany(x => x.GeneratedCoupons)
              .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.Program)
             .WithMany(x => x.Coupons)
             .HasForeignKey(x => x.ProgramId)
             .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Partner)
             .WithMany()
             .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
