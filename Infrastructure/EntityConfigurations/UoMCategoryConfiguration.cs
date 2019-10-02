using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class UoMCategoryConfiguration : IEntityTypeConfiguration<UoMCategory>
    {
        public void Configure(EntityTypeBuilder<UoMCategory> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.MeasureType).IsRequired();
        }
    }
}
