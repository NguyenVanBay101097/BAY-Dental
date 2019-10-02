using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountRegisterPaymentInvoiceRelConfiguration : IEntityTypeConfiguration<AccountRegisterPaymentInvoiceRel>
    {
        public void Configure(EntityTypeBuilder<AccountRegisterPaymentInvoiceRel> builder)
        {
            builder.HasKey(x => new { x.PaymentId, x.InvoiceId });
            builder.HasOne(x => x.Payment)
                .WithMany(x => x.AccountRegisterPaymentInvoiceRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.Invoice)
               .WithMany()
               .HasForeignKey(x => x.InvoiceId);
        }
    }
}
