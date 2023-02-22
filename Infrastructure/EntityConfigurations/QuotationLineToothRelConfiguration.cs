using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class QuotationLineToothRelConfiguration : IEntityTypeConfiguration<QuotationLineToothRel>
    {
        public void Configure(EntityTypeBuilder<QuotationLineToothRel> builder)
        {
            builder.HasKey(x => new { x.QuotationLineId, x.ToothId });

            builder.HasOne(x => x.QuotationLine)
              .WithMany(x => x.QuotationLineToothRels)
              .HasForeignKey(x => x.QuotationLineId);

            builder.HasOne(x => x.Tooth)
             .WithMany(x => x.QuotationLineToothRels)
             .HasForeignKey(x => x.ToothId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
