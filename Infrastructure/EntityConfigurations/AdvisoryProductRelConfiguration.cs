using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AdvisoryProductRelConfiguration : IEntityTypeConfiguration<AdvisoryProductRel>
    {
        public void Configure(EntityTypeBuilder<AdvisoryProductRel> builder)
        {
            builder.HasKey(x => new { x.AdvisoryId, x.ProductId });

            builder.HasOne(x => x.Advisory)
                .WithMany(x => x.AdvisoryProductRels)
                .HasForeignKey(x => x.AdvisoryId);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.AdvisoryProductRels)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
