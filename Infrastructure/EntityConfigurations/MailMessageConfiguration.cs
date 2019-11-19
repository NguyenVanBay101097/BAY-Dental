using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MailMessageConfiguration : IEntityTypeConfiguration<MailMessage>
    {
        public void Configure(EntityTypeBuilder<MailMessage> builder)
        {
            builder.Property(x => x.MessageType).IsRequired();

            builder.HasOne(x => x.Author)
          .WithMany()
          .HasForeignKey(x => x.AuthorId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
