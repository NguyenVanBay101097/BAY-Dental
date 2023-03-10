using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    //Làm sao xác định được với button in đơn thuốc thì sẽ dùng mẫu in nào? nếu ko tìm thấy dòng nào thì tìm mẫu in mặc định trong bảng PrintTemplate
    public class PrintTemplateConfig : BaseEntity
    {
        /// <summary>
        /// html
        /// Không cần xài nữa
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// tmp_sale_order : phiếu điều trị
        /// tmp_account_payment : biên lai khách hàng thanh toán
        /// tmp_advisory : tình trạng răng
        /// tmp_partner_advance : khách hàng đóng tạm ứng
        /// tmp_partner_refund : khách hàng hoàn tạm ứng
        /// tmp_supplier_payment : biên lai thanh toán nhà cung cấp
        /// tmp_phieu_thu : phiếu thu
        /// tmp_phieu_chi : phiếu chi
        /// tmp_customer_debt : công nợ khách hàng
        /// tmp_agent_commission : phiếu chi hoa hồng
        /// tmp_purchase_order   : phiếu mua hàng
        /// tmp_purchase_refund  : phiếu trả hàng
        /// tmp_quotation : phiếu báo giá
        /// tmp_salary_employee : phiếu thanh toán lương nhán viên
        /// tmp_salary_advance : phiếu tạm ứng
        /// tmp_salary : phiếu chi lương
        /// tmp_stock_picking_incoming :phiếu nhập kho
        /// tmp_stock_picking_outgoing :phiếu xuất kho
        /// tmp_stock_inventory : phiếu kiểm kho
        /// tmp_toathuoc : đơn thuốc
        /// tmp_medicine_order : hóa đơn thuốc
        /// tmp_labo_order : phiếu labo
        /// Dùng để xác định cho button in nào?
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// khổ in
        /// </summary>
        public Guid? PrintPaperSizeId { get; set; }
        public PrintPaperSize PrintPaperSize { get; set; }

        /// <summary>
        /// Mẫu in custom của khách hàng
        /// </summary>
        public Guid? PrintTemplateId { get; set; }
        public PrintTemplate PrintTemplate { get; set; }

        /// <summary>
        /// làm mẫu in mặc định
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
