using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TCareCampaignJobService
    {
        private readonly IConfiguration _configuration;
        public TCareCampaignJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task CreateTodoItem(string db)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(catalogConnection);

            await using var context = new CatalogDbContext(builder.Options, null, null);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                context.PartnerTitles.Add(new PartnerTitle { Name = "test" });
                await context.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure
                await transaction.RollbackAsync();
            }


            Console.WriteLine("Run complete");
        }
    }
}
