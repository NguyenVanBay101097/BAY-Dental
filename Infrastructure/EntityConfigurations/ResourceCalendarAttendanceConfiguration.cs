using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResourceCalendarAttendanceConfiguration : IEntityTypeConfiguration<ResourceCalendarAttendance>
    {
        public void Configure(EntityTypeBuilder<ResourceCalendarAttendance> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.DayOfWeek).IsRequired();
            builder.Property(x => x.DayPeriod).IsRequired();

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Calendar)
                .WithMany(x => x.ResourceCalendarAttendances)
                .HasForeignKey(x => x.CalendarId);
        }
    }
}
