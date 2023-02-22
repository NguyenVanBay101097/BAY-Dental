using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResourceCalendarLeavesConfiguration : IEntityTypeConfiguration<ResourceCalendarLeaves>
    {
        public void Configure(EntityTypeBuilder<ResourceCalendarLeaves> builder)
        {
            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Calendar)
            .WithMany(x=>x.Leaves)
            .HasForeignKey(x => x.CalendarId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
