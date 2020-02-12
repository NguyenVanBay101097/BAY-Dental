using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoiceReportConfiguration : IEntityTypeConfiguration<AccountInvoiceReport>
    {
        public void Configure(EntityTypeBuilder<AccountInvoiceReport> builder)
        {
            builder.ToView("account_invoice_report");
            builder.HasNoKey();

            builder.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.product_id);

            builder.HasOne(x => x.Categ)
             .WithMany()
             .HasForeignKey(x => x.categ_id);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.company_id);

            builder.HasOne(x => x.Journal)
            .WithMany()
            .HasForeignKey(x => x.journal_id)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Invoice)
           .WithMany()
           .HasForeignKey(x => x.invoice_id);

            builder.HasOne(x => x.Partner)
           .WithMany()
           .HasForeignKey(x => x.partner_id);

            builder.HasOne(x => x.User)
         .WithMany()
         .HasForeignKey(x => x.user_id);

            builder.HasOne(x => x.Account)
                 .WithMany()
                 .HasForeignKey(x => x.account_id);

            builder.HasOne(x => x.AccountLine)
                .WithMany()
                .HasForeignKey(x => x.account_line_id);
        }
    }
}
