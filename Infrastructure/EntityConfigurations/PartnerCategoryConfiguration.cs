﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerCategoryConfiguration : IEntityTypeConfiguration<PartnerCategory>
    {
        public void Configure(EntityTypeBuilder<PartnerCategory> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Parent)
           .WithMany()
           .HasForeignKey(x => x.ParentId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
