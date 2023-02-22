using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SurveyUserInputLineConfiguration : IEntityTypeConfiguration<SurveyUserInputLine>
    {
        public void Configure(EntityTypeBuilder<SurveyUserInputLine> builder)
        {

            builder.HasOne(x => x.Question)
          .WithMany()
          .HasForeignKey(x => x.QuestionId);

            builder.HasOne(x => x.Answer)
           .WithMany()
           .HasForeignKey(x => x.AnswerId);

            builder.HasOne(x => x.UserInput)
          .WithMany(x => x.Lines)
          .HasForeignKey(x => x.UserInputId);

            builder.HasOne(x => x.CreatedBy)
           .WithMany()
           .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
