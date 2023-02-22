using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResGroupImpliedRelConfiguration : IEntityTypeConfiguration<ResGroupImpliedRel>
    {
        public void Configure(EntityTypeBuilder<ResGroupImpliedRel> builder)
        {
            builder.HasKey(x => new { x.GId, x.HId });
            builder.HasOne(x => x.G)
             .WithMany(x => x.ImpliedRels)
             .HasForeignKey(x => x.GId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.H)
              .WithMany()
              .HasForeignKey(x => x.HId);
        }
    }
}
