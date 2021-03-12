using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductBomConfiguration : IEntityTypeConfiguration<ProductBom>
    {

        public void Configure(EntityTypeBuilder<ProductBom> builder)
        {
            builder.HasOne(x => x.MaterialProduct)
              .WithMany()
              .HasForeignKey(x => x.MaterialProductId);

            builder.HasOne(x => x.Product)
              .WithMany(x=>x.Boms)
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
