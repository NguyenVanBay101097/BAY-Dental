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
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/',
    },
    { name: 'Khách hàng', icon: 'fas fa-users', children: [], link: '/partners/customers' },
    {
      name: 'Lịch hẹn', icon: 'fas fa-calendar-alt', children: [], link: '/appointments/kanban',
    },
    {
      name: 'Thống kê labo',
      icon: 'fas fa-tooth',
      children: [],
      link: '/labo-orders/statistics'
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
      children: [
        { name: 'Phiếu xuất kho', link: '/stock/outgoing-pickings' },
        { name: 'Phiếu nhập kho', link: '/stock/incoming-pickings' },
      ],
    },
    {
      name: 'Lương',
      icon: 'fas fa-money-bill-alt',
      children: [
        { name: 'Đợt lương', link: '/hr/payslip-runs' },
        { name: 'Phiếu lương', link: '/hr/payslips' },
        { name: 'Chấm công', link: 'time-keepings' },
        { name: 'Loại mẫu lương', link: '/hr/payroll-structure-types' },
        { name: 'Mẫu lương', link: '/hr/payroll-structures' },
        { name: 'Loại chấm công', link: '/work-entry-types' },
        { name: 'Lịch làm việc', link: '/resource-calendars' },
        { name: 'Cấu hình lương', link: '/hr/salary-configs' }
      ],
    },
    {
      name: 'Thu chi',
      icon: 'fas fa-dollar-sign',
      children: [
        { name: 'Phiếu thu', link: '/phieu-thu-chi', params: { type: 'thu' } },
        { name: 'Phiếu chi', link: '/phieu-thu-chi', params: { type: 'chi' } },
        { name: 'Loại thu', link: '/loai-thu-chi', params: { type: 'thu' } },
        { name: 'Loại chi', link: '/loai-thu-chi', params: { type: 'chi' } },
      ],
    },
    {
      name: 'Thẻ tiền mặt',
      icon: 'far fa-credit-card',
      groups: 'sale.group_service_card',
      children: [
        { name: 'Đơn bán thẻ', link: '/service-card-orders' },
        { name: 'Loại thẻ', link: '/service-card-types' },
        { name: 'Danh sách thẻ', link: '/service-cards' },
      ],
    },
    {
      name: 'Khuyến mãi',
      icon: 'fas fa-gift',
      // groups: 'sale.group_sale_coupon_promotion',
      children: [
        // { name: 'Chương trình coupon', link: '/coupon-programs' },
        { name: 'Chương trình khuyến mãi', link: '/promotion-programs' },
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
      ],
      groups: 'tcare.group_tcare',
    },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Nhãn khách hàng', link: '/partner-categories' },
        { name: "Nguồn khách hàng", link: "/partner-sources" },
        { name: 'Nhà cung cấp', link: '/partners/suppliers' },
        { name: 'Dịch vụ', link: '/products/services' },
        { name: 'Nhóm dịch vụ', link: '/product-categories/service' },
        { name: 'Vật tư', link: '/products/products' },
        { name: 'Nhóm vật tư', link: '/product-categories/product' },
        { name: 'Thuốc', link: '/products/medicines' },
        { name: 'Nhóm thuốc', link: '/product-categories/medicine' },
        { name: 'Đơn thuốc mẫu', link: '/sample-prescriptions' },
        { name: 'Tiểu sử bệnh', link: '/histories' },
        { name: 'Danh xưng', link: '/partner-titles' },
        { name: 'Nhân viên', link: '/employees' },
        { name: 'Đơn vị tính', link: '/uoms', groups: 'product.group_uom', },
        { name: 'Nhóm Đơn vị tính', link: '/uom-categories', groups: 'product.group_uom' },
        { name: 'Bảng hoa hồng', link: '/commissions' },
        { name: 'Nhân viên', link: '/employees' },
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
        { name: 'Kết quả kinh doanh', link: '/financial-report' },
        { name: 'Tiền mặt, ngân hàng', link: '/report-general-ledgers/cash-bank' },
        { name: 'Thống kê doanh thu', link: '/revenue-report' },
        { name: 'Thống kê điều trị', link: '/sale-report' },
        { name: 'Công nợ khách hàng', link: '/report-account-common/partner', params: { result_selection: 'customer' } },
        { name: 'Công nợ nhà cung cấp', link: '/report-account-common/partner', params: { result_selection: 'supplier' } },
        { name: 'Xuất nhập tồn', link: '/stock-report-xuat-nhap-ton' },
        { name: 'Thống kê tình hình thu nợ khách hàng', link: '/real-revenue-report' },
        { name: 'Thống kê công việc', link: '/dot-kham-report' },
        { name: 'Thống kê hoa hồng', link: '/commission-settlements/report' },
        { name: 'Khách hàng lân cận phòng khám', link: '/partner-report-location' },
        { name: 'Thống kê nguồn khách hàng', link: '/report-partner-sources' },
        { name: 'Thống kê khách hàng cũ mới', link: '/sale-report/partner' },
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
