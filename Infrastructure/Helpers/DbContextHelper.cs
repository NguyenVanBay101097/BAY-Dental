using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Helpers
{
    public static class DbContextHelper
    {
        public static CatalogDbContext GetCatalogDbContext(string db, IConfiguration configuration)
        {
            var section = configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            if (db != "localhost")
                catalogConnection = catalogConnection.Substring(0, catalogConnection.LastIndexOf('_')) + db;

            DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(catalogConnection);

            return new CatalogDbContext(builder.Options, null, null);
        }
    }
}
