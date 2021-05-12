using Hangfire;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SaleCouponProgramJobService
    {
        private readonly IConfiguration _configuration;

        public SaleCouponProgramJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Run(string db)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);

            var today = DateTime.Today;

            var program = await context.SaleCouponPrograms.Where(x => x.Status == "waiting" || x.Status == "running").ToListAsync();

            
        }
    }
}
