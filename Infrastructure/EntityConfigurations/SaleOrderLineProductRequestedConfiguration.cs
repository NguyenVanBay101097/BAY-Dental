using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineProductRequestedConfiguration : IEntityTypeConfiguration<SaleOrderLineProductRequested>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLineProductRequested> builder)
        {
            builder.HasOne(x => x.Product)
          .WithMany()
          .HasForeignKey(x => x.ProductId)
          .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SaleOrderLine)
         .WithMany()
         .HasForeignKey(x => x.SaleOrderLineId)
         .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
