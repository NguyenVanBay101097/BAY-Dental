using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class HrPayrollStructureTypeConfiguration : IEntityTypeConfiguration<HrPayrollStructureType>
    {
        public void Configure(EntityTypeBuilder<HrPayrollStructureType> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.WageType).IsRequired();

            builder.HasOne(x => x.DefaultResourceCalendar)
                .WithMany()
                .HasForeignKey(x => x.DefaultResourceCalendarId);

            builder.HasOne(x => x.DefaultStruct)
              .WithMany()
              .HasForeignKey(x => x.DefaultStructId);

            builder.HasOne(x => x.DefaultWorkEntryType)
              .WithMany()
              .HasForeignKey(x => x.DefaultWorkEntryTypeId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
