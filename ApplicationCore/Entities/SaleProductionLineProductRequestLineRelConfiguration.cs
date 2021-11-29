using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleProductionLineProductRequestLineRelConfiguration : IEntityTypeConfiguration<SaleProductionLineProductRequestLineRel>
    {
        public void Configure(EntityTypeBuilder<SaleProductionLineProductRequestLineRel> builder)
        {
            builder.HasKey(x => new { x.SaleProductionLineId, x.ProductRequestLineId });

            builder.HasOne(x => x.SaleProductionLine)
                .WithMany(x => x.ProductRequestLineRels)
                .HasForeignKey(x => x.SaleProductionLineId);

            builder.HasOne(x => x.ProductRequestLine)
                .WithMany(x => x.SaleProductionLineRels)
                .HasForeignKey(x => x.ProductRequestLineId);
        }
    }
}
