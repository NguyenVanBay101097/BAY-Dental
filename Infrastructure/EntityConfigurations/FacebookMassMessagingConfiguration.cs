using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookMassMessagingConfiguration : IEntityTypeConfiguration<FacebookMassMessaging>
    {
        public void Configure(EntityTypeBuilder<FacebookMassMessaging> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.FacebookPage)
                .WithMany()
                .HasForeignKey(x => x.FacebookPageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
