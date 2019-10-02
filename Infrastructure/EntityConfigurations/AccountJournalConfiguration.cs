using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountJournalConfiguration : IEntityTypeConfiguration<AccountJournal>
    {
        public void Configure(EntityTypeBuilder<AccountJournal> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Code).IsRequired();

            builder.HasOne(x => x.Sequence)
                .WithMany()
                .HasForeignKey(x => x.SequenceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RefundSequence)
               .WithMany()
               .HasForeignKey(x => x.RefundSequenceId);

            builder.HasOne(x => x.DefaultCreditAccount)
                .WithMany()
                .HasForeignKey(x => x.DefaultCreditAccountId);

            builder.HasOne(x => x.DefaultDebitAccount)
                .WithMany()
                .HasForeignKey(x => x.DefaultDebitAccountId);

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
