using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareMessagingPartnerRelConfiguration : IEntityTypeConfiguration<TCareMessagingPartnerRel>
    {
        public void Configure(EntityTypeBuilder<TCareMessagingPartnerRel> builder)
        {
            builder.HasKey(x => new { x.MessagingId, x.PartnerId });

            builder.HasOne(x => x.Messaging)
                .WithMany(x => x.PartnerRecipients)
                .HasForeignKey(x => x.MessagingId);

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);
        }
    }
}
