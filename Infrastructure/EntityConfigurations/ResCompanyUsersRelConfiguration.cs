using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResCompanyUsersRelConfiguration : IEntityTypeConfiguration<ResCompanyUsersRel>
    {
        public void Configure(EntityTypeBuilder<ResCompanyUsersRel> builder)
        {
            builder.HasKey(x => new { x.CompanyId, x.UserId });
            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.User)
               .WithMany(x => x.ResCompanyUsersRels)
               .HasForeignKey(x => x.UserId);
        }
    }
}
