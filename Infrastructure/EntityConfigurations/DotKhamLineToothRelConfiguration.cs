using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class DotKhamLineToothRelConfiguration : IEntityTypeConfiguration<DotKhamLineToothRel>
    {
        public void Configure(EntityTypeBuilder<DotKhamLineToothRel> builder)
        {
            builder.HasKey(x => new { x.LineId, x.ToothId });

            builder.HasOne(x => x.Line)
              .WithMany(x => x.ToothRels)
              .HasForeignKey(x => x.LineId);

            builder.HasOne(x => x.Tooth)
             .WithMany()
             .HasForeignKey(x => x.ToothId);
        }
    }
}
