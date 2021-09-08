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

export const keyWords =

{
    'tmp_medicine_order': [
        {
            text: "Thông tin chi nhánh",
            value: [
                { text: 'Logo chi nhánh', value: 'company.logo' },
                { text: 'Tên chi nhánh', value: 'company.name' },
                { text: 'Địa chỉ chi nhánh', value: 'company.address' },
                { text: 'Điện thoại chi nhánh', value: 'company.phone' },
                { text: 'Email chi nhánh', value: 'company.email' },
            ]
        },
        {
            text: "Thông tin hóa đơn",
            value: [
                { text: 'Ngày lập hóa đơn', value: 'order_date.day' },
                { text: 'Tháng lập hóa đơn', value: 'order_date.month' },
                { text: 'Năm lập hóa đơn', value: 'order_date.year' },
                { text: 'Mã hóa đơn', value: 'name' },
                { text: 'Tổng tiền', value: 'amount' },
                { text: 'Tài khoản thanh toán', value: 'account_payment' },
                { text: 'Số tiền thanh toán', value: 'account_payment.amount' },
            ]
        },
        {
            text: "Thông tin khách hàng",
            value: [
                { text: 'Tên khách hàng', value: 'partner.name' },
                { text: 'Địa chỉ khách hàng', value: 'partner.address' },
            ]
        },
        {
            text: "Thông tin bác sĩ",
            value: [
                { text: 'Tên bác sĩ', value: 'employee.name' },
            ]
        },
        {
            text: "Thông tin đơn thuốc",
            value: [
                { text: 'Danh sách đơn thuốc', value: 'medicine_order_lines' },
                { text: 'Tên thuốc', value: 'product.name' },
                { text: 'Ngày uống', value: 'toa_thuoc_line.number_of_times' },
                { text: 'Số lần uống', value: 'toa_thuoc_line.amount_of_times' },
                { text: 'Đơn vị tính', value: 'product.uomname' },
                { text: 'Thời điểm sử dụng thuốc', value: 'toa_thuoc_line.get_use_at_display' },
                { text: 'Số lượng thuốc', value: 'quantity' },
                { text: 'Đơn giá thuốc', value: 'price' },
                { text: 'Thành tiền', value: 'amount_total' },
            ]
        },
        {
            text: "Thông tin khám bệnh",
            value: [
                { text: 'Lời dặn bác sĩ', value: 'toa_thuoc.note' },
                { text: 'Chẩn đoán bệnh', value: 'toa_thuoc.diagnostic' },
                { text: 'Ngày tái khám', value: 'toa_thuoc.re_examination_date' },
            ]
        }
    ],
};