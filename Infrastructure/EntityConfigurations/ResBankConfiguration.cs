using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResBankConfiguration : IEntityTypeConfiguration<ResBank>
    {
        public void Configure(EntityTypeBuilder<ResBank> builder)
        {
            builder.Property(x => x.Name).IsRequired();
        }
    }
}
