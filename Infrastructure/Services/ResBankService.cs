using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ResBankService : BaseService<ResBank>, IResBankService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;
        public ResBankService(IAsyncRepository<ResBank> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IWebHostEnvironment hostingEnvironment
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<PagedResult2<ResBankBasic>> GetPagedResultAsync(ResBankPaged paged)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(paged.Search))
                query = query.Where(x => x.Name.Contains(paged.Search) || x.BIC.Contains(paged.Search));

            var items = await query.OrderBy(x => x.Name).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<ResBankBasic>(totalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResBankBasic>>(items)
            };
        }

        public async Task<IEnumerable<ResBankSimple>> AutocompleteAsync(ResPartnerBankPaged val)
        {
            var rpBank = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                rpBank = rpBank.Where(x => x.Name.Contains(val.Search) || x.BIC.Contains(val.Search));

            var items = await rpBank.Skip(val.Offset).Take(val.Limit)
                .Select(x => new ResBankSimple
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return items;
        }

        public async Task ImportSampleData()
        {
            var resBankObj = GetService<IResBankService>();
            var existData = await resBankObj.SearchQuery().AnyAsync();
            if (existData)
                return;
            var resBanks = new List<ResBank>();

            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\res_bank.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(file_path);
            var records = doc.GetElementsByTagName("record");
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var names = record.GetElementsByTagName("Name");
                var bics = record.GetElementsByTagName("BIC");
                resBanks.Add(new ResBank()
                {
                    Name = names[0].InnerText.Trim(),
                    BIC = bics[0].InnerText.Trim()
                });
            }

            await resBankObj.CreateAsync(resBanks);
        }
    }
}
