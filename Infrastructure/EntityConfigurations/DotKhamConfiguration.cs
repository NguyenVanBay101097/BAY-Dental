﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class DotKhamConfiguration : IEntityTypeConfiguration<DotKham>
    {
        public void Configure(EntityTypeBuilder<DotKham> builder)
        {
            builder.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Partner)
               .WithMany()
               .HasForeignKey(x => x.PartnerId);

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

            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId);

            builder.HasOne(x => x.Assistant)
                .WithMany()
                .HasForeignKey(x => x.AssistantId);
        }
    }
}
