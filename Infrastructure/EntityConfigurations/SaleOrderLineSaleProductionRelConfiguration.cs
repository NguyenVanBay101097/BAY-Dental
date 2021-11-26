using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineSaleProductionRelConfiguration : IEntityTypeConfiguration<SaleOrderLineSaleProductionRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLineSaleProductionRel> builder)
        {
            builder.HasKey(x => new { x.OrderLineId, x.SaleProductionId });

            builder.HasOne(x => x.OrderLine)
                .WithMany()
                .HasForeignKey(x => x.OrderLineId);

            builder.HasOne(x => x.SaleProduction)
                .WithMany(x => x.SaleOrderLineRels)
                .HasForeignKey(x => x.SaleProductionId);
        }
    }
}
