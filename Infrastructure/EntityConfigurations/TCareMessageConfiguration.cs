using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareMessageConfiguration : IEntityTypeConfiguration<TCareMessage>
    {
        public void Configure(EntityTypeBuilder<TCareMessage> builder)
        {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.ProfilePartner)
                .WithMany()
                .HasForeignKey(x => x.ProfilePartnerId);

            builder.HasOne(x => x.Campaign)
                .WithMany()
                .HasForeignKey(x => x.CampaignId);

            builder.HasOne(x => x.ChannelSocical)
                .WithMany()
                .HasForeignKey(x => x.ChannelSocicalId);
        }
    }
}
