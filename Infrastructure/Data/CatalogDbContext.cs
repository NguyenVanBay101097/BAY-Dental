using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.EntityConfigurations;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class CatalogDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IDbContext
    {
        private readonly AppTenant _tenant;
        private readonly ConnectionStrings _connectionStrings;
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ITenant<AppTenant> tenant,
            IOptions<ConnectionStrings> connectionStrings)
          : base(options)
        {
            _tenant = tenant?.Value;
            _connectionStrings = connectionStrings?.Value;
        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerSource> PartnerSources { get; set; }
        public DbSet<PartnerPartnerCategoryRel> PartnerPartnerCategoryRel { get; set; } //khong nen sua lai ten table nay
        public DbSet<Company> Companies { get; set; }
        public DbSet<UoM> UoMs { get; set; }
        public DbSet<UoMCategory> UoMCategories { get; set; }
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<SaleOrderLine> SaleOrderLines { get; set; }
        public DbSet<IRSequence> IRSequences { get; set; }
        public DbSet<AccountInvoice> AccountInvoices { get; set; }
        public DbSet<AccountInvoiceAccountMoveLineRel> AccountInvoiceAccountMoveLineRel { get; set; }
        public DbSet<AccountInvoiceLine> AccountInvoiceLines { get; set; }
        public DbSet<AccountMove> AccountMoves { get; set; }
        public DbSet<AccountMoveLine> AccountMoveLines { get; set; }
        public DbSet<AccountJournal> AccountJournals { get; set; }
        public DbSet<AccountAccount> AccountAccounts { get; set; }
        public DbSet<AccountAccountType> AccountAccountTypes { get; set; }
        public DbSet<AccountFullReconcile> AccountFullReconciles { get; set; }
        public DbSet<AccountInvoicePaymentRel> AccountInvoicePaymentRel { get; set; }
        public DbSet<AccountPartialReconcile> AccountPartialReconciles { get; set; }
        public DbSet<AccountPayment> AccountPayments { get; set; }
        public DbSet<AccountRegisterPayment> AccountRegisterPayments { get; set; }
        public DbSet<AccountRegisterPaymentInvoiceRel> AccountRegisterPaymentInvoiceRel { get; set; }
        public DbSet<PartnerCategory> PartnerCategories { get; set; }
        public DbSet<Tooth> Teeth { get; set; }
        public DbSet<ToothCategory> ToothCategories { get; set; }
        public DbSet<AccountInvoiceLineToothRel> AccountInvoiceLineToothRel { get; set; }
        public DbSet<Routing> Routings { get; set; }
        public DbSet<RoutingLine> RoutingLines { get; set; }
        public DbSet<DotKham> DotKhams { get; set; }
        public DbSet<DotKhamLine> DotKhamLines { get; set; }
        public DbSet<DotKhamLineOperation> DotKhamLineOperations { get; set; }
        public DbSet<ToaThuoc> ToaThuocs { get; set; }
        public DbSet<ToaThuocLine> ToaThuocLines { get; set; }
        public DbSet<SamplePrescription> SamplePrescriptions { get; set; }
        public DbSet<SamplePrescriptionLine> SamplePrescriptionLines { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<StockWarehouse> StockWarehouses { get; set; }
        public DbSet<StockLocation> StockLocations { get; set; }
        public DbSet<StockPickingType> StockPickingTypes { get; set; }
        public DbSet<StockPicking> StockPickings { get; set; }
        public DbSet<StockMove> StockMoves { get; set; }
        public DbSet<StockQuant> StockQuants { get; set; }
        public DbSet<StockQuantMoveRel> StockQuantMoveRel { get; set; }
        public DbSet<IRModelData> IRModelDatas { get; set; }
        public DbSet<LaboOrderLine> LaboOrderLines { get; set; }
        public DbSet<ProductCompanyRel> ProductCompanyRels { get; set; }
        public DbSet<ApplicationRoleFunction> ApplicationRoleFunctions { get; set; }
        public DbSet<ResGroup> ResGroups { get; set; }
        public DbSet<IRModel> IRModels { get; set; }
        public DbSet<IRModelAccess> IRModelAccesses { get; set; }
        public DbSet<ResGroupsUsersRel> ResGroupsUsersRels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeCategory> EmployeeCategories { get; set; }
        public DbSet<ProductStep> ProductSteps { get; set; }
        public DbSet<DotKhamStep> DotKhamSteps { get; set; }
        public DbSet<IrAttachment> IrAttachments { get; set; }
        public DbSet<PartnerHistoryRel> PartnerHistoryRels { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<ProductPricelist> ProductPricelists { get; set; }
        public DbSet<ProductPricelistItem> ProductPricelistItems { get; set; }
        public DbSet<IRRule> IRRules { get; set; }
        public DbSet<RuleGroupRel> RuleGroupRels { get; set; }
        public DbSet<SaleOrderLineInvoiceRel> SaleOrderLineInvoiceRels { get; set; }
        public DbSet<SaleOrderLineToothRel> SaleOrderLineToothRels { get; set; }
        public DbSet<LaboOrder> LaboOrders { get; set; }
        public DbSet<LaboOrderLineToothRel> LaboOrderLineToothRels { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<ResCompanyUsersRel> ResCompanyUsersRels { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }
        public DbSet<ResBank> ResBanks { get; set; }
        public DbSet<ResPartnerBank> ResPartnerBanks { get; set; }
        public DbSet<MailMessage> MailMessages { get; set; }
        public DbSet<MailTrackingValue> MailTrackingValues { get; set; }
        public DbSet<MailMessageResPartnerRel> MailMessageResPartnerRels { get; set; }
        public DbSet<MailNotification> MailNotifications { get; set; }
        public DbSet<AppointmentMailMessageRel> AppointmentMailMessageRels { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<CardCard> CardCards { get; set; }
        public DbSet<SaleSettings> SaleSettings { get; set; }
        public DbSet<CardHistory> CardHistories { get; set; }
        public DbSet<SaleCouponProgram> SaleCouponPrograms { get; set; }
        public DbSet<SaleCoupon> SaleCoupons { get; set; }
        public DbSet<PromotionProgram> PromotionPrograms { get; set; }
        public DbSet<PromotionRule> PromotionRules { get; set; }
        public DbSet<PromotionProgramCompanyRel> PromotionProgramCompanyRels { get; set; }
        public DbSet<PromotionRuleProductCategoryRel> PromotionRuleProductCategoryRels { get; set; }
        public DbSet<PromotionRuleProductRel> PromotionRuleProductRels { get; set; }
        public DbSet<ResGroupImpliedRel> ResGroupImpliedRels { get; set; }
        public DbSet<ResConfigSettings> ResConfigSettings { get; set; }
        public DbSet<IrModuleCategory> IrModuleCategories { get; set; }
        public DbSet<IrConfigParameter> IrConfigParameters { get; set; }
        public DbSet<SaleCouponProgramProductRel> SaleCouponProgramProductRels { get; set; }
        public DbSet<SaleOrderNoCodePromoProgram> SaleOrderNoCodePromoPrograms { get; set; }
        public DbSet<ZaloOAConfig> ZaloOAConfigs { get; set; }
        public DbSet<IRModelField> IRModelFields { get; set; }
        public DbSet<IRProperty> IRProperties { get; set; }
        public DbSet<MarketingCampaign> MarketingCampaigns { get; set; }
        public DbSet<MarketingCampaignActivity> MarketingCampaignActivities { get; set; }
        public DbSet<PartnerMapPSIDFacebookPage> PartnerMapPSIDFacebookPages { get; set; }
        public DbSet<MarketingTrace> MarketingTraces { get; set; }
        public DbSet<FacebookUserProfile> FacebookUserProfiles { get; set; }
        public DbSet<MarketingMessage> MarketingMessages { get; set; }
        public DbSet<MarketingMessageButton> MarketingMessageButtons { get; set; }
        public DbSet<FacebookMassMessaging> FacebookMassMessagings { get; set; }
        public DbSet<FacebookMessagingTrace> FacebookMessagingTraces { get; set; }
        public DbSet<FacebookTag> FacebookTags { get; set; }
        public DbSet<FacebookUserProfileTagRel> FacebookUserProfileTagRels { get; set; }
        public DbSet<MarketingCampaignActivityFacebookTagRel> MarketingCampaignActivityFacebookTagRels { get; set; }
        public DbSet<SaleOrderLineInvoice2Rel> SaleOrderLineInvoice2Rels { get; set; }
        public DbSet<AccountMovePaymentRel> AccountMovePaymentRels { get; set; }
        public DbSet<SaleOrderPaymentRel> SaleOrderPaymentRels { get; set; }
        public DbSet<ServiceCardType> ServiceCardTypes { get; set; }
        public DbSet<ServiceCardOrder> ServiceCardOrders { get; set; }
        public DbSet<ServiceCardOrderLine> ServiceCardOrderLines { get; set; }
        public DbSet<ServiceCardOrderLineInvoiceRel> ServiceCardOrderLineInvoiceRels { get; set; }
        public DbSet<ServiceCardCard> ServiceCardCards { get; set; }
        public DbSet<SaleOrderServiceCardCardRel> SaleOrderServiceCardCardRels { get; set; }
        public DbSet<ServiceCardOrderPaymentRel> ServiceCardOrderPaymentRels { get; set; }
        public DbSet<TCareCampaign> TCareCampaigns { get; set; }
        public DbSet<TCareRule> TCareRules { get; set; }
        public DbSet<TCareProperty> TCareProperties { get; set; }
        public DbSet<TCareMessaging> TCareMessagings { get; set; }
        public DbSet<StockHistory> StockHistories { get; set; }
        public DbSet<AccountInvoiceReport> AccountInvoiceReports { get; set; }
        public DbSet<ModelAccessReport> ModelAccessReports { get; set; }
        public DbSet<SaleReport> SaleReports { get; set; }
        public DbSet<FacebookConnect> FacebookConnects { get; set; }
        public DbSet<FacebookConnectPage> FacebookConnectPages { get; set; }
        //Facebook
        public DbSet<FacebookPage> FacebookPages { get; set; }

        public DbSet<FacebookScheduleAppointmentConfig> FacebookScheduleAppointmentConfigs { get; set; }
        public DbSet<PartnerImage> PartnerImages { get; set; }

        public DbSet<Commission> Commissions { get; set; }
        public DbSet<CommissionProductRule> CommissionProductRules { get; set; }
        public DbSet<SaleOrderLinePaymentRel> SaleOrderLinePaymentRels { get; set; }
        public DbSet<SaleOrderLinePartnerCommission> SaleOrderLinePartnerCommissions { get; set; }
        public DbSet<CommissionSettlement> CommissionSettlements { get; set; }


        //nguyen thang
        public DbSet<ProductUoMRel> ProductUoMRels { get; set; }
        public DbSet<AccountFinancialReport> AccountFinancialReports { get; set; }
        public DbSet<AccountFinancialReportAccountAccountTypeRel> AccountFinancialReportAccountAccountTypeRels { get; set; }

        public DbSet<LoaiThuChi> LoaiThuChis { get; set; }
        public DbSet<PhieuThuChi> PhieuThuChis { get; set; }
        public DbSet<TCareScenario> TCareScenarios { get; set; }

        public DbSet<ChamCong> ChamCongs { get; set; }
        public DbSet<SetupChamcong> setupChamcongs { get; set; }
        public DbSet<WorkEntryType> WorkEntryTypes { get; set; }
        public DbSet<HrPayrollStructure> HrPayrollStructures { get; set; }
        public DbSet<HrSalaryRule> HrSalaryRules { get; set; }
        public DbSet<HrSalaryRuleCategory> HrSalaryRuleCategories { get; set; }
        public DbSet<HrPayrollStructureType> HrPayrollStructureTypes { get; set; }
        public DbSet<ResourceCalendar> ResourceCalendars { get; set; }
        public DbSet<ResourceCalendarAttendance> ResourceCalendarAttendances { get; set; }
        public DbSet<ResourceCalendarLeaves> ResourceCalendarLeaves { get; set; }
        public DbSet<HrPayslip> HrPayslips { get; set; }
        public DbSet<HrPayslipLine> HrPayslipLines { get; set; }
        public DbSet<HrPayslipWorkedDays> HrPayslipWorkedDays { get; set; }
        public DbSet<HrPayslipRun> HrPayslipRuns { get; set; }
        public DbSet<HrSalaryConfig> HrSalaryConfigs { get; set; }
        public DbSet<PartnerTitle> PartnerTitles { get; set; }
        public DbSet<TCareMessage> TCareMessages { get; set; }
        public DbSet<TCareMessagingPartnerRel> TCareMessagingPartnerRels { get; set; }

        public DbSet<ServiceCardOrderPayment> ServiceCardOrderPayments { get; set; }
        public DbSet<TCareMessageTemplate> TCareMessageTemplates { get; set; }
        public DbSet<TCareConfig> TCareConfigs { get; set; }

        public DbSet<SalaryPayment> SalaryPayments { get; set; }
        public DbSet<DotKhamLineToothRel> DotKhamLineToothRels { get; set; }
        public DbSet<LaboOrderToothRel> LaboOrderToothRels { get; set; }
        public DbSet<LaboFinishLine> LaboFinishLines { get; set; }
        public DbSet<LaboBiteJoint> LaboBiteJoints { get; set; }
        public DbSet<LaboBridge> LaboBridges { get; set; }
        public DbSet<LaboOrderProductRel> laboOrderProductRels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new TCareMessageConfiguration());
            builder.ApplyConfiguration(new ProductCategoryConfiguration());
            builder.ApplyConfiguration(new CompanyConfiguration());
            builder.ApplyConfiguration(new PartnerConfiguration());
            builder.ApplyConfiguration(new PartnerSourceConfiguration());
            builder.ApplyConfiguration(new PartnerCategoryConfiguration());
            builder.ApplyConfiguration(new PartnerPartnerCategoryRelConfiguration());
            builder.ApplyConfiguration(new PartnerTitleConfiguration());
            builder.ApplyConfiguration(new UoMConfiguration());
            builder.ApplyConfiguration(new UoMCategoryConfiguration());
            builder.ApplyConfiguration(new SaleOrderConfiguration());
            builder.ApplyConfiguration(new SaleOrderLineConfiguration());
            builder.ApplyConfiguration(new IRSequenceConfiguration());
            builder.ApplyConfiguration(new AccountInvoiceConfiguration());
            builder.ApplyConfiguration(new AccountInvoiceAccountMoveLineRelConfiguration());
            builder.ApplyConfiguration(new AccountInvoiceLineConfiguration());
            builder.ApplyConfiguration(new AccountMoveConfiguration());
            builder.ApplyConfiguration(new AccountMoveLineConfiguration());
            builder.ApplyConfiguration(new AccountJournalConfiguration());
            builder.ApplyConfiguration(new AccountAccountConfiguration());
            builder.ApplyConfiguration(new AccountAccountTypeConfiguration());
            builder.ApplyConfiguration(new AccountFullReconcileConfiguration());
            builder.ApplyConfiguration(new AccountInvoicePaymentRelConfiguration());
            builder.ApplyConfiguration(new AccountPartialReconcileConfiguration());
            builder.ApplyConfiguration(new AccountPaymentConfiguration());
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new AccountRegisterPaymentInvoiceRelConfiguration());
            builder.ApplyConfiguration(new AccountRegisterPaymentConfiguration());
            builder.ApplyConfiguration(new ToothConfiguration());
            builder.ApplyConfiguration(new ToothCategoryConfiguration());
            builder.ApplyConfiguration(new AccountInvoiceLineToothRelConfiguration());
            builder.ApplyConfiguration(new RoutingConfiguration());
            builder.ApplyConfiguration(new RoutingLineConfiguration());
            builder.ApplyConfiguration(new DotKhamConfiguration());
            builder.ApplyConfiguration(new DotKhamLineConfiguration());
            builder.ApplyConfiguration(new DotKhamLineOperationConfiguration());
            builder.ApplyConfiguration(new ToaThuocConfiguration());
            builder.ApplyConfiguration(new ToaThuocLineConfiguration());
            builder.ApplyConfiguration(new SamplePrescriptionConfiguration());
            builder.ApplyConfiguration(new SamplePrescriptionLineConfiguration());
            builder.ApplyConfiguration(new AppointmentConfiguration());
            builder.ApplyConfiguration(new StockWarehouseConfiguration());
            builder.ApplyConfiguration(new StockLocationConfiguration());
            builder.ApplyConfiguration(new StockPickingTypeConfiguration());
            builder.ApplyConfiguration(new StockPickingConfiguration());
            builder.ApplyConfiguration(new StockMoveConfiguration());
            builder.ApplyConfiguration(new StockQuantConfiguration());
            builder.ApplyConfiguration(new StockQuantMoveRelConfiguration());
            builder.ApplyConfiguration(new IRModelDataConfiguration());
            builder.ApplyConfiguration(new LaboOrderLineConfiguration());
            builder.ApplyConfiguration(new ProductCompanyRelConfiguration());
            builder.ApplyConfiguration(new StockHistoryConfiguration());
            builder.ApplyConfiguration(new AccountInvoiceReportConfiguration());
            builder.ApplyConfiguration(new ApplicationRoleFunctionConfiguration());
            builder.ApplyConfiguration(new ResGroupConfiguration());
            builder.ApplyConfiguration(new IRModelConfiguration());
            builder.ApplyConfiguration(new IRModelAccessConfiguration());
            builder.ApplyConfiguration(new ResGroupsUsersRelConfiguration());
            builder.ApplyConfiguration(new EmployeeConfiguration());
            builder.ApplyConfiguration(new EmployeeCategoryConfiguration());
            builder.ApplyConfiguration(new ModelAccessReportConfiguration());
            builder.ApplyConfiguration(new ProductStepConfiguration());
            builder.ApplyConfiguration(new DotKhamStepConfiguration());
            builder.ApplyConfiguration(new IrAttachmentConfiguration());
            builder.ApplyConfiguration(new PartnerHistoryRelConfiguration());
            builder.ApplyConfiguration(new HistoryConfiguration());
            builder.ApplyConfiguration(new ProductPricelistConfiguration());
            builder.ApplyConfiguration(new ProductPricelistItemConfiguration());
            builder.ApplyConfiguration(new IRRuleConfiguration());
            builder.ApplyConfiguration(new RuleGroupRelConfiguration());
            builder.ApplyConfiguration(new SaleOrderLineInvoiceRelConfiguration());
            builder.ApplyConfiguration(new SaleOrderLineToothRelConfiguration());
            builder.ApplyConfiguration(new LaboOrderConfiguration());
            builder.ApplyConfiguration(new LaboOrderLineToothRelConfiguration());
            builder.ApplyConfiguration(new SaleReportConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderLineConfiguration());
            builder.ApplyConfiguration(new ResCompanyUsersRelConfiguration());
            builder.ApplyConfiguration(new UserRefreshTokenConfiguration());
            builder.ApplyConfiguration(new ProductPriceHistoryConfiguration());
            builder.ApplyConfiguration(new ResBankConfiguration());
            builder.ApplyConfiguration(new ResPartnerBankConfiguration());
            builder.ApplyConfiguration(new MailMessageConfiguration());
            builder.ApplyConfiguration(new MailTrackingValueConfiguration());
            builder.ApplyConfiguration(new MailMessageResPartnerRelConfiguration());
            builder.ApplyConfiguration(new MailNotificationConfiguration());
            builder.ApplyConfiguration(new AppointmentMailMessageRelConfiguration());
            builder.ApplyConfiguration(new CardTypeConfiguration());
            builder.ApplyConfiguration(new CardCardConfiguration());
            builder.ApplyConfiguration(new CardHistoryConfiguration());
            builder.ApplyConfiguration(new SaleCouponProgramConfiguration());
            builder.ApplyConfiguration(new SaleCouponConfiguration());
            builder.ApplyConfiguration(new PromotionProgramConfiguration());
            builder.ApplyConfiguration(new PromotionRuleConfiguration());
            builder.ApplyConfiguration(new PromotionProgramCompanyRelConfiguration());
            builder.ApplyConfiguration(new PromotionRuleProductCategoryRelConfiguration());
            builder.ApplyConfiguration(new PromotionRuleProductRelConfiguration());
            builder.ApplyConfiguration(new ResGroupImpliedRelConfiguration());
            builder.ApplyConfiguration(new ResConfigSettingsConfiguration());
            builder.ApplyConfiguration(new IrModuleCategoryConfiguration());
            builder.ApplyConfiguration(new IrConfigParameterConfiguration());
            builder.ApplyConfiguration(new SaleCouponProgramProductRelConfiguration());
            builder.ApplyConfiguration(new SaleOrderNoCodePromoProgramConfiguration());
            builder.ApplyConfiguration(new ZaloOAConfigConfiguration());
            builder.ApplyConfiguration(new IRModelFieldConfiguration());
            builder.ApplyConfiguration(new IRPropertyConfiguration());
            builder.ApplyConfiguration(new FacebookPageConfiguration());
            builder.ApplyConfiguration(new MarketingCampaignConfiguration());
            builder.ApplyConfiguration(new MarketingCampaignActivityConfiguration());
            builder.ApplyConfiguration(new PartnerMapPSIDFacebookPageConfiguration());
            builder.ApplyConfiguration(new MarketingTraceConfiguration());
            builder.ApplyConfiguration(new FacebookConnectConfiguration());
            builder.ApplyConfiguration(new FacebookConnectPageConfiguration());
            builder.ApplyConfiguration(new FacebookUserProfileConfiguration());
            builder.ApplyConfiguration(new MarketingMessageConfiguration());
            builder.ApplyConfiguration(new MarketingMessageButtonConfiguration());
            builder.ApplyConfiguration(new FacebookMassMessagingConfiguration());
            builder.ApplyConfiguration(new FacebookMessagingTraceConfiguration());
            builder.ApplyConfiguration(new FacebookTagConfiguration());
            builder.ApplyConfiguration(new FacebookUserProfileTagRelConfiguration());
            builder.ApplyConfiguration(new MarketingCampaignActivityFacebookTagRelConfiguration());
            builder.ApplyConfiguration(new FacebookScheduleAppointmentConfigConifuration());
            builder.ApplyConfiguration(new SaleOrderLineInvoice2RelConfiguration());
            builder.ApplyConfiguration(new AccountMovePaymentRelConfiguration());
            builder.ApplyConfiguration(new SaleOrderPaymentRelConfiguration());
            builder.ApplyConfiguration(new ServiceCardTypeConfiguration());
            builder.ApplyConfiguration(new ServiceCardOrderConfiguration());
            builder.ApplyConfiguration(new ServiceCardOrderLineConfiguration());
            builder.ApplyConfiguration(new ServiceCardOrderLineInvoiceRelConfiguration());
            builder.ApplyConfiguration(new ServiceCardCardConfiguration());
            builder.ApplyConfiguration(new SaleOrderServiceCardCardRelConfiguration());
            builder.ApplyConfiguration(new ServiceCardOrderPaymentRelConfiguration());
            builder.ApplyConfiguration(new ProductUoMRelConfiguration());
            builder.ApplyConfiguration(new TCareCampaignConfiguration());
            builder.ApplyConfiguration(new TCareRuleConfiguration());
            builder.ApplyConfiguration(new TCarePropertyConfiguration());
            builder.ApplyConfiguration(new TCareMessagingConfiguration());
            builder.ApplyConfiguration(new PartnerImageConfiguration());
            builder.ApplyConfiguration(new LoaiThuChiConfiguration());
            builder.ApplyConfiguration(new PhieuThuChiConfiguration());
            builder.ApplyConfiguration(new AccountFinancialReportAccountAccountTypeRelConfiguration());
            builder.ApplyConfiguration(new AccountFinancialReportConfiguration());
            builder.ApplyConfiguration(new CommissionConfiguration());
            builder.ApplyConfiguration(new CommissionProductRuleConfiguration());
            builder.ApplyConfiguration(new SaleOrderLinePaymentRelConfiguration());
            builder.ApplyConfiguration(new CommissionSettlementConfiguration());
            builder.ApplyConfiguration(new TCareScenarioConfiguration());
            builder.ApplyConfiguration(new ChamCongConfiguration());
            builder.ApplyConfiguration(new HrPayrollStructureConfiguration());
            builder.ApplyConfiguration(new HrSalaryRuleConfiguration());
            builder.ApplyConfiguration(new HrSalaryRuleCategoryConfiguration());
            builder.ApplyConfiguration(new HrPayrollStructureTypeConfiguration());
            builder.ApplyConfiguration(new ResourceCalendarConfiguration());
            builder.ApplyConfiguration(new ResourceCalendarAttendanceConfiguration());
            builder.ApplyConfiguration(new ResourceCalendarLeavesConfiguration());
            builder.ApplyConfiguration(new WorkEntryTypeConfiguration());
            builder.ApplyConfiguration(new HrPayslipConfiguration());
            builder.ApplyConfiguration(new HrPayslipLineConfiguration());
            builder.ApplyConfiguration(new HrPayslipWorkedDaysConfiguration());
            builder.ApplyConfiguration(new HrPayslipRunConfiguration());
            builder.ApplyConfiguration(new HrSalaryConfiguration());
            builder.ApplyConfiguration(new TCareMessagingPartnerRelConfiguration());
            builder.ApplyConfiguration(new ServiceCardOrderPaymentConfiguration());
            builder.ApplyConfiguration(new TCareMessageTemplateConfiguration());
            builder.ApplyConfiguration(new TCareConfigConfiguration());
            builder.ApplyConfiguration(new SalaryPaymentConfiguration());
            builder.ApplyConfiguration(new DotKhamLineToothRelConfiguration());
            builder.ApplyConfiguration(new LaboOrderToothRelConfiguration());
            builder.ApplyConfiguration(new LaboFinishLineConfiguration());
            builder.ApplyConfiguration(new LaboBiteJointConfiguration());
            builder.ApplyConfiguration(new LaboBridgeConfiguration());
            builder.ApplyConfiguration(new LaboOrderProductRelConfiguration());

            //var methodInfo = typeof(DbContext).GetRuntimeMethod(nameof(DatePart), new[] { typeof(string), typeof(DateTime) });
            //builder
            //    .HasDbFunction(methodInfo)
            //    .HasTranslation(args => new SqlFunctionExpression(nameof(DatePart), typeof(int?), new[]
            //            {
            //            new SqlFragmentExpression(args.ToArray()[0].ToString()),
            //            args.ToArray()[1]
            //            }));

            var methodInfo = typeof(CatalogDbContext).GetMethod(nameof(CatalogDbContext.DatePart));
            //builder
            //    .HasDbFunction(methodInfo)
            //    .HasTranslation(args => SqlFunctionExpression.Create("DatePart", new[]
            //            {
            //            new SqlFragmentExpression(args.ToArray()[0].ToString()),
            //            args.ToArray()[1]
            //            }, typeof(int?), null));

            builder.HasDbFunction(methodInfo, b => b.HasTranslation(e =>
            {
                var ea = e.ToArray();
                var args = new[]
                {
                new SqlFragmentExpression((ea[0] as SqlConstantExpression).Value.ToString()),
                ea[1]
            };
                return SqlFunctionExpression.Create(nameof(DatePart), args, typeof(int?), null);
            }));

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }

        public int? DatePart(string datePartArg, DateTime? date) => throw new Exception();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (_tenant != null)
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);

                    if (_tenant.Hostname != "localhost")
                        builder["Database"] = $"TMTDentalCatalogDb__{_tenant.Hostname}";

                    optionsBuilder.UseSqlServer(builder.ConnectionString);
                }
                else
                {
                    var defaultConnectionString = _connectionStrings.CatalogConnection;
                    optionsBuilder.UseSqlServer(defaultConnectionString);
                }
            }

            base.OnConfiguring(optionsBuilder);
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}
