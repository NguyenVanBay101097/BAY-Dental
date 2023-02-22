
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.EntityConfigurations
{
    class AccountFinancialRevenueReportAccountAccountTypeRelConfiguration : IEntityTypeConfiguration<AccountFinancialRevenueReportAccountAccountTypeRel>
    {
        public void Configure(EntityTypeBuilder<AccountFinancialRevenueReportAccountAccountTypeRel> builder)
        {
            builder.HasKey(x => new { x.AccountTypeId, x.FinancialRevenueReportId });

            builder.HasOne(x => x.AccountType)
                 .WithMany()
                 .HasForeignKey(x => x.AccountTypeId);

            builder.HasOne(x => x.FinancialRevenueReport)
                .WithMany(x => x.FinancialRevenueReportAccountTypeRels)
                .HasForeignKey(x => x.FinancialRevenueReportId);

        }
    }
}