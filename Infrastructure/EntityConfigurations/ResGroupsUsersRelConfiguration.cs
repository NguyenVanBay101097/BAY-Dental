using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResGroupsUsersRelConfiguration : IEntityTypeConfiguration<ResGroupsUsersRel>
    {
        public void Configure(EntityTypeBuilder<ResGroupsUsersRel> builder)
        {
            builder.HasKey(x => new { x.GroupId, x.UserId });
            builder.HasOne(x => x.Group)
                .WithMany(x => x.ResGroupsUsersRels)
                .HasForeignKey(x => x.GroupId);

            builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId);
        }
    }
}
