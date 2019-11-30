using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CardTypeConfiguration : IEntityTypeConfiguration<CardType>
    {
        public void Configure(EntityTypeBuilder<CardType> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Pricelist)
                .WithMany()
                .HasForeignKey(x => x.PricelistId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
