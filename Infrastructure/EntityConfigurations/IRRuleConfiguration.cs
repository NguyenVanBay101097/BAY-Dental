using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class IRRuleConfiguration : IEntityTypeConfiguration<IRRule>
    {
        public void Configure(EntityTypeBuilder<IRRule> builder)
        {
            builder.Property(x => x.Name)
               .IsRequired();

            builder.HasOne(x => x.Model)
              .WithMany()
              .HasForeignKey(x => x.ModelId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
             .WithMany()
             .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
           .WithMany()
           .HasForeignKey(x => x.WriteById);
        }
    }

}
