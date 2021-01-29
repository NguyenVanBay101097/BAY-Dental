import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-layout-sidebar',
  templateUrl: './layout-sidebar.component.html',
  styleUrls: ['./layout-sidebar.component.css']
})
export class LayoutSidebarComponent implements OnInit {
  activeIndex = -1;
  folded = false;
  menuItems: { name: string, icon?: string, link?: string, groups?: string, children?: { name: string, link?: string, params?: Object, groups?: string }[] }[] = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/dashboard',
    },
    {
      name: 'Khách hàng', icon: 'fas fa-users', children: [], link: '/partners/customers'
    },
    {
      name: 'Lịch hẹn', icon: 'fas fa-calendar-alt', children: [], link: '/appointments/kanban',
    },
    { name: 'Bán thuốc', icon: 'fas fa-capsules', children: [], link: '/medicine-orders', groups: 'medicineOrder.group_medicine' },

    // {
    //   name: 'Thống kê labo',
    //   icon: 'fas fa-tooth',
    //   children: [],
    //   link: '/labo-orders/statistics'
    // },
    // ,
    {
      name: 'Labo',
      icon: 'fas fa-tooth',
      children: [
        { name: 'Quản lý phiếu Labo', link: '/labo-orders' },
        { name: 'Đơn hàng Labo', link: '/labo-orders/order' },
        { name: 'Xuất Labo cho khách', link: '/labo-orders/export' },
      ],
    },
    {
      name: 'Khảo sát đánh giá',
      icon: 'fas fa-poll',
      children: [
        // { name: 'Danh sách khảo sát', link: '/labo-orders' },
        { name: 'Quản lý phần việc', link: '/surveys/manage' },
        { name: 'Cấu hình đánh giá', link: '/surveys/config' },
      ],
    },
    {
      name: 'Mua hàng',
      icon: 'fas fa-shopping-cart',
      children: [
        { name: 'Mua hàng', link: '/purchase/orders', params: { type: 'order' } },
        { name: 'Trả hàng', link: '/purchase/orders', params: { type: 'refund' } },
      ]
    },
    {
      name: 'Kho',
      icon: 'fas fa-th',
      link: '/stock/stock-report-xuat-nhap-ton',
      children: [
      ],
    },
    {
      name: 'Lương',
      icon: 'fas fa-money-bill-alt',
      children: [
        { name: 'Bảng lương', link: '/hr/payslip-run/form' },
        // { name: 'Phiếu lương', link: '/hr/payslips' },
        { name: 'Chấm công', link: 'time-keepings' },
        { name: 'Quản lý tạm ứng - chi lương', link: '/salary-payment' },
        { name: 'Báo cáo thanh toán lương', link: 'hr/salary-reports' },
      ],
    },
    {
      name: 'Sổ quỹ', icon: 'fas fa-wallet', children: [], link: '/cash-book',
    },
    // {
    //   name: 'Thu chi',
    //   icon: 'fas fa-dollar-sign',
    //   children: [
    //     { name: 'Phiếu thu', link: '/phieu-thu-chi', params: { type: 'thu' } },
    //     { name: 'Phiếu chi', link: '/phieu-thu-chi', params: { type: 'chi' } },
    //     { name: 'Loại thu', link: '/loai-thu-chi', params: { type: 'thu' } },
    //     { name: 'Loại chi', link: '/loai-thu-chi', params: { type: 'chi' } },
    //   ],
    // },
    {
      name: 'Thẻ tiền mặt',
      icon: 'far fa-credit-card',
      groups: 'sale.group_service_card',
      children: [
        { name: 'Tạo đơn bán thẻ', link: '/service-card-orders/create-card-order' },
        { name: 'Đơn bán thẻ', link: '/service-card-orders' },
        { name: 'Loại thẻ', link: '/service-card-types' },
        { name: 'Danh sách thẻ', link: '/service-cards' },
      ],
    },
    {
      name: 'Khuyến mãi',
      icon: 'fas fa-gift',
      groups: 'sale.group_sale_coupon_promotion',
      children: [
        { name: 'Chương trình coupon', link: '/programs/coupon-programs' },
        { name: 'Chương trình khuyến mãi', link: '/programs/promotion-programs' },
      ],
    },
    {
      name: 'Thành viên',
      icon: 'fas fa-credit-card',
      groups: 'sale.group_loyalty_card',
      children: [
        { name: 'Thẻ thành viên', link: '/card-cards' },
        { name: 'Loại thẻ thành viên', link: '/card-types' },
      ],
    },
    {
      name: 'Chăm sóc tự động',
      icon: 'fab fa-facebook-f',
      children: [
        { name: 'Kết nối facebook page', link: '/socials/facebook-connect' },
        { name: 'Kết nối Zalo', link: '/zalo-config' },
        { name: 'Danh sách kênh', link: '/socials/channels' },
        { name: 'Kịch bản', link: '/tcare/scenarios' },
        { name: 'Thống kê gửi tin', link: '/tcare/messagings' },
        { name: 'Mẫu tin nhắn', link: '/tcare/message-templates' },
        { name: 'Thiết lập tự động', link: '/tcare/config' },
      ],
      groups: 'tcare.group_tcare',
    },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Thông tin khách hàng', link: '/partners/customer-management' },
        // { name: 'Nhãn khách hàng', link: '/partner-categories' },
        // { name: "Nguồn khách hàng", link: "/partner-sources" },
        { name: 'Nhà cung cấp', link: '/partners/suppliers' },
        { name: 'Dịch vụ - Vật tư - Thuốc', link: '/products' },
        { name: 'Đơn thuốc mẫu', link: '/sample-prescriptions' },
        // { name: 'Tiểu sử bệnh', link: '/histories' },
        // { name: 'Danh xưng', link: '/partner-titles' },
        { name: 'Đơn vị tính', link: '/uoms', groups: 'product.group_uom', },
        { name: 'Nhóm Đơn vị tính', link: '/uom-categories', groups: 'product.group_uom' },
        { name: 'Bảng hoa hồng', link: '/commissions' },
        { name: 'Nhân viên', link: '/employees' },
        { name: 'Thông số Labo', link: '/labo-orders/labo-managerment' },
        { name: 'Loại thu chi', link: '/loai-thu-chi' },
        // { name: 'Loại chi', link: '/loai-thu-chi', params: { type: 'chi' }},
        // { name: 'Vật liệu Labo', link: '/products/labos' },
        // { name: 'Đường hoàn tất', link: '/labo-finish-lines' },
        // { name: 'Gửi kèm Labo', link: '/products/labo-attachs' },
        // { name: 'Kiểu nhịp Labo', link: '/labo-bridges' },
        // { name: 'Khớp cắn Labo', link: '/labo-bite-joints' },
      ]
    },
    {
      name: 'Cấu hình',
      icon: 'fas fa-cogs',
      children: [
        { name: 'Chi nhánh', link: '/companies' },
        { name: 'Người dùng', link: '/users' },
        { name: 'Nhóm quyền', link: '/roles' },
        { name: 'Cấu hình chung', link: '/config-settings' }
      ]
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      children: [
        { name: 'Báo cáo tổng quan', link: '/sale-dashboard-reports' },
        { name: 'Kết quả kinh doanh', link: '/financial-report' },
        { name: 'Tiền mặt, ngân hàng', link: '/report-general-ledgers/cash-bank' },
        { name: 'Thống kê doanh thu', link: '/revenue-report' },
        { name: 'Thống kê điều trị', link: '/sale-report' },
        { name: 'Công nợ khách hàng', link: '/report-account-common/partner', params: { result_selection: 'customer' } },
        { name: 'Công nợ nhà cung cấp', link: '/report-account-common/partner', params: { result_selection: 'supplier' } },
        { name: 'Xuất nhập tồn', link: '/stock-report-xuat-nhap-ton' },
        { name: 'Thống kê tình hình thu nợ khách hàng', link: '/real-revenue-report' },
        // { name: 'Thống kê công việc', link: '/dot-kham-report' },
        { name: 'Thống kê hoa hồng', link: '/commission-settlements/report' },
        { name: 'Khách hàng lân cận phòng khám', link: '/partner-report-location' },
        { name: 'Thống kê nguồn khách hàng', link: '/report-partner-sources' },
        { name: 'Thống kê khách hàng cũ mới', link: '/sale-report/partner' },
        // { name: 'Thống kê khách hàng', link: '/customer-statistics' },
        // { name: 'Thống kê khách hàng cũ mới', link: '/sale-report/old-new-partner' },
      ]
    },
  ];
  constructor(public sidebarService: NavSidebarService, public authService: AuthService) { }

  ngOnInit() {
  }

  onMenuClick(index) {
    if (this.sidebarService.collapsed) {
      this.activeIndex = -1;
    } else {
      if (this.activeIndex == index) {
        this.activeIndex = -1;
      } else {
        this.activeIndex = index;
      }
    }
  }

}
