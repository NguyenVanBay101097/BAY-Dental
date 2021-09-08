export const types: { text: string, value: string }[] = [
    { text: 'Phiếu điều trị', value: 'tmp_sale_order' },
    { text: 'Biên lai khách hàng thanh toán', value: 'tmp_account_payment' },
    { text: 'Tình trạng răng', value: 'tmp_advisory' },
    { text: 'Khách hàng tạm ứng', value: 'tmp_partner_advance' },
    { text: 'Biên lai thanh toán nhà cung cấp', value: 'tmp_supplier_payment' },
    { text: 'Phiếu thu', value: 'tmp_phieu_thu' },
    { text: 'Phiếu chi', value: 'tmp_phieu_chi' },
    { text: 'Công nợ khách hàng', value: 'tmp_partner_debt' },
    { text: 'Phiếu chi hoa hồng', value: 'tmp_agent_commission' },
    { text: 'Phiếu mua hàng', value: 'tmp_purchase_order' },
    { text: 'Phiếu trả hàng', value: 'tmp_purchase_refund' },
    { text: 'Phiếu báo giá', value: 'tmp_quotation' },
    { text: 'Phiếu thanh toán lương nhân viên', value: 'tmp_salary_employee' },
    { text: 'Phiếu tạm ứng', value: 'tmp_salary_advance' },
    { text: 'Phiếu chi lương', value: 'tmp_salary' },
    { text: 'Phiếu nhập kho', value: 'tmp_stock_picking_incoming' },
    { text: 'Phiếu xuất kho', value: 'tmp_stock_picking_outgoing' },
    { text: 'Phiếu kiểm kho', value: 'tmp_stock_inventory' },
    { text: 'Đơn thuốc', value: 'tmp_toathuoc' },
    { text: 'Hóa đơn thuốc', value: 'tmp_medicine_order' },
    { text: 'Phiếu labo', value: 'tmp_labo_order' },
];

export const keyWords: { [key: string]: { [keys: string]: { text: string, value: string }[] }[] } =

{
    'tmp_medicine_order': [
        {
            "Thông tin chi nhánh": [
                { text: 'Ảnh chi nhánh', value: 'company.logo' },
                { text: 'Tên chi nhánh', value: 'company.name' },
                { text: 'Địa chỉ chi nhánh', value: 'company.logo' },
            ]
        }
    ],
};