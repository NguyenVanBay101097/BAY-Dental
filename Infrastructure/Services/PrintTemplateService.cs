using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task<string> GeneratePrintHtml(PrintTemplate self, Guid resId, PrintPaperSize paperSize = null)
        {
            var modelDataService = GetService<IIRModelDataService>();
            var paper = paperSize != null ? paperSize : await modelDataService.GetRef<PrintPaperSize>("base.paperformat_a4");
            if (paper == null)
                throw new Exception("Không tìm thấy khổ giấy mặc định");

            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            var template = Template.Parse(layoutHtml);
            var renderLayout = await template.RenderAsync(paper);

            var renderContent = await RenderTemplate(self, resId);

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

        public async Task<string> RenderTemplate(PrintTemplate self, Guid resId)
        {
            var data = await GetObjectRender(self.Model, resId);
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
                        data = await _mapper.ProjectTo<ToaThuocPrintTemplate>(toaThuocObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "sale.order":
                    {
                        var saleOrderObj = GetService<ISaleOrderService>();
                        data = await _mapper.ProjectTo<SaleOrderPrintTemplate>(saleOrderObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "labo.order":
                    {
                        var laboOrderObj = GetService<ILaboOrderService>();
                        data = await _mapper.ProjectTo<LaboOrderPrintTemplate>(laboOrderObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "purchase.order":
                    {
                        var purchaseOrderObj = GetService<IPurchaseOrderService>();
                        data = await _mapper.ProjectTo<PurchaseOrderPrintTemplate>(purchaseOrderObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "medicine.order":
                    {
                        var medicineOrderObj = GetService<IMedicineOrderService>();
                        data = await _mapper.ProjectTo<MedicineOrderPrintTemplate>(medicineOrderObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "phieu.thu.chi":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        data = await _mapper.ProjectTo<PhieuThuChiPrintTemplate>(phieuThuChiObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "customer.debt":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        data = await _mapper.ProjectTo<PhieuThuChiPrintTemplate>(phieuThuChiObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "agent.commission":
                    {
                        var phieuThuChiObj = GetService<IPhieuThuChiService>();
                        data = await _mapper.ProjectTo<PhieuThuChiPrintTemplate>(phieuThuChiObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "stock.picking":
                    {
                        var stockPickingObj = GetService<IStockPickingService>();
                        data = await _mapper.ProjectTo<StockPickingPrintTemplate>(stockPickingObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "stock.inventory":
                    {
                        var stockInventoryObj = GetService<IStockInventoryService>();
                        data = await _mapper.ProjectTo<StockInventoryPrintTemplate>(stockInventoryObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "salary.employee":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        data = await _mapper.ProjectTo<SalaryPaymentPrintTemplate>(salaryPaymentObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "salary.advance":
                    {
                        var salaryPaymentObj = GetService<ISalaryPaymentService>();
                        data = await _mapper.ProjectTo<SalaryPaymentPrintTemplate>(salaryPaymentObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "salary":
                    {
                        var hrPayslipRunObj = GetService<IHrPayslipRunService>();
                        data = await _mapper.ProjectTo<HrPayslipRunPrintTemplate>(hrPayslipRunObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "partner.advance":
                    {
                        var partnerAdvanceObj = GetService<IPartnerAdvanceService>();
                        data = await _mapper.ProjectTo<PartnerAdvancePrintTemplate>(partnerAdvanceObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "quotation":
                    {
                        var quotationObj = GetService<IQuotationService>();
                        data = await _mapper.ProjectTo<QuotationPrintTemplate>(quotationObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "sale.order.payment":
                    {
                        var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
                        data = await _mapper.ProjectTo<SaleOrderPaymentPrintTemplate>(saleOrderPaymentObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "supplier.payment":
                    {
                        var accountPaymentObj = GetService<IAccountPaymentService>();
                        data = await _mapper.ProjectTo<AccountPaymentPrintTemplate>(accountPaymentObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
                        return data;
                    }
                case "advisory":
                    {
                        var advisoryObj = GetService<IAdvisoryService>();
                        data = await _mapper.ProjectTo<SaleOrderPrintTemplate>(advisoryObj.SearchQuery(x => x.Id == resId)).FirstOrDefaultAsync();
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
