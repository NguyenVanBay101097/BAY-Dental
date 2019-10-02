using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.Name)
                .IsRequired();

        builder.HasOne(x => x.Partner)
            .WithMany()
            .HasForeignKey(x => x.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Company)
           .WithMany()
           .HasForeignKey(x => x.CompanyId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}