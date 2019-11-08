using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
        {
            builder.HasOne(x => x.Partner)
             .WithMany()
             .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ProductUOM)
          .WithMany()
          .HasForeignKey(x => x.ProductUOMId);

            builder.HasOne(x => x.Order)
              .WithMany(x => x.OrderLines)
              .HasForeignKey(x => x.OrderId);

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
