using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CardCardConfiguration : IEntityTypeConfiguration<CardCard>
    {
        public void Configure(EntityTypeBuilder<CardCard> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Type)
              .WithMany()
              .HasForeignKey(x => x.TypeId)
              .OnDelete(DeleteBehavior.Restrict);

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
