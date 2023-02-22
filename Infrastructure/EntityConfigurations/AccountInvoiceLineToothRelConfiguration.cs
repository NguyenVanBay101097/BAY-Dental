using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoiceLineToothRelConfiguration : IEntityTypeConfiguration<AccountInvoiceLineToothRel>
    {
        public void Configure(EntityTypeBuilder<AccountInvoiceLineToothRel> builder)
        {
            builder.HasKey(x => new { x.InvoiceLineId, x.ToothId });

            builder.HasOne(x => x.InvoiceLine)
              .WithMany(x => x.AccountInvoiceLineToothRels)
              .HasForeignKey(x => x.InvoiceLineId);

            builder.HasOne(x => x.Tooth)
             .WithMany()
             .HasForeignKey(x => x.ToothId);
        }
    }
}
