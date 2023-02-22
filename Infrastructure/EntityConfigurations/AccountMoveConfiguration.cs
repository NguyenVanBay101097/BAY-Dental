using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountMoveConfiguration : IEntityTypeConfiguration<AccountMove>
    {
        public void Configure(EntityTypeBuilder<AccountMove> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.State).IsRequired();

            builder.HasOne(x => x.Journal)
               .WithMany()
               .HasForeignKey(x => x.JournalId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.InvoiceUser)
               .WithMany()
               .HasForeignKey(x => x.InvoiceUserId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
