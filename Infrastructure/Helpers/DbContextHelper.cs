using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infrastructure.Helpers
{
    public static class DbContextHelper
    {
        public static CatalogDbContext GetCatalogDbContext(string db, IConfiguration configuration)
        {
            var section = configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];

            SqlConnectionStringBuilder connectionStringbuilder = new SqlConnectionStringBuilder(catalogConnection);
            if (db != "localhost")
                connectionStringbuilder["Database"] = $"TMTDentalCatalogDb__{db}";

            DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(connectionStringbuilder.ConnectionString);

            return new CatalogDbContext(builder.Options, null, null);
        }
    }
}
