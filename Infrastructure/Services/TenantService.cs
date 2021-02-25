using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TenantService : AdminBaseService<AppTenant>, ITenantService
    {
        private readonly IMapper _mapper;
        private readonly AdminAppSettings _appSettings;
        public TenantService(IAsyncRepository<AppTenant> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IOptions<AdminAppSettings> appSettings
            )
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _appSettings = appSettings?.Value;
        }

        public async Task CheckUpdateDateExpired(TenantUpdateDateExpiredViewModel val)
        {
            var tenant = await GetByIdAsync(val.Id);
            var oldDateExpired = tenant.DateExpired;
            var oldActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
            tenant.DateExpired = val.DateExpired;
            tenant.ActiveCompaniesNbr = val.ActiveCompaniesNbr;
            await UpdateAsync(tenant);

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpResponseMessage response = null;
                using (var client = new HttpClient(new RetryHandler(clientHandler)))
                {
                    response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Có lỗi xảy ra");
            }
            catch (Exception e)
            {
                tenant.DateExpired = oldDateExpired;
                tenant.ActiveCompaniesNbr = oldActiveCompaniesNbr;
                await UpdateAsync(tenant);
                throw e;
            }
        }

        public async Task<PagedResult2<TenantBasic>> GetPagedResultAsync(TenantPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<TenantBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<TenantBasic>>(items)
            };
        }

        private IQueryable<AppTenant> GetQueryPaged(TenantPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search) ||
                x.Hostname.Contains(val.Search));

            return query;
        }
    }
}
