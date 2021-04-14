using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class SaleOrderPaymentProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaleOrderPaymentProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory , IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var saleOrderPaymentRule = context.IRRules.Where(x => x.Code == "sale.sale_order_payment_comp_rule").FirstOrDefault();
                if (saleOrderPaymentRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SaleOrderPayment").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "SaleOrder Payment", Model = "SaleOrderPayment" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sale.sale_order_payment_comp_rule",
                        Name = "SaleOrder Payment multi-company",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                var companyId =  claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
                var journalAdvance = context.AccountJournals.Where(x => x.Type == "advance" && x.CompanyId == companyId).FirstOrDefault();
                if (journalAdvance == null)
                {
                    var currentLiabilities = context.AccountAccountTypes.Where(x=> x.Name == "Current Liabilities").FirstOrDefault();
                    if (currentLiabilities == null)
                        return Task.CompletedTask;

                    var accKHTU = context.AccountAccounts.Where(x => x.Code == "KHTU" && x.CompanyId == companyId).FirstOrDefault();

                    if (accKHTU == null)
                    {
                        accKHTU = new AccountAccount
                        {
                            Name = "Khách hàng tạm ứng",
                            Code = "KHTU",
                            InternalType = currentLiabilities.Type,
                            UserTypeId = currentLiabilities.Id,
                            CompanyId = companyId,
                        };

                        context.SaveChanges();
                    }

                    journalAdvance = new AccountJournal
                    {
                        Name = "Nhật ký khách hàng tạm ứng",
                        Type = "advance",
                        UpdatePosted = true,
                        Code = "ADVANCE",
                        DefaultDebitAccountId = accKHTU.Id,
                        DefaultCreditAccountId = accKHTU.Id,
                        CompanyId = companyId,
                    };

                    context.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
