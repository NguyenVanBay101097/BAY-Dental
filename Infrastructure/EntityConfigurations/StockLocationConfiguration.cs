using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockLocationConfiguration : IEntityTypeConfiguration<StockLocation>
    {
        public void Configure(EntityTypeBuilder<StockLocation> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Usage).IsRequired();

            builder.HasOne(x => x.ParentLocation)
                .WithMany(x => x.Childs)
                .HasForeignKey(x => x.ParentLocationId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
