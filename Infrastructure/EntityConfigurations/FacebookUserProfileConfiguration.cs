﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookUserProfileConfiguration : IEntityTypeConfiguration<FacebookUserProfile>
    {
        public void Configure(EntityTypeBuilder<FacebookUserProfile> builder)
        {
            builder.HasOne(x => x.FbPage)
                .WithMany()
                .HasForeignKey(x => x.FbPageId);

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
