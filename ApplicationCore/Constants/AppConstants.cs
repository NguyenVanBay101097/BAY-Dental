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
        public const string SaleOrderLineCode = "SaleOrderLine";
        public const string SaleReport = "SaleReport";

    }
}
