using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramPartnerRelConfiguration : IEntityTypeConfiguration<SaleCouponProgramPartnerRel>
    {
        public void Configure(EntityTypeBuilder<SaleCouponProgramPartnerRel> builder)
        {
            builder.HasKey(x => new { x.ProgramId, x.PartnerId });
            builder.HasOne(x => x.Program)
             .WithMany(x => x.DiscountSpecificPartners)
             .HasForeignKey(x => x.ProgramId);

            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId);
        }
    }
}
