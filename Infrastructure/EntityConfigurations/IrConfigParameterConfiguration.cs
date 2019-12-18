using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class IrConfigParameterConfiguration : IEntityTypeConfiguration<IrConfigParameter>
    {
        public void Configure(EntityTypeBuilder<IrConfigParameter> builder)
        {
            builder.Property(x => x.Key).IsRequired();
            builder.Property(x => x.Value).IsRequired();

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
