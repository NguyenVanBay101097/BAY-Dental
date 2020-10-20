using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ServiceCardOrderPaymentConfiguration : IEntityTypeConfiguration<ServiceCardOrderPayment>
    {
        public void Configure(EntityTypeBuilder<ServiceCardOrderPayment> builder)
        {
            builder.HasOne(x => x.Journal)
            .WithMany()
            .HasForeignKey(x => x.JournalId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order)
              .WithMany(x => x.Payments)
              .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
