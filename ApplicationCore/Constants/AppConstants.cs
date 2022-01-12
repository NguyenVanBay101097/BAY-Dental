using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Constants
{
    public static class AppConstants
    {
        public const string LockRequestKeyTemplate = "{0}_{1}";

        public static string GetLockRequestKey(string host, string name)
        {
            host = host.Replace(".", "_");
            return string.Format(LockRequestKeyTemplate, host, name);
        }


        /// <summary>
        /// code của mẫu in
        /// </summary>
        public const string SaleOrderPaperCode = "SaleOrder";
        public const string PaymentPaperCode = "Payment";
        public const string SalaryEmployeePaperCode = "SalaryEmployee";
        public const string SalaryPaymentPaperCode = "SalaryPayment";
        public const string LaboOrderPaperCode = "LaboOrder";
        public const string ToaThuocPaperCode = "ToaThuoc";
        public const string MedicineOrderPaperCode = "MedicineOrder";
        public const string PhieuThuChiPaperCode = "PhieuThuChi";
        public const string StockPickingPaperCode = "StockPicking";
        public const string StockInventoryPaperCode = "StockInventory";
        public const string SaleOrderPaymentPaperCode = "SaleOrderPayment";
        public const string PurchaseOrderPaperCode = "PurchaseOrder";
        public const string QuotationPaperCode = "Quotation";
        public const string RevenueTimeReport = "RevenueTimeReport";
        public const string RevenueServiceReport = "RevenueServiceReport";
        public const string RevenueEmployeeReport = "RevenueEmployeeReport";
        public const string RevenueReport = "RevenueReport";
        public const string AdvisoryPaperCode = "Advisory";
        public const string SaleOrderLineCode = "SaleOrderLine";
        public const string SaleReport = "SaleReport";
        public const string RevenuePartnerReport = "RevenuePartnerReport";
        public const string PartnerOldNewReport = "PartnerOldNewReport";
        public const string ReportPartnerDebit = "ReportPartnerDebit";
        public const string ReportInsuranceDebt = "ReportInsuranceDebt";
        public const string ServiceOverviewReport = "ServiceOverviewReport";
        public const string DoneSurveyAssignmentReport = "DoneSurveyAssignmentReport";


        public static PrintTemplateType[] PrintTemplateTypeDemo = new PrintTemplateType[] {
           new PrintTemplateType {Type = "tmp_sale_order" , PathTemplate = "PrintTemplate/SaleOrder/Template.html" , Model = "sale.order", NameIRModel = "print_template_sale_order" },
           new PrintTemplateType {Type = "tmp_labo_order" , PathTemplate = "PrintTemplate/LaboOrder/Template.html" , Model = "labo.order", NameIRModel = "print_template_labo_orde" },
           new PrintTemplateType {Type = "tmp_purchase_order" , PathTemplate = "PrintTemplate/PurchaseOrder/Template_order.html", Model = "purchase.order" , NameIRModel = "print_template_purchase_order" },
           new PrintTemplateType {Type = "tmp_purchase_refund" , PathTemplate = "PrintTemplate/PurchaseOrder/Template_refund.html", Model = "purchase.order" , NameIRModel = "print_template_purchase_refund" },
           new PrintTemplateType {Type = "tmp_toathuoc" , PathTemplate = "PrintTemplate/ToaThuoc/Template.html", Model = "toa.thuoc" , NameIRModel = "print_template_toa_thuoc" },       
           new PrintTemplateType {Type = "tmp_medicine_order" , PathTemplate = "PrintTemplate/MedicineOrder/Template.html", Model = "medicine.order" , NameIRModel = "print_template_medicine_order" },
           new PrintTemplateType {Type = "tmp_phieu_thu" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Phieu_Thu.html", Model = "phieu.thu.chi" , NameIRModel = "print_template_phieu_thu" },
           new PrintTemplateType {Type = "tmp_phieu_chi" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Phieu_Chi.html", Model = "phieu.thu.chi" , NameIRModel = "print_template_phieu_chi" },
           new PrintTemplateType {Type = "tmp_customer_debt" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Customer_Debt.html", Model = "phieu.thu.chi" , NameIRModel = "print_template_customer_debt" },
           new PrintTemplateType {Type = "tmp_agent_commission" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Agent_Commission.html", Model = "phieu.thu.chi" , NameIRModel = "print_template_agent_commission" },
           new PrintTemplateType {Type = "tmp_stock_picking_incoming" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Picking_Incoming.html", Model = "stock.picking" , NameIRModel = "print_template_stock_picking_incoming" },
           new PrintTemplateType {Type = "tmp_stock_picking_outgoing" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Picking_Outgoing.html", Model = "stock.picking" , NameIRModel = "print_template_stock_picking_outgoing" },
           new PrintTemplateType {Type = "tmp_stock_inventory" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Inventory.html", Model = "stock.inventory" , NameIRModel = "print_template_stock_inventory" },
           new PrintTemplateType {Type = "tmp_salary_employee" , PathTemplate = "PrintTemplate/Salary/Template_Salary_Employee.html", Model = "salary.payment" , NameIRModel = "print_template_salary_employee" },
           new PrintTemplateType {Type = "tmp_salary_advance" , PathTemplate = "PrintTemplate/Salary/Template_Salary_Advance.html", Model = "salary.payment" , NameIRModel = "print_template_salary_advance" },
           new PrintTemplateType {Type = "tmp_salary" , PathTemplate = "PrintTemplate/Salary/Template_Salary.html", Model = "salary" , NameIRModel = "print_template_salary" },
           new PrintTemplateType {Type = "tmp_partner_advance" , PathTemplate = "PrintTemplate/PartnerAdvance/Template_advance.html", Model = "partner.advance" , NameIRModel = "print_template_partner_advance" },
           new PrintTemplateType {Type = "tmp_partner_refund" , PathTemplate = "PrintTemplate/PartnerAdvance/Template_refund.html", Model = "partner.advance" , NameIRModel = "print_template_partner_refund" },
           new PrintTemplateType {Type = "tmp_quotation" , PathTemplate = "PrintTemplate/Quotation/Template.html", Model = "quotation" , NameIRModel = "print_template_quotation" },
           new PrintTemplateType {Type = "tmp_account_payment" , PathTemplate = "PrintTemplate/AccountPayment/Template_partner_payment.html", Model = "sale.order.payment" , NameIRModel = "print_template_account_payment" },       
           new PrintTemplateType {Type = "tmp_supplier_payment" , PathTemplate = "PrintTemplate/AccountPayment/Template_supplier_payment.html", Model = "supplier.payment" , NameIRModel = "print_template_supplier_payment" },
           new PrintTemplateType {Type = "tmp_supplier_payment_inbound" , PathTemplate = "PrintTemplate/AccountPayment/Template_supplier_payment_inbound.html", Model = "supplier.payment" , NameIRModel = "print_template_supplier_payment_inbound" },
           new PrintTemplateType {Type = "tmp_advisory" , PathTemplate = "PrintTemplate/Advisory/Template.html", Model = "advisory" , NameIRModel = "print_template_advisory" },
           new PrintTemplateType {Type = "tmp_appointment" , PathTemplate = "PrintTemplate/Appointment/Template.html", Model = "appointment" , NameIRModel = "print_template_appointment" },
        };
    }

    public class PrintTemplateType
    {
        public string Type { get; set; }
        public string PathTemplate { get; set; }

        public string Model { get; set; }

        public string NameIRModel { get; set; }

    }
}
