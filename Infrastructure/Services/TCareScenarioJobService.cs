using Microsoft.Extensions.Configuration;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace Infrastructure.Services
{
    public class TCareScenarioJobService : ITCareScenarioJobService
    {
        private readonly IConfiguration _configuration;
        public TCareScenarioJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Run(string db, IEnumerable<Guid> ids)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);

            try
            {
                var query = context.TCareScenarios.AsQueryable();
                // bình thường sẽ chỉ chạy kịch bản auto_everyday
                if (ids != null)
                {
                    query = query.Where(x => ids.Contains(x.Id));
                } else
                {
                    query = query.Where(x => x.Type == "auto_everyday");
                }
                var scenarios = await query.Include(x=>x.Campaigns).Select(x=> new {campIds = x.Campaigns.Select(y=>y.Id)}).ToListAsync();

                // call campaign job
                var campIds = scenarios.SelectMany(x=>x.campIds).ToList();
                BackgroundJob.Enqueue<TCareCampaignJobService>(x=> x.Run(db, campIds));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
