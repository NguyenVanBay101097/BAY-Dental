using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareScenarioConfiguration : IEntityTypeConfiguration<TCareScenario>
    {
        public void Configure(EntityTypeBuilder<TCareScenario> builder)
        {
            builder.HasOne(x => x.ChannelSocial)
                .WithMany()
                .HasForeignKey(x => x.ChannelSocialId);

            builder.HasOne(x => x.CreatedBy)
              .WithMany()
              .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
              .WithMany()
              .HasForeignKey(x => x.WriteById);
        }
    }
}
