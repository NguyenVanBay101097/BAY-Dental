using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PrintTemplateConfigService : BaseService<PrintTemplateConfig>, IPrintTemplateConfigService
    {
        private readonly IMapper _mapper;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IPrintPaperSizeService _printPaperSizeService;
        public PrintTemplateConfigService(IAsyncRepository<PrintTemplateConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IPrintTemplateService printTemplateService, IPrintPaperSizeService printPaperSizeService)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _printTemplateService = printTemplateService;
            _printPaperSizeService = printPaperSizeService;
        }

        public async Task<PrintTemplateConfigDisplay> GetDisplay(PrintTemplateConfigChangeType val)
        {
            var printConfig = await SearchQuery(x => x.Type == val.Type)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.Company)
                .FirstOrDefaultAsync();

            var display = _mapper.Map<PrintTemplateConfigDisplay>(printConfig);

            if(display == null)
            {
                var printTmp = await _printTemplateService.SearchQuery(x => x.Type == val.Type).FirstOrDefaultAsync();
                if(printTmp == null)
                {
                    throw new Exception("Không tìm thấy mẫu in có sẵn");
                }

                display = new PrintTemplateConfigDisplay();
                display.Content = printTmp.Content;
                display.Type = printTmp.Type;
            }

            return display;
        }

        public async Task CreateOrUpdate(PrintTemplateConfigSave val)
        {
            var printConfig = await SearchQuery(x => x.Type == val.Type)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.Company)
                .FirstOrDefaultAsync();

            if(printConfig != null)
            {
                printConfig = _mapper.Map(val, printConfig);

                await UpdateAsync(printConfig);
            }
            else
            {
                var res = _mapper.Map<PrintTemplateConfig>(val);
                await CreateAsync(res);
            }
          
        }

        public override ISpecification<PrintTemplateConfig> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.print_template_config_comp_rule":
                    return new InitialSpecification<PrintTemplateConfig>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }

        public async Task<object> getDataTest(string type)
        {
            var comObj = GetService<ICompanyService>();
            var company = await comObj.SearchQuery().Include(x => x.Partner).FirstOrDefaultAsync();
            object obj = new object();

            var partner = new Partner()
            {
                Name = "Nguyễn Văn A",
                DisplayName = "Nguyễn Văn A",
                Street = "Tô ký",
                WardName = "Tân chánh hiệp",
                DistrictName = "Quận 12",
                CityName = "HCM",
                Gender = "male",
                BirthYear = 1997
            };

            var employee = new Employee()
            {
                Name = "Nguyễn Văn B"
            };

            var medicine = new Product()
            {
                Name = "Acetaminophen",
                UOM = new UoM()
                {
                    Name = "Vỉ"
                },
            };

            var toathuocLine = new ToaThuocLine()
            {
                Quantity = 3,
                Product = medicine,
                NumberOfDays = 1,
                NumberOfTimes = 1,
                AmountOfTimes = 1,
                UseAt = "after_meal",
            };

            switch (type)
            {
                case "tmp_toathuoc":

                    var temp = new ToaThuoc()
                    {
                        Company = company,
                        Date = DateTime.Now,
                        Name = "TT310821-00162",
                        Partner = partner,
                        Employee = employee,
                        Lines = new List<ToaThuocLine>() {
                            toathuocLine
                        },
                        Note = "Nhớ ăn uống điều độ",
                        Diagnostic = "Sâu răng",
                        ReExaminationDate = DateTime.Now.AddMonths(6),

                    };
                    obj = _mapper.Map<ToaThuocPrintViewModel>(temp);

                    break;
                case "tmp_medicine_order":
                    var mo = new MedicineOrder()
                    {
                        Company = company,
                        OrderDate = DateTime.Now,
                        Partner = partner,
                        Employee = employee,
                        MedicineOrderLines = new List<MedicineOrderLine>()
                        {
                            new MedicineOrderLine()
                            {
                                Product = medicine,
                                ToaThuocLine = toathuocLine,
                                Quantity = 1,
                                Price = 100000,
                                AmountTotal = 100000
                            }
                        },
                        Amount = 100000,
                        AccountPayment = new AccountPayment() {  Amount = 100000},
                        ToaThuoc = new ToaThuoc()
                        {
                            Note = "Nhớ ăn uống điều độ",
                            Diagnostic = "Sâu răng",
                            ReExaminationDate = DateTime.Now.AddMonths(6),
                        }
                    };
                    obj = _mapper.Map<MedicineOrderPrint>(mo);
                    break;
                default:
                    break;
            }

            return obj;
        }

    }
}
