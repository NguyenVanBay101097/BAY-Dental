using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class QuotationPromotionConfiguration : IEntityTypeConfiguration<QuotationPromotion>
    {
        public void Configure(EntityTypeBuilder<QuotationPromotion> builder)
        {
            builder.HasOne(x => x.SaleCouponProgram)
            .WithMany()
            .HasForeignKey(x => x.SaleCouponProgramId);

            builder.HasOne(x => x.Quotation)
            .WithMany(x => x.Promotions)
            .HasForeignKey(x => x.QuotationId);

            builder.HasOne(x => x.QuotationLine)
              .WithMany(x => x.Promotions)
              .HasForeignKey(x => x.QuotationLineId);


            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
