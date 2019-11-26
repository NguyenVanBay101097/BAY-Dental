using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AppointmentMailMessageRelConfiguration : IEntityTypeConfiguration<AppointmentMailMessageRel>
    {
        public void Configure(EntityTypeBuilder<AppointmentMailMessageRel> builder)
        {
            builder.HasKey(x => new { x.AppointmentId, x.MailMessageId });

            builder.HasOne(x => x.Appointment)
              .WithMany(x => x.AppointmentMailMessageRels)
              .HasForeignKey(x => x.AppointmentId);

            builder.HasOne(x => x.MailMessage)
             .WithMany()
             .HasForeignKey(x => x.MailMessageId);
        }
    }
}
