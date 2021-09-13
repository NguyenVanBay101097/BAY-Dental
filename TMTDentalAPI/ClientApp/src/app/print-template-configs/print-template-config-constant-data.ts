export const types: { text: string, value: string }[] = [
    { text: 'Phiếu điều trị', value: 'tmp_sale_order' },
    { text: 'Biên lai khách hàng thanh toán', value: 'tmp_account_payment' },
    { text: 'Tình trạng răng', value: 'tmp_advisory' },
    { text: 'Khách hàng đóng tạm ứng', value: 'tmp_partner_advance' },
    { text: 'Khách hàng hoàn tạm ứng', value: 'tmp_partner_refund' },
    { text: 'Biên lai thanh toán nhà cung cấp', value: 'tmp_supplier_payment' },
    { text: 'Phiếu thu', value: 'tmp_phieu_thu' },
    { text: 'Phiếu chi', value: 'tmp_phieu_chi' },
    { text: 'Phiếu thu công nợ khách hàng', value: 'tmp_customer_debt' },
    { text: 'Phiếu chi hoa hồng', value: 'tmp_agent_commission' },
    { text: 'Phiếu mua hàng', value: 'tmp_purchase_order' },
    { text: 'Phiếu trả hàng', value: 'tmp_purchase_refund' },
    { text: 'Phiếu báo giá', value: 'tmp_quotation' },
    { text: 'Phiếu thanh toán lương nhân viên', value: 'tmp_salary' },
    { text: 'Phiếu tạm ứng', value: 'tmp_salary_advance' },
    { text: 'Phiếu chi lương', value: 'tmp_salary_employee' },
    { text: 'Phiếu nhập kho', value: 'tmp_stock_picking_incoming' },
    { text: 'Phiếu xuất kho', value: 'tmp_stock_picking_outgoing' },
    { text: 'Phiếu kiểm kho', value: 'tmp_stock_inventory' },
    { text: 'Đơn thuốc', value: 'tmp_toathuoc' },
    { text: 'Hóa đơn thuốc', value: 'tmp_medicine_order' },
    { text: 'Phiếu labo', value: 'tmp_labo_order' },
];

let companyInfo =
{
    text: "Thông tin chi nhánh",
    value: [
        { text: 'Logo chi nhánh', value: '{{company.logo}}' },
        { text: 'Tên chi nhánh', value: '{{company.name}}' },
        { text: 'Địa chỉ chi nhánh', value: '{{company.address}}' },
        { text: 'Điện thoại chi nhánh', value: '{{company.phone}}' },
        { text: 'Email chi nhánh', value: '{{company.email}}' },
    ]
};

