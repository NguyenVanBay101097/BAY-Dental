using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MemberLevelConfiguration : IEntityTypeConfiguration<MemberLevel>
    {
        public void Configure(EntityTypeBuilder<MemberLevel> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Point).IsRequired();
        }
    }
}
