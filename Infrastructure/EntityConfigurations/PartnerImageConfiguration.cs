using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerImageConfiguration : IEntityTypeConfiguration<PartnerImage>
    {
        public void Configure(EntityTypeBuilder<PartnerImage> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.UploadId).IsRequired();

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.Partner)
                .WithMany(x => x.PartnerImages)
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.DotKham)
                .WithMany(x => x.DotKhamImages)
                .HasForeignKey(x => x.DotkhamId);

        }
    }
}
