using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class PrintPaperSizeService : BaseService<PrintPaperSize>, IPrintPaperSizeService
    {
        private readonly IMapper _mapper;
        public PrintPaperSizeService(IAsyncRepository<PrintPaperSize> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<PrintPaperSizeBasic>> GetPagedResultAsync(PrintPaperSizePaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
       

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<PrintPaperSizeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<PrintPaperSizeBasic>>(items)
            };

            return paged;
        }

        public async Task<PrintPaperSize> CreatePrintPaperSize(PrintPaperSizeSave val)
        {
            var paperSize = _mapper.Map<PrintPaperSize>(val);
            await CreateAsync(paperSize);
            return paperSize;
        }

        public async Task UpdatePrintPaperSize(Guid id, PrintPaperSizeSave val)
        {
            var paperSize = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();

            paperSize = _mapper.Map(val, paperSize);
            await UpdateAsync(paperSize);
        }
    }
}
