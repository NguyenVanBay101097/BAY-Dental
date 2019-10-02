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

        public async Task<IEnumerable<DotKhamStep>> GetVisibleSteps(Guid dotKhamId, string show = "dotkham")
        {
            var dotKhamService = GetService<IDotKhamService>();
            var dotKham = await dotKhamService.GetByIdAsync(dotKhamId);
            if (show == "all")
            {
                return await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId)
                    .Include(x => x.Product).Include(x => x.DotKham).ToListAsync();
            }
            else
            {
                return await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId &&
                (!x.DotKhamId.HasValue || x.DotKhamId == dotKham.Id))
                    .Include(x => x.Product).Include(x => x.DotKham).ToListAsync();
            }
        }

        public async Task<IEnumerable<IEnumerable<DotKhamStep>>> GetVisibleSteps2(Guid dotKhamId, string show = "dotkham")
        {
            var dotKhamService = GetService<IDotKhamService>();
            var dotKham = await dotKhamService.GetByIdAsync(dotKhamId);
            var res = new List<List<DotKhamStep>>();
            if (show == "all")
            {
                res = await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId)
                    .Include(x => x.Product).Include(x => x.DotKham).OrderBy(x => x.Order).GroupBy(x => x.ProductId).Select(x => x.ToList()).ToListAsync();
            }
            else
            {
                res = await SearchQuery(x => x.InvoicesId == dotKham.InvoiceId &&
                (!x.DotKhamId.HasValue || x.DotKhamId == dotKham.Id))
                    .Include(x => x.Product).Include(x => x.DotKham).OrderBy(x => x.Order).GroupBy(x => x.ProductId).Select(x => x.ToList()).ToListAsync();
            }
            var invLinesObj = GetService<IAccountInvoiceLineService>();
            foreach (var lines in res)
            {
                foreach (var step in lines)
                {
                    //Đổi tên dịch vụ Product.name là têm của dịch vụ InvoiceLine
                    var invLine = invLinesObj.SearchQuery(x => x.InvoiceId == step.InvoicesId && x.ProductId == step.ProductId).ToList().FirstOrDefault();
                    step.Product.Name = invLine.Name;
                }
            }
            return res;
        }
    }
}
