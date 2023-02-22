using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ServiceCardOrderLineInvoiceRelConfiguration : IEntityTypeConfiguration<ServiceCardOrderLineInvoiceRel>
    {
        public void Configure(EntityTypeBuilder<ServiceCardOrderLineInvoiceRel> builder)
        {
            builder.HasKey(x => new { x.OrderLineId, x.InvoiceLineId });

            builder.HasOne(x => x.OrderLine)
                .WithMany(x => x.OrderLineInvoiceRels)
                .HasForeignKey(x => x.OrderLineId);

            builder.HasOne(x => x.InvoiceLine)
                .WithMany(x => x.CardOrderLineRels)
                .HasForeignKey(x => x.InvoiceLineId);
        }
    }
}