let pipes =
{
    text: "Một số định dạng bổ sung",
    value: [
        { text: 'Ngày hôm nay', value: '{{date.now}}' },
        { text: 'định dạng số', value: '| math.format "N0" "vi-VN"' },
        { text: 'định dạng ngày/tháng/năm', value: `| date.to_string '%d/%m/%Y'` },
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

const keyWords =

{
    'tmp_medicine_order': [
        companyInfo,//info key of company
        {
            text: "Thông tin phiếu",
            value: [
                { text: 'Ngày lập hóa đơn', value: '{{order_date.day}}' },
                { text: 'Tháng lập hóa đơn', value: '{{order_date.month}}' },
                { text: 'Năm lập hóa đơn', value: '{{order_date.year}}' },
                { text: 'Mã hóa đơn', value: '{{name}}' },
                { text: 'Thanh toán', value: '{{account_payment.amount}}' },
                { text: 'Tổng tiền', value: '{{amount}}' },
            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Tên khách hàng', value: '{{partner.name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{partner.address}}' },
                { text: 'Tên bác sĩ', value: '{{employee.name}}' },
                { text: 'Lời dặn bác sĩ', value: '{{toa_thuoc.note}}' },
                { text: 'Chẩn đoán bệnh', value: '{{toa_thuoc.diagnostic}}' },
                { text: 'Ngày tái khám', value: '{{toa_thuoc.re_examination_date}}' },
            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Danh sách thuốc', value: 'medicine_order_lines' },
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
                { text: 'Ngày lập đơn', value: '{{date_created.day}}' },
                { text: 'Tháng lập đơn', value: '{{date_created.month}}' },
                { text: 'Năm lập đơn', value: '{{date_created.year}}' },
                { text: 'Mã đơn', value: '{{name}}' },
            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Lời dặn', value: '{{note}}' },
                { text: 'Chấn đoán bệnh', value: '{{diagnostic}}' },
                { text: 'Ngày tái khám', value: '{{re_examination_date}}' },
                { text: 'Tên khách hàng', value: '{{partner.display_name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{partner.address}}' },
                { text: 'Giới tính khách hàng', value: '{{partner.display_gender}}' },
                { text: 'Tuổi khách hàng', value: '{{partner.age}}' },
                { text: 'Tên bác sĩ', value: '{{employee_name}}' },
            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Danh sách thuốc', value: 'lines' },
                { text: 'Số thứ tự', value: '{{for.index + 1}}' },
                { text: 'Tên thuốc', value: '{{line.product_name}}' },
                { text: 'Số lượng', value: '{{line.quantity}}' },
                { text: 'Số lần uống/ngày', value: '{{line.number_of_times}}' },
                { text: 'Số lượng uống/lần', value: '{{line.amount_of_times}}' },
                { text: 'Đơn vị mỗi lần uống', value: '{{line.product_uomname}}' },
                { text: 'Số ngày uống', value: '{{line.number_of_days}}' },
                { text: 'Thời điểm uống', value: '{{line.get_use_at_display}}' },
            ]
        }
    ],

    'tmp_labo_order': [
        companyInfo,//info key of company
        {
            text: "Thông tin đặt hàng",
            value: [
                { text: 'Ngày đặt hàng', value: '{{date_order.day}}' },
                { text: 'Tháng đặt hàng', value: '{{date_order.month}}' },
                { text: 'Năm đặt hàng', value: '{{date_order.year}}' },
                { text: 'Mã đặt hàng', value: '{{name}}' },

            ]
        },
        {
            text: "Thông tin chung",
            value: [
                { text: 'Tên bác sĩ', value: '{{sale_order_line.employee.name}}' },
                { text: 'Số phiếu điều trị', value: '{{sale_order_line.order.name }}' },
                { text: 'Tên khách hàng', value: '{{customer.name}}' },
                { text: 'Tên nhà cung cấp', value: '{{partner.name}}' },
                { text: 'Ngày gửu', value: '{{date_order}}' },
                { text: 'Ngày nhận dự kiến', value: '{{date_planned}}' },

            ]
        },
        {
            text: "Thông tin chi tiết",
            value: [
                { text: 'Loại phục hình', value: '{{sale_order_line.name}}' },
                { text: 'Hãng', value: '{{sale_order_line.product.firm}}' },
                { text: 'Răng', value: '{{teeth}' },
                { text: 'Màu sắc', value: '{{color}' },
                { text: 'Số lượng', value: '{{quantity}' },
                { text: 'Chỉ định', value: '{{indicated}' },
                { text: 'Ghi chú', value: '{{note}' },
            ]
        },
        {
            text: "Thông số Labo",
            value: [
                { text: 'Vật liệu', value: '{{product.name}}' },
                { text: 'Đường hoàn tất', value: '{{labo_finish_line.name}}' },
                { text: 'Khớp cắn', value: '{{labo_bite_joint.name}}' },
                { text: 'Kiểu nhịp', value: '{{labo_bridge.name}}' },
                { text: 'Gửu kèm', value: '{{labo_order_products}}' },
                { text: 'Ghi chú kỹ thuật', value: '{{technical_note}}' },
                { text: 'Mã bảo hành', value: '{{warranty_code}}' },
                { text: 'Hạn bảo hành', value: '{{warranty_period}}' },
            ]
        }
    ],
    'tmp_purchase_order': [
        companyInfo,
        {
            text: 'Thông tin phiếu',
            value: [
                { text: 'Ngày tạo', value: '{{date_order.day}}' },
                { text: 'Tháng tạo', value: '{{date_order.month}}' },
                { text: 'Năm tạo', value: '{{date_order.year}}' },
                { text: 'Mã phiếu', value: '{{name}}' },
                { text: 'Tổng tiền', value: '{{amount_total}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Tên nhà cung cấp', value: '{{partner_name}}' },
                { text: 'Mã tham chiếu kho', value: '{{stock_picking_name}}' },
                { text: 'Ghi chú', value: '{{note}}' },
                { text: 'Người lập phiếu', value: '{{user_name}}' },
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
                { text: 'Số tiền thanh toán', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{reason}}' },
                { text: 'Tên khách hàng', value: '{{partner.display_name}}' },
                { text: 'Địa chỉ khách hàng', value: '{{partner.address}}' },
                { text: 'SĐT khách hàng', value: '{{partner.phone}}' },
                { text: 'Người lập phiếu', value: '{{user.name}}' },
            ]
        }
    ],
    'tmp_phieu_thu': [
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
                { text: 'Người nộp', value: '{{partner_name}}' },
                { text: 'Địa chỉ người nộp', value: '{{address}}' },
                { text: 'Loại thu', value: '{{loai_thu_chi_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Phương thức', value: '{{journal_name}}' },
                { text: 'Nội dung', value: '{{reason}}' },
                { text: 'Người lập phiếu', value: '{{created_by_name}}' },
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
                { text: 'Ngày tạo', value: '{{date_created.day}}' },
                { text: 'Tháng tạo', value: '{{date_created.month}}' },
                { text: 'Năm tạo', value: '{{date_created.year}}' },
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người kiểm', value: '{{date_created}}' },
                { text: 'Ghi chú', value: '{{note}}' },
                { text: 'Nhân viên kiểm kho', value: '{{created_by_name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'lines' },
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
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Đối tác', value: '{{partner.name}}' },
                { text: 'Ghi chú', value: '{{note}}' },
                { text: 'Ngày xuất kho', value: '{{date_created}}' },
                { text: 'Ngày lập phiếu', value: '{{created_by_name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'move_lines' },
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
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Đối tác', value: '{{partner.name}}' },
                { text: 'Ghi chú', value: '{{note}}' },
                { text: 'Ngày nhập kho', value: '{{date_created}}' },
                { text: 'Ngày lập phiếu', value: '{{created_by_name}}' },
            ]
        },
        {
            text: 'Thông tin chi tiết',
            value: [
                { text: 'Danh sách sản phẩm', value: 'move_lines' },
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
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nộp tiền', value: '{{partner_name}}' },
                { text: 'Phương thức thanh toán', value: '{{journal_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{note}}' },
                { text: 'Người lập phiếu', value: '{{user_name}}' },
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
                { text: 'Mã phiếu', value: '{{name}}' },
            ]
        },
        {
            text: 'Thông tin chung',
            value: [
                { text: 'Người nhận tiền', value: '{{partner_name}}' },
                { text: 'Phương thức thanh toán', value: '{{journal_name}}' },
                { text: 'Số tiền', value: '{{amount}}' },
                { text: 'Số tiền bằng chữ', value: '{{amount_text}}' },
                { text: 'Nội dung', value: '{{note}}' },
                { text: 'Người lập phiếu', value: '{{user_name}}' },
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
    ]
};

export function getKeyWords() {
    var res = JSON.parse(JSON.stringify(keyWords));
    res['tmp_purchase_refund'] = JSON.parse(JSON.stringify(res['tmp_purchase_order']));
    for (const [key, value] of Object.entries(res)) {
        (value as any).push(pipes as any);
    }
    return res;
}