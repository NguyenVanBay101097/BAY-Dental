using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineInvoiceRelConfiguration : IEntityTypeConfiguration<SaleOrderLineInvoiceRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLineInvoiceRel> builder)
        {
            builder.HasKey(x => new { x.OrderLineId, x.InvoiceLineId });

            builder.HasOne(x => x.OrderLine)
                .WithMany(x => x.SaleOrderLineInvoiceRels)
                .HasForeignKey(x => x.OrderLineId);

            builder.HasOne(x => x.InvoiceLine)
                .WithMany(x => x.SaleLines)
                .HasForeignKey(x => x.InvoiceLineId);
        }
    }
}
