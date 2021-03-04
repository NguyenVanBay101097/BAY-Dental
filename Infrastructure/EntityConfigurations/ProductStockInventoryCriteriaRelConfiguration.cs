using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductStockInventoryCriteriaRelConfiguration : IEntityTypeConfiguration<ProductStockInventoryCriteriaRel>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductStockInventoryCriteriaRel> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.StockInventoryCriteriaId });

            builder.HasOne(x => x.StockInventoryCriteria)
             .WithMany()
             .HasForeignKey(x => x.StockInventoryCriteriaId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany(x=> x.ProductStockInventoryCriteriaRels)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
