using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DotKhamLineOperationService : BaseService<DotKhamLineOperation>, IDotKhamLineOperationService
    {

        public DotKhamLineOperationService(IAsyncRepository<DotKhamLineOperation> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task MarkDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .ToListAsync();
            var lineIds = new List<Guid>();
            foreach (var op in self)
            {
                op.State = "done";
                op.DateFinished = DateTime.Now;
                if (op.LineId.HasValue)
                {
                    lineIds.Add(op.LineId.Value);
                }
            }

            var lineObj = GetService<IDotKhamLineService>();
            //await lineObj.CheckDone(lineIds.Distinct().ToList());

            await UpdateAsync(self);
        }

        public async Task StartOperation(Guid id)
        {
            var operation = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            operation.State = "progress";
            operation.DateStart = DateTime.Now;
            await UpdateAsync(operation);
            var dklObj = GetService<IDotKhamLineService>();
            if (operation.LineId.HasValue) 
                await dklObj.CheckUpdateStartOperation(operation.LineId.Value);
        }
    }
}
