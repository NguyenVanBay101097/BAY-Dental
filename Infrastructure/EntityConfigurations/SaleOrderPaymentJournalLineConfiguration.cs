using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPaymentJournalLineConfiguration : IEntityTypeConfiguration<SaleOrderPaymentJournalLine>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPaymentJournalLine> builder)
        {
            builder.HasOne(x => x.Journal)
                   .WithMany()
                   .HasForeignKey(x => x.JournalId);

            builder.HasOne(x => x.SaleOrderPayment)
                    .WithMany(x => x.JournalLines)
                    .HasForeignKey(x => x.SaleOrderPaymentId);

            builder.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
