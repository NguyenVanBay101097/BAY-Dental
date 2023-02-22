using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MarketingMessageButtonConfiguration : IEntityTypeConfiguration<MarketingMessageButton>
    {
        public void Configure(EntityTypeBuilder<MarketingMessageButton> builder)
        {
            builder.HasOne(x => x.Message)
            .WithMany(x => x.Buttons)
            .HasForeignKey(x => x.MessageId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
