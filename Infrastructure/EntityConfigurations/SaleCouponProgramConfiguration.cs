using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramConfiguration : IEntityTypeConfiguration<SaleCouponProgram>
    {
        public void Configure(EntityTypeBuilder<SaleCouponProgram> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.DiscountLineProduct)
              .WithMany()
              .HasForeignKey(x => x.DiscountLineProductId);

            builder.HasOne(x => x.RewardProduct)
                .WithMany()
                .HasForeignKey(x => x.RewardProductId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
