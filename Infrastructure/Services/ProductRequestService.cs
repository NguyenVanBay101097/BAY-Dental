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

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

            var paged = new PagedResult2<ProductRequestBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ProductRequestBasic>>(items)
            };

            return paged;
        }

        public async Task<ProductRequestDisplay> DefaultGet()
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetByIdAsync(UserId);
            var request = new ProductRequestDisplay();
            request.UserId = UserId;
            request.User = _mapper.Map<ApplicationUserSimple>(user);
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

            res.Lines = await requestLineObj.SearchQuery(x => x.RequestId == res.Id)
                .OrderBy(x => x.Sequence)
                .Include(x => x.Product)
                .Include(x => x.ProducUOM)
                .Include(x => x.SaleOrderLine).ToListAsync();

            var display = _mapper.Map<ProductRequestDisplay>(res);

            return display;
        }

        public async Task<ProductRequest> CreateRequest(ProductRequestSave val)
        {
            var request = _mapper.Map<ProductRequest>(val);
            SaveRequestLines(val, request);
            await CreateAsync(request);

            return request;
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
            var request = await SearchQuery(x => x.Id == id).Include(x => x.Lines).ThenInclude(s => s.SaleProductionLineRels).FirstOrDefaultAsync();

            request = _mapper.Map(val, request);

            SaveRequestLines(val, request);

            await UpdateAsync(request);
        }

        private void SaveRequestLines(ProductRequestSave val, ProductRequest request)
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
                    if (line.SaleProductionLineId.HasValue)
                        item.SaleProductionLineRels.Add(new SaleProductionLineProductRequestLineRel { SaleProductionLineId = line.SaleProductionLineId.Value });
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
            var productionlineObj = GetService<ISaleProductionLineService>();
            var saleProducionLineIds = new List<Guid>().AsEnumerable();
            var selfs = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ThenInclude(s => s.SaleProductionLineRels).ToListAsync();

            foreach (var request in selfs)
            {
                var lineIds = request.Lines.SelectMany(x => x.SaleProductionLineRels).Select(s => s.SaleProductionLineId).ToList();
                saleProducionLineIds = saleProducionLineIds.Union(lineIds);
                request.State = "confirmed";
            }

            if (saleProducionLineIds.Any())
                await productionlineObj.ComputeQtyRequested(saleProducionLineIds);

            await UpdateAsync(selfs);


        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var productionlineObj = GetService<ISaleProductionLineService>();
            var saleProducionLineIds = new List<Guid>().AsEnumerable();
            var selfs = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines).ThenInclude(s => s.SaleProductionLineRels)
                .ToListAsync();

            foreach (var request in selfs)
            {
                if (request.State == "done")
                    throw new Exception("Bạn không thể hủy yêu cầu vật tư đã xuất");

                var lineIds = request.Lines.SelectMany(x => x.SaleProductionLineRels).Select(s => s.SaleProductionLineId).ToList();
                saleProducionLineIds = saleProducionLineIds.Union(lineIds);

                request.State = "cancel";
            }


            await UpdateAsync(selfs);

            //save requested quantity
            if (saleProducionLineIds.Any())
                await productionlineObj.ComputeQtyRequested(saleProducionLineIds);

        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var pickingObj = GetService<IStockPickingService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Employee)
                .Include(x => x.Lines).ThenInclude(s => s.Product)
                .ToListAsync();

            //tạo phiếu xuất kho
            foreach (var request in self)
            {
                if (request.State != "confirmed")
                    continue;

                var picking = await _CreatePicking(request);
                await pickingObj.ActionDone(new List<Guid>() { picking.Id });

                request.State = "done";
                request.PickingId = picking.Id;
            }

            await UpdateAsync(self);
        }

        private async Task<StockPicking> _CreatePicking(ProductRequest request)
        {
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var pickingObj = GetService<IStockPickingService>();
            var stockMoveObj = GetService<IStockMoveService>();

            var pickingType = await pickingTypeObj.SearchQuery(x => x.Code == "outgoing" && x.Warehouse.CompanyId == request.CompanyId).FirstOrDefaultAsync();
            if (pickingType == null)
                throw new ArgumentNullException("pickingType");

            if (!pickingType.DefaultLocationSrcId.HasValue || !pickingType.DefaultLocationDestId.HasValue)
                throw new Exception("DefaultLocationSrcId DefaultLocationDestId required");

            var locationId = pickingType.DefaultLocationSrcId.Value;
            var locationDestId = pickingType.DefaultLocationDestId.Value;

            var picking_vals = new StockPicking()
            {
                Origin = request.Name,
                PartnerId = request.Employee.PartnerId,
                PickingTypeId = pickingType.Id,
                CompanyId = request.CompanyId.Value,
                LocationId = locationId,
                LocationDestId = locationDestId,
            };

            await pickingObj.CreateAsync(picking_vals);

            var sequence = 1;
            var moves = new List<StockMove>();
            var productTypes = new string[] { "product", "consu" };
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
                    CompanyId = picking_vals.CompanyId,
                    Origin = request.Name,
                };
                moves.Add(move);
            }

            await stockMoveObj.CreateAsync(moves);
            return picking_vals;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var selfs = await SearchQuery(x => ids.Contains(x.Id))
                .ToListAsync();

            foreach (var request in selfs)
            {
                if (request.State != "draft" && request.State != "cancel")
                    throw new Exception("Bạn chỉ có thể xóa phiếu yêu cầu vật tư ở trạng thái đã hủy");
            }


            await DeleteAsync(selfs);
        }

        public override ISpecification<ProductRequest> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "productrequest.product_request_comp_rule":
                    return new InitialSpecification<ProductRequest>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        //public override Task DeleteAsync(IEnumerable<ProductRequest> entities)
        //{
        //    if (entities.Any(x => x.State == "done"))
        //        throw new Exception("Bạn không thể xóa yêu cầu vật tư đã xuất");
        //    return base.DeleteAsync(entities);
        //}
    }
}
