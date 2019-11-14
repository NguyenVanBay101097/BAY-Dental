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
  menuItems: { name: string, icon?: string, link?: string, children?: { name: string, link?: string, params?: Object }[] }[] = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/',
    },
    {
      name: 'Điều trị',
      icon: 'fas fa-medkit',
      children: [
        // { name: 'Phiếu điều trị', link: '/customer-invoices' },
        { name: 'Phiếu điều trị', link: '/sale-orders' },
        { name: 'Đợt khám', link: '/dot-khams' },
        { name: 'Lịch hẹn', link: '/appointments/kanban' },
      ]
    },
    {
      name: 'Quản lý labo',
      icon: 'fas fa-tooth',
      children: [
        { name: 'Phiếu Labo', link: '/labo-orders' },
        { name: 'Báo cáo labo', link: '/labo-order-lines' },
      ]
    },
    {
      name: 'Quản lý kho',
      icon: 'fas fa-th',
      children: [
        { name: 'Phiếu xuất kho', link: '/outgoing-pickings' },
        { name: 'Phiếu xuất kho', link: '/incoming-pickings' },
      ],
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
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        // { name: 'Thanh toán điều trị', link: '/accountpayments', params: { partner_type: 'customer' } },
        { name: 'Khách hàng', link: '/partners', params: { customer: true } },
        { name: 'Nhóm khách hàng', link: '/partner-categories' },
        { name: 'Nhà cung cấp', link: '/partners', params: { supplier: true } },
        { name: 'Nhân viên', link: '/employees' },
        { name: 'Nhóm nhân viên', link: '/employee-categories' },
        // { name: 'Sản phẩm', link: '/products' },
        // { name: 'Nhóm sản phẩm', link: '/product-categories' },
        { name: 'Dịch vụ điều trị', link: '/products/service' },
        { name: 'Nhóm dịch vụ điều trị', link: '/product-categories/service' },
        { name: 'Vật tư', link: '/products/product' },
        { name: 'Nhóm vật tư', link: '/product-categories/product' },
        { name: 'Thuốc', link: '/products/medicine' },
        { name: 'Nhóm thuốc', link: '/product-categories/medicine' },
        { name: 'Labo', link: '/product-labos' },
        { name: 'Tiểu sử bệnh', link: '/histories' },
      ]
    },
    {
      name: 'Cấu hình',
      icon: 'fas fa-cogs',
      children: [
        { name: 'Chi nhánh', link: '/companies' },
        { name: 'Người dùng', link: '/users' },
        { name: 'Nhóm quyền', link: '/res-groups' },
        { name: 'Bảng giá', link: '/price-list-list' },
      ]
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      children: [
        { name: 'Công nợ khách hàng', link: '/account-common-partner-reports', params: { result_selection: 'customer' } },
        { name: 'Công nợ nhà cung cấp', link: '/account-common-partner-reports', params: { result_selection: 'supplier' } },
        { name: 'Xuất nhập tồn', link: '/stock-report-xuat-nhap-ton' },
        { name: 'Thống kê hóa đơn', link: '/account-invoice-reports' },
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
