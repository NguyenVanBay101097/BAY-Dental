using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCarePropertyConfiguration : IEntityTypeConfiguration<TCareProperty>
    {
        public void Configure(EntityTypeBuilder<TCareProperty> builder)
        {
            builder.Property(x => x.Type).IsRequired();

            builder.HasOne(x => x.Rule)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.RuleId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
