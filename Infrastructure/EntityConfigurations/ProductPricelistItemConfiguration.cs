using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductPricelistItemConfiguration : IEntityTypeConfiguration<ProductPricelistItem>
    {
        public void Configure(EntityTypeBuilder<ProductPricelistItem> builder)
        {
            builder.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Categ)
             .WithMany()
             .HasForeignKey(x => x.CategId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PriceList)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
