export const types: { text: string, value: string }[] = [
    { text: 'Đơn thuốc', value: 'tmp_toathuoc' },
    { text: 'Hóa đơn thuốc', value: 'tmp_medicine_order' },
    { text: 'Phiếu labo', value: 'tmp_labo_order' },
    { text: 'Phiếu điều trị', value: 'tmp_sale_order' },
    { text: 'Biên lai thanh toán điều trị', value: 'tmp_account_payment' },
    // { text: 'Tình trạng răng', value: 'tmp_advisory' },
    { text: 'Khách hàng đóng tạm ứng', value: 'tmp_partner_advance' },
    { text: 'Khách hàng hoàn tạm ứng', value: 'tmp_partner_refund' },
    // { text: 'Biên lai thanh toán nhà cung cấp', value: 'tmp_supplier_payment' },
    // { text: 'Biên lai nhà cung cấp hoàn tiền', value: 'tmp_supplier_payment_inbound' },
    // { text: 'Phiếu thu', value: 'tmp_phieu_thu' },
    // { text: 'Phiếu chi', value: 'tmp_phieu_chi' },
    { text: 'Phiếu thu công nợ khách hàng', value: 'tmp_customer_debt' },
    // { text: 'Phiếu chi hoa hồng', value: 'tmp_agent_commission' },
    { text: 'Phiếu mua hàng', value: 'tmp_purchase_order' },
    { text: 'Phiếu trả hàng', value: 'tmp_purchase_refund' },
    { text: 'Phiếu báo giá', value: 'tmp_quotation' },
    { text: 'Phiếu thanh toán lương nhân viên', value: 'tmp_salary' },
    { text: 'Phiếu tạm ứng lương', value: 'tmp_salary_advance' },
    { text: 'Phiếu chi lương', value: 'tmp_salary_employee' },
    { text: 'Phiếu nhập kho', value: 'tmp_stock_picking_incoming' },
    { text: 'Phiếu xuất kho', value: 'tmp_stock_picking_outgoing' },
    { text: 'Phiếu kiểm kho', value: 'tmp_stock_inventory' },

];

let companyInfo =
{
    text: "Thông tin chi nhánh",
    value: [
        { text: 'Logo chi nhánh', value: '{{o.company.logo}}' },
        { text: 'Tên chi nhánh', value: '{{o.company.name}}' },
        { text: 'Địa chỉ chi nhánh', value: '{{o.company.address}}' },
        { text: 'Điện thoại chi nhánh', value: '{{o.company.phone}}' },
        { text: 'Email chi nhánh', value: '{{o.company.email}}' },
    ]
};

let pipes =
{
    text: "Một số định dạng hỗ trợ",
    value: [
        { text: 'Ngày hôm nay', value: '{{date.now}}' },
        { text: 'định dạng số', value: '| math.format "N0"' },
        { text: 'định dạng ngày/tháng/năm', value: `| date.to_string '%d/%m/%Y'` },
        { text: 'định dạng lấy ngày', value: `| date.to_string '%d'` },
        { text: 'định dạng lấy tháng', value: `| date.to_string '%m'` },
        { text: 'định dạng lấy năm', value: `| date.to_string '%Y'` },
        { text: 'định dạng chữ hoa', value: '| string.upcase' },
        { text: 'định dạng chữ thường', value: '| string.downcase' },
        { text: 'định dạng danh sách', value: '{{for line in <Mã danh sách>}}\n<nội dung>\n{{end}}' },
        { text: 'định dạng điều kiện', value: '{{if <điều kiện>}}\n<nội dung>\n{{else}}\n<nội dung>\n{{end}}' },
    ],
    links: [
        { text: 'Link tham khảo 1', value: `https://github.com/scriban/scriban/blob/master/doc/language.md` },
        { text: 'Link tham khảo 2', value: `https://github.com/scriban/scriban/blob/master/doc/builtins.md` },
    ]
};

const keyWorDatas =

