using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductRequestService : BaseService<ProductRequest>, IProductRequestService
    {
        private readonly IMapper _mapper;
        public ProductRequestService(IAsyncRepository<ProductRequest> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ProductRequestBasic>> GetPagedResultAsync(ProductRequestPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }


            if (val.SaleOrderId.HasValue)
                query = query.Where(x => x.SaleOrderId == val.SaleOrderId);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Employee).Include(x => x.User).Include(x => x.Picking).OrderByDescending(x => x.DateCreated);

            if (val.Limit <= 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await query.ToListAsync();

            var paged = new PagedResult2<ProductRequestBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ProductRequestBasic>>(items)
            };

            return paged;
        }

        public async Task<ProductRequestDisplay> DefaultGet(ProductRequestDefaultGet val)
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetByIdAsync(UserId);
            var request = new ProductRequestDisplay();
            request.UserId = UserId;
            request.User = _mapper.Map<ApplicationUserSimple>(user);
            request.SaleOrderId = val.SaleOrderId;
            request.Date = DateTime.Now;
            request.State = "draft";
            return request;
        }

        public async Task<ProductRequestDisplay> GetDisplay(Guid id)
        {
            var requestLineObj = GetService<IProductRequestLineService>();

            var res = await SearchQuery(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Employee)
                .Include(x => x.Picking)
                .Include(x => x.SaleOrder)
                .FirstOrDefaultAsync();

            res.Lines = await requestLineObj.SearchQuery(x => x.RequestId == res.Id).Include(x => x.Product).Include(x => x.ProducUOM).Include(x => x.SaleOrderLine).ToListAsync();

            var display = _mapper.Map<ProductRequestDisplay>(res);

            return display;
        }

        public async Task<ProductRequest> CreateRequest(ProductRequestSave val)
        {
            var prequest = _mapper.Map<ProductRequest>(val);
            SaveReuqestLines(val, prequest);
            await CreateAsync(prequest);

            return prequest;
        }

        public async override Task<ProductRequest> CreateAsync(ProductRequest entity)
        {
            var sequenceService = GetService<IIRSequenceService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var order = await saleOrderObj.SearchQuery(x => x.Id == entity.SaleOrderId).FirstOrDefaultAsync();
            entity.Name = await sequenceService.NextByCode("product.request");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertProductRequestSequence();
                entity.Name = await sequenceService.NextByCode("product.request");
            }

            entity.CompanyId = order.CompanyId;
            await base.CreateAsync(entity);

            return entity;

        }

        private async Task _InsertProductRequestSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu yêu cầu vật tư",
                Code = "product.request",
                Prefix = "YCVT",
                Padding = 5
            });
        }

        public async Task UpdateRequest(Guid id, ProductRequestSave val)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();

            medicineOrder = _mapper.Map(val, medicineOrder);

            SaveReuqestLines(val, medicineOrder);

            await UpdateAsync(medicineOrder);

        }

        private void SaveReuqestLines(ProductRequestSave val, ProductRequest request)
        {
            var lineToRemoves = new List<ProductRequestLine>();

            foreach (var existLine in request.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                request.Lines.Remove(line);
            }

            int sequence = 0;
            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<ProductRequestLine>(line);
                    item.Sequence = sequence++;
                    request.Lines.Add(item);
                }
                else
                {
                    var item = request.Lines.SingleOrDefault(c => c.Id == line.Id);
                    if (item != null)
                    {
                        _mapper.Map(line, item);
                        item.Sequence = sequence++;
                    }
                }

            }
        }


        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var selfs = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ToListAsync();

            foreach (var request in selfs)
                request.State = "confirmed";

            await UpdateAsync(selfs);
            await SaveUpdateRequestedQuantity(ids, selfs);
        }

        private async Task SaveUpdateRequestedQuantity(IEnumerable<Guid> ids, List<ProductRequest> selfs = null, bool isCU = true)
        {
            selfs = selfs == null ? await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ToListAsync() : selfs;

            //luu lai quantity da yeu cau
            //check đã tồn tại thì update quantity, else create, neu ko phai isCU thi giam quantity, neu quantity =0 thi xoa
            var requestedObj = GetService<ISaleOrderLineProductRequestedService>();
            var toCreates = new List<SaleOrderLineProductRequested>();
            var toUpdates = new List<SaleOrderLineProductRequested>();
            var toRemoves = new List<SaleOrderLineProductRequested>();

            foreach (var self in selfs)
            {
                foreach (var line in self.Lines)
                {
                    var exist = await requestedObj.SearchQuery(x => x.SaleOrderLineId == line.SaleOrderLineId && x.ProductId == line.ProductId).FirstOrDefaultAsync();
                    if (exist == null)
                    {
                        toCreates.Add(new SaleOrderLineProductRequested()
                        { ProductId = line.ProductId.Value, SaleOrderLineId = line.SaleOrderLineId.Value, RequestedQuantity = line.ProductQty });
                    }
                    else
                    {
                        exist.RequestedQuantity = isCU == true ? (exist.RequestedQuantity + line.ProductQty) : (exist.RequestedQuantity - line.ProductQty);
                        if (exist.RequestedQuantity <= 0)
                            toRemoves.Add(exist);
                        else
                            toUpdates.Add(exist);
                    }
                }
            }
            if (toCreates.Any())
            {
                await requestedObj.CreateAsync(toCreates);
            }
            if (toUpdates.Any())
            {
                await requestedObj.CreateAsync(toUpdates);
            }
            if (toRemoves.Any())
            {
                await requestedObj.DeleteAsync(toRemoves);
            }
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var request in self)
            {
                if (request.State == "done")
                    throw new Exception("Bạn không thể xóa yêu cầu vật tư đã xuất");

                request.State = "draft";
            }


            await UpdateAsync(self);

            await SaveUpdateRequestedQuantity(ids, self, false);
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.User)
                .Include(x => x.Employee)
                .Include(x => x.Picking)
                .Include(x => x.SaleOrder)
                .Include(x => x.Lines)
                .ThenInclude(s => s.Product)
                .ToListAsync();

            //tạo phiếu xuất kho
            await _CreatePicking(self);

            await UpdateAsync(self);
        }

        private async Task _CreatePicking(IEnumerable<ProductRequest> self)
        {
            var productTypes = new string[] { "product", "consu" };
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var stockMoveObj = GetService<IStockMoveService>();
            var pickingObj = GetService<IStockPickingService>();
            var whObj = GetService<IStockWarehouseService>();
            var locationObj = GetService<IStockLocationService>();

            foreach (var request in self)
            {
                if (request.State == "draft")
                    throw new Exception("Phiếu nháp không thể xuất kho ");

                if (!request.Lines.Any(x => productTypes.Contains(x.Product.Type)))
                    continue;

                var pickingTypeCode = "outgoing";
                var pickingType = await pickingTypeObj.SearchQuery(x => x.Code == pickingTypeCode && x.Warehouse.CompanyId == request.CompanyId).FirstOrDefaultAsync();
                if (pickingType == null)
                    throw new ArgumentNullException("pickingType");

                if (!pickingType.DefaultLocationSrcId.HasValue || !pickingType.DefaultLocationDestId.HasValue)
                    throw new Exception("DefaultLocationSrcId DefaultLocationDestId required");

                var wh = await whObj.SearchQuery(x => x.CompanyId == request.CompanyId).FirstOrDefaultAsync();
                if (wh == null)
                    throw new ArgumentNullException("wh");

                var locationDest = await locationObj.GetDefaultCustomerLocation();
                if (locationDest == null)
                    throw new ArgumentNullException("locationDest");

                var locationId = wh.LocationId;
                var locationDestId = locationDest.Id;

                var picking_vals = new StockPicking()
                {
                    Origin = request.Name,
                    PartnerId = request.User.PartnerId,
                    PickingTypeId = pickingType.Id,
                    CompanyId = request.CompanyId.Value,
                    LocationId = locationId,
                    LocationDestId = locationDestId,
                };

                await pickingObj.CreateAsync(picking_vals);

                var sequence = 1;
                var moves = new List<StockMove>();
                foreach (var line in request.Lines.Where(x => productTypes.Contains(x.Product.Type)))
                {
                    var move = new StockMove
                    {
                        Name = line.Product.Name,
                        ProductUOMId = line.ProductUOMId.HasValue ? line.ProductUOMId.Value : line.Product.UOMId,
                        PickingId = picking_vals.Id,
                        ProductId = line.Product.Id,
                        ProductUOMQty = line.ProductQty,
                        LocationId = locationId,
                        LocationDestId = locationDestId,
                        Sequence = sequence++,
                        CompanyId = picking_vals.CompanyId
                    };
                    moves.Add(move);
                }

                await stockMoveObj.CreateAsync(moves);

                request.PickingId = picking_vals.Id;
                request.State = "done";

                await pickingObj.ActionDone(new List<Guid>() { picking_vals.Id });
            }
        }

        public override ISpecification<ProductRequest> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "productrequest.product_request_comp_rule":
                    return new InitialSpecification<ProductRequest>(x => companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
