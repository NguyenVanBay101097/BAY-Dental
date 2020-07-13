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
    { name: 'Khách hàng', icon: 'fas fa-users', children: [], link: '/customers' },
    {
      name: 'Lịch hẹn', icon: 'fas fa-calendar-alt', children: [], link: '/appointments/kanban',
    },
    // {
    //   name: 'Điều trị',
    //   icon: 'fas fa-medkit',
    //   children: [
    //     { name: 'Phiếu điều trị', link: '/sale-orders' },
    //     { name: 'Phiếu tư vấn', link: '/sale-quotations' },
    //     // { name: 'Đợt khám', link: '/dot-khams' },
    //   ]
    // },
    // {
    //   name: 'Phiếu labo',
    //   icon: 'fas fa-tooth',
    //   link: '/labo-orders',
    //   children: []
    // },
    {
      name: 'Thống kê labo',
      icon: 'fas fa-tooth',
      children: [],
      link: '/labo-statistics'
    },
    {
      name: 'Mua hàng',
      icon: 'fas fa-shopping-cart',
      children: [
        { name: 'Mua hàng', link: '/purchase-orders', params: { type: 'order' } },
        { name: 'Trả hàng', link: '/purchase-orders', params: { type: 'refund' } },
      ]
    },
    {
      name: 'Kho',
      icon: 'fas fa-th',
      children: [
        { name: 'Phiếu xuất kho', link: '/outgoing-pickings' },
        { name: 'Phiếu nhập kho', link: '/incoming-pickings' },
      ],
    },
    {
      name: 'Thẻ tiền mặt',
      icon: 'far fa-credit-card',
      groups: 'sale.group_service_card',
      children: [
        { name: 'Đơn bán thẻ', link: '/service-card-orders/list' },
        { name: 'Loại thẻ', link: '/service-card-types' },
        { name: 'Danh sách thẻ', link: '/service-cards' },
      ],
    },
    // {
    //   name: 'T-Care',
    //   icon: 'fab fa-facebook-f',
    //   children: [
    //     { name: 'Kết nối facebook page', link: '/facebook-connect' },
    //     { name: 'Kết nối Zalo', link: '/zalo-config' },
    //     { name: 'Danh sách kênh', link: '/channels' },
    //   ],
    // },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Nhãn khách hàng', link: '/partner-categories' },
        { name: "Nguồn khách hàng", link: "/partner-sources" },
        { name: 'Nhà cung cấp', link: '/suppliers' },
        { name: 'Dịch vụ', link: '/product-services' },
        { name: 'Nhóm dịch vụ', link: '/product-categories/service' },
        { name: 'Vật tư', link: '/product-products' },
        { name: 'Nhóm vật tư', link: '/product-categories/product' },
        { name: 'Thuốc', link: '/product-medicines' },
        { name: 'Nhóm thuốc', link: '/product-categories/medicine' },
        { name: 'Đơn thuốc mẫu', link: '/sample-prescriptions' },
        { name: 'Tiểu sử bệnh', link: '/histories' },
        { name: 'Đơn vị tính', link: '/uoms', groups: 'product.group_uom', },
        { name: 'Nhóm Đơn vị tính', link: '/uom-categories', groups: 'product.group_uom' },
        { name: 'Thẻ thành viên', link: '/card-cards', groups: 'sale.group_loyalty_card' },
        { name: 'Loại thẻ thành viên', link: '/card-types', groups: 'sale.group_loyalty_card' },
        { name: 'Chương trình coupon', link: '/coupon-programs', groups: 'sale.group_sale_coupon_promotion' },
        { name: 'Chương trình khuyến mãi', link: '/promotion-programs', groups: 'sale.group_sale_coupon_promotion' },
      ]
    },
    {
      name: 'Cấu hình',
      icon: 'fas fa-cogs',
      children: [
        { name: 'Chi nhánh', link: '/companies' },
        { name: 'Người dùng', link: '/users' },
        { name: 'Nhóm quyền', link: '/res-groups' },
        // { name: 'Bảng giá', link: '/pricelists' },
        { name: 'Cấu hình chung', link: '/config-settings' },
        // { name: 'Kịch bản', link: '/tcare-campaigns' },
      ]
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      children: [
        { name: 'Công nợ khách hàng', link: '/account-common-partner-reports', params: { result_selection: 'customer' } },
        { name: 'Công nợ nhà cung cấp', link: '/account-common-partner-reports', params: { result_selection: 'supplier' } },
        { name: 'Xuất nhập tồn', link: '/stock-report-xuat-nhap-ton' },
        // { name: 'Thống kê hóa đơn', link: '/account-invoice-reports' },
        { name: 'Thống kê doanh thu', link: '/revenue-report' },
        { name: 'Thống kê tình hình thu nợ khách hàng', link: '/real-revenue-report' },
        { name: 'Thống kê điều trị', link: '/sale-report' },
        { name: 'Khách hàng lân cận phòng khám', link: '/partner-report-location' },
        { name: 'Thống kê nguồn khách hàng', link: '/report-partner-sources' },
        { name: 'Báo cáo thu chi', link: '/journal-reports' },
        { name: 'Thống kê khách hàng cũ mới', link: '/sale-report-partner' },
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
