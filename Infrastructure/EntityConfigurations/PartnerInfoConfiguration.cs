using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerInfoConfiguration :  IEntityTypeConfiguration<PartnerInfo>
    {
        public void Configure(EntityTypeBuilder<PartnerInfo> builder)
        {
            builder.HasNoKey().ToView(null);

            builder.HasOne(x => x.Company)
              .WithMany()
              .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.MemberLevel)
             .WithMany()
             .HasForeignKey(x => x.MemberLevelId);
        }
    }
}
