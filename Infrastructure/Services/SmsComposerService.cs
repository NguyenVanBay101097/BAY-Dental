using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;
        private readonly AppTenant _tenant;
        public SmsComposerService( IConfiguration configuration, ITenant<AppTenant> tenant, IAsyncRepository<SmsComposer> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _configuration = configuration;
        }

        public async Task ActionSendSms(SmsComposer entity)
        {
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
                    //await _smsSendMessageService.CreateSmsSms(context, entity, partnerIds, companyId);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
