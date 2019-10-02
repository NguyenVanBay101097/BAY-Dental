using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboOrderLineConfiguration : IEntityTypeConfiguration<LaboOrderLine>
    {
        public void Configure(EntityTypeBuilder<LaboOrderLine> builder)
        {
            builder.HasOne(x => x.Customer)
              .WithMany()
              .HasForeignKey(x => x.CustomerId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Supplier)
             .WithMany()
             .HasForeignKey(x => x.SupplierId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Invoice)
               .WithMany()
               .HasForeignKey(x => x.InvoiceId);

            builder.HasOne(x => x.DotKham)
             .WithMany()
             .HasForeignKey(x => x.DotKhamId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
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
