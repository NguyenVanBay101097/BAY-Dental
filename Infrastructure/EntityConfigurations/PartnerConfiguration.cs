﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
    {
        public void Configure(EntityTypeBuilder<Partner> builder)
        {
            builder.Property(x => x.Name).IsRequired();         

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Source)
            .WithMany()
            .HasForeignKey(x => x.SourceId);

            builder.HasOne(x => x.ReferralUser)
            .WithMany()
            .HasForeignKey(x => x.ReferralUserId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
            .WithMany()
            .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Title)
            .WithMany()
            .HasForeignKey(x => x.TitleId);

            builder.HasOne(x => x.Consultant)
            .WithMany()
            .HasForeignKey(x => x.ConsultantId);
        }
    }
}
