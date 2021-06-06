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
    public class SmsBrandnameProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.7";
        private IServiceScopeFactory _serviceScopeFactory;
        public SmsBrandnameProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
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

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                ///Add rule SmsAccount : tài khoản brandname
                var smsAccountRule = context.IRRules.Where(x => x.Code == "sms.sms_account_comp_rule").FirstOrDefault();
                if (smsAccountRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsAccount").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Account", Model = "SmsAccount" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_account_comp_rule",
                        Name = "Sms Account company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }


                ///Add rule sms Campaign : sms chiến dịch
                var smsCampaignRule = context.IRRules.Where(x => x.Code == "sms.sms_campaign_comp_rule").FirstOrDefault();
                if (smsCampaignRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsCampaign").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Campaign", Model = "SmsCampaign" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_campaign_comp_rule",
                        Name = "Sms Campaign company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                ///Add rule sms config : thiết lập tin nhắn tự động
                var smsConfigRule = context.IRRules.Where(x => x.Code == "sms.sms_config_comp_rule").FirstOrDefault();
                if (smsConfigRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsConfig").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Config", Model = "SmsConfig" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_config_comp_rule",
                        Name = "Sms Config company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                ///Add rule sms Template : tin nhắn sms mẫu
                var smsTemplateRule = context.IRRules.Where(x => x.Code == "sms.sms_template_comp_rule").FirstOrDefault();
                if (smsConfigRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsTemplate").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Template", Model = "SmsTemplate" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_template_comp_rule",
                        Name = "Sms Template company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                ///Add rule sms Message : tin nhắn sms
                var smsMessageRule = context.IRRules.Where(x => x.Code == "sms.sms_message_comp_rule").FirstOrDefault();
                if (smsConfigRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsMessage").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Message", Model = "SmsMessage" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_message_comp_rule",
                        Name = "Sms Message company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

                ///Add rule sms Message detail : chi tiết tin nhắn sms - báo cáo thống kê tin nhắn
                var smsMessageDetailRule = context.IRRules.Where(x => x.Code == "sms.sms_message_detail_comp_rule").FirstOrDefault();
                if (smsConfigRule == null)
                {
                    var model = context.IRModels.Where(x => x.Model == "SmsMessageDetail").FirstOrDefault();
                    if (model == null)
                    {
                        model = new IRModel { Name = "Sms Message Detail", Model = "SmsMessageDetail" };
                        context.IRModels.Add(model);
                        context.SaveChanges();
                    }

                    context.IRRules.Add(new IRRule
                    {
                        Code = "sms.sms_message_detail_comp_rule",
                        Name = "Sms Message Detail company rule",
                        ModelId = model.Id
                    });
                    context.SaveChanges();
                }

            }

            return Task.CompletedTask;
        }
    }
}
