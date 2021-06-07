using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessageAppointmentRelConfiguration : IEntityTypeConfiguration<SmsMessageAppointmentRel>
    {
        public void Configure(EntityTypeBuilder<SmsMessageAppointmentRel> builder)
        {
            builder.HasKey(x => new { x.AppointmentId, x.SmsMessageId });

            builder.HasOne(x => x.SmsMessage)
               .WithMany(x => x.SmsMessageAppointmentRels)
               .HasForeignKey(x => x.SmsMessageId);

            builder.HasOne(x => x.Appointment)
                .WithMany()
                .HasForeignKey(x => x.AppointmentId);
        }
    }
}
