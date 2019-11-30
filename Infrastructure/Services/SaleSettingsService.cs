using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SaleSettingsService : BaseService<SaleSettings>, ISaleSettingsService
    {
        public SaleSettingsService(IAsyncRepository<SaleSettings> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<SaleSettings> GetSettings()
        {
            var setting = await SearchQuery().FirstOrDefaultAsync();
            if (setting == null)
                setting = await CreateAsync(new SaleSettings());
            return setting;
        }
    }
}
