using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class VPartnerInfoConfiguration : IEntityTypeConfiguration<VPartnerInfo>
    {
        public void Configure(EntityTypeBuilder<VPartnerInfo> builder)
        {
            builder.ToView("VPartnerInfo");
            builder.HasNoKey();

        }
    }
}
