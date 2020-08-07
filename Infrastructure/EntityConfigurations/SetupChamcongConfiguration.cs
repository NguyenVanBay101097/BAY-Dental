using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SetupChamcongConfiguration : IEntityTypeConfiguration<SetupChamcong>
    {
        public void Configure(EntityTypeBuilder<SetupChamcong> builder)
        {
            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
