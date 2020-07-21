
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.EntityConfigurations
{
    class AccountFinancialReportAccountAccountTypeRelConfiguration : IEntityTypeConfiguration<AccountFinancialReportAccountAccountTypeRel>
    {
        public void Configure(EntityTypeBuilder<AccountFinancialReportAccountAccountTypeRel> builder)
        {
            builder.HasKey(x => new { x.AccountTypeId, x.FinancialReportId });

            builder.HasOne(x => x.AccountType)
                 .WithMany()
                 .HasForeignKey(x => x.AccountTypeId);

            builder.HasOne(x => x.FinancialReport)
                .WithMany(x => x.FinancialReportAccountTypeRels)
                .HasForeignKey(x => x.FinancialReportId);

        }
    }
}