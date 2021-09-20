using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
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
            var renderLayout = await template.RenderAsync(paper);

            var renderContent = await RenderTemplate(self, resIds);

            var result = ConnectLayoutForContent(renderLayout, renderContent);
            return result;
        }

        public string ConnectLayoutForContent(string layout, string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.SelectSingleNode("//div[@class='container']").InnerHtml += content;
            var newHtml = doc.DocumentNode.OuterHtml;
            return newHtml;
        }

        public async Task<string> RenderTemplate(PrintTemplate self, IEnumerable<Guid> resIds)
        {
            object data = default(object);
            if (resIds.Count() <= 1)
                data = await GetObjectRender(self.Model, resIds.FirstOrDefault());
            else
                data = await GetMutipleObjectRender(self.Model, resIds);

            var template = Template.Parse(self.Content);
            var result = await template.RenderAsync(data);
            return result;
        }

        public async Task<object> GetObjectRender(string model, Guid resId)
        {
            object data = default(object);
            switch (model)
            {
                case "toa.thuoc":
                    {
                        var toaThuocObj = GetService<IToaThuocService>();
                        var res = await toaThuocObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company).ThenInclude(x => x.Partner)
                            .Include(x => x.Partner)
                            .Include(x => x.Employee)
                            .Include(x => x.Lines).ThenInclude(s => s.ProductUoM)
                            .Include(x => x.Lines).ThenInclude(s => s.Product)
                            .FirstOrDefaultAsync();

                        data = _mapper.Map<ToaThuocPrintTemplate>(res);
                        var json = JsonConvert.SerializeObject(data);
                        return data;
                    }
                case "sale.order":
                    {
                        var saleOrderObj = GetService<ISaleOrderService>();
                        data = await saleOrderObj.GetPrintTemplate(resId);
                        return data;
                    }
                case "labo.order":
                    {
                        var laboOrderObj = GetService<ILaboOrderService>();
                        var res = await laboOrderObj.SearchQuery(x => x.Id == resId)
                          .Include(x => x.Company).Include(x => x.Partner)
                          .Include(x => x.Customer)
                          .Include(x => x.LaboBiteJoint).Include(x => x.LaboBridge)
                          .Include(x => x.LaboFinishLine).Include(x => x.SaleOrderLine)
                          .Include(x => x.Product)
                          .Include(x => x.LaboOrderToothRel).ThenInclude(s => s.Tooth)
                          .FirstOrDefaultAsync();

                        data = _mapper.Map<LaboOrderPrintTemplate>(res);
                        return data;
                    }
                case "purchase.order":
                    {
                        var purchaseOrderObj = GetService<IPurchaseOrderService>();
                        var res = await purchaseOrderObj.SearchQuery(x => x.Id == resId)
                          .Include(x => x.Company).Include(x => x.Partner)
                          .Include(x => x.Journal).Include(x => x.CreatedBy)
                          .Include(x => x.Picking)
                          .Include(x => x.OrderLines).ThenInclude(x => x.Product)
                          .Include(x => x.OrderLines).ThenInclude(x => x.ProductUOM)
                          .FirstOrDefaultAsync();

                        data = _mapper.Map<PurchaseOrderPrintTemplate>(res);
                        return data;
                    }
                case "medicine.order":
                    {
                        var medicineOrderObj = GetService<IMedicineOrderService>();
                        var res = await medicineOrderObj.GetPrint(resId);
                        data = _mapper.Map<MedicineOrderPrintTemplate>(res);
                        return data;
                    }
                case "phieu.thu.chi":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        var res = await phieuThuChiObj.GetPrintTemplate(resId);
                        data = _mapper.Map<PhieuThuChiPrintTemplate>(res);
                        return data;
                    }
                case "customer.debt":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        var res = await phieuThuChiObj.GetPrintTemplate(resId);
                        data = _mapper.Map<PhieuThuChiPrintTemplate>(res);
                        return data;
                    }
                case "agent.commission":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        var res = await phieuThuChiObj.GetPrintTemplate(resId);
                        data = _mapper.Map<PhieuThuChiPrintTemplate>(res);
                        return data;
                    }
                case "stock.picking":
                    {
                        var stockPickingObj = GetService<IStockPickingService>();
                        var res = await stockPickingObj.SearchQuery(x => x.Id == resId).Include(x => x.Partner).Include(x => x.PickingType)
                        .Include(x => x.MoveLines).ThenInclude(s => s.Product).ThenInclude(c => c.Categ)
                        .Include(x => x.MoveLines).ThenInclude(s => s.ProductUOM)
                        .Include(x => x.Company).ThenInclude(s => s.Partner)
                        .Include(x => x.CreatedBy)
                        .FirstOrDefaultAsync();

                        data = _mapper.Map<StockPickingPrintTemplate>(res);
                        return data;
                    }
                case "stock.inventory":
                    {
                        var stockInventoryObj = GetService<IStockInventoryService>();
                        var inventory = await stockInventoryObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company)
                            .ThenInclude(s => s.Partner)
                            .Include(x => x.Criteria)
                            .Include(x => x.Location)
                            .Include(x => x.Product)
                            .Include(x => x.Category)
                            .Include(x => x.Lines).ThenInclude(s => s.Product)
                            .Include(x => x.Lines).ThenInclude(s => s.ProductUOM)
                            .Include(x => x.CreatedBy)
                            .FirstOrDefaultAsync();

                        data = _mapper.Map<StockInventoryPrintTemplate>(inventory);
                        return data;
                    }
                case "salary":
                    {
                        var hrPayslipRunObj = GetService<IHrPayslipRunService>();
                        var res = await hrPayslipRunObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company).ThenInclude(s => s.Partner)
                            .Include(x => x.CreatedBy)
                            .Include(x => x.Slips).ThenInclude(x => x.Employee)
                            .Include(x => x.Slips).ThenInclude(x => x.SalaryPayment)
                            .FirstOrDefaultAsync();

                        data = _mapper.Map<HrPayslipRunPrintTemplate>(res);
                        return data;
                    }
                case "partner.advance":
                    {
                        var partnerAdvanceObj = GetService<IPartnerAdvanceService>();
                        var res = await partnerAdvanceObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company)
                            .Include(x => x.Partner)
                            .Include(x => x.Journal)
                            .Include(x => x.CreatedBy)
                            .FirstOrDefaultAsync();

                        data = _mapper.Map<PartnerAdvancePrintTemplate>(res);
                        return data;
                    }
                case "quotation":
                    {
                        var quotationObj = GetService<IQuotationService>();
                        var quotation = await quotationObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Partner)
                            .Include(x => x.Employee)
                            .Include(x => x.Lines)
                            .Include(x => x.Company).ThenInclude(x => x.Partner)
                            .Include(x => x.Payments).FirstOrDefaultAsync();

                        data = _mapper.Map<QuotationPrintTemplate>(quotation);
                        return data;
                    }
                case "sale.order.payment":
                    {
                        var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
                        var payment = await saleOrderPaymentObj.SearchQuery(x => x.Id == resId).Include(x => x.Company.Partner)
                                .Include(x => x.Order.Partner)
                                .Include(x => x.CreatedBy)
                                .Include(x => x.JournalLines).ThenInclude(x => x.Journal)
                                .FirstOrDefaultAsync();

                        data = _mapper.Map<SaleOrderPaymentPrintTemplate>(payment);
                        return data;
                    }
                case "supplier.payment":
                    {
                        var accountPaymentObj = GetService<IAccountPaymentService>();
                        var payment = await accountPaymentObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company.Partner)
                            .Include(x => x.Partner)
                            .Include(x => x.Journal)
                            .Include(x => x.CreatedBy)
                            .Include(x => x.SaleOrderPaymentRels).ThenInclude(s => s.Payment)
                            .FirstOrDefaultAsync();

                        data = _mapper.Map<AccountPaymentPrintTemplate>(payment);
                        return data;
                    }
                case "salary.employee":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        var salaryPayment = await salaryPaymentObj.SearchQuery(x => x.Id == resId)
                            .Include(x => x.Company)
                            .Include(x => x.Employee)
                            .Include(x => x.Journal)
                            .FirstOrDefaultAsync();
                        var salary = _mapper.Map<SalaryPaymentPrintTemplate>(salaryPayment);
                        data = new SalaryPaymentsPrint() { Salaries = new List<SalaryPaymentPrintTemplate>() { salary } };
                        return data;
                    }
                case "salary.advance":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        var salaryPayment = await salaryPaymentObj.SearchQuery(x => x.Id == resId)
                           .Include(x => x.Company)
                           .Include(x => x.Employee)
                           .Include(x => x.Journal)
                           .FirstOrDefaultAsync();
                        var salary = _mapper.Map<SalaryPaymentPrintTemplate>(salaryPayment);
                        data = new SalaryPaymentsPrint() { Salaries = new List<SalaryPaymentPrintTemplate>() { salary } };
                        return data;
                    }
                case "advisory":
                    {
                        var advisoryObj = GetService<IAdvisoryService>();
                        data = await advisoryObj.PrintTemplate(new List<Guid>() { resId });
                        return data;
                    }

            }


            return data;
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
                    return "customer.debt";
                case "tmp_agent_commission":
                    return "agent.commission";
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
                default:
                    return null;
            }
        }
    }
}
