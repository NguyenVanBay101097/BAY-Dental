using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StockLocationService : BaseService<StockLocation>, IStockLocationService
    {

        public StockLocationService(IAsyncRepository<StockLocation> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<StockLocation> GetDefaultCustomerLocation()
        {
            var irModelObj = GetService<IIRModelDataService>();
            var modelData = await irModelObj.GetObjectReference("stock.stock_location_customers");
            StockLocation location = null;
            if (modelData != null)
                location = await GetByIdAsync(modelData.ResId);
            if (location == null)
                location = await SearchQuery(x => x.Usage == "customer").FirstOrDefaultAsync();
            return location;
        }

        public async Task<StockLocation> GetDefaultSupplierLocation()
        {
            var irModelObj = GetService<IIRModelDataService>();
            var modelData = await irModelObj.GetObjectReference("stock.stock_location_suppliers");
            StockLocation location = null;
            if (modelData != null)
                location = await GetByIdAsync(modelData.ResId);
            if (location == null)
                location = await SearchQuery(x => x.Usage == "supplier").FirstOrDefaultAsync();
            return location;
        }

        public override async Task<IEnumerable<StockLocation>> CreateAsync(IEnumerable<StockLocation> self)
        {
            await base.CreateAsync(self);

            foreach (var location in self)
            {
                if (location.ParentLocationId.HasValue && location.ParentLocation == null)
                    location.ParentLocation = await GetByIdAsync(location.ParentLocationId);
                location.CompleteName = (await _CompleteName(new List<StockLocation>() { location }))[location.Id];
                location.NameGet = await _NameGet(location);
            }

            await _ParentStoreCompute();
            return self;
        }

        private async Task _ParentStoreCompute()
        {
            var query = await SearchQuery(x => !x.ParentLocationId.HasValue, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            var pos = 0;
            foreach (var root in query)
            {
                pos = await BrowseRec(root, pos);
            }
        }

        private async Task<int> BrowseRec(StockLocation root, int pos)
        {
            Expression<Func<StockLocation, bool>> where = x => x.ParentLocationId == root.Id;
            var pos2 = pos + 1;
            var res = await SearchQuery(where, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            foreach (var item in res.ToList())
                pos2 = await BrowseRec(item, pos2);

            root.ParentLeft = pos;
            root.ParentRight = pos2;
            await UpdateAsync(root);
            return pos2 + 1;
        }

        private async Task<IDictionary<Guid, string>> _CompleteName(IEnumerable<StockLocation> locations)
        {
            var res = new Dictionary<Guid, string>();
            foreach (var location in locations)
            {
                res.Add(location.Id, location.Name);
                var parent = location.ParentLocation;
                while (parent != null)
                {
                    res[location.Id] = parent.Name + " / " + res[location.Id];
                    if (parent.ParentLocation == null && parent.ParentLocationId.HasValue)
                        parent.ParentLocation = await GetByIdAsync(parent.ParentLocationId);
                    parent = parent.ParentLocation;
                }
            }

            return res;
        }

        public async Task<string> _NameGet(StockLocation location)
        {
            var name = location.Name;
            while (location.ParentLocation != null && location.Usage != "view")
            {
                location = location.ParentLocation;
                name = location.Name + "/" + name;

                if (location.ParentLocationId.HasValue && location.ParentLocation == null)
                    location.ParentLocation = await GetByIdAsync(location.ParentLocationId);
            }

            return name;
        }
    }
}
