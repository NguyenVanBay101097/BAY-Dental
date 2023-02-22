using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockQuantMoveRelConfiguration : IEntityTypeConfiguration<StockQuantMoveRel>
    {
        public void Configure(EntityTypeBuilder<StockQuantMoveRel> builder)
        {
            builder.HasKey(x => new { x.MoveId, x.QuantId });
            builder.HasOne(x => x.Quant)
                .WithMany(x => x.StockQuantMoveRels)
                .HasForeignKey(x => x.QuantId);

            builder.HasOne(x => x.Move)
                .WithMany()
                .HasForeignKey(x => x.MoveId);
        }
    }
}
