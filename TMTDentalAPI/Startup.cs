using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Middlewares;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Caches;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using TMTDentalAPI.Hubs;
using TMTDentalAPI.Middlewares;
using TMTDentalAPI.Services;
using Umbraco.Web.Mapping;
using TMTDentalAPI.JobFilters;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Umbraco.Web.Models.ContentEditing;
using System.Reflection;
using System.IO;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
using Microsoft.Net.Http.Headers;
using ApplicationCore.Utilities;
using TMTDentalAPI.ActionFilters;
using TMTDentalAPI.OdataControllers;
using Serilog;
using MediatR;
using Infrastructure.HangfireJobService;

namespace TMTDentalAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.Configuration.UseBatches();
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();
            services.AddDbContext<TenantDbContext>(c => c.UseSqlServer(Configuration.GetConnectionString("TenantConnection")));

            services.AddEntityFrameworkSqlServer().AddDbContext<CatalogDbContext>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.Configure<SendGridConfig>(Configuration.GetSection("SendGrid"));

            services.Configure<ZaloAuthConfig>(Configuration.GetSection("ZaloAuthConfig"));
            // Register the ConfigurationBuilder instance of FacebookAuthSettings
            services.Configure<FacebookAuthSettings>(Configuration.GetSection(nameof(FacebookAuthSettings)));
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddSingleton(new TCareCampaignJobService(Configuration));
            services.AddSingleton(new TCareMessageJobService(Configuration));
            services.AddSingleton(new TCareMessagingJobService(Configuration));
            services.AddSingleton(new FacebookWebhookJobService(Configuration));

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequireLowercase = false;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Lockout = new LockoutOptions
                {
                    DefaultLockoutTimeSpan = new TimeSpan(),
                    MaxFailedAccessAttempts = 5,
                };
            })
               .AddEntityFrameworkStores<CatalogDbContext>()
               .AddDefaultTokenProviders();

            // configure jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                        {
                            var tenant = services.BuildServiceProvider().GetService<AppTenant>();
                            List<SecurityKey> keys = new List<SecurityKey>();
                            var key = Encoding.ASCII.GetBytes(appSettings.Secret + (tenant != null ? tenant.Hostname : ""));
                            var signingKey = new SymmetricSecurityKey(key);
                            keys.Add(signingKey);
                            return keys;
                        }
                    };
                });

            #region -- Add Singlton, Scope of the service
            services.AddDbContext<IDbContext, CatalogDbContext>();
            services.AddScoped<IDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IPartnerCategoryService, PartnerCategoryService>();

            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IIRSequenceService, IRSequenceService>();
            services.AddScoped<IQuotationService, QuotationService>();
            services.AddScoped<IQuotationLineService, QuotationLineService>();
            services.AddScoped<IPaymentQuotationService, PaymentQuotationService>();

            services.AddScoped<IResourceCalendarService, ResourceCalendarService>();
            services.AddScoped<IResourceCalendarAttendanceService, ResourceCalendarAttendanceService>();

            services.AddScoped<ISaleOrderService, SaleOrderService>();
            services.AddScoped<ISaleOrderLineService, SaleOrderLineService>();
            services.AddScoped<IAccountInvoiceService, AccountInvoiceService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IAccountJournalService, AccountJournalService>();
            services.AddScoped<IAccountAccountService, AccountAccountService>();
            services.AddScoped<IAccountAccountTypeService, AccountAccountTypeService>();
            services.AddScoped<IAccountInvoiceLineService, AccountInvoiceLineService>();
            services.AddScoped<IAccountMoveService, AccountMoveService>();
            services.AddScoped<IAccountMoveLineService, AccountMoveLineService>();
            services.AddScoped<IAccountRegisterPaymentService, AccountRegisterPaymentService>();
            services.AddScoped<IAccountPaymentService, AccountPaymentService>();
            services.AddScoped<IAccountFullReconcileService, AccountFullReconcileService>();
            services.AddScoped<IAccountPartialReconcileService, AccountPartialReconcileService>();
            services.AddScoped<IUoMService, UoMService>();
            services.AddScoped<IUoMCategoryService, UoMCategoryService>();
            services.AddScoped<IToothCategoryService, ToothCategoryService>();
            services.AddScoped<IToothService, ToothService>();
            services.AddScoped<IRoutingService, RoutingService>();
            services.AddScoped<IDotKhamService, DotKhamService>();
            services.AddScoped<IDotKhamLineService, DotKhamLineService>();
            services.AddScoped<IDotKhamLineOperationService, DotKhamLineOperationService>();
            services.AddScoped<IToaThuocService, ToaThuocService>();
            services.AddScoped<ISamplePrescriptionService, SamplePrescriptionService>();
            services.AddScoped<ISamplePrescriptionLineService, SamplePrescriptionLineService>();
            services.AddScoped<IStockWarehouseService, StockWarehouseService>();
            services.AddScoped<IStockLocationService, StockLocationService>();
            services.AddScoped<IStockPickingTypeService, StockPickingTypeService>();
            services.AddScoped<IStockPickingService, StockPickingService>();
            services.AddScoped<IStockMoveService, StockMoveService>();
            services.AddScoped<IStockQuantService, StockQuantService>();
            services.AddScoped<IIRModelDataService, IRModelDataService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IToaThuocLineService, ToaThuocLineService>();
            services.AddScoped<ILaboOrderLineService, LaboOrderLineService>();
            services.AddScoped<IAccountCommonPartnerReportService, AccountCommonPartnerReportService>();
            services.AddScoped<IStockReportService, StockReportService>();
            services.AddScoped<IAccountInvoiceReportService, AccountInvoiceReportService>();
            services.AddScoped<IApplicationRoleFunctionService, ApplicationRoleFunctionService>();
            services.AddScoped<IUoMCategoryService, UoMCategoryService>();
            services.AddScoped<IResGroupService, ResGroupService>();
            services.AddScoped<IIRModelAccessService, IRModelAccessService>();
            services.AddScoped<IIRModelService, IRModelService>();
            services.AddScoped<IEmployeeCategoryService, EmployeeCategoryService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IProductStepService, ProductStepService>();
            services.AddScoped<IDotKhamStepService, DotKhamStepService>();
            services.AddScoped<IIrAttachmentRepository, IrAttachmentRepository>();
            services.AddScoped<IIrAttachmentService, IrAttachmentService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IIRRuleService, IRRuleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductPricelistService, ProductPricelistService>();
            services.AddScoped<IProductPricelistItemService, ProductPricelistItemService>();
            services.AddScoped<ILaboOrderService, LaboOrderService>();
            services.AddScoped<IPurchaseOrderLineService, PurchaseOrderLineService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ISaleReportService, SaleReportService>();
            services.AddScoped<IRealRevenueReportService, RealRevenueReportService>();
            services.AddScoped<IMailMessageService, MailMessageService>();
            services.AddScoped<INotificationHubService, NotificationHubService>();
            services.AddScoped<IAppointmentHubService, AppointmentHubService>();
            services.AddScoped<IResBankService, ResBankService>();
            services.AddScoped<IResPartnerBankService, ResPartnerBankService>();
            services.AddScoped<IProductAppointmentRelService, ProductAppointmentRelService>();
            services.AddScoped<IJournalReportService, JournalReportService>();
            services.AddScoped<IServiceCardTypeService, ServiceCardTypeService>();
            services.AddScoped<IServiceCardOrderService, ServiceCardOrderService>();
            services.AddScoped<IServiceCardOrderLineService, ServiceCardOrderLineService>();
            services.AddScoped<IServiceCardCardService, ServiceCardCardService>();
            services.AddScoped<ISaleOrderServiceCardCardRelService, SaleOrderServiceCardCardRelService>();
            services.AddScoped<IPartnerImageService, PartnerImageService>();

            services.AddScoped<IReportFinancialService, ReportFinancialService>();
            services.AddScoped<ICardTypeService, CardTypeService>();
            services.AddScoped<ICardCardService, CardCardService>();
            services.AddScoped<ISaleSettingsService, SaleSettingsService>();
            services.AddScoped<ICardHistoryService, CardHistoryService>();
            services.AddScoped<ISaleCouponProgramService, SaleCouponProgramService>();
            services.AddScoped<ISaleCouponService, SaleCouponService>();
            services.AddScoped<IPromotionProgramService, PromotionProgramService>();
            services.AddScoped<IPromotionRuleService, PromotionRuleService>();
            services.AddScoped<IResConfigSettingsService, ResConfigSettingsService>();
            services.AddScoped<IIrModuleCategoryService, IrModuleCategoryService>();
            services.AddScoped<IImportSampleDataService, ImportSampleDataService>();
            services.AddScoped<IZaloOAConfigService, ZaloOAConfigService>();
            services.AddScoped(typeof(BirthdayMessageJobService));
            services.AddScoped<IRevenueReportService, RevenueReportService>();
            services.AddScoped<IIRPropertyService, IRPropertyService>();
            services.AddScoped<IIRModelFieldService, IRModelFieldService>();
            services.AddScoped<IProductPriceHistoryService, ProductPriceHistoryService>();
            services.AddScoped<IFacebookPageService, FacebookPageService>();
            services.AddScoped<IMarketingCampaignService, MarketingCampaignService>();
            services.AddScoped<IMarketingCampaignActivityService, MarketingCampaignActivityService>();
            services.AddScoped<IMarketingCampaignActivityJobService, MarketingCampaignActivityJobService>();
            services.AddScoped<IMarketingTraceService, MarketingTraceService>();
            services.AddScoped<IMarketingMessageService, MarketingMessageService>();
            services.AddScoped<IPartnerMapPSIDFacebookPageService, PartnerMapPSIDFacebookPageService>();
            services.AddScoped<IFacebookConnectService, FacebookConnectService>();
            services.AddScoped<IIrConfigParameterService, IrConfigParameterService>();
            services.AddScoped<IFacebookConnectPageService, FacebookConnectPageService>();
            services.AddScoped<IFacebookUserProfileService, FacebookUserProfileService>();
            services.AddScoped<IFacebookMassMessagingService, FacebookMassMessagingService>();
            services.AddScoped<IFacebookMessagingTraceService, FacebookMessagingTraceService>();
            services.AddScoped<IFacebookMessageSender, FacebookMessageSender>();
            services.AddScoped<IFacebookMassMessageJobService, FacebookMassMessageJobService>();
            services.AddScoped<IFacebookTagService, FacebookTagService>();
            services.AddScoped<IFacebookScheduleAppointmentConfigService, FacebookScheduleAppointmentConfigService>();
            services.AddScoped<ITCareCampaignService, TCareCampaignService>();
            services.AddScoped<ITCareRuleService, TCareRuleService>();
            services.AddScoped<ITCarePropertyService, TCarePropertyService>();
            services.AddScoped<ITCareMessagingService, TCareMessagingService>();
            services.AddScoped<ITCareJobService, TCareJobService>();
            services.AddScoped<ITCareCampaignJobService, TCareCampaignJobService>();
            services.AddScoped<ITCareScenarioJobService, TCareScenarioJobService>();
            services.AddScoped<IPartnerSourceService, PartnerSourceService>();
            services.AddScoped<ILoaiThuChiService, LoaiThuChiService>();
            services.AddScoped<IPhieuThuChiService, PhieuThuChiService>();
            services.AddScoped<IAccountFinancialReportService, AccountFinancialReportService>();
            services.AddScoped<IReportJournalService, ReportJournalService>();
            services.AddScoped<IAccountReportGeneralLedgerService, AccountReportGeneralLedgerService>();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<ICommissionProductRuleService, CommissionProductRuleService>();
            services.AddScoped<ISaleOrderLinePartnerCommissionService, SaleOrderLinePartnerCommissionService>();
            services.AddScoped<ISaleOrderLinePaymentRelService, SaleOrderLinePaymentRelService>();
            services.AddScoped<ICommissionReportService, CommissionReportService>();
            services.AddScoped<ICommissionSettlementService, CommissionSettlementService>();
            services.AddScoped<IPartnerTitleService, PartnerTitleService>();
            services.AddScoped<ITCareScenarioService, TCareScenarioService>();
            services.AddScoped<IFacebookWebhookJobService, FacebookWebhookJobService>();
            services.AddScoped<ITCareReportService, TCareReportService>();
            services.AddScoped<IChamCongService, ChamCongService>();
            services.AddScoped<ISetupChamcongService, SetupChamcongService>();
            services.AddScoped<IWorkEntryTypeService, WorkEntryTypeService>();
            services.AddScoped<IHrPayrollStructureService, HrPayrollStructureService>();
            services.AddScoped<IHrPayrollStructureTypeService, HrPayrollStructureTypeService>();
            services.AddScoped<IHrSalaryRuleService, HrSalaryRuleService>();
            services.AddScoped<IResourceCalendarService, ResourceCalendarService>();
            services.AddScoped<IResourceCalendarAttendanceService, ResourceCalendarAttendanceService>();
            services.AddScoped<IHrPayslipService, HrPayslipService>();
            services.AddScoped<IHrPayslipLineService, HrPayslipLineService>();
            services.AddScoped<IHrPayslipWorkedDayService, HrPayslipWorkedDayService>();
            services.AddScoped<IHrPayslipRunService, HrPayslipRunService>();
            services.AddScoped<IHrSalaryConfigService, HrSalaryConfigService>();
            services.AddScoped<IResourceCalendarLeaveService, ResourceCalendarLeaveService>();
            services.AddScoped<IPartnerPartnerCategoryRelService, PartnerPartnerCategoryRelService>();
            services.AddScoped<ISalaryPaymentService, SalaryPaymentService>();
            services.AddScoped<ILaboBiteJointService, LaboBiteJointService>();
            services.AddScoped<ILaboBridgeService, LaboBridgeService>();
            services.AddScoped<ILaboFinishLineService, LaboFinishLineService>();
            services.AddScoped<IMedicineOrderService, MedicineOrderService>();
            services.AddScoped<IMedicineOrderLineService, MedicineOrderLineService>();
            services.AddScoped<IProductRequestService, ProductRequestService>();
            services.AddScoped<IProductRequestLineService, ProductRequestLineService>();
            services.AddScoped<IProductBomService, ProductBomService>();

            services.AddScoped<ITCareMessageTemplateService, TCareMessageTemplateService>();
            services.AddScoped<ITCareConfigService, TCareConfigService>();
            services.AddScoped<ITCareMessageService, TCareMessageService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IStockInventoryService, StockInventoryService>();
            services.AddScoped<IStockInventoryLineService, StockInventoryLineService>();
            //services.AddScoped<IVFundBookService, VFundBookService>();
            services.AddScoped<IPartnerOldNewReportService, PartnerOldNewReportService>();
            services.AddScoped<ICashBookService, CashBookService>();
            services.AddScoped<ISurveyQuestionService, SurveyQuestionService>();
            services.AddScoped<ISurveyAssignmentService, SurveyAssignmentService>();
            services.AddScoped<ISurveyCallContentService, SurveyCallContentService>();
            services.AddScoped<ISurveyUserInputService, SurveyUserInputService>();
            services.AddScoped<ISurveyTagService, SurveyTagService>();
            services.AddScoped<ISaleOrderLineProductRequestedService, SaleOrderLineProductRequestedService>();

            services.AddScoped<ISurveyUserInputLineService, SurveyUserInputLineService>();
            services.AddScoped<IStockInventoryCriteriaService, StockInventoryCriteriaService>();
            services.AddScoped<IPartnerAdvanceService, PartnerAdvanceService>();

            services.AddScoped<ISaleOrderPaymentService, SaleOrderPaymentService>();
            services.AddScoped<ISaleOrderPaymentHistoryLineService, SaleOrderPaymentHistoryLineService>();
            services.AddScoped<ISaleOrderPaymentJournalLineService, SaleOrderPaymentJournalLineService>();
            services.AddScoped<IApplicationRoleService, ApplicationRoleService>();

            services.AddScoped<ISaleOrderPromotionService, SaleOrderPromotionService>();
            services.AddScoped<ISaleOrderPromotionLineService, SaleOrderPromotionLineService>();
            services.AddScoped<IPrintPaperSizeService, PrintPaperSizeService>();
            services.AddScoped<IConfigPrintService, ConfigPrintService>();
            services.AddScoped<IViewToStringRenderService, ViewToStringRenderService>();

            services.AddScoped<IQuotationPromotionService, QuotationPromotionService>();
            services.AddScoped<IQuotationPromotionLineService, QuotationPromotionLineService>();
            services.AddScoped<IAccountFinancialRevenueReportService, AccountFinancialRevenueReportService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IReportCustomerDebtService, ReportCustomerDebtService>();

            services.AddScoped<ISmsAccountService, SmsAccountService>();
            services.AddScoped<ISmsComposerService, SmsComposerService>();
            services.AddScoped<ISmsBirthdayAutomationConfigService, SmsBirthdayAutomationConfigService>();
            services.AddScoped<ISmsAppointmentAutomationConfigService, SmsAppointmentAutomationConfigService>();
            services.AddScoped<ISmsCareAfterOrderAutomationConfigService, SmsCareAfterOrderAutomationConfigService>();
            services.AddScoped<ISmsThanksCustomerAutomationConfigService, SmsThanksCustomerAutomationConfigService>();
            services.AddScoped<ISmsMessageDetailService, SmsMessageDetailService>();
            services.AddScoped<ISmsTemplateService, SmsTemplateService>();
            services.AddScoped<ISmsJobService, SmsJobService>();
            services.AddScoped<ISmsMessageJobService, SmsMessageJobService>();
            services.AddScoped<ISmsMessageService, SmsMessageService>();
            services.AddScoped<ISmsCampaignService, SmsCampaignService>();
            services.AddScoped<ICustomerReceiptService, CustomerReceiptService>();
            services.AddScoped<IOverviewReportService, OverviewReportService>();

            services.AddMemoryCache();

            services.AddSingleton<IMyCache, MyMemoryCache>();
            services.AddSingleton<IMailSender, SendGridSender>();


            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            #endregion

            services.AddScoped<IToothDiagnosisService, ToothDiagnosisService>();
            services.AddScoped<IAdvisoryService, AdvisoryService>();
            services.AddScoped<IMemberLevelService, MemberLevelService>();
            services.AddScoped<IExportExcelService, ExportExcelService>();

            #region -- Add profile mapper of entity
            Action<IMapperConfigurationExpression> mapperConfigExp = mc =>
            {
                mc.AddProfile(new ProductCategoryProfile());
                mc.AddProfile(new ProductProfile());
                mc.AddProfile(new IRSequenceProfile());
                mc.AddProfile(new UoMProfile());
                mc.AddProfile(new UoMCategoryProfile());
                mc.AddProfile(new PartnerProfile());
                mc.AddProfile(new PartnerCategoryProfile());
                mc.AddProfile(new PartnerSourceProfile());
                mc.AddProfile(new PartnerTitleProfile());
                mc.AddProfile(new ProvinceProfile());
                mc.AddProfile(new DistrictProfile());
                mc.AddProfile(new WardProfile());
                mc.AddProfile(new SaleOrderProfile());
                mc.AddProfile(new SaleOrderLineProfile());
                mc.AddProfile(new ApplicationUserProfile());
                mc.AddProfile(new AccountAccountProfile());
                mc.AddProfile(new AccountAccountTypeProfile());
                mc.AddProfile(new AccountJournalProfile());
                mc.AddProfile(new AccountInvoiceProfile());
                mc.AddProfile(new AccountInvoiceLineProfile());
                mc.AddProfile(new AccountRegisterPaymentProfile());
                mc.AddProfile(new PartnerPartnerCategoryRelProfile());
                mc.AddProfile(new ToothCategoryProfile());
                mc.AddProfile(new ToothProfile());
                mc.AddProfile(new RoutingProfile());
                mc.AddProfile(new RoutingLineProfile());
                mc.AddProfile(new DotKhamProfile());
                mc.AddProfile(new DotKhamLineProfile());
                mc.AddProfile(new DotKhamLineOperationProfile());
                mc.AddProfile(new ToaThuocProfile());
                mc.AddProfile(new ToaThuocLineProfile());
                mc.AddProfile(new SamplePrescriptionProfile());
                mc.AddProfile(new SamplePrescriptionLineProfile());
                mc.AddProfile(new StockPickingTypeProfile());
                mc.AddProfile(new StockPickingProfile());
                mc.AddProfile(new StockMoveProfile());
                mc.AddProfile(new AppointmentProfile());
                mc.AddProfile(new LaboOrderLineProfile());
                mc.AddProfile(new CompanyProfile());
                mc.AddProfile(new ApplicationRoleProfile());
                mc.AddProfile(new IRModelAccessProfile());
                mc.AddProfile(new ResGroupProfile());
                mc.AddProfile(new EmployeeCategoryProfile());
                mc.AddProfile(new EmployeeProfile());
                mc.AddProfile(new ProductStepProfile());
                mc.AddProfile(new DotKhamStepProfile());
                mc.AddProfile(new HistoriesProfile());
                mc.AddProfile(new PartnerHistoryRelProfile());
                mc.AddProfile(new IRModelProfile());
                mc.AddProfile(new AccountPaymentProfile());
                mc.AddProfile(new ProductPricelistProfile());
                mc.AddProfile(new IRRuleProfile());
                mc.AddProfile(new LaboOrderProfile());
                mc.AddProfile(new PurchaseOrderProfile());
                mc.AddProfile(new PurchaseOrderLineProfile());
                mc.AddProfile(new MailMessageProfile());
                mc.AddProfile(new ResBankProfile());
                mc.AddProfile(new ResPartnerBankProfile());
                mc.AddProfile(new CardTypeProfile());
                mc.AddProfile(new CardCardProfile());
                mc.AddProfile(new SaleSettingsProfile());
                mc.AddProfile(new SaleCouponProgramProfile());
                mc.AddProfile(new SaleCouponProfile());
                mc.AddProfile(new PromotionProgramProfile());
                mc.AddProfile(new PromotionRuleProfile());
                mc.AddProfile(new ResConfigSettingsProfile());
                mc.AddProfile(new AccountMoveLineProfile());
                mc.AddProfile(new ZaloOAConfigProfile());
                mc.AddProfile(new FacebookPageProfile());
                mc.AddProfile(new PartnerMapPSIDFacebookPageProfile());
                mc.AddProfile(new MarketingCampaignProfile());
                mc.AddProfile(new MarketingCampaignActivityProfile());
                mc.AddProfile(new FacebookConnectPageProfile());
                mc.AddProfile(new FacebookConnectProfile());
                mc.AddProfile(new MarketingMessageButtonProfile());
                mc.AddProfile(new FacebookUserProfiles());
                mc.AddProfile(new FacebookMassMessagingProfile());
                mc.AddProfile(new FacebookTagProfile());
                mc.AddProfile(new FacebookScheduleAppointmentConfigProfile());
                mc.AddProfile(new AccountMoveProfile());
                mc.AddProfile(new ServiceCardTypeProfile());
                mc.AddProfile(new ServiceCardOrderProfile());
                mc.AddProfile(new ServiceCardCardProfile());
                mc.AddProfile(new SaleOrderServiceCardCardRelProfile());
                mc.AddProfile(new ServiceCardOrderLineProfile());
                mc.AddProfile(new ServiceCardOrderPaymentProfile());
                mc.AddProfile(new TCareCampaignProfile());
                mc.AddProfile(new TCareRuleProfile());
                mc.AddProfile(new TCareMessagingProfile());
                mc.AddProfile(new IrAttachmentProfile());
                mc.AddProfile(new PartnerImageProfile());
                mc.AddProfile(new LoaiThuChiProfile());
                mc.AddProfile(new PhieuThuChiProfile());
                mc.AddProfile(new AccountFinancialReportProfile());
                mc.AddProfile(new CommissionProfile());
                mc.AddProfile(new CommissionProductRuleProfile());
                mc.AddProfile(new SaleOrderLinePaymentRelProfile());
                mc.AddProfile(new TCareScenarioProfile());
                mc.AddProfile(new ChamCongProfile());
                mc.AddProfile(new SetupChamcongProfile());
                mc.AddProfile(new WorkEntryTypeProfile());
                mc.AddProfile(new HrPayrollStructureProfile());
                mc.AddProfile(new HrSalaryRuleProfile());
                mc.AddProfile(new HrPayrollStructureTypeProfile());
                mc.AddProfile(new ResourceCalendarAttendanceProfile());
                mc.AddProfile(new ResourceCalendarProfile());
                mc.AddProfile(new HrPayslipProfile());
                mc.AddProfile(new HrPayslipLineProfile());
                mc.AddProfile(new HrPayslipWorkedDayProfile());
                mc.AddProfile(new HrPayslipRunProfile());
                mc.AddProfile(new HrSalaryConfigProfile());
                mc.AddProfile(new ResourceCalendarLeaveProfile());
                mc.AddProfile(new TCareMessageTemplateProfile());
                mc.AddProfile(new TCareConfigProfile());
                mc.AddProfile(new SalaryPaymentProfile());
                mc.AddProfile(new LaboFinishLineProfile());
                mc.AddProfile(new LaboBiteJointProfile());
                mc.AddProfile(new LaboBridgeProfile());
                mc.AddProfile(new MedicineOrderProfile());
                mc.AddProfile(new MedicineOrderLineProfile());
                mc.AddProfile(new VFundBookProfile());
                mc.AddProfile(new PartnerOldNewReportProfile());
                mc.AddProfile(new SurveyQuestionProfile());
                mc.AddProfile(new SurveyAnswerProfile());
                mc.AddProfile(new SurveyAssignmentProfile());
                mc.AddProfile(new SurveyCallContentProfile());
                mc.AddProfile(new SurveyUserInputProfile());
                mc.AddProfile(new SurveyUserInputLineProfile());
                mc.AddProfile(new SurveyTagProfile());
                mc.AddProfile(new ProductBomProfile());
                mc.AddProfile(new ProductRequestProfile());
                mc.AddProfile(new ProductRequestLineProfile());
                mc.AddProfile(new SaleOrderLineProductRequestedProfile());
                mc.AddProfile(new StockInventoryProfile());
                mc.AddProfile(new StockInventoryLineProfile());
                mc.AddProfile(new StockLocationProfile());
                mc.AddProfile(new StockInventoryCriteriaProfile());
                mc.AddProfile(new ProductStockInventoryCriteriaRelProfile());
                mc.AddProfile(new QuotationProfile());
                mc.AddProfile(new QuotationLineProfile());
                mc.AddProfile(new PaymentQuotationProfile());
                mc.AddProfile(new ToothDiagnosisProfile());
                mc.AddProfile(new AdvisoryProfile());
                mc.AddProfile(new PartnerAdvanceProfile());
                mc.AddProfile(new SaleOrderPaymentProfile());
                mc.AddProfile(new SaleOrderPaymentHistoryLineProfile());
                mc.AddProfile(new SaleOrderPaymentJournalLineProfile());
                mc.AddProfile(new SaleOrderPromotionProfile());
                mc.AddProfile(new SaleOrderPromotionLineProfile());
                mc.AddProfile(new QuotationPromotionProfile());
                mc.AddProfile(new QuotationPromotionLineProfile());
                mc.AddProfile(new ConfigPrintProfile());
                mc.AddProfile(new PrintPaperSizeProfile());
                mc.AddProfile(new MemberLevelProfile());

                mc.AddProfile(new SmsAccountProfile());
                mc.AddProfile(new SmsTemplateProfile());
                mc.AddProfile(new SmsThanksCustomerAutomationConfigProfile());
                mc.AddProfile(new SmsCareAfterOrderAutomationConfigProfile());
                mc.AddProfile(new SmsAppointmentAutomationConfigProfile());
                mc.AddProfile(new SmsBirthdayAutomationConfigProfile());
                mc.AddProfile(new SmsMessageDetailProfile());
                mc.AddProfile(new SmsComposerProfile());
                mc.AddProfile(new SmsCampaignProfile());
                mc.AddProfile(new SmsMessageProfile());
                mc.AddProfile(new AgentProfile());
                mc.AddProfile(new CustomerReceiptProfile());
            };

            #endregion

            var mappingConfig = new MapperConfiguration(mapperConfigExp);
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            services.AddCors();
            services.AddMemoryCache();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            //services.AddHangfireServer(option =>
            //{
            //    option.FilterProvider = new ServerFilterProvider();
            //});

            services.AddHangfireServer();

            GlobalJobFilters.Filters.Add(new LogEverythingAttribute());
            GlobalJobFilters.Filters.Add(new ServerTenantFilter());
            GlobalJobFilters.Filters.Add(new ClientTenantFilter());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });

            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
                options.Filters.Add(typeof(CheckTenantExpiredActionFilter));
            }).AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

            });


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddHttpContextAccessor();

            services.AddSignalR();

            services.AddOData();

            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<OutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in options.InputFormatters.OfType<InputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CatalogDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseMultitenancy<AppTenant>();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseCors("AllowAll");
            app.UseCors(
                  //options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                  //options => options.WithOrigins("https://abc.tdental.vn:44377").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                  options => options.WithOrigins("https://localhost:44377").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            );

            app.UseMiddleware<GetTokenFromQueryStringMiddleware>();
            app.UseMiddleware<MigrateDbMiddleware>();
            app.UseMiddleware(typeof(ProcessUpdateMiddleware));
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseHangfireServer();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapControllers();

                endpoints.Select()
                   .Expand()
                   .Filter()
                   .OrderBy()
                   .MaxTop(20)
                   .Count();

                endpoints.MapODataRoute("odata", "odata", OdataEdmConfig.GetEdmModel());
                endpoints.EnableDependencyInjection();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
