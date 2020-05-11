using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class HistoryService : BaseService<History>, IHistoryService
    {

        private readonly IMapper _mapper;
        public HistoryService(IAsyncRepository<History> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            :base (repository,httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<HistorySimple>> GetAutocompleteAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            return _mapper.Map<IEnumerable<HistorySimple>>(items);
        }

        public async Task<PagedResult2<HistorySimple>> GetPagedResultAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<HistorySimple>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HistorySimple>>(items)
            };
        }

        public async Task<IEnumerable<HistorySimple>> GetResultNotLimitAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);

            return _mapper.Map<IEnumerable<HistorySimple>>(query);
        }

        private IQueryable<History> GetQueryPaged(HistoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            query = query.OrderBy(x => x.Name);
            return query;
        }

        public async Task<bool> CheckDuplicate(Guid? id, HistorySimple val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Name.Trim()))
                query = query.Where(x => x.Name.Trim().ToLower().Equals(val.Name.Trim().ToLower()));
            if (id != Guid.Empty)
                query = query.Where(x=>x.Id!= id);

            var count = await query.CountAsync();
            if(count > 0)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        public async Task ImportExcelHistories(HistoryImportExcelBaseViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<History>();

            var errors = new List<string>();
           

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        //var active = Convert.ToString(worksheet.Cells[row, 2].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên tiểu sử bệnh là bắt buộc");


                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }



                        var item = new History
                        {
                            Name = name,
                            Active = true,
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                throw new Exception($" {string.Join(", ", errors)}");

            var vals = new List<History>();
            foreach (var item in data)
            {
                var pd = new History();
                pd.Name = item.Name;
                vals.Add(pd);
            }

            await CreateAsync(vals);
        }


    }
}
