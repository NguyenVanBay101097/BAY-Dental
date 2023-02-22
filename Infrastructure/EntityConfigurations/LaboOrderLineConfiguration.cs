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
            builder.HasOne(x => x.Partner)
             .WithMany()
             .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ToothCategory)
           .WithMany()
           .HasForeignKey(x => x.ToothCategoryId);

            builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId);

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

            builder.HasOne(x => x.SaleOrderLine)
                .WithMany()
                .HasForeignKey(x => x.SaleOrderLineId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
