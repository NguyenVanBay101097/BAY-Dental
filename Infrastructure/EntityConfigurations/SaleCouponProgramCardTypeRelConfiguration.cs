using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramCardTypeRelConfiguration : IEntityTypeConfiguration<SaleCouponProgramCardTypeRel>
    {

        public void Configure(EntityTypeBuilder<SaleCouponProgramCardTypeRel> builder)
        {
            builder.HasKey(x => new { x.ProgramId, x.CardTypeId });

            builder.HasOne(x => x.Program)
                .WithMany(x => x.DiscountCardTypes)
                .HasForeignKey(x => x.ProgramId);

            builder.HasOne(x => x.CardType)
                .WithMany()
                .HasForeignKey(x => x.CardTypeId);
        }
    }
}
