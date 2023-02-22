using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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
            var configPrints = new List<ConfigPrintBasic>()
            {
                new ConfigPrintBasic { Name = "Hồ sơ điều trị", Code = AppConstants.SaleOrderPaperCode },
                new ConfigPrintBasic { Name = "Biên lai thanh toán", Code = AppConstants.PaymentPaperCode },
                new ConfigPrintBasic { Name = "Phiếu thanh toán lương nhân viên", Code = AppConstants.SalaryEmployeePaperCode },
                new ConfigPrintBasic { Name = "Phiếu tạm ứng - chi lương", Code = AppConstants.SalaryPaymentPaperCode },
                new ConfigPrintBasic { Name = "Đặt hàng Labo", Code =  AppConstants.LaboOrderPaperCode },
                new ConfigPrintBasic { Name = "Đơn thuốc", Code = AppConstants.ToaThuocPaperCode },
                new ConfigPrintBasic { Name = "Hóa đơn thuốc", Code = AppConstants.MedicineOrderPaperCode },
                new ConfigPrintBasic { Name = "Phiếu thu - chi", Code = AppConstants.PhieuThuChiPaperCode },
                new ConfigPrintBasic { Name = "Phiếu xuất - nhập kho", Code = AppConstants.StockPickingPaperCode },
                new ConfigPrintBasic { Name = "Phiếu kiểm kho", Code = AppConstants.StockInventoryPaperCode },

            };


            var allConfigs = await SearchQuery(x => x.CompanyId == CompanyId).Include(x => x.PrintPaperSize).ToListAsync();
            var configPrint_dict = allConfigs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First());
            var modelDataObj = GetService<IIRModelDataService>();
            var defaultPaperSize = await modelDataObj.GetRef<PrintPaperSize>("base.paperformat_a4");

            foreach (var item in configPrints)
            {
                if (configPrint_dict.ContainsKey(item.Code))
                {
                    var config = configPrint_dict[item.Code];
                    item.Id = config.Id;
                    item.IsInfoCompany = config.IsInfoCompany;
                    item.PrintPaperSize = config.PrintPaperSize != null ? _mapper.Map<PrintPaperSizeBasic>(config.PrintPaperSize) : _mapper.Map<PrintPaperSizeBasic>(defaultPaperSize);
                }
                else
                {
                    item.PrintPaperSize = _mapper.Map<PrintPaperSizeBasic>(defaultPaperSize);
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
  

        public override ISpecification<ConfigPrint> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "config.config_print_comp_rule":
                    return new InitialSpecification<ConfigPrint>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
