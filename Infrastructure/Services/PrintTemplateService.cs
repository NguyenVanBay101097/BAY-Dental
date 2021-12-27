using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PrintTemplateService : BaseService<PrintTemplate>, IPrintTemplateService
    {
        private readonly IMapper _mapper;
        public PrintTemplateService(IAsyncRepository<PrintTemplate> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<string> GetPrintTemplate(PrintTemplateDefault val)
        {
            var template = await SearchQuery(x => x.Type == val.Type).FirstOrDefaultAsync();
            if (template == null)
                throw new Exception("Loại mẫu in hiện chưa có mẫu sẵn");

            return template.Content;
        }

        public async Task<string> GeneratePrintHtml(PrintTemplate self, IEnumerable<Guid> resIds, PrintPaperSize paperSize = null)
        {
            var modelDataService = GetService<IIRModelDataService>();
            var paper = paperSize != null ? paperSize : await modelDataService.GetRef<PrintPaperSize>("base.paperformat_a4");
            if (paper == null)
                throw new Exception("Không tìm thấy khổ giấy mặc định");

            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            var template = Template.Parse(layoutHtml);
            var renderLayout = await template.RenderAsync(new { o = paper });

            var renderContent = await RenderTemplate(self, resIds);

            var result = ConnectLayoutForContent(renderLayout, renderContent);
            return result;
        }

        public async Task<string> GeneratePrintHtml(PrintTemplate self, IEnumerable<object> data, PrintPaperSize paperSize = null)
        {
            var modelDataService = GetService<IIRModelDataService>();
            var paper = paperSize != null ? paperSize : await modelDataService.GetRef<PrintPaperSize>("base.paperformat_a4");
            if (paper == null)
                throw new Exception("Không tìm thấy khổ giấy mặc định");

            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            var template = Template.Parse(layoutHtml);
            var renderLayout = await template.RenderAsync(new { o = paper });

            var renderContent = await RenderTemplate(self, data);

            var result = ConnectLayoutForContent(renderLayout, renderContent);
            return result;
        }

        public string ConnectLayoutForContent(string layout, string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.SelectSingleNode("//body").InnerHtml += content;
            var newHtml = doc.DocumentNode.OuterHtml;
            return newHtml;
        }

        public async Task<string> RenderTemplate(PrintTemplate self, IEnumerable<Guid> resIds)
        {
            //mảng data từ resids
            var data = await GetObjectRender(self.Model, resIds);
            return await RenderTemplate(self, data);
        }

        public async Task<string> RenderTemplate(PrintTemplate self, IEnumerable<object> data)
        {
            //mảng data từ resids
            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();

            var results = "";
            foreach (var item in data)
            {
                var scriptObject = new ScriptObject();
                scriptObject.Add("o", item);
                scriptObject.Add("u", user);

                var context = new TemplateContext();
                context.PushCulture(CultureInfo.CurrentCulture);
                context.PushGlobal(scriptObject);
                //làm sao page break
                var template = Template.Parse(self.Content);
                var result = await template.RenderAsync(context);
                var tmp = $"<div class=\"page-break\">{result}</div>";
                results += tmp;
            }

            return results;
        }

        public async Task<IEnumerable<object>> GetObjectRender(string model, IEnumerable<Guid> resIds)
        {
            switch (model)
            {
                case "toa.thuoc":
                    {
                        var toaThuocObj = GetService<IToaThuocService>();
                        var res = await toaThuocObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company).ThenInclude(x => x.Partner)
                            .Include(x => x.Partner)
                            .Include(x => x.Employee)
                            .Include(x => x.Lines).ThenInclude(s => s.ProductUoM)
                            .Include(x => x.Lines).ThenInclude(s => s.Product)
                            .ToListAsync();

                        return res;
                    }
                case "sale.order":
                    {
                        var saleOrderObj = GetService<ISaleOrderService>();
                        var res = await saleOrderObj.GetPrintTemplate(resIds);
                        return res;
                    }
                case "labo.order":
                    {
                        var laboOrderObj = GetService<ILaboOrderService>();
                        var res = await laboOrderObj.SearchQuery(x => resIds.Contains(x.Id))
                          .Include(x => x.Company).Include(x => x.Partner)
                          .Include(x => x.Customer)
                          .Include(x => x.LaboBiteJoint).Include(x => x.LaboBridge)
                          .Include(x => x.LaboFinishLine).Include(x => x.SaleOrderLine)
                          .Include(x => x.Product)
                          .Include(x => x.LaboOrderToothRel).ThenInclude(s => s.Tooth)
                          .Include(x => x.LaboOrderProductRel).ThenInclude(s => s.Product)
                          .Include(x => x.SaleOrderLine).ThenInclude(x => x.Employee)
                          .Include(x => x.SaleOrderLine).ThenInclude(x => x.Order)
                          .Include(x => x.SaleOrderLine).ThenInclude(x => x.Product)
                          .ToListAsync();

                        return res;
                    }
                case "purchase.order":
                    {
                        var purchaseOrderObj = GetService<IPurchaseOrderService>();
                        var res = await purchaseOrderObj.SearchQuery(x => resIds.Contains(x.Id))
                          .Include(x => x.Company).Include(x => x.Partner)
                          .Include(x => x.Journal).Include(x => x.CreatedBy)
                          .Include(x => x.Picking)
                          .Include(x => x.OrderLines).ThenInclude(x => x.Product)
                          .Include(x => x.OrderLines).ThenInclude(x => x.ProductUOM)
                          .ToListAsync();

                        return res;
                    }
                case "medicine.order":
                    {
                        var medicineOrderObj = GetService<IMedicineOrderService>();
                        var res = await medicineOrderObj.GetPrint(resIds);
                        return res;
                    }
                case "phieu.thu.chi":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        var res = await phieuThuChiObj.GetPrintTemplate(resIds);

                        return res;
                    }
                case "stock.picking":
                    {
                        var stockPickingObj = GetService<IStockPickingService>();
                        var res = await stockPickingObj.SearchQuery(x => resIds.Contains(x.Id)).Include(x => x.Partner).Include(x => x.PickingType)
                        .Include(x => x.MoveLines).ThenInclude(s => s.Product).ThenInclude(c => c.Categ)
                        .Include(x => x.MoveLines).ThenInclude(s => s.ProductUOM)
                        .Include(x => x.Company).ThenInclude(s => s.Partner)
                        .Include(x => x.CreatedBy)
                        .ToListAsync();

                        return res;
                    }
                case "stock.inventory":
                    {
                        var stockInventoryObj = GetService<IStockInventoryService>();
                        var inventories = await stockInventoryObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company)
                            .ThenInclude(s => s.Partner)
                            .Include(x => x.Criteria)
                            .Include(x => x.Location)
                            .Include(x => x.Product)
                            .Include(x => x.Category)
                            .Include(x => x.Lines).ThenInclude(s => s.Product)
                            .Include(x => x.Lines).ThenInclude(s => s.ProductUOM)
                            .Include(x => x.CreatedBy)
                            .ToListAsync();


                        return inventories;
                    }
                case "salary":
                    {
                        var hrPayslipObj = GetService<IHrPayslipService>();
                        var res = await hrPayslipObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company).ThenInclude(s => s.Partner)
                            .Include(x => x.CreatedBy)
                            .Include(x => x.PayslipRun)
                            .Include(x => x.Employee)
                            .Include(x => x.SalaryPayment)
                            .ToListAsync();

                        var display = _mapper.Map<HrPayslipDisplay>(res.FirstOrDefault());
                        var json = JsonConvert.SerializeObject(display);

                        return res;
                    }
                case "partner.advance":
                    {
                        var partnerAdvanceObj = GetService<IPartnerAdvanceService>();
                        var res = await partnerAdvanceObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company)
                            .Include(x => x.Partner)
                            .Include(x => x.Journal)
                            .Include(x => x.CreatedBy)
                            .ToListAsync();

                        return res;
                    }
                case "quotation":
                    {
                        var quotationObj = GetService<IQuotationService>();
                        var quotations = await quotationObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Partner)
                            .Include(x => x.Employee)
                            .Include(x => x.Lines)
                            .Include(x => x.Company).ThenInclude(x => x.Partner)
                            .Include(x => x.Lines).ThenInclude(x => x.ProductUOM)
                            .Include(x => x.Payments).ToListAsync();

                        return quotations;
                    }
                case "sale.order.payment":
                    {
                        var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
                        var payments = await saleOrderPaymentObj.SearchQuery(x => resIds.Contains(x.Id)).Include(x => x.Company.Partner)
                                .Include(x => x.Order.Partner)
                                .Include(x => x.CreatedBy)
                                .Include(x => x.JournalLines).ThenInclude(x => x.Journal)
                                .Include(x => x.Lines).ThenInclude(x => x.SaleOrderLine)
                                .ToListAsync();
                        return payments;
                    }
                case "supplier.payment":
                    {
                        var accountPaymentObj = GetService<IAccountPaymentService>();
                        var payments = await accountPaymentObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company.Partner)
                            .Include(x => x.Partner)
                            .Include(x => x.Journal)
                            .Include(x => x.CreatedBy)
                            .Include(x => x.SaleOrderPaymentRels).ThenInclude(s => s.Payment)
                            .ToListAsync();

                        return payments;
                    }
                case "salary.payment":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        var salaryPayments = await salaryPaymentObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company).ThenInclude(x => x.Partner)
                            .Include(x => x.Employee)
                            .Include(x => x.Journal)
                            .ToListAsync();

                        return salaryPayments;
                    }
                case "appointment":
                    {
                        var appObj = GetService<IAppointmentService>();
                        var app = await appObj.SearchQuery(x => resIds.Contains(x.Id))
                                 .Include(x => x.Company).ThenInclude(x => x.Partner)
                                 .Include(x => x.Partner)
                                 .Include(x => x.Doctor)
                                 .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                                 .ToListAsync();
                        return app;
                    }
                //case "advisory":
                //    {
                //        var advisoryObj = GetService<IAdvisoryService>();
                //        data = await advisoryObj.PrintTemplate(resIds);
                //        return data;
                //    }
                default:
                    return null;
            }
        }

        public async Task<object> GetMutipleObjectRender(string model, IEnumerable<Guid> resIds)
        {
            object data = default(object);
            switch (model)
            {
                case "salary.employee":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        var salaryPayments = await salaryPaymentObj.SearchQuery(x => resIds.Contains(x.Id))
                            .Include(x => x.Company)
                            .Include(x => x.Employee)
                            .Include(x => x.Journal)
                            .ToListAsync();
                        var salaries = _mapper.Map<IEnumerable<SalaryPaymentPrintTemplate>>(salaryPayments);
                        data = new SalaryPaymentsPrint() { Salaries = salaries };
                        return data;
                    }
                case "salary.advance":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        var salaryPayments = await salaryPaymentObj.SearchQuery(x => resIds.Contains(x.Id))
                           .Include(x => x.Company)
                           .Include(x => x.Employee)
                           .Include(x => x.Journal)
                           .ToListAsync();
                        var salaries = _mapper.Map<IEnumerable<SalaryPaymentPrintTemplate>>(salaryPayments);
                        data = new SalaryPaymentsPrint() { Salaries = salaries };
                        return data;
                    }
                case "advisory":
                    {
                        var advisoryObj = GetService<IAdvisoryService>();
                        data = await advisoryObj.PrintTemplate(resIds);
                        return data;
                    }

            }

            return data;
        }

        public string GetModelTemplate(string type)
        {
            switch (type)
            {
                case "tmp_toathuoc":
                    return "toa.thuoc";
                case "tmp_sale_order":
                    return "sale.order";
                case "tmp_labo_order":
                    return "labo.order";
                case "tmp_purchase_order":
                    return "purchase.order";
                case "tmp_purchase_refund":
                    return "purchase.order";
                case "tmp_medicine_order":
                    return "medicine.order";
                case "tmp_phieu_thu":
                    return "phieu.thu.chi";
                case "tmp_phieu_chi":
                    return "phieu.thu.chi";
                case "tmp_customer_debt":
                    return "phieu.thu.chi";
                case "tmp_agent_commission":
                    return "phieu.thu.chi";
                case "tmp_stock_picking_incoming":
                    return "stock.picking";
                case "tmp_stock_picking_outgoing":
                    return "stock.picking";
                case "tmp_stock_inventory":
                    return "stock.inventory";
                case "tmp_salary_employee":
                    return "salary.employee";
                case "tmp_salary_advance":
                    return "salary.advance";
                case "tmp_salary":
                    return "salary";
                case "tmp_partner_advance":
                    return "partner.advance";
                case "tmp_partner_refund":
                    return "partner.advance";
                case "tmp_quotation":
                    return "quotation";
                case "tmp_account_payment":
                    return "sale.order.payment";
                case "tmp_supplier_payment":
                    return "supplier.payment";
                case "tmp_supplier_payment_inbound":
                    return "supplier.payment";
                case "tmp_advisory":
                    return "advisory";
                case "tmp_appointment":
                    return "appointment";
                default:
                    return null;

            }
        }

        public async Task<PrintTemplate> GetDefaultTemplate(string type)
        {
            var modelDataService = GetService<IIRModelDataService>();
            switch (type)
            {
                case "tmp_toathuoc":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_toa_thuoc");
                case "tmp_sale_order":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_sale_order");
                case "tmp_labo_order":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_labo_orde");
                case "tmp_purchase_order":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_purchase_order");
                case "tmp_purchase_refund":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_purchase_refund");
                case "tmp_medicine_order":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_medicine_order");
                case "tmp_phieu_thu":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_phieu_thu");
                case "tmp_phieu_chi":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_phieu_chi");
                case "tmp_customer_debt":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_customer_debt");
                case "tmp_agent_commission":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_agent_commission");
                case "tmp_stock_picking_incoming":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_stock_picking_incoming");
                case "tmp_stock_picking_outgoing":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_stock_picking_outgoing");
                case "tmp_stock_inventory":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_stock_inventory");
                case "tmp_salary_employee":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_salary_employee");
                case "tmp_salary_advance":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_salary_advance");
                case "tmp_salary":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_salary");
                case "tmp_partner_advance":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_partner_advance");
                case "tmp_partner_refund":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_partner_refund");
                case "tmp_quotation":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_quotation");
                case "tmp_account_payment":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_account_payment");
                case "tmp_supplier_payment":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_supplier_payment");
                case "tmp_supplier_payment_inbound":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_supplier_payment_inbound");
                case "tmp_advisory":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_advisory");
                case "tmp_appointment":
                    return await modelDataService.GetRef<PrintTemplate>("base.print_template_appointment");
                default:
                    return null;
            }
        }
    }
}
