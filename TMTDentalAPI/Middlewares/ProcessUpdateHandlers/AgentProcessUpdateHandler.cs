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
    public class AgentProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;


        public AgentProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    Version version1 = new Version(_version);
                    Version version2 = new Version(tenant.Version);
                    if (version2.CompareTo(version1) >= 0)
                        return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var agentRule = context.IRRules.Where(x => x.Code == "base.agent_comp_rule").FirstOrDefault();
                if (agentRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "Agent").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Agent ", Model = "Agent" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "base.agent_comp_rule",
                        Name = "Agent multi-company",
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
                    var accCNKH = context.AccountAccounts.Where(x => x.Code == "CNKH" && x.CompanyId == company.Id).FirstOrDefault();
                    if (accCNKH == null)
                    {
                        accCNKH = new AccountAccount
                        {
                            Name = "Công nợ khách hàng",
                            Code = "CNKH",
                            InternalType = currentLiabilities.Type,
                            UserTypeId = currentLiabilities.Id,
                            CompanyId = company.Id,
                        };

                        context.AccountAccounts.Add(accCNKH);
                        context.SaveChanges();

                        var seq = context.IRSequences.Where(x => x.Prefix == "DEBT/{yyyy}/").FirstOrDefault();
                        if (seq == null)
                        {
                            seq = new IRSequence
                            {
                                Name = "Ghi công nợ",
                                Prefix = "DEBT" + "/{yyyy}/",
                                Padding = 4,
                                NumberIncrement = 1,
                                NumberNext = 1,
                                CompanyId = company.Id,
                            };

                            context.IRSequences.Add(seq);
                            context.SaveChanges();
                        }


                        var journalCNKH = context.AccountJournals.Where(x => x.Type == "debt" && x.CompanyId == company.Id).FirstOrDefault();
                        if (journalCNKH == null)
                        {
                            journalCNKH = new AccountJournal
                            {
                                Name = "Ghi công nợ",
                                Type = "debt",
                                UpdatePosted = true,
                                Code = "DEBT",
                                DefaultDebitAccountId = accCNKH.Id,
                                DefaultCreditAccountId = accCNKH.Id,
                                SequenceId = seq.Id,
                                CompanyId = company.Id,
                            };

                            context.AccountJournals.Add(journalCNKH);
                            context.SaveChanges();
                        }
                    }

                    var accHH = context.AccountAccounts.Where(x => x.Code == "HHNGT" && x.CompanyId == company.Id).FirstOrDefault();
                    if (accHH == null)
                    {
                        accHH = new AccountAccount
                        {
                            Name = "Hoa hồng người giới thiệu",
                            Code = "HHNGT",
                            InternalType = currentLiabilities.Type,
                            UserTypeId = currentLiabilities.Id,
                            CompanyId = company.Id,
                        };

                        context.AccountAccounts.Add(accHH);
                        context.SaveChanges();

                        var seq = context.IRSequences.Where(x => x.Prefix == "COMM/{yyyy}/").FirstOrDefault();
                        if (seq == null)
                        {
                            seq = new IRSequence
                            {
                                Name = "Hoa hồng người giới thiệu",
                                Prefix = "COMM" + "/{yyyy}/",
                                Padding = 4,
                                NumberIncrement = 1,
                                NumberNext = 1,
                                CompanyId = company.Id,
                            };

                            context.IRSequences.Add(seq);
                            context.SaveChanges();
                        }


                        var journalHHA = context.AccountJournals.Where(x => x.Type == "commission" && x.CompanyId == company.Id).FirstOrDefault();
                        if (journalHHA == null)
                        {
                            journalHHA = new AccountJournal
                            {
                                Name = "hoa hồng",
                                Type = "commission",
                                UpdatePosted = true,
                                Code = "COMMISSION",
                                DefaultDebitAccountId = accCNKH.Id,
                                DefaultCreditAccountId = accCNKH.Id,
                                SequenceId = seq.Id,
                                CompanyId = company.Id,
                            };

                            context.AccountJournals.Add(journalHHA);
                            context.SaveChanges();
                        }
                    }

                }



            }

            return Task.CompletedTask;
        }
    }
}
