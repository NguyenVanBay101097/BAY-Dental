using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ServiceCardOrderPartnerRelConfiguration : IEntityTypeConfiguration<ServiceCardOrderPartnerRel>
    {
        public void Configure(EntityTypeBuilder<ServiceCardOrderPartnerRel> builder)
        {
            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.CardOrder)
           .WithMany(x => x.PartnerRels)
           .HasForeignKey(x => x.CardOrderId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
