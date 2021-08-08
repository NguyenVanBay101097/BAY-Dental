using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CustomerReceiptProductRelConfiguration : IEntityTypeConfiguration<CustomerReceiptProductRel>
    {
        public void Configure(EntityTypeBuilder<CustomerReceiptProductRel> builder)
        {
            builder.HasKey(x => new { x.CustomerReceiptId, x.ProductId });

            builder.HasOne(x => x.CustomerReceipt)
              .WithMany(x => x.CustomerReceiptProductRels)
              .HasForeignKey(x => x.CustomerReceiptId);

            builder.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId);
        }
    }
}
