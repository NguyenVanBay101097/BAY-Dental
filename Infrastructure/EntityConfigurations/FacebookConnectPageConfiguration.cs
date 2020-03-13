using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookConnectPageConfiguration : IEntityTypeConfiguration<FacebookConnectPage>
    {
        public void Configure(EntityTypeBuilder<FacebookConnectPage> builder)
        {
            builder.HasOne(x => x.Connect)
                .WithMany(x => x.Pages)
                .HasForeignKey(x => x.ConnectId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
