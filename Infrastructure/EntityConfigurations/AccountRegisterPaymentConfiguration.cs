﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountRegisterPaymentConfiguration : IEntityTypeConfiguration<AccountRegisterPayment>
    {
        public void Configure(EntityTypeBuilder<AccountRegisterPayment> builder)
        {
            builder.Property(x => x.PaymentType).IsRequired();

            builder.HasOne(x => x.Journal)
                .WithMany()
                .HasForeignKey(x => x.JournalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
