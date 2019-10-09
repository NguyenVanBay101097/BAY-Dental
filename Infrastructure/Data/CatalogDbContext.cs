using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.EntityConfigurations;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
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

        public DbQuery<StockHistory> StockHistories { get; set; }
        public DbQuery<AccountInvoiceReport> AccountInvoiceReports { get; set; }
        public DbQuery<ModelAccessReport> ModelAccessReports { get; set; }
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new ProductCategoryConfiguration());
            builder.ApplyConfiguration(new CompanyConfiguration());
            builder.ApplyConfiguration(new PartnerConfiguration());
            builder.ApplyConfiguration(new PartnerCategoryConfiguration());
            builder.ApplyConfiguration(new PartnerPartnerCategoryRelConfiguration());
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

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (_tenant != null)
            //{
            //    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            //    //builder["Database"] = $"TMTDentalCatalogDb__{_tenant.Hostname}";
            //    builder["Database"] = $"{_tenant.Hostname}.TMTDentalCatalogDb";
            //    optionsBuilder.UseSqlServer(builder.ConnectionString);
            //}
            //else
            //{
            //    var defaultConnectionString = _connectionStrings.CatalogConnection;
            //    optionsBuilder.UseSqlServer(defaultConnectionString);
            //}

            var defaultConnectionString = _connectionStrings.CatalogConnection;
            optionsBuilder.UseSqlServer(defaultConnectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
