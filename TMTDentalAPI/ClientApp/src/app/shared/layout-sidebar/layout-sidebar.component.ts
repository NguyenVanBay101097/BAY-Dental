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
  menuItems: { name: string, icon?: string, link?: string, children?: { name: string, link?: string, params?: Object, groups?: string }[] }[] = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/',
    },
    {
      name: 'Lịch hẹn', icon: 'fas fa-calendar-alt', children: [], link: '/appointments/kanban',
    },
    {
      name: 'Điều trị',
      icon: 'fas fa-medkit',
      children: [
        { name: 'Phiếu điều trị', link: '/sale-orders' },
        { name: 'Phiếu tư vấn', link: '/sale-quotations' },
        // { name: 'Đợt khám', link: '/dot-khams' },
      ]
    },
    {
      name: 'Phiếu labo',
      icon: 'fas fa-tooth',
      link: '/labo-orders',
      children: []
    },
    // {
    //   name: 'Quản lý labo',
    //   icon: 'fas fa-tooth',
    //   children: [
    //     { name: 'Phiếu Labo', link: '/labo-orders' },
    //     { name: 'Báo cáo labo', link: '/labo-order-lines' },
    //   ]
    // },
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
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Kênh xã hội', link: '/socials-channel' },
        { name: 'Chiến dịch marketing', link: '/marketing-campaigns' },
        // { name: 'Thanh toán điều trị', link: '/accountpayments', params: { partner_type: 'customer' } },
        { name: 'Khách hàng', link: '/partners', params: { customer: true } },
        { name: 'Nhóm khách hàng', link: '/partner-categories' },
        { name: 'Nhà cung cấp', link: '/partners', params: { supplier: true } },
        { name: 'Nhân viên', link: '/employees' },
        // { name: 'Nhóm nhân viên', link: '/employee-categories' },
        // { name: 'Sản phẩm', link: '/products' },
        // { name: 'Nhóm sản phẩm', link: '/product-categories' },
        { name: 'Dịch vụ', link: '/product-services' },
        { name: 'Nhóm dịch vụ', link: '/product-categories/service' },
        { name: 'Vật tư', link: '/product-products' },
        { name: 'Nhóm vật tư', link: '/product-categories/product' },
        { name: 'Thuốc', link: '/product-medicines' },
        { name: 'Nhóm thuốc', link: '/product-categories/medicine' },
        // { name: 'Labo', link: '/product-labos' },
        { name: 'Tiểu sử bệnh', link: '/histories' },
        // { name: 'Ngân hàng', link: '/res-banks' },
        // { name: 'Tài khoản ngân hàng', link: '/res-partner-banks' },
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
        { name: 'Khách hàng theo khu vực', link: '/partner-report-location' },
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
