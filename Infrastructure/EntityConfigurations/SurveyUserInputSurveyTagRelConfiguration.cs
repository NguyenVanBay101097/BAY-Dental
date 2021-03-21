using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SurveyUserInputSurveyTagRelConfiguration : IEntityTypeConfiguration<SurveyUserInputSurveyTagRel>
    {
        public void Configure(EntityTypeBuilder<SurveyUserInputSurveyTagRel> builder)
        {
            builder.HasKey(x => new { x.UserInputId, x.SurveyTagId });

            builder.HasOne(x => x.UserInput)
              .WithMany(x => x.SurveyUserInputSurveyTagRels)
              .HasForeignKey(x => x.UserInputId);

            builder.HasOne(x => x.SurveyTag)
             .WithMany(x => x.SurveyUserInputSurveyTagRels)
             .HasForeignKey(x => x.SurveyTagId);
        }
    }
}
