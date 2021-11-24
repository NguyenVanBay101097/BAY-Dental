using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class ResInsuranceProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.1";
        private IServiceScopeFactory _serviceScopeFactory;


        public ResInsuranceProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                //Version version1 = new Version(_version);
                //Version version2 = new Version(tenant.Version);
                //if (version2.CompareTo(version1) >= 0)
                //    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var resInsuranceRule = context.IRRules.Where(x => x.Code == "base.res_insurance_comp_rule").FirstOrDefault();
                if (resInsuranceRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "ResInsurance").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "ResInsurance ", Model = "ResInsurance" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "base.res_insurance_comp_rule",
                        Name = "ResInsurance multi-company",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }


                var resInsurancePaymentRule = context.IRRules.Where(x => x.Code == "base.res_insurance_payment_comp_rule").FirstOrDefault();
                if (resInsurancePaymentRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "ResInsurancePayment").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "ResInsurance Payment", Model = "ResInsurancePayment" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "base.res_insurance_payment_comp_rule",
                        Name = "ResInsurance Payment multi-company",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                var companies = context.Companies.ToList();
                var currentLiabilities = context.AccountAccountTypes.Where(x => x.Name == "Current Liabilities").FirstOrDefault();
                if (currentLiabilities == null)
                    return Task.CompletedTask;

                foreach (var company in companies)
                {
                    var accCNBH = context.AccountAccounts.Where(x => x.Code == "CNBH" && x.CompanyId == company.Id).FirstOrDefault();
                    if (accCNBH == null)
                    {
                        accCNBH = new AccountAccount
                        {
                            Name = "Công nợ bảo hiểm",
                            Code = "CNBH",
                            InternalType = currentLiabilities.Type,
                            UserTypeId = currentLiabilities.Id,
                            CompanyId = company.Id,
                            Reconcile = true
                        };

                        context.AccountAccounts.Add(accCNBH);
                        context.SaveChanges();


                        var seq = context.IRSequences.Where(x => x.Prefix == "CNBH/{yyyy}/").FirstOrDefault();
                        if (seq == null)
                        {
                            seq = new IRSequence
                            {
                                Name = "Công nợ bảo hiểm",
                                Prefix = "CNBH" + "/{yyyy}/",
                                Padding = 4,
                                NumberIncrement = 1,
                                NumberNext = 1,
                            };

                            context.IRSequences.Add(seq);
                            context.SaveChanges();
                        }



                        var journalCNKH = context.AccountJournals.Where(x => x.Type == "insurance" && x.CompanyId == company.Id).FirstOrDefault();
                        if (journalCNKH == null)
                        {
                            journalCNKH = new AccountJournal
                            {
                                Name = "Bảo hiểm",
                                Type = "insurance",
                                UpdatePosted = true,
                                Code = "INSURANCE",
                                DefaultDebitAccountId = accCNBH.Id,
                                DefaultCreditAccountId = accCNBH.Id,
                                SequenceId = seq.Id,
                                CompanyId = company.Id,
                            };

                            context.AccountJournals.Add(journalCNKH);
                            context.SaveChanges();
                        }
                    }

                    var seqInsuranceIncome = context.IRSequences.Where(x => x.Code == "account.payment.insurance.invoice").FirstOrDefault();
                    if (seqInsuranceIncome == null)
                    {
                        seqInsuranceIncome = new IRSequence
                        {
                            Name = "",
                            Prefix = "THUBH" + "/{yyyy}/",
                            Padding = 4,
                            NumberIncrement = 1,
                            NumberNext = 1,
                            Code = "account.payment.insurance.invoice"
                        };

                        context.IRSequences.Add(seqInsuranceIncome);
                        context.SaveChanges();
                    }

                }



            }

            return Task.CompletedTask;
        }
    }
}
