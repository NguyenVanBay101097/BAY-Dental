using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramProductCategoryRelConfiguration : IEntityTypeConfiguration<SaleCouponProgramProductCategoryRel>
    {
        public void Configure(EntityTypeBuilder<SaleCouponProgramProductCategoryRel> builder)
        {
            builder.HasKey(x => new { x.ProgramId, x.ProductCategoryId });
            builder.HasOne(x => x.Program)
             .WithMany(x => x.DiscountSpecificProductCategories)
             .HasForeignKey(x => x.ProgramId);

            builder.HasOne(x => x.ProductCategory)
              .WithMany()
              .HasForeignKey(x => x.ProductCategoryId);
        }
    }
}
