using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PartnerAdvanceConfiguration : IEntityTypeConfiguration<PartnerAdvance>
    {
        public void Configure(EntityTypeBuilder<PartnerAdvance> builder)
        {
            builder.HasOne(x => x.Partner)
               .WithMany(x => x.PartnerAdvances)
               .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Journal)
               .WithMany()
               .HasForeignKey(x => x.JournalId);

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
