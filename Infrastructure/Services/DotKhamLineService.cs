using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class DotKhamLineService : BaseService<DotKhamLine>, IDotKhamLineService
    {
        private readonly IMapper _mapper;
        private readonly IProductStepService _productStepService;

        public DotKhamLineService(IAsyncRepository<DotKhamLine> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProductStepService productStepService)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _productStepService = productStepService;
        }

        public async Task CheckDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var line in self)
            {
                //if (line.Operations.All(x => x.State == "done"))
                //{
                //    line.State = "done";
                //    line.DateFinished = DateTime.Now;
                //}
            }

            await UpdateAsync(self);
        }

        public override async Task<IEnumerable<DotKhamLine>> CreateAsync(IEnumerable<DotKhamLine> entities)
        {
            foreach (var entity in entities)
            {
                //if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
                //{
                //    var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                //    entity.Name = await sequenceService.NextByCode("dot.kham.line");
                //}
            }
            await base.CreateAsync(entities);
            await ActionConfirm(entities.Select(x => x.Id));
            return entities;
        }

        public async Task CheckUpdateStartOperation(Guid id)
        {
            var dkl = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            //if (dkl.Operations.Any(x => x.State == "progress"))
            //{
            //    dkl.State = "progress";
            //    if (!dkl.DateStart.HasValue)
            //        dkl.DateStart = DateTime.Now;
            //    await UpdateAsync(dkl);
            //}

            //if (dkl.Operations.All(x => x.State == "done"))
            //{
            //    dkl.State = "done";
            //    if (!dkl.DateFinished.HasValue)
            //        dkl.DateFinished = DateTime.Now;
            //    await UpdateAsync(dkl);
            //}
        }

        public async Task<IEnumerable<IEnumerable<DotKhamLineBasic>>> GetAllForDotKham2(Guid dotKhamId)
        {
            var lines = await SearchQuery(x => x.DotKhamId == dotKhamId).Include(x => x.Product).GroupBy(x => x.ProductId).Select(y => y.ToList()).ToListAsync();
            var res = _mapper.Map<IEnumerable<IEnumerable<DotKhamLineBasic>>>(lines);
            return res;
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var line in self)
            {
                await _GenerateOperations(line);
            }
        }

        public async Task _GenerateOperations(DotKhamLine self)
        {
            var ops = new List<DotKhamLineOperation>();
            //if (self.Routing != null)
            //{
            //    foreach(var rl in self.Routing.Lines)
            //    {
            //        ops.Add(new DotKhamLineOperation
            //        {
            //            ProductId = rl.ProductId,
            //            LineId = self.Id,
            //            Sequence = rl.Sequence,
            //        });
            //    }
            //}
            //else
            //{
            //    ops.Add(new DotKhamLineOperation
            //    {
            //        ProductId = self.ProductId,
            //        LineId = self.Id,
            //    });
            //}

            var opObj = GetService<IDotKhamLineOperationService>();
            await opObj.CreateAsync(ops);
        }
        public async Task<PagedResult2<DotKhamLineBasic>> GetPagedResultAsync(DotKhamLinePaged val)
        {
            var dotKhamObj = GetService<IDotKhamService>();

            ISpecification<DotKhamLine> ex = new InitialSpecification<DotKhamLine>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                ex = ex.And(new InitialSpecification<DotKhamLine>(x => x.DotKham.Partner.Name.Contains(val.Search) || x.DotKham.Partner.NameNoSign.Contains(val.Search) ||
               x.DotKham.Partner.Ref.Contains(val.Search)));

            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                query = query.Where(x => x.DotKham.Date.Date >= val.DateFrom.Value.Date);
            }

            if (val.DateTo.HasValue)
            {
                query = query.Where(x => x.DotKham.Date.Date <= val.DateTo.Value.Date);
            }

            var total = await query.Where(ex.AsExpression()).CountAsync();

            if (val.Limit > 0) query = query.Skip(val.Offset).Take(val.Limit);
            var items = await _mapper.ProjectTo<DotKhamLineBasic>(query.Where(ex.AsExpression())).ToListAsync();

            return new PagedResult2<DotKhamLineBasic>(total, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<DotKhamLineBasic>>(items)
            };
        }
    }
}
