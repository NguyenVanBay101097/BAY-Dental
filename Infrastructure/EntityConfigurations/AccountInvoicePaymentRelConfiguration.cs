using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoicePaymentRelConfiguration : IEntityTypeConfiguration<AccountInvoicePaymentRel>
    {
        public void Configure(EntityTypeBuilder<AccountInvoicePaymentRel> builder)
        {
            builder.HasKey(x => new { x.PaymentId, x.InvoiceId });

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.AccountInvoicePaymentRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.Invoice)
                .WithMany(x => x.AccountInvoicePaymentRels)
                .HasForeignKey(x => x.InvoiceId);
        }
    }
}
