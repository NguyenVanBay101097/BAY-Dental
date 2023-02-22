using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelService : BaseService<IRModel>, IIRModelService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IRModelService(IAsyncRepository<IRModel> repository, IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment)
        : base(repository, httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<PagedResult2<IRModel>> GetPagedAsync(int offset = 0, int limit = 10, string filter = "")
        {
            ISpecification<IRModel> spec = new InitialSpecification<IRModel>(x => !x.Transient);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                spec = spec.And(new InitialSpecification<IRModel>(x => x.Name.Contains(filter)));
            }

            // the implementation below using ForEach and Count. We need a List.
            var itemsOnPage = await SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name), limit: limit, offSet: offset).ToListAsync();
            var totalItems = await CountAsync(spec);
            return new PagedResult2<IRModel>(totalItems, offset, limit)
            {
                Items = itemsOnPage
            };
        }

        public async Task InsertSampleData()
        {
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_data.csv");
            if (!File.Exists(file_path))
                return;
            var models = new List<IRModel>();
            using (TextReader reader = File.OpenText(file_path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                var records = csv.GetRecords<IRModelCsvLine>().ToList();
                foreach (var record in records)
                {
                    var model = new IRModel
                    {
                        Model = record.model,
                        Name = record.name,
                    };
                    models.Add(model);
                }
            }

            await CreateAsync(models);
        }

        public async Task<IDictionary<string, IRModel>> InsertIfNotExist(IDictionary<string, IRModel> dict)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var new_dict = new Dictionary<string, IRModel>();
            foreach (var item in dict)
            {
                var reference = item.Key;
                var model = await modelDataObj.GetRef<IRModel>(reference);
                if (model == null)
                {
                    model = item.Value;
                    await CreateAsync(model);

                    var referenceSplit = reference.Split(".");

                    var modelData = new IRModelData()
                    {
                        Model = "ir.model",
                        Name = referenceSplit[1],
                        Module = referenceSplit[0],
                        ResId = model.Id.ToString(),
                    };
                    await modelDataObj.CreateAsync(modelData);
                }

                new_dict.Add(reference, model);
            }

            return new_dict;
        }
    }
}
