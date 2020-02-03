using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductStepConfiguration : IEntityTypeConfiguration<ProductStep>
    {
        public void Configure(EntityTypeBuilder<ProductStep> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Product)
            .WithMany(x => x.Steps)
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
