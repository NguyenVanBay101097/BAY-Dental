import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PermissionService } from '../permission.service';

@Component({
  selector: 'app-layout-sidebar',
  templateUrl: './layout-sidebar.component.html',
  styleUrls: ['./layout-sidebar.component.css']
})
export class LayoutSidebarComponent implements OnInit {
  activeIndex = -1;
  folded = false;

  menuItems: any[] = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/dashboard',
    },
    {
      name: 'Khách hàng',
      icon: 'fas fa-users',
      children: [],
      link: '/partners/customers',
      permissions: ['Basic.Partner.Read']
    },
    // {
    //   name: 'Lịch sử điều trị', icon: 'fa fa-history', children: [], link: '/treatment-lines/history'
    // },
    {
      name: 'Quản lý lịch hẹn',
      icon: 'fas fa-calendar-alt',
      children: [
        { name: 'Lịch hẹn', link: '/appointments/kanban', permissions: ['Basic.Appointment.Read'] },
        { name: 'Quá hạn/ Hủy hẹn', link: '/appointments/over-cancel', permissions: ['Basic.Appointment.Read'] }
      ],
      permissions: ['Basic.Appointment.Read']
    },
    {
      name: 'Bán thuốc',
      icon: 'fas fa-capsules',
      children: [],
      link: '/medicine-orders',
      groups: 'medicineOrder.group_medicine',
      permissions: ['Medicine.ToaThuoc.Read']
    },
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
        { name: 'Quản lý phiếu Labo', link: '/labo-orders/service', permissions: ['Labo.LaboOrder.Read', '.LaboOrder.Create', 'Labo.LaboOrder.Update', 'Labo.LaboOrder.Delete'] },
        { name: 'Đơn hàng Labo', link: '/labo-orders/order', permissions: ['Labo.OrderLabo.Read', 'Labo.OrderLabo.Update'] },
        { name: 'Xuất Labo cho khách', link: '/labo-orders/export', permissions: ['Labo.ExportLabo.Read', 'Labo.ExportLabo.Update'] },
      ],
      permissions: ['Labo.LaboOrder.Read', 'Labo.OrderLabo.Read', 'Labo.ExportLabo.Read']
    },
    // {
    //   name: 'Khảo sát đánh giá',
    //   icon: 'fas fa-poll',
    //   children: [
    //     { name: 'Danh sách khảo sát', link: '/surveys', groups:'survey_Nhanvien.survey_assignment_Nhanvien,survey_Quanly.survey_assignment_Quanly' },
    //     { name: 'Quản lý phân việc', link: '/surveys/manage', groups: 'survey_Quanly.survey_assignment_Quanly' },
    //     { name: 'Cấu hình đánh giá', link: '/surveys/config', groups: 'survey_Quanly.survey_assignment_Quanly' },
    //   ],
    //   groups: 'survey_Nhanvien.survey_assignment_Nhanvien,survey_Quanly.survey_assignment_Quanly'
    // },
    {
      name: 'Khảo sát đánh giá',
      icon: 'fas fa-poll',
      children: [
        { name: 'Danh sách khảo sát', link: '/surveys', groups: 'survey.group_user,survey.group_manager', permissions: ['Survey.UserInput.Read'] },
        { name: 'Quản lý phân việc', link: '/surveys/manage', groups: 'survey.group_manager', permissions: ['Survey.Assignment.Read'] },
        { name: 'Câu hỏi khảo sát', link: '/surveys/config', groups: 'survey.group_manager', permissions: ['Survey.Question.Read', 'Survey.Question.Create', 'Survey.Question.Update', 'Survey.Question.Delete'] },
      ],
      groups: 'survey.group_survey',
      permissions: ['Survey.UserInput.Read', 'Survey.Assignment.Read', 'Survey.Question.Read']
    },
    {
      name: 'Mua hàng',
      icon: 'fas fa-shopping-cart',
      children: [
        { name: 'Mua hàng', link: '/purchase/orders', params: { type: 'order' }, permissions: ['Purchase.Order.Read', 'Purchase.Order.Create', 'Purchase.Order.Update', 'Purchase.Order.Delete'] },
        { name: 'Trả hàng', link: '/purchase/orders', params: { type: 'refund' }, permissions: ['Purchase.Order.Read'] },
      ],
      permissions: ['Purchase.Order.Read']
    },
    {
      name: 'Kho',
      icon: 'fas fa-th',
      link: '/stock/stock-report-xuat-nhap-ton',
      children: [
      ],
      permissions: ["Report.Stock","Stock.Picking.Read","Basic.ProductRequest.Read","Stock.Inventory.Read"]
    },
    {
      name: 'Lương',
      icon: 'fas fa-money-bill-alt',
      children: [
        { name: 'Bảng lương', link: '/hr/payslip-run/form', permissions: ['Salary.HrPayslipRun.Read', 'Salary.HrPayslipRun.Create', 'Salary.HrPayslipRun.Update'] },
        // { name: 'Phiếu lương', link: '/hr/payslips' },
        { name: 'Chấm công', link: '/time-keepings', permissions: ['Salary.ChamCong.Read'] },
        { name: 'Quản lý tạm ứng - chi lương', link: '/salary-payment', permissions: ['Salary.SalaryPayment.Read'] },
        { name: 'Báo cáo thanh toán lương', link: '/hr/salary-reports', permissions: ['Salary.AccountCommonPartnerReport'] },
      ],
      permissions: ['Salary.HrPayslipRun.Read', 'Salary.ChamCong.Read', 'Salary.SalaryPayment.Read', 'Salary.AccountCommonPartnerReport']
    },
    {
      name: 'Sổ quỹ', icon: 'fas fa-wallet', children: [], link: '/cash-book', permissions: ['Account.Read']
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
        { name: 'Tạo đơn bán thẻ', link: '/service-card-orders/create-card-order', permissions: ['ServiceCard.Order.Create'] },
        { name: 'Đơn bán thẻ', link: '/service-card-orders', permissions: ['ServiceCard.Order.Read'] },
        { name: 'Loại thẻ', link: '/service-card-types', permissions: ['ServiceCard.Type.Create'] },
        { name: 'Danh sách thẻ', link: '/service-cards', permissions: ['ServiceCard.Card.Create'] },
      ],
      permissions: ['ServiceCard.Order.Create', 'ServiceCard.Order.Read', 'ServiceCard.Type.Create', 'ServiceCard.Card.Create']
    },
    {
      name: 'Khuyến mãi',
      icon: 'fas fa-gift',
      groups: 'sale.group_sale_coupon_promotion',
      children: [
        { name: 'Chương trình coupon', link: '/programs/coupon-programs', permissions: ['SaleCoupon.SaleCoupons.Read'] },
        { name: 'Chương trình khuyến mãi', link: '/programs/promotion-programs', permissions: ['SaleCoupon.SaleCouponProgram.Read'] },
      ],
      permissions: ['SaleCoupon.SaleCoupons.Read', 'SaleCoupon.SaleCouponProgram.Read']
    },
    {
      name: 'Thành viên',
      icon: 'fas fa-credit-card',
      groups: 'sale.group_loyalty_card',
      children: [
        { name: 'Thẻ thành viên', link: '/card-cards', permissions: ['LoyaltyCard.CardCard.Read'] },
        { name: 'Loại thẻ thành viên', link: '/card-types', permissions: ['LoyaltyCard.CardType.Read'] },
      ],
      permissions: ['LoyaltyCard.CardCard.Read', 'LoyaltyCard.CardType.Read']
    },
    {
      name: 'Chăm sóc tự động',
      icon: 'fab fa-facebook-f',
      children: [
        { name: 'Kết nối facebook page', link: '/socials/facebook-connect' },
        { name: 'Kết nối Zalo', link: '/zalo-config' },
        { name: 'Danh sách kênh', link: '/socials/channels', permissions: ['TCare.Channel.Read'] },
        { name: 'Kịch bản', link: '/tcare/scenarios', permissions: ['TCare.Scenario.Read'] },
        { name: 'Thống kê gửi tin', link: '/tcare/messagings', permissions: ['TCare.Messaging.Read'] },
        { name: 'Mẫu tin nhắn', link: '/tcare/message-templates', permissions: ['TCare.MessTemplate.Read'] },
        { name: 'Thiết lập tự động', link: '/tcare/config', permissions: ['TCare.Config.Create'] },
      ],
      groups: 'tcare.group_tcare',
      permissions: ['TCare.Channel.Read', 'TCare.Scenario.Read', 'TCare.Messaging.Read', 'TCare.MessTemplate.Read', 'TCare.Config.Create']
    },
    {
      name: 'Hoa hồng',
      icon: 'fas fa-poll',
      children: [
        { name: 'Người giới thiệu', link: '/agents/commission' },
      ],
      permissions: ['Catalog.Agent.Read']
    },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Thông tin khách hàng', link: '/partners/customer-management', permissions: ['Catalog.PartnerCategory.Read'] },
        { name: 'Nhãn khảo sát', link: '/surveys/survey-tag', groups: 'survey.group_survey' },
        // { name: "Nguồn khách hàng", link: "/partner-sources" },
        { name: 'Nhà cung cấp', link: '/partners/suppliers' },
        { name: 'Người giới thiệu', link: '/agents' },
        { name: 'Dịch vụ - Vật tư - Thuốc', link: '/products', permissions: ['Catalog.Products.Read'] },
        { name: 'Đơn thuốc mẫu', link: '/sample-prescriptions', permissions: ['Catalog.SamplePrescription.Read'] },
        // { name: 'Tiểu sử bệnh', link: '/histories' },
        // { name: 'Danh xưng', link: '/partner-titles' },
        { name: 'Đơn vị tính', link: '/uoms', groups: 'product.group_uom', permissions: ['UoM.UoMs.Read'] },
        { name: 'Nhóm Đơn vị tính', link: '/uom-categories', groups: 'product.group_uom', permissions:["UoM.UoMCategory.Read"] },
        { name: 'Bảng hoa hồng', link: '/commissions/v2', permissions: ['Catalog.Commission.Read'] },
        { name: 'Nhân viên', link: '/employees', permissions: ['Catalog.Employee.Read'] },
        { name: 'Thông số Labo', link: '/labo-orders/labo-managerment',permissions:["Catalog.Products.Read"] },
        { name: 'Loại thu chi', link: '/loai-thu-chi', permissions: ['Account.LoaiThuChi.Read'] },
        { name: 'Tiêu chí kiểm kho', link: '/stock/criterias', permissions: ['Stock.Criteria.Read'] },
        { name: 'Thông tin chẩn đoán răng', link: '/tooth-diagnosis' },
        // { name: 'Loại chi', link: '/loai-thu-chi', params: { type: 'chi' }},
        // { name: 'Vật liệu Labo', link: '/products/labos' },
        // { name: 'Đường hoàn tất', link: '/labo-finish-lines' },
        // { name: 'Gửi kèm Labo', link: '/products/labo-attachs' },
        // { name: 'Kiểu nhịp Labo', link: '/labo-bridges' },
        // { name: 'Khớp cắn Labo', link: '/labo-bite-joints' },
      ],
      permissions: [
        'Catalog.PartnerCategory.Read',
        'Catalog.Products.Read',
        'Catalog.Agent.Read',
        'Catalog.SamplePrescription.Read',
        'UoM.UoMs.Read',
        'Catalog.Commission.Read',
        'Catalog.Employee.Read',
        'Stock.Criteria.Read',
        'Account.LoaiThuChi.Read'
      ]
    },
    {
      name: 'Cấu hình',
      icon: 'fas fa-cogs',
      children: [
        { name: 'Chi nhánh', link: '/companies', permissions: ['System.Company.Read'] },
        { name: 'Người dùng', link: '/users', permissions: ['System.ApplicationUser.Read'] },
        { name: 'Nhóm quyền', link: '/roles', permissions: ['System.ApplicationRole.Read'] },
        { name: 'Cấu hình chung', link: '/config-settings' }
      ],
      permissions: ['System.Company.Read', 'System.ApplicationUser.Read', 'System.ApplicationRole.Read']
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      children: [
        { name: 'Báo cáo tổng quan', link: '/sale-dashboard-reports' },
        { name: 'Kết quả kinh doanh', link: '/financial-report', permissions: ['Report.Financial'] },
        { name: 'Tiền mặt, ngân hàng', link: '/report-general-ledgers/cash-bank', permissions: ['Report.CashBankAccount'] },
        { name: 'Thống kê doanh thu', link: '/revenue-report', permissions: ['Report.Revenue'] },
        { name: 'Thống kê điều trị', link: '/sale-report', permissions: ['Report.Sale'] },
        { name: 'Công nợ khách hàng', link: '/report-account-common/partner', params: { result_selection: 'customer' }, permissions: ['Report.AccountPartner'] },
        { name: 'Công nợ nhà cung cấp', link: '/report-account-common/partner', params: { result_selection: 'supplier' }, permissions: ['Report.AccountPartner'] },
        { name: 'Xuất nhập tồn', link: '/stock-report-xuat-nhap-ton', permissions: ['Report.Stock'] },
        { name: 'Thống kê tình hình thu nợ khách hàng', link: '/real-revenue-report', permissions: ['Report.RealRevenue'] },
        // { name: 'Thống kê công việc', link: '/dot-kham-report' },
        { name: 'Thống kê hoa hồng', link: '/commission-settlements/report', permissions: ['Report.Commission'] },
        { name: 'Khách hàng lân cận phòng khám', link: '/partner-report-location', permissions: ['Report.PartnerLocation'] },
        { name: 'Thống kê nguồn khách hàng', link: '/report-partner-sources', permissions: ['Report.PartnerSource'] },
        { name: 'Thống kê khách hàng cũ mới', link: '/sale-report/partner', permissions: ['Report.PartnerOldNew'] },
        // { name: 'Thống kê khách hàng', link: '/customer-statistics' },
        // { name: 'Thống kê khách hàng cũ mới', link: '/sale-report/old-new-partner' },
      ],
      permissions: [
        'Report.Financial',
        'Report.CashBankAccount',
        'Report.Revenue',
        'Report.Sale',
        'Report.AccountPartner',
        'Report.AccountPartner',
        'Report.Stock',
        'Report.RealRevenue',
        'Report.Commission',
        'Report.PartnerLocation',
        'Report.PartnerSource',
        'Report.PartnerOldNew'
      ]
    },
  ];

  menus: any[] = [];


  constructor(public sidebarService: NavSidebarService, public authService: AuthService,
    private permissionService: PermissionService) { }

  ngOnInit() {
    this.menus = this.filterMenus();
    this.permissionService.permissionStoreChangeEmitter.subscribe(() => {
      this.menus = this.filterMenus();
    });
  }

  filterMenus() {
    var menuItems = this.menuItems.filter(x => {
      if (x.groups) {
        return this.permissionService.hasDefined(x.groups);
      }

      return true;
    });

    var list: any[] = [];

    for (var i = 0; i < menuItems.length; i++) {
      var menuItem = menuItems[i];
      if (this.hasPermission(menuItem)) {
       // list.push(menuItem);
        if (menuItem.children) {
          var childArr : any[] = []; 
          menuItem.children.forEach(child => {
            if (this.hasPermission(child)) {
              childArr.push(child);
            }
          });
          menuItem.children = childArr;
        }
        list.push(menuItem);
      }
    }

    return list;
  }

  hasPermission(menuItem) {
    var pm = localStorage.getItem("user_permission");
    if (pm != null) {
      var user_permission = JSON.parse(pm);
      if (user_permission.isUserRoot) {
        return true;
      }

      if (menuItem.permissions) {
        var listPermission = user_permission.permission;
        for (var i = 0; i < menuItem.permissions.length; i++) {
          var permission = menuItem.permissions[i];
          if (listPermission.includes(permission)) {
            return true;
          }
        }

        return false;
      }

      return true;
    }

    return false;
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
