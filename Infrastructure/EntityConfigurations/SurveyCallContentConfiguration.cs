using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SurveyCallContentConfiguration : IEntityTypeConfiguration<SurveyCallContent>
    {
        public void Configure(EntityTypeBuilder<SurveyCallContent> builder)
        {
            builder.HasOne(x => x.Assignment)
            .WithMany(x=> x.CallContents)
           .HasForeignKey(x => x.AssignmentId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
             .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
           .WithMany()
           .HasForeignKey(x => x.WriteById);
        }
    }
}
