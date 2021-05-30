using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AdvisoryToothRelConfiguration : IEntityTypeConfiguration<AdvisoryToothRel>
    {
        public void Configure(EntityTypeBuilder<AdvisoryToothRel> builder)
        {
            builder.HasKey(x => new { x.AdvisoryId, x.ToothId });

            builder.HasOne(x => x.Advisory)
                .WithMany(x => x.AdvisoryToothRels)
                .HasForeignKey(x => x.AdvisoryId);

            builder.HasOne(x => x.Tooth)
                .WithMany(x => x.AdvisoryToothRels)
                .HasForeignKey(x => x.ToothId);
        }
    }
}