{
    'tmp_medicine_order': [
        companyInfo,//info key of company
        {
            text: "Thông tin phiếu",
            value: [
                { text: 'Ngày lập hóa đơn', value: '{{o.order_date.day}}' },
                { text: 'Tháng lập hóa đơn', value: '{{o.order_date.month}}' },
                { text: 'Năm lập hóa đơn', value: '{{o.order_date.year}}' },
                { text: 'Mã hóa đơn', value: '{{o.name}}' },
                { text: 'Thanh toán', value: '{{o.account_payment.amount}}' },
                { text: 'Tổng tiền', value: '{{o.amount}}' },
            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Tên khách hàng', value: '{{o.partner.name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.partner.address}}' },
                { text: 'Tên bác sĩ', value: '{{o.employee.name}}' },
                { text: 'Lời dặn bác sĩ', value: '{{o.toa_thuoc.note}}' },
                { text: 'Chẩn đoán bệnh', value: '{{o.toa_thuoc.diagnostic}}' },
                { text: 'Ngày tái khám', value: '{{o.toa_thuoc.re_examination_date}}' },
            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Danh sách thuốc', value: 'o.medicine_order_lines' },
                { text: 'Tên thuốc', value: '{{line.product.name}}' },
                { text: 'Số lần uống/ngày', value: '{{line.toa_thuoc_line.number_of_times}}' },
                { text: 'Số lượng uống/lần', value: '{{line.toa_thuoc_line.amount_of_times}}' },
                { text: 'Đơn vị mỗi lần uống', value: '{{line.product.uomname}}' },
                { text: 'Thời điểm uống', value: '{{line.toa_thuoc_line.get_use_at_display}}' },
                { text: 'Số lượng thuốc', value: '{{line.quantity}}' },
                { text: 'Đơn giá thuốc', value: '{{line.price}}' },
                { text: 'Thành tiền', value: '{{line.amount_total}}' },
            ]
        }
    ],

    'tmp_toathuoc': [
        companyInfo,//info key of company
        {
            text: "Thông tin phiếu",
            value: [
                { text: 'Ngày lập đơn', value: '{{o.date_created.day}}' },
                { text: 'Tháng lập đơn', value: '{{o.date_created.month}}' },
                { text: 'Năm lập đơn', value: '{{o.date_created.year}}' },
                { text: 'Mã đơn', value: '{{o.name}}' },
            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Lời dặn', value: '{{o.note}}' },
                { text: 'Chấn đoán bệnh', value: '{{o.diagnostic}}' },
                { text: 'Ngày tái khám', value: '{{o.re_examination_date}}' },
                { text: 'Tên khách hàng', value: '{{o.partner.display_name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.partner.address}}' },
                { text: 'Giới tính khách hàng', value: '{{o.partner.get_gender}}' },
                { text: 'Tuổi khách hàng', value: '{{partner.get_age}}' },
                { text: 'Tên bác sĩ', value: '{{o.employee.name}}' },
            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Danh sách thuốc', value: 'o.lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Tên thuốc', value: '{{line.product_name}}' },
                { text: 'Số lượng', value: '{{line.quantity}}' },
                { text: 'Số lần uống/ngày', value: '{{line.number_of_times}}' },
                { text: 'Số lượng uống/lần', value: '{{line.amount_of_times}}' },
                { text: 'Đơn vị mỗi lần uống', value: '{{line.product_uo_m.name}}' },
                { text: 'Số ngày uống', value: '{{line.number_of_days}}' },
                { text: 'Thời điểm uống', value: '{{line.use_at_display}}' },
            ]
        }
    ],

    'tmp_labo_order': [
        companyInfo,//info key of company
        {
            text: "Thông tin đặt hàng",
            value: [
                { text: 'Ngày đặt hàng', value: '{{o.date_order.day}}' },
                { text: 'Tháng đặt hàng', value: '{{o.date_order.month}}' },
                { text: 'Năm đặt hàng', value: '{{o.date_order.year}}' },
                { text: 'Mã đặt hàng', value: '{{o.name}}' },

            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Tên bác sĩ', value: '{{o.sale_order_line.employee.name}}' },
                { text: 'Số phiếu điều trị', value: '{{o.sale_order_line.order.name }}' },
                { text: 'Tên khách hàng', value: '{{o.customer.name}}' },
                { text: 'Tên nhà cung cấp', value: '{{o.partner.name}}' },
                { text: 'Ngày gửu', value: '{{o.date_order}}' },
                { text: 'Ngày nhận dự kiến', value: '{{o.date_planned}}' },

            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Loại phục hình', value: '{{o.sale_order_line.name}}' },
                { text: 'Hãng', value: '{{o.sale_order_line.product.firm}}' },
                { text: 'Răng', value: '{{o.teeth_display}' },
                { text: 'Màu sắc', value: '{{o.color}' },
                { text: 'Số lượng', value: '{{o.quantity}' },
                { text: 'Chỉ định', value: '{{o.indicated}' },
                { text: 'Ghi chú', value: '{{o.note}' },
            ]
        },
        {
            text: "Thông số Labo",
            value: [
                { text: 'Vật liệu', value: '{{o.product.name}}' },
                { text: 'Đường hoàn tất', value: '{{o.labo_finish_line.name}}' },
                { text: 'Khớp cắn', value: '{{o.labo_bite_joint.name}}' },
                { text: 'Kiểu nhịp', value: '{{o.labo_bridge.name}}' },
                { text: 'Gửu kèm', value: '{{o.labo_order_products_display}}' },
                { text: 'Ghi chú kỹ thuật', value: '{{o.technical_note}}' },
                { text: 'Mã bảo hành', value: '{{o.warranty_code}}' },
                { text: 'Hạn bảo hành', value: '{{o.warranty_period}}' },
            ]
        }
    ],
    'tmp_purchase_order': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date_order.day}}' },
                { text: 'Tháng tạo', value: '{{o.date_order.month}}' },
                { text: 'Năm tạo', value: '{{o.date_order.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
                { text: 'Tổng tiền', value: '{{o.amount_total}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên nhà cung cấp', value: '{{o.partner.name}}' },
                { text: 'Mã tham chiếu kho', value: '{{o.picking.name }}' },
                { text: 'Ghi chú', value: '{{o.note}}' },
                { text: 'Người lập phiếu', value: '{{u.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách hàng hóa', value: 'order_lines' },
                { text: 'Số thứ tự', value: '{{line.sequence}}' },
                { text: 'Tên hàng hóa', value: '{{line.name}}' },
                { text: 'Số lượng', value: '{{line.product_qty}}' },
                { text: 'Đơn vị', value: '{{line.product_uomname}}' },
                { text: 'Đơn giá', value: '{{line.price_unit}}' },
                { text: 'Chiết khấu (%)', value: '{{line.discount}}' },
                { text: 'Thành tiền', value: '{{line.price_subtotal}}' },
            ]
        },
    ],
    'tmp_purchase_refund': [],
    'tmp_customer_debt': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date_created.day}}' },
                { text: 'Tháng tạo', value: '{{o.date_created.month}}' },
                { text: 'Năm tạo', value: '{{o.date_created.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Ngày thanh toán', value: '{{o.date_created}}' },
                { text: 'Phương thức thanh toán', value: '{{o.journal.name}}' },
                { text: 'Số tiền thanh toán', value: '{{o.amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{o.amount_text}}' },
                { text: 'Nội dung', value: '{{o.reason}}' },
                { text: 'Tên khách hàng', value: '{{o.partner.display_name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.partner.address}}' },
                { text: 'SĐT khách hàng', value: '{{o.partner.phone}}' },
                { text: 'Người lập phiếu', value: '{{u.name}}' },
            ]
        }
    ],
    'tmp_phieu_thu': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date_created.day}}' },
                { text: 'Tháng tạo', value: '{{o.date_created.month}}' },
                { text: 'Năm tạo', value: '{{o.date_created.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nộp', value: '{{o.partner.name}}' },
                { text: 'Địa chỉ người nộp', value: '{{o.partner.address}}' },
                { text: 'Loại thu', value: '{{o.loai_thu_chi.name}}' },
                { text: 'Số tiền', value: '{{o.amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{o.amount_text}}' },
                { text: 'Phương thức', value: '{{o.journal.name}}' },
                { text: 'Nội dung', value: '{{o.reason}}' },
                { text: 'Người lập phiếu', value: '{{o.created_by.name}}' },
            ]
        }
    ],
    'tmp_phieu_chi': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.day}}' },
                { text: 'Tháng tạo', value: '{{date.month}}' },
                { text: 'Năm tạo', value: '{{date.year}}' },
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nhận', value: '{{partner_name}}' },
                { text: 'Địa chỉ người nhận', value: '{{address}}' },
                { text: 'Loại chi', value: '{{loai_thu_chi_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Phương thức', value: '{{journal_name}}' },
                { text: 'Nội dung', value: '{{reason}}' },
                { text: 'Người lập phiếu', value: '{{created_by_name}}' },
            ]
        }
    ],
    'tmp_stock_inventory': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date_created.day}}' },
                { text: 'Tháng tạo', value: '{{o.date_created.month}}' },
                { text: 'Năm tạo', value: '{{o.date_created.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người kiểm', value: '{{o.date_created}}' },
                { text: 'Ghi chú', value: '{{o.note}}' },
                { text: 'Nhân viên kiểm kho', value: '{{o.created_by.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'o.lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Mã sản phẩm', value: '{{line.product.default_code}}' },
                { text: 'Tên sản phẩm', value: '{{line.product.name}}' },
                { text: 'Đơn vị tính', value: '{{line.product_uom.name}}' },
                { text: 'Số lượng tồn kho', value: '{{line.theoretical_qty}}' },
                { text: 'Số lượng thực tế', value: '{{line.product_qty}}' },
            ]
        }
    ],
    'tmp_stock_picking_outgoing': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.now.day}}' },
                { text: 'Tháng tạo', value: '{{date.now.month}}' },
                { text: 'Năm tạo', value: '{{date.now.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Đối tác', value: '{{o.partner.name}}' },
                { text: 'Ghi chú', value: '{{o.note}}' },
                { text: 'Người lập phiếu', value: '{{o.created_by.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'o.move_lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Mã sản phẩm', value: '{{line.product.default_code}}' },
                { text: 'Tên sản phẩm', value: '{{line.name}}' },
                { text: 'Loại sản phẩm', value: '{{line.product.categ.name}}' },
                { text: 'Số lượng', value: '{{line.product_uomqty}}' },
                { text: 'Đơn vị tính', value: '{{line.product_uom.name}}' },
            ]
        }
    ],
    'tmp_stock_picking_incoming': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.now.day}}' },
                { text: 'Tháng tạo', value: '{{date.now.month}}' },
                { text: 'Năm tạo', value: '{{date.now.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Đối tác', value: '{{o.partner.name}}' },
                { text: 'Ghi chú', value: '{{o.note}}' },
                { text: 'Ngày lập phiếu', value: '{{o.created_by.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'o.move_lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Mã sản phẩm', value: '{{line.product.default_code}}' },
                { text: 'Tên sản phẩm', value: '{{line.name}}' },
                { text: 'Loại sản phẩm', value: '{{line.product.categ.name}}' },
                { text: 'Số lượng', value: '{{line.product_uomqty}}' },
                { text: 'Đơn vị tính', value: '{{line.product_uom.name}}' },
            ]
        }
    ],
    'tmp_partner_advance': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.day}}' },
                { text: 'Tháng tạo', value: '{{date.month}}' },
                { text: 'Năm tạo', value: '{{date.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nộp tiền', value: '{{o.partner.name}}' },
                { text: 'Phương thức thanh toán', value: '{{o.journal.name}}' },
                { text: 'Số tiền', value: '{{o.amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{o.amount_text}}' },
                { text: 'Nội dung', value: '{{o.note}}' },
                { text: 'Người lập phiếu', value: '{{u.name}}' },
            ]
        }
    ],
    'tmp_partner_refund': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.day}}' },
                { text: 'Tháng tạo', value: '{{date.month}}' },
                { text: 'Năm tạo', value: '{{date.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nhận tiền', value: '{{o.partner.name}}' },
                { text: 'Phương thức thanh toán', value: '{{o.journal.name}}' },
                { text: 'Số tiền', value: '{{o.amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{o.amount_text}}' },
                { text: 'Nội dung', value: '{{o.note}}' },
                { text: 'Người lập phiếu', value: '{{u.name}}' },
            ]
        }
    ],
    'tmp_agent_commission': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date_created.day}}' },
                { text: 'Tháng tạo', value: '{{date_created.month}}' },
                { text: 'Năm tạo', value: '{{date_created.year}}' },
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Ngày thanh toán', value: '{{date_created}}' },
                { text: 'Phương thức thanh toán', value: '{{journal_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{reason}}' },
                { text: 'Tên người giới thiệu', value: '{{agent.name}}' },
                { text: 'Địa chỉ người giới thiệu', value: '{{agent.address}}' },
                { text: 'Số điện thoại người giới thiệu', value: '{{agent.phone}}' },
                { text: 'Người lập phiếu', value: '{{user.name}}' },
                { text: 'Người nhận tiền', value: '{{agent.name}}' },
            ]
        }
    ],
    'tmp_salary': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.payslip_run.date.day}}' },
                { text: 'Tháng tạo', value: '{{o.payslip_run.date.month}}' },
                { text: 'Năm tạo', value: '{{o.payslip_run.date.year}}' },
                { text: 'Người lập phiếu', value: '{{o.created_by.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Tên nhân viên', value: '{{o.employee.name}}' },
                { text: 'Mã nhân viên', value: '{{o.employee.name}}' },
                { text: 'Lương/tháng', value: '{{o.employee.wage}}' },
                { text: 'Số ngày nghỉ/tháng', value: '{{o.employee.leave_per_month}}' },
                { text: 'Số giờ làm/ngày', value: '{{o.employee.regular_hour}}' },
                { text: 'Số ngày công', value: '{{o.worked_day}}' },
                { text: 'Số ngày nghỉ', value: '{{o.actual_leave_per_month}}' },
                { text: 'Ngày nghỉ không lương', value: '{{o.leave_per_month_unpaid}}' },
                { text: 'Ngày làm thêm', value: '{{o.over_time_day}}' },
                { text: 'Số giờ tăng ca', value: '{{o.over_time_hour}}' },
                { text: 'Lương cơ bản', value: '{{o.total_basic_salary}}' },
                { text: 'Lương tăng ca', value: '{{o.over_time_hour_salary}}' },
                { text: 'Lương làm thêm', value: '{{o.over_time_day_salary}}' },
                { text: 'Phụ cấp xác định', value: '{{o.employee.allowance}}' },
                { text: 'Phụ cấp khác', value: '{{o.other_allowance}}' },
                { text: 'Thưởng', value: '{{o.reward_salary}}' },
                { text: 'Phụ cấp lễ tết', value: '{{o.holiday_allowance}}' },
                { text: 'Hoa hồng', value: '{{o.commission_salary}}' },
                { text: 'Phạt', value: '{{o.amercement_money}}' },
                { text: 'Tổng thu nhập', value: '{{o.total_salary}}' },
                { text: 'Thuế', value: '{{o.tax}}' },
                { text: 'BHXH', value: '{{o.social_insurance}}' },
                { text: 'Thực lĩnh', value: '{{o.net_salary}}' },
                { text: 'Tạm ứng', value: '{{o.advance_payment}}' },
            ]
        }
    ],
    'tmp_salary_advance': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date.day}}' },
                { text: 'Tháng tạo', value: '{{o.date.month}}' },
                { text: 'Năm tạo', value: '{{o.date.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
                { text: 'Người lập phiếu', value: '{{o.created_by.name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Người nhận', value: '{{o.employee.name}}' },
                { text: 'Số tiền', value: '{{o.amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{o.amount_text}}' },
                { text: 'Phương thức thanh toán', value: '{{o.journal.name}}' },
                { text: 'Nội dung   ', value: '{{o.reason}}' },
            ]
        }
    ],
    'tmp_salary_employee': [],
    'tmp_sale_order': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date_order.day}}' },
                { text: 'Tháng tạo', value: '{{o.date_order.month}}' },
                { text: 'Năm tạo', value: '{{o.date_order.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên khách hàng', value: '{{o.partner.name}}' },
                { text: 'SĐT khách hàng', value: '{{o.partner.phone}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.partner.address}}' },
                { text: 'Tổng tiền', value: '{{o.amount_total}}' },
                { text: 'Đã thanh toán', value: '{{o.total_paid}}' },
                { text: 'Số tiền còn lại', value: '{{o.residual}}' },
            ]
        },
        {
            text: 'Thông tin danh sách dịch vụ',
            value: [
                { text: 'Danh sách dịch vụ', value: 'o.order_lines' },
                { text: 'Tên dịch vụ', value: '{{line.product_name}}' },
                { text: 'Số lượng', value: '{{line.product_uomqty}}' },
                { text: 'Đơn giá', value: '{{line.price_unit}}' },
                { text: 'Giảm giá', value: '{{line.amount_discount_total}}' },
                { text: 'Thành tiền', value: '{{line.price_sub_total}}' },

            ]
        },
        {
            text: 'Thông tin theo dõi thanh toán',
            value: [
                { text: 'Danh sách theo dõi thanh toán', value: 'o.sale_order_payments' },
                { text: 'Danh sách hóa đơn thanh toán', value: 'sop.payment_rels' },
                { text: 'Mã thanh toán', value: '{{item.payment.name}}' },
                { text: 'Ngày thanh toán', value: '{{item.payment.payment_date}}' },
                { text: 'Số tiền cần thanh toán', value: '{{sop.amount}}' },
                { text: 'Phương thức thanh toán', value: '{{item.payment.journal.name}}' },
                { text: 'Số tiền thanh toán', value: '{{item.payment.amount}}' },
            ]
        },
        {
            text: 'Thông tin đợt khám',
            value: [
                { text: 'Danh sách đợt khám', value: 'o.dot_khams' },
                { text: 'Danh sách dịch vụ đợt khám', value: 'dotkham.lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Ngày khám', value: '{{dotkham.date}}' },
                { text: 'Tên bác sĩ', value: '{{dotkham.doctor.name}}' },
                { text: 'Mô tả', value: '{{dotkham.reason}}' },
                { text: 'Tên dịch vụ', value: '{{line.product.name}}' },
                { text: 'Công đoạn', value: '{{line.name_step}}' },
                { text: 'Răng, Chi tiết điều trị', value: '{{line.teeth ? (line.teeth | array.map "name" | array.join ",") : ""}}}' },
                { text: 'ghi chú', value: '{{line.note}}' },
            ]
        },
    ],
    'tmp_quotation': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.now.day}}' },
                { text: 'Tháng tạo', value: '{{date.now.month}}' },
                { text: 'Năm tạo', value: '{{date.now.year}}' },
                { text: 'Mã phiếu', value: '{{o.name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên khách hàng có mã KH', value: '{{o.partner.display_name}}' },
                { text: 'SĐT khách hàng', value: '{{o.partner.phone}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.partner.address}}' },
                { text: 'Ghi chú', value: '{{o.note}}' },
                { text: 'Ngày báo giá', value: '{{o.date_quotation}}' },
                { text: 'Người báo giá', value: '{{o.employee.name}}' },
                { text: 'Số ngày áp dụng', value: '{{o.date_applies}}' },
                { text: 'Ngày hết hạn', value: '{{o.date_end_quotation}}' },
                { text: 'Tổng tiền', value: '{{o.total_amount}}' },
                { text: 'Tên khách hàng', value: '{{o.partner.name}}' },
            ]
        },
        {
            text: 'Thông tin danh sách dịch vụ',
            value: [
                { text: 'Danh sách dịch vụ', value: 'o.lines' },
                { text: 'Tên dịch vụ', value: '{{line.name}}' },
                { text: 'Số lượng', value: '{{line.qty}}' },
                { text: 'Đơn giá', value: '{{(line.sub_price - line.amount_discount_total)}}' },
                { text: 'Thành tiền', value: '{{line.amount}}' },

            ]
        },
        {
            text: 'Thông tin tiến độ thanh toán',
            value: [
                { text: 'Danh sách tiến độ thanh toán', value: 'o.payments' },
                { text: 'STT tiến độ', value: '{{payment.sequence}}' },
                { text: 'Thanh toán', value: '{{payment.payment}}' },
                { text: 'Loại giảm giá', value: '{{payment.discount_percent_type}}' },
                { text: 'Ngày thanh toán', value: '{{payment.date}}' },
                { text: 'Số tiền cần thanh toán', value: '{{payment.amount}}' },

            ]
        }
    ],
    'tmp_advisory': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date.now.day}}' },
                { text: 'Tháng tạo', value: '{{date.now.month}}' },
                { text: 'Năm tạo', value: '{{date.now.year}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên khách hàng có Mã KH', value: '{{o.partner.display_name}}' },
                { text: 'Tên khách hàng', value: '{{o.partner.name}}' },
                { text: 'Địa chỉ khách hàng ', value: '{{o.partner.address}}' },
                { text: 'SĐT khách hàng ', value: '{{o.partner.phone}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách tiểu sử răng', value: 'advisories' },
                { text: 'Ngày tạo', value: '{{item.date}}' },
                { text: 'Người tạo', value: '{{item.employee.name}}' },
                { text: 'Răng hàm', value: '{{item.tooth_type}}' },
                { text: 'Răng', value: '{{item.tooths}}' },
                { text: 'Chẩn đoán', value: '{{item.diagnosis}}' },
                { text: 'Dịch vụ', value: '{{item.services}}' },
            ]
        }
    ],
    'tmp_account_payment': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{o.date.day}}' },
                { text: 'Tháng tạo', value: '{{o.date.month}}' },
                { text: 'Năm tạo', value: '{{o.date.year}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên khách hàng có mã KH', value: '{{o.order.partner.display_name}}' },
                { text: 'Tên khách hàng', value: '{{o.order.partner.name}}' },
                { text: 'SĐT khách hàng', value: '{{o.order.partner.phone}}' },
                { text: 'Địa chỉ khách hàng', value: '{{o.order.partner.address}}' },
                { text: 'Ngày thanh toán', value: '{{o.date}}' },
                { text: 'danh sách Phương thức thanh toán', value: '{{o.journal_lines}}' },
                { text: 'Số tiền', value: '{{o.amount}}' },
                { text: 'Nội dung', value: '{{o.note}}' },
                { text: 'Tên nhân viên', value: '{{u.name}}' },
            ]
        }
    ],
    'tmp_supplier_payment': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{payment_date.day}}' },
                { text: 'Tháng tạo', value: '{{payment_date.month}}' },
                { text: 'Năm tạo', value: '{{payment_date.year}}' },
                { text: 'Mã phiếu', value: '{{payment_name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Ngày thanh toán', value: '{{payment_date}}' },
                { text: 'Phương thức thanh toán', value: '{{journal_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{communication}}' },
                { text: 'Nhà cung cấp', value: '{{partner_name}}' },
                { text: 'Địa chỉ', value: '{{partner_address}}' },
                { text: 'SĐT nhà cung cấp', value: '{{partner_phone}}' },
                { text: 'Người lập phiếu', value: '{{user_name}}' },
                { text: 'Người nhận tiền', value: '{{partner_name}}' },
            ]
        }
    ],
    'tmp_supplier_payment_inbound': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{payment_date.day}}' },
                { text: 'Tháng tạo', value: '{{payment_date.month}}' },
                { text: 'Năm tạo', value: '{{payment_date.year}}' },
                { text: 'Mã phiếu', value: '{{payment_name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Ngày thanh toán', value: '{{payment_date}}' },
                { text: 'Phương thức thanh toán', value: '{{journal_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{communication}}' },
                { text: 'Nhà cung cấp', value: '{{partner_name}}' },
                { text: 'Địa chỉ', value: '{{partner_address}}' },
                { text: 'SĐT nhà cung cấp', value: '{{partner_phone}}' },
                { text: 'Người lập phiếu', value: '{{partner_name}}' },
                { text: 'Người nhận tiền', value: '{{user_name}}' },
            ]
        }
    ]
};

function getKeyWords() {
    var res = keyWorDatas;
    res['tmp_purchase_refund'] = res['tmp_purchase_order'];
    res['tmp_salary_employee'] = res['tmp_salary_advance'];
    for (const [key, value] of Object.entries(res)) {
        (value as any).push(pipes as any);
    }
    return res;
}

export const keyWords = getKeyWords();