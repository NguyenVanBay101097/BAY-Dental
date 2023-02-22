using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderNoCodePromoProgramConfiguration : IEntityTypeConfiguration<SaleOrderNoCodePromoProgram>
    {
        public void Configure(EntityTypeBuilder<SaleOrderNoCodePromoProgram> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.ProgramId });
            builder.HasOne(x => x.Order)
             .WithMany(x => x.NoCodePromoPrograms)
             .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.Program)
              .WithMany()
              .HasForeignKey(x => x.ProgramId);
        }
    }
}
