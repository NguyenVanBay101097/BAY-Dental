using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockInventoryConfiguration : IEntityTypeConfiguration<StockInventory>
    {
        public void Configure(EntityTypeBuilder<StockInventory> builder)
        {
            builder.HasOne(x => x.Location)
           .WithMany()
           .HasForeignKey(x => x.LocationId);

            builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
