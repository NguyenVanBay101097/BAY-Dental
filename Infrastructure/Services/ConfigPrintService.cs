using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class ConfigPrintService : BaseService<ConfigPrint>, IConfigPrintService
    {
        private readonly IMapper _mapper;
        public ConfigPrintService(IAsyncRepository<ConfigPrint> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task<IEnumerable<ConfigPrintBasic>> LoadConfigPrint()
        {
            var configPrints = new List<ConfigPrintBasic>();
            var listPaperFormat = GetListPaperFormat();
            var configPrint_dict = await SearchQuery(x => x.CompanyId == CompanyId).Include(x => x.PrintPaperSize).ToDictionaryAsync(x => x.Name, x => x);

            var modelDataObj = GetService<IIRModelDataService>();
            var defaultPaperSize = await modelDataObj.GetRef<PrintPaperSize>("base.paperformat_a4");

            foreach (var item in listPaperFormat)
            {
                if (configPrint_dict.ContainsKey(item))
                {
                    configPrints.Add(_mapper.Map<ConfigPrintBasic>(configPrint_dict[item]));
                }      
                else
                {
                    var configPrint = new ConfigPrintBasic();
                    configPrint.Name = item;
                    configPrint.PrintPaperSize = _mapper.Map<PrintPaperSizeBasic>(defaultPaperSize);
                    configPrints.Add(configPrint);
                }
            }

            return configPrints;
        }

        public async Task CreateUpdateConfigPrint(IEnumerable<ConfigPrintSave> vals)
        {
            var listCreate = new List<ConfigPrint>();
            var listUpdate = new List<ConfigPrint>();

            foreach (var config in vals)
            {
                if (config.Id == Guid.Empty)
                {
                    var item = _mapper.Map<ConfigPrint>(config);
                    listCreate.Add(item);
                }
                else
                {
                    var item = await SearchQuery(x => x.Id == config.Id).FirstOrDefaultAsync();
                    item = _mapper.Map(config, item);
                    listUpdate.Add(item);

                }
            }

            if (listCreate.Any())
                await CreateAsync(listCreate);

            if (listUpdate.Any())
                await UpdateAsync(listUpdate);
        }

        private IEnumerable<string> GetListPaperFormat()
        {
            string[] papers = new string[]
            {
                "SaleOrderPaperFormat",
                "PaymentPaperFormat",
                "SalaryEmployeePaperFormat",
                "SalaryPaymentPaperFormat",
                "LaboOrderPaperFormat",
                "ToaThuocPaperFormat",
                "MedicineOrderPaperFormat",
                "PhieuThuChiPaperFormat",
                "StockPickingPaperFormat",
                "StockInventoryPaperFormat"
            };

            return papers;
        }

    }
}
