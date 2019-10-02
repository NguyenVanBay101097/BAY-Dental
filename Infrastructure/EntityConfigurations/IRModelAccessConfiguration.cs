using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class IRModelAccessConfiguration : IEntityTypeConfiguration<IRModelAccess>
    {
        public void Configure(EntityTypeBuilder<IRModelAccess> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Model)
        .WithMany(x => x.ModelAccesses)
        .HasForeignKey(x => x.ModelId);

            builder.HasOne(x => x.Group)
              .WithMany(x => x.ModelAccesses)
              .HasForeignKey(x => x.GroupId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
