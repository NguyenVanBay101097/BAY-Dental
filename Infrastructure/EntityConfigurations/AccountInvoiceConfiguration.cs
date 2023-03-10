using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoiceConfiguration : IEntityTypeConfiguration<AccountInvoice>
    {
        public void Configure(EntityTypeBuilder<AccountInvoice> builder)
        {
            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account)
             .WithMany()
             .HasForeignKey(x => x.AccountId);

            builder.HasOne(x => x.Journal)
            .WithMany()
            .HasForeignKey(x => x.JournalId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Move)
           .WithMany()
           .HasForeignKey(x => x.MoveId);

            builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
