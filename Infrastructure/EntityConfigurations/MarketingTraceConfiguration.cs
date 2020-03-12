using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MarketingTraceConfiguration : IEntityTypeConfiguration<MarketingTrace>
    {
        public void Configure(EntityTypeBuilder<MarketingTrace> builder)
        {
            builder.HasOne(x => x.Activity)
                .WithMany(x => x.Traces)
                .HasForeignKey(x => x.ActivityId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
