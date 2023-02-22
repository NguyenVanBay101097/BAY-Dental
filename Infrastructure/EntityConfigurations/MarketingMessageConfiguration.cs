using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MarketingMessageConfiguration : IEntityTypeConfiguration<MarketingMessage>
    {
        public void Configure(EntityTypeBuilder<MarketingMessage> builder)
        {
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Template).IsRequired();
           
          
            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
