using ApplicationCore.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UpdateExpiredDateTenantService
    {
        private readonly AdminAppSettings _appSettings;
        public UpdateExpiredDateTenantService(AdminAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task ComputeTenant(IEnumerable<AppTenant> tenants, SqlConnection conn)
        {
            var today = DateTime.Today;
            var dictTenantExtendHistories = (await conn.QueryAsync<TenantExtendHistory>("SELECT * FROM TenantExtendHistories")).GroupBy(x => x.TenantId).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var item in tenants)
            {
                if (dictTenantExtendHistories.ContainsKey(item.Id))
                {
                    var tenantExtendHistory = dictTenantExtendHistories[item.Id].Where(x => x.StartDate == today).OrderBy(x => x.DateCreated).LastOrDefault();
                    if (tenantExtendHistory != null)
                    {
                        var newTenant = new AppTenant();
                        newTenant.DateExpired = tenantExtendHistory.ExpirationDate;
                        newTenant.ActiveCompaniesNbr = tenantExtendHistory.ActiveCompaniesNbr;
                        await UpdateExpired(item, conn);
                    }
                }
            }
        }

        public async Task UpdateExpired(AppTenant tenant, SqlConnection conn)
        {
            await conn.ExecuteAsync("UPDATE Tenants SET ActiveCompaniesNbr = @activeCompaniesNbr, DateExpired = @dateExpired WHERE Id = @id", new { activeCompaniesNbr = tenant.ActiveCompaniesNbr, dateExpired = tenant.DateExpired, id = tenant.Id });
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
                throw e;
            }
        }

    }
}
