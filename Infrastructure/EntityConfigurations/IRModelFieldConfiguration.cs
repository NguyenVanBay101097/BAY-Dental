using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class IRModelFieldConfiguration : IEntityTypeConfiguration<IRModelField>
    {
        public void Configure(EntityTypeBuilder<IRModelField> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Model).IsRequired();
            builder.Property(x => x.TType).IsRequired();

            builder.HasOne(x => x.IRModel)
                .WithMany()
                .HasForeignKey(x => x.IRModelId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
