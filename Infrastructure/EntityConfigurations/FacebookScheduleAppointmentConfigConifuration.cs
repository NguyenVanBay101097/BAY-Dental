using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookScheduleAppointmentConfigConifuration : IEntityTypeConfiguration<FacebookScheduleAppointmentConfig>
    {
        public void Configure(EntityTypeBuilder<FacebookScheduleAppointmentConfig> builder)
        {

            builder.HasOne(x => x.FacebookPage)
        .WithMany()
        .HasForeignKey(x => x.FBPageId);

            builder.HasOne(x => x.CreatedBy)
               .WithMany()
               .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
