using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLineInvoice2RelConfiguration : IEntityTypeConfiguration<SaleOrderLineInvoice2Rel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLineInvoice2Rel> builder)
        {
            builder.HasKey(x => new { x.OrderLineId, x.InvoiceLineId });

            builder.HasOne(x => x.OrderLine)
                .WithMany(x => x.SaleOrderLineInvoice2Rels)
                .HasForeignKey(x => x.OrderLineId);

            builder.HasOne(x => x.InvoiceLine)
                .WithMany(x => x.SaleLineRels)
                .HasForeignKey(x => x.InvoiceLineId);
        }
    }
}
