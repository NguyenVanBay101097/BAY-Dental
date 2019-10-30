using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineToothRelConfiguration : IEntityTypeConfiguration<SaleOrderLineToothRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLineToothRel> builder)
        {
            builder.HasKey(x => new { x.SaleLineId, x.ToothId });

            builder.HasOne(x => x.SaleLine)
              .WithMany(x => x.SaleOrderLineToothRels)
              .HasForeignKey(x => x.SaleLineId);

            builder.HasOne(x => x.Tooth)
             .WithMany()
             .HasForeignKey(x => x.ToothId);
        }
    }
}
