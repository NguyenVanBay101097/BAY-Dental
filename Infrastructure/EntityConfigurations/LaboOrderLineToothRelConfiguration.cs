using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboOrderLineToothRelConfiguration : IEntityTypeConfiguration<LaboOrderLineToothRel>
    {
        public void Configure(EntityTypeBuilder<LaboOrderLineToothRel> builder)
        {
            builder.HasKey(x => new { x.LaboLineId, x.ToothId });

            builder.HasOne(x => x.LaboLine)
              .WithMany(x => x.LaboOrderLineToothRels)
              .HasForeignKey(x => x.LaboLineId);

            builder.HasOne(x => x.Tooth)
             .WithMany()
             .HasForeignKey(x => x.ToothId);
        }
    }
}
