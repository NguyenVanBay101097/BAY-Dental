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

        public static PrintTemplateType[] PrintTemplateType = new PrintTemplateType[] {
           new PrintTemplateType {Type = "tmp_sale_order" , PathTemplate = "~/TMTDentalAPI/Views/SaleOrder/Print.cshtml" },
           new PrintTemplateType {Type = "tmp_account_payment" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_advisory" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_partner_advance" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_supplier_payment" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_agent_commission" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_quotation" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_salary_employee" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_salary_advance" , PathTemplate = "" },
           new PrintTemplateType {Type = "tmp_salary" , PathTemplate = "" },
        };

        public static PrintTemplateType[] PrintTemplateTypeDemo = new PrintTemplateType[] {
           new PrintTemplateType {Type = "tmp_labo_order" , PathTemplate = "PrintTemplate/LaboOrder/Template.html" },
           new PrintTemplateType {Type = "tmp_purchase_order" , PathTemplate = "PrintTemplate/PurchaseOrder/Template.html" },
           new PrintTemplateType {Type = "tmp_purchase_refund" , PathTemplate = "PrintTemplate/PurchaseOrder/Template.html" },
           new PrintTemplateType {Type = "tmp_toathuoc" , PathTemplate = "PrintTemplate/ToaThuoc/Template.html" },       
           new PrintTemplateType {Type = "tmp_medicine_order" , PathTemplate = "PrintTemplate/MedicineOrder/Template.html" },
           new PrintTemplateType {Type = "tmp_phieu_thu" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Phieu_Thu.html" },
           new PrintTemplateType {Type = "tmp_phieu_chi" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Phieu_Chi.html" },
           new PrintTemplateType {Type = "tmp_customer_debt" , PathTemplate = "PrintTemplate/PhieuThuChi/Template_Customer_Debt.html" },
           new PrintTemplateType {Type = "tmp_stock_picking_incoming" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Picking_Incoming.html" },
           new PrintTemplateType {Type = "tmp_stock_picking_outgoing" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Picking_Outgoing.html" },
           new PrintTemplateType {Type = "tmp_stock_inventory" , PathTemplate = "PrintTemplate/StockPicking/Template_Stock_Inventory.html" },
        };
    }

    public class PrintTemplateType
    {
        public string Type { get; set; }
        public string PathTemplate { get; set; }
    }
}
