using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class DotKhamStepService : BaseService<DotKhamStep>, IDotKhamStepService
    {
        readonly public IMapper _mapper;
        public DotKhamStepService(IMapper mapper, IAsyncRepository<DotKhamStep> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<DotKhamStep> CloneInsert(DotKhamStepCloneInsert val)
        {
            var step = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            var steps = await SearchQuery(x => x.SaleLineId == step.SaleLineId, x => x.OrderBy(s => s.Order)).ToListAsync();
            var clone = new DotKhamStep(step);
            await CreateAsync(clone);

            if (val.CloneInsert == "down")
            {
                var index = steps.IndexOf(step);
                steps.Insert(index + 1, clone);
            }
            else
            {
                var index = steps.IndexOf(step);
                steps.Insert(index, clone);
            }

            //re sequence
            var sequence = 0;
            foreach (var st in steps)
                st.Order = sequence++;
            await UpdateAsync(steps);

            return clone;
        }

        public async Task<DotKhamStepDisplay> GetDisplay(Guid id)
        {
            var step = await SearchQuery(x => x.Id == id).Include(x => x.Product).FirstOrDefaultAsync();
            return _mapper.Map<DotKhamStepDisplay>(step);
        }

        public async Task Unlink(IEnumerable<DotKhamStep> self)
        {
            if (self.Any(x => x.IsDone))
                throw new Exception("Không thể xóa chi tiết khám đã hoàn thành");
            await DeleteAsync(self);
        }
    }
}
