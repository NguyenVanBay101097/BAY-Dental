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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class StockPickingService : BaseService<StockPicking>, IStockPickingService
    {
        private readonly IMapper _mapper;

        public StockPickingService(IAsyncRepository<StockPicking> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<StockPickingBasic>> GetPagedResultAsync(StockPickingPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Include(x => x.Partner).Include(x => x.CreatedBy).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<StockPickingBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<StockPickingBasic>>(items)
            };
        }

        private IQueryable<StockPicking> GetQueryPaged(StockPickingPaged val)
        {
            var query = SearchQuery();
            if (val.PickingTypeId.HasValue)
                query = query.Where(x => x.PickingTypeId == val.PickingTypeId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.PickingType.Code == val.Type);
            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);
            }
            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            query = query.OrderByDescending(s => s.DateCreated);
            return query;
        }

        public async Task<StockPickingDisplay> DefaultGet(StockPickingDefaultGet val)
        {
            var res = new StockPickingDisplay();
            res.CompanyId = CompanyId;
            if (val.DefaultPickingTypeId.HasValue)
            {
                var pickingTypeObj = GetService<IStockPickingTypeService>();
                var pickingType = await pickingTypeObj.GetByIdAsync(val.DefaultPickingTypeId.Value);
                res.PickingTypeId = pickingType.Id;
                res.LocationId = pickingType.DefaultLocationSrcId.Value;
                res.LocationDestId = pickingType.DefaultLocationDestId.Value;
            }

            return res;
        }

        public async Task<StockPickingDisplay> DefaultGetOutgoing()
        {
            var res = new StockPickingDisplay();
            var companyId = CompanyId;
            res.CompanyId = companyId;
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var pickingType = await pickingTypeObj.SearchQuery(x => x.Code == "outgoing" && x.Warehouse.CompanyId == companyId).FirstOrDefaultAsync();
            if (pickingType == null)
                throw new Exception("Không tìm thấy hoạt động xuất kho");

            res.PickingTypeId = pickingType.Id;
            res.LocationId = pickingType.DefaultLocationSrcId.Value;
            res.LocationDestId = pickingType.DefaultLocationDestId.Value;

            return res;
        }

        public async Task<StockPickingDisplay> DefaultGetIncoming()
        {
            var res = new StockPickingDisplay();
            var companyId = CompanyId;
            res.CompanyId = companyId;
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var pickingType = await pickingTypeObj.SearchQuery(x => x.Code == "incoming" && x.Warehouse.CompanyId == companyId).FirstOrDefaultAsync();
            if (pickingType == null)
                throw new Exception("Không tìm thấy hoạt động nhập kho");

            res.PickingTypeId = pickingType.Id;
            res.LocationId = pickingType.DefaultLocationSrcId.Value;
            res.LocationDestId = pickingType.DefaultLocationDestId.Value;

            return res;
        }

        public async Task<StockPickingOnChangePickingTypeResult> OnChangePickingType(StockPickingOnChangePickingType val)
        {
            var res = new StockPickingOnChangePickingTypeResult();
            if (val.PickingTypeId.HasValue)
            {
                var pickingTypeObj = GetService<IStockPickingTypeService>();
                var pickingType = await pickingTypeObj.GetByIdAsync(val.PickingTypeId);
                res.LocationId = pickingType.DefaultLocationSrcId;
                res.LocationDestId = pickingType.DefaultLocationDestId;
            }
            return res;
        }

        public async Task<StockPicking> GetPickingForDisplay(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Partner).Include(x => x.CreatedBy)
                .Include(x => x.MoveLines)
                .Include("MoveLines.Product.Categ")
                 .Include("MoveLines.ProductUOM")
                .FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<StockPicking>> CreateAsync(IEnumerable<StockPicking> entities)
        {
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var seqObj = GetService<IIRSequenceService>();

            var pickingTypeIds = entities.Select(x => x.PickingTypeId);
            var pickingTypes = await pickingTypeObj.SearchQuery(x => pickingTypeIds.Any(z => z == x.Id)).ToListAsync();
            foreach (var entity in entities)
            {
                if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
                {
                    var pickingTypeId = entity.PickingTypeId;
                    var pickingType = pickingTypes.FirstOrDefault(x => x.Id == entity.PickingTypeId);
                    var sequenceId = pickingType.IRSequenceId;
                    entity.Name = await seqObj.NextById(sequenceId);
                }
            }
            return await base.CreateAsync(entities);
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.MoveLines)
                .Include("MoveLines.Location")
                .Include("MoveLines.Product")
                .Include("MoveLines.Product.ProductCompanyRels")
                .Include("MoveLines.LocationDest")
                .ToListAsync();
            var moveObj = GetService<IStockMoveService>();
            foreach (var picking in self)
            {
                await moveObj.ActionDone(picking.MoveLines);
                picking.State = "done";
            }

            await UpdateAsync(self);
        }

        public override ISpecification<StockPicking> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "stock.stock_picking_rule":
                    return new InitialSpecification<StockPicking>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.MoveLines)
                .Include(x => x.PickingType)
                .ToListAsync();
            var moveObj = GetService<IStockMoveService>();
            if (self.Any(x => x.State == "done"))
            {
                if (self.Any(x => x.PickingType.Code == "incoming"))
                {
                    throw new Exception("Không thể xóa phiếu nhập kho đã hoàn thành");
                }
                else if (self.Any(x => x.PickingType.Code == "outgoing"))
                {
                    throw new Exception("Không thể xóa phiếu xuất kho đã hoàn thành");
                }
                else
                {
                    throw new Exception("Không thể xóa phiếu đã hoàn thành");
                }
            }
            await moveObj.DeleteAsync(self.SelectMany(x => x.MoveLines));
            await DeleteAsync(self);
        }
    }
}
