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

            builder.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Journal)
            .WithMany()
            .HasForeignKey(x => x.JournalId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Partner)
           .WithMany()
           .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Account)
                 .WithMany()
                 .HasForeignKey(x => x.AccountId);

            builder.HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.Assistant)
             .WithMany()
             .HasForeignKey(x => x.AssistantId);

            builder.HasOne(x => x.InvoiceUser)
            .WithMany()
            .HasForeignKey(x => x.InvoiceUserId);
        }
    }
}
