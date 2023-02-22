using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MailMessageSubtypeConfiguration : IEntityTypeConfiguration<MailMessageSubtype>
    {
        public void Configure(EntityTypeBuilder<MailMessageSubtype> builder)
        {
            builder.Property(x => x.Name)
               .IsRequired();
        }
    }
}
