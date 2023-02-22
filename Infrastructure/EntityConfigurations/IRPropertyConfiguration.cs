using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class IRPropertyConfiguration : IEntityTypeConfiguration<IRProperty>
    {
        public void Configure(EntityTypeBuilder<IRProperty> builder)
        {
            builder.Property(x => x.Type).IsRequired();

            builder.HasOne(x => x.Field)
                .WithMany()
                .HasForeignKey(x => x.FieldId);

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
