using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerHistoryRelConfiguration : IEntityTypeConfiguration<PartnerHistoryRel>
    {
        public void Configure(EntityTypeBuilder<PartnerHistoryRel> builder)
        {
            builder.HasKey(x => new { x.PartnerId, x.HistoryId });

            builder.HasOne(x => x.History)
                .WithMany(x => x.PartnerHistoryRels)
                .HasForeignKey(x => x.HistoryId);

            builder.HasOne(x => x.Partner)
                .WithMany(x => x.PartnerHistoryRels)
                .HasForeignKey(x=>x.PartnerId);
        }
    }
}
