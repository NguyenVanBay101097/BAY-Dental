using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockInventoryLineConfiguration : IEntityTypeConfiguration<StockInventoryLine>
    {
        public void Configure(EntityTypeBuilder<StockInventoryLine> builder)
        {
            builder.HasOne(x => x.Location)
           .WithMany()
           .HasForeignKey(x => x.LocationId);

            builder.HasOne(x => x.Company)
           .WithMany()
           .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Inventory)
           .WithMany(x=>x.Lines)
           .HasForeignKey(x => x.InventoryId);

            builder.HasOne(x => x.Product)
           .WithMany()
           .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ProductUOM)
           .WithMany()
           .HasForeignKey(x => x.ProductUOMId);

            builder.HasOne(x => x.CreatedBy)
              .WithMany()
              .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
