using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StockWarehouseService : BaseService<StockWarehouse>, IStockWarehouseService
    {

        public StockWarehouseService(IAsyncRepository<StockWarehouse> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public override async Task<StockWarehouse> CreateAsync(StockWarehouse wh)
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var locationObj = GetService<IStockLocationService>();
            //create view location
            var objectReference = await irModelDataObj.GetObjectReference("stock.stock_location_locations");
            var viewLocation = new StockLocation
            {
                CompanyId = wh.CompanyId,
                Name = wh.Code,
                Usage = "view",
                ParentLocationId = objectReference.ResId,
            };

            await locationObj.CreateAsync(viewLocation);
            wh.ViewLocationId = viewLocation.Id;

            //create location
            var location = new StockLocation
            {
                Name = "Dự trữ",
                Usage = "internal",
                ParentLocationId = viewLocation.Id,
                CompanyId = wh.CompanyId,
            };
            await locationObj.CreateAsync(location);
            wh.LocationId = location.Id;
            wh.Location = location;

            await base.CreateAsync(wh);

            await CreateSequencesAndPickingTypes(wh);
            return wh;
        }

        private async Task CreateSequencesAndPickingTypes(StockWarehouse warehouse)
        {
            var irSequenceObj = GetService<IIRSequenceService>();
            var locObj = GetService<IStockLocationService>();
            var irModelDataObj = GetService<IIRModelDataService>();
            var pickingTypeObj = GetService<IStockPickingTypeService>();

            //create new sequence
            var inSequence = new IRSequence
            {
                Name = warehouse.Name + " Thứ tự nhập",
                Prefix = warehouse.Code + "/IN/",
                Padding = 5,
                NumberIncrement = 1,
                NumberNext = 1,
            };
            var outSequence = new IRSequence
            {
                Name = warehouse.Name + " Thứ tự xuất",
                Prefix = warehouse.Code + "/OUT/",
                Padding = 5,
                NumberIncrement = 1,
                NumberNext = 1,
            };
            var intSequence = new IRSequence
            {
                Name = warehouse.Name + " Thứ tự nội bộ",
                Prefix = warehouse.Code + "/INT/",
                Padding = 5,
                NumberIncrement = 1,
                NumberNext = 1,
            };

            await irSequenceObj.CreateAsync(new List<IRSequence>() { inSequence , outSequence, intSequence });

            //create picking types
            var customerLoc = await locObj.GetDefaultCustomerLocation();
            var supplierLoc = await locObj.GetDefaultSupplierLocation();
            if (customerLoc == null || supplierLoc == null)
            {
                throw new Exception("Không tìm thấy địa điểm khách hàng hoặc nhà cung cấp.");
            }

            var inType = new StockPickingType
            {
                Name = "Nhận hàng",
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "incoming",
                IRSequenceId = inSequence.Id,
                UseCreateLots = true,
                UseExistingLots = false,
                DefaultLocationSrcId = supplierLoc.Id,
                DefaultLocationDestId = warehouse.LocationId,
            };

            await pickingTypeObj.CreateAsync(inType);

            var outType = new StockPickingType
            {
                Name = "Giao hàng",
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "outgoing",
                UseCreateLots = false,
                UseExistingLots = true,
                IRSequenceId = outSequence.Id,
                ReturnPickingTypeId = inType.Id,
                DefaultLocationSrcId = warehouse.LocationId,
                DefaultLocationDestId = customerLoc.Id,
            };

            await pickingTypeObj.CreateAsync(outType);
            inType.ReturnPickingTypeId = outType.Id;

            await pickingTypeObj.UpdateAsync(inType);

            warehouse.OutTypeId = outType.Id;
            warehouse.InTypeId = inType.Id;

            await UpdateAsync(warehouse);
        }
    }
}
