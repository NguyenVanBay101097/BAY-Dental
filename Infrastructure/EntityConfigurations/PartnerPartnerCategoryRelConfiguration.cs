using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerPartnerCategoryRelConfiguration : IEntityTypeConfiguration<PartnerPartnerCategoryRel>
    {
        public void Configure(EntityTypeBuilder<PartnerPartnerCategoryRel> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.PartnerId });

            builder.HasOne(x => x.Partner)
            .WithMany(x => x.PartnerPartnerCategoryRels)
            .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.PartnerPartnerCategoryRels)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}
