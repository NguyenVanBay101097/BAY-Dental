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

        public static string[] PrintTemplateType = new string[] {
            "tmp_sale_order",
            "tmp_account_payment",
            "tmp_account_payment" ,
            "tmp_advisory" ,
            "tmp_partner_advance",
            "tmp_supplier_payment",
            "tmp_phieu_thu",
            "tmp_phieu_chi",
            "tmp_partner_debt",
            "tmp_agent_commission",
            "tmp_purchase_order",
            "tmp_purchase_refund",
            "tmp_quotation",
            "tmp_salary_employee",
            "tmp_salary_advance",
            "tmp_salary",
            "tmp_stock_picking_incoming",
            "tmp_stock_picking_outgoing",
            "tmp_stock_inventory",
            "tmp_toathuoc",
            "tmp_medicine_order",
            "tmp_labo_order",

        };
    }
}
