using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookPageConfiguration : IEntityTypeConfiguration<FacebookPage>
    {
        public void Configure(EntityTypeBuilder<FacebookPage> builder)
        {
            builder.HasOne(x => x.AutoConfig)
      .WithMany()
      .HasForeignKey(x => x.AutoConfigId);
            builder.HasOne(x => x.CreatedBy)
        .WithMany()
        .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

        }
    }
}
