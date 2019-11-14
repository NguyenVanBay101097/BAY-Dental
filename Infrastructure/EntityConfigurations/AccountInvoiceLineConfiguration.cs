using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoiceLineConfiguration : IEntityTypeConfiguration<AccountInvoiceLine>
    {
        public void Configure(EntityTypeBuilder<AccountInvoiceLine> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Tooth)
           .WithMany()
           .HasForeignKey(x => x.ToothId);

            builder.HasOne(x => x.LaboLine)
                .WithMany(x => x.InvoiceLines)
                .HasForeignKey(x => x.LaboLineId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.PurchaseLine)
                .WithMany(x => x.InvoiceLines)
                .HasForeignKey(x => x.PurchaseLineId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Invoice)
              .WithMany(x => x.InvoiceLines)
              .HasForeignKey(x => x.InvoiceId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Account)
             .WithMany()
             .HasForeignKey(x => x.AccountId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.UoM)
           .WithMany()
           .HasForeignKey(x => x.UoMId);

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.ToothCategory)
             .WithMany()
             .HasForeignKey(x => x.ToothCategoryId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
