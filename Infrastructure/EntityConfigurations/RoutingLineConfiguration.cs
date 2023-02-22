using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class RoutingLineConfiguration : IEntityTypeConfiguration<RoutingLine>
    {
        public void Configure(EntityTypeBuilder<RoutingLine> builder)
        {
            builder.HasOne(x => x.Routing)
              .WithMany(x => x.Lines)
              .HasForeignKey(x => x.RoutingId);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
