using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SmsComposerService : BaseService<SmsComposer>, ISmsComposerService
    {
        private readonly ISmsSendMessageService _smsSendMessageService;
        private readonly IConfiguration _configuration;
        private readonly AppTenant _tenant;
        public SmsComposerService(ISmsSendMessageService smsSendMessageService, IConfiguration configuration, ITenant<AppTenant> tenant, IAsyncRepository<SmsComposer> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _configuration = configuration;
            _smsSendMessageService = smsSendMessageService;
        }

        public override async Task<SmsComposer> CreateAsync(SmsComposer entity)
        {
            entity = await base.CreateAsync(entity);
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            await using var context = DbContextHelper.GetCatalogDbContext(hostName, _configuration);
            try
            {
                if (!string.IsNullOrEmpty(entity.ResIds))
                {
                    var companyId = CompanyId;
                    var partnerIds = new List<Guid>();
                    var ids = entity.ResIds.Split(',');
                    foreach (var id in ids)
                    {
                        partnerIds.Add(Guid.Parse(id));
                    }
                    await _smsSendMessageService.CreateSmsSms(context, entity, partnerIds, companyId);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return entity;
        }
    }
}
