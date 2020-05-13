using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductUoMRelConfiguration : IEntityTypeConfiguration<ProductUoMRel>
    {
        public void Configure(EntityTypeBuilder<ProductUoMRel> builder)
        {
            builder.HasKey(x => new { x.UoMId, x.ProductId });

            builder.HasOne(x => x.Product)
                .WithMany(x=>x.ProductUoMRels)
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.UoM)
                .WithMany(x=>x.ProductUoMRels)
                .HasForeignKey(x => x.UoMId);
        }
    }
}
