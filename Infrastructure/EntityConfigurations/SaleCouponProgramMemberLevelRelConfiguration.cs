using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleCouponProgramMemberLevelRelConfiguration : IEntityTypeConfiguration<SaleCouponProgramMemberLevelRel>
    {
        public void Configure(EntityTypeBuilder<SaleCouponProgramMemberLevelRel> builder)
        {
            builder.HasKey(x => new { x.ProgramId, x.MemberLevelId });
            builder.HasOne(x => x.Program)
                .WithMany(x => x.DiscountMemberLevels)
                .HasForeignKey(x => x.ProgramId);
            builder.HasOne(x => x.MemberLevel)
                .WithMany()
                .HasForeignKey(x => x.MemberLevelId);
        }
    }
}
