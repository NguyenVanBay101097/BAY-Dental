using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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

        public async Task MarkProgress(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach(var line in self)
            {
                line.State = "progress";
            }

            await UpdateAsync(self);
        }

        public async Task MarkDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))              
                .ToListAsync();
            foreach (var line in self)
            {
                line.State = "done";
                //foreach(var op in line.Operations)
                //{
                //    op.State = "done";
                //}
            }

            await UpdateAsync(self);
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
            foreach(var entity in entities)
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

        public async Task<IEnumerable<DotKhamLineBasic>> GetAllForDotKham(Guid dotKhamId)
        {
            var lines = await SearchQuery(x => x.DotKhamId == dotKhamId).Include(x => x.Product)
                .Include(x => x.User).OrderBy(x => x.Sequence).ToListAsync();
            var res = _mapper.Map<IEnumerable<DotKhamLineBasic>>(lines);
            return res;
        }

        public async Task<IEnumerable<IEnumerable<DotKhamLineBasic>>> GetAllForDotKham2(Guid dotKhamId)
        {
            var lines = await SearchQuery(x => x.DotKhamId == dotKhamId).Include(x => x.Product).GroupBy(x => x.ProductId).Select(y=>y.ToList()).ToListAsync();
            var res = _mapper.Map<IEnumerable<IEnumerable<DotKhamLineBasic>>>(lines);
            return res;
        }

        public async Task ChangeRouting(DotKhamLineChangeRouting val)
        {
            var dkl = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            if (dkl.State != "draft")
                throw new Exception("Bạn chỉ có thể đổi quy trình khi ở trạng thái chưa tiến hành");
            var routingObj = GetService<IRoutingService>();
            var routing = await routingObj.SearchQuery(x => x.Id == val.RoutingId && x.ProductId == dkl.ProductId).Include(x => x.Lines).FirstOrDefaultAsync();
            //xóa tất cả các line cũ và thêm lại từ routing mới
            //dkl.Operations.Clear();
            //foreach(var line in routing.Lines)
            //{
            //    dkl.Operations.Add(new DotKhamLineOperation
            //    {
            //        ProductId = line.ProductId,
            //        Sequence = line.Sequence,
            //    });
            //}

            await UpdateAsync(dkl);
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach(var line in self)
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
    }
}
