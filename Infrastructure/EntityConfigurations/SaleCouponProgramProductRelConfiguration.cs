using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramProductRelConfiguration : IEntityTypeConfiguration<SaleCouponProgramProductRel>
    {
        public void Configure(EntityTypeBuilder<SaleCouponProgramProductRel> builder)
        {
            builder.HasKey(x => new { x.ProgramId, x.ProductId });
            builder.HasOne(x => x.Program)
             .WithMany(x => x.DiscountSpecificProducts)
             .HasForeignKey(x => x.ProgramId);

            builder.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId);
        }
    }
}
