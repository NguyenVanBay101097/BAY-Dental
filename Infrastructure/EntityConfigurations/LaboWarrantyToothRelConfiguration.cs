using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboWarrantyToothRelConfiguration : IEntityTypeConfiguration<LaboWarrantyToothRel>
    {
        public void Configure(EntityTypeBuilder<LaboWarrantyToothRel> builder)
        {
            builder.HasKey(x => new { x.LaboWarrantyId, x.ToothId });

            builder.HasOne(x => x.LaboWarranty)
                .WithMany(x => x.LaboWarrantyToothRels)
                .HasForeignKey(x => x.LaboWarrantyId);

            builder.HasOne(x => x.Tooth)
                .WithMany(x => x.LaboWarrantyToothRels)
                .HasForeignKey(x => x.ToothId);
        }
    }
}
