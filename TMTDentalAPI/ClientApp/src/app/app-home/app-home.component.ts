import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { WebService } from '../core/services/web.service';
import { ChangePasswordDialogComponent } from '../shared/change-password-dialog/change-password-dialog.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { SearchAllService } from '../shared/search-all.service';
import { UserProfileEditComponent } from '../shared/user-profile-edit/user-profile-edit.component';
import { UserChangeCurrentCompanyVM, UserService } from '../users/user.service';

@Component({
  selector: 'app-app-home',
  templateUrl: './app-home.component.html',
  styleUrls: ['./app-home.component.css']
})
export class AppHomeComponent implements OnInit {

  public navItems = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', url: '/dashboard',
    },
    {
      name: 'Khách hàng',
      icon: 'fas fa-users',
      url: '/partners/customers',
      permissions: ['Basic.Partner.Read']
    },
    {
      name: 'Lịch hẹn',
      icon: 'fas fa-calendar-alt',
      url: '/appointments/kanban',
      permissions: ['Basic.Appointment.Read']
    },
    {
      name: 'Bán thuốc',
      icon: 'fas fa-capsules',
      url: '/medicine-orders',
      groups: 'medicineOrder.group_medicine',
      permissions: ['Medicine.ToaThuoc.Read']
    },
    {
      name: 'Labo',
      icon: 'fas fa-tooth',
      url: '/labo-orders',
      children: [
        { name: 'Quản lý phiếu Labo', url: '/labo-orders/service', permissions: ['Labo.LaboOrder.Read', '.LaboOrder.Create', 'Labo.LaboOrder.Update', 'Labo.LaboOrder.Delete'] },
        { name: 'Đơn hàng Labo', url: '/labo-orders/order', permissions: ['Labo.OrderLabo.Read', 'Labo.OrderLabo.Update'] },
        { name: 'Xuất Labo cho khách', url: '/labo-orders/export', permissions: ['Labo.ExportLabo.Read', 'Labo.ExportLabo.Update'] },
      ],
      permissions: ['Labo.LaboOrder.Read', 'Labo.OrderLabo.Read', 'Labo.ExportLabo.Read']
    },
    {
      name: 'Khảo sát đánh giá',
      icon: 'fas fa-poll',
      url: '/surveys',
      children: [
        { name: 'Danh sách khảo sát', url: '/surveys/list', permissions: ['Survey.UserInput.Read'], groups:'survey.group_user,survey.group_manager' },
        { name: 'Quản lý phân việc', url: '/surveys/manage', permissions: ['Survey.Assignment.Read'], groups:'survey.group_manager' },
        { name: 'Câu hỏi khảo sát', url: '/surveys/config', permissions: ['Survey.Question.Read', 'Survey.Question.Create', 'Survey.Question.Update', 'Survey.Question.Delete'], groups:'survey.group_manager' },
        { name: 'Nhãn khảo sát', url: '/surveys/survey-tag', groups: 'survey.group_survey' },
      ],
      groups: 'survey.group_survey',
      permissions: ['Survey.UserInput.Read', 'Survey.Assignment.Read', 'Survey.Question.Read']
    },
    {
      name: 'Mua hàng',
      icon: 'fas fa-shopping-cart',
      url: '/purchase',
      children: [
        { name: 'Mua hàng', url: '/purchase/orders', linkProps: { queryParams: { type: 'order' }}, permissions: ['Purchase.Order.Read', 'Purchase.Order.Create', 'Purchase.Order.Update', 'Purchase.Order.Delete'] },
        { name: 'Trả hàng', url: '/purchase/orders', linkProps: { queryParams: { type: 'refund' }}, permissions: ['Purchase.Order.Read'] },
      ],
      permissions: ['Purchase.Order.Read']
    },
    {
      name: 'Kho',
      icon: 'fas fa-th',
      url: '/stock/stock-report-xuat-nhap-ton',
      permissions: ["Report.Stock", "Stock.Picking.Read", "Basic.ProductRequest.Read", "Stock.Inventory.Read"]
    },
    {
      name: 'Lương',
      icon: 'fas fa-money-bill-alt',
      url: '/hr',
      children: [
        { name: 'Bảng lương', url: '/hr/payslip-run/form', permissions: ['Salary.HrPayslipRun.Read', 'Salary.HrPayslipRun.Create', 'Salary.HrPayslipRun.Update'] },
        { name: 'Chấm công', url: '/hr/time-keepings', permissions: ['Salary.ChamCong.Read'] },
        { name: 'Quản lý tạm ứng - chi lương', url: '/hr/salary-payment', permissions: ['Salary.SalaryPayment.Read'] },
        { name: 'Báo cáo thanh toán lương', url: '/hr/salary-reports', permissions: ['Salary.AccountCommonPartnerReport'] },
      ],
      permissions: ['Salary.HrPayslipRun.Read', 'Salary.ChamCong.Read', 'Salary.SalaryPayment.Read', 'Salary.AccountCommonPartnerReport']
    },
    {
      name: 'Sổ quỹ', icon: 'fas fa-wallet', url: '/cash-book', permissions: ['Account.Read']
    },
    {
      name: 'Khuyến mãi',
      icon: 'fas fa-gift',
      groups: 'sale.group_sale_coupon_promotion',
      url: '/programs/promotion-programs',
      permissions: ['SaleCoupon.SaleCouponProgram.Read']
    },
    {
      name: 'SMS Brandname',
      icon: 'fas fa-sms',
      url: '/sms',
      children: [
        { name: 'Chúc mừng sinh nhật', url: '/sms/birthday-partners', permissions: ['SMS.Message.Read'] },
        { name: 'Nhắc lịch hẹn', url: '/sms/appointment-reminder', permissions: ['SMS.Message.Read'] },
        { name: 'Chăm sóc sau điều trị', url: '/sms/care-after-order', permissions: ['SMS.Message.Read'] },
        { name: 'Tin nhắn cảm ơn', url: '/sms/thanks-customer', permissions: ['SMS.Message.Read'] },
        // { name: 'Quản lý chiến dịch', url: '/sms/campaign', permissions: ['SMS.Campaign.Read'] },
        { name: 'Theo dõi tin nhắn', url: '/sms/statistic', permissions: ['SMS.Report.AllMessage'] },
        { name: 'Báo cáo', url: '/sms/report', permissions: ['SMS.Report.AllSMS'] },
        { name: 'Tin nhắn mẫu', url: '/sms/templates', permissions: ['SMS.Template.Read'] },
        { name: 'Danh sách Brandname', url: '/sms/accounts', permissions: ['SMS.Account.Read'] },
      ],
      groups: 'sms.group_sms',
      permissions: ['SMS.Account.Read', 'SMS.Campaign.Read', 'SMS.Message.Read', 'SMS.Template.Read', 'SMS.Config.Read', 'SMS.Report.AllMessage', 'SMS.Report.AllSMS']
    },
    {
      name: 'Người giới thiệu',
      icon: 'fas fa-poll',
      url: '/agents',
      children: [
        { name: 'Danh sách', url: '/agents/list' },
        { name: 'Thống kê hoa hồng', url: '/agents/commission' },
      ],
      permissions: ['Catalog.Agent.Read','Report.Commission']
    },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      url: '/catalog',
      children: [
        { name: 'Thông tin khách hàng', url: '/catalog/customer-management' },
        { name: 'Hạng thành viên', url: '/catalog/member-level/management'},
        { name: 'Nhà cung cấp', url: '/catalog/suppliers', permissions: ['Basic.Partner.Read'] },
        { name: 'Dịch vụ - Vật tư - Thuốc', url: '/catalog/products', permissions: ['Catalog.Products.Read'] },
        { name: 'Đơn thuốc mẫu', url: '/catalog/sample-prescriptions', permissions: ['Catalog.SamplePrescription.Read'] },
        { name: 'Đơn vị tính', url: '/catalog/uoms', groups: 'product.group_uom', permissions: ['UoM.UoMs.Read'] },
        { name: 'Nhóm Đơn vị tính', url: '/catalog/uom-categories', groups: 'product.group_uom', permissions: ["UoM.UoMCategory.Read"] },
        { name: 'Bảng hoa hồng', url: '/catalog/commissions/v2', permissions: ['Catalog.Commission.Read'] },
        { name: 'Nhân viên', url: '/catalog/employees', permissions: ['Catalog.Employee.Read'] },
        { name: 'Thông số Labo', url: '/catalog/labo-managerment', permissions: ['Catalog.LaboFinishLine.Read', 'Catalog.LaboBridge.Read', 'Catalog.LaboBiteJoint.Read'] },
        { name: 'Loại thu chi', url: '/catalog/loai-thu-chi', permissions: ['Account.LoaiThuChi.Read'] },
        { name: 'Tiêu chí kiểm kho', url: '/catalog/stock/criterias', permissions: ['Stock.StockInventoryCriteria.Read'] },
        { name: 'Chẩn đoán răng', url: '/catalog/tooth-diagnosis', permissions: ['Catalog.ToothDiagnosis.Read'] },
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
      url: '/setting',
      children: [
        { name: 'Chi nhánh', url: '/setting/companies', permissions: ['System.Company.Read'] },
        { name: 'Nhóm quyền', url: '/setting/roles', permissions: ['System.ApplicationRole.Read'] },
        { name: 'Cấu hình chung', url: '/setting/config-settings' }
      ],
      permissions: ['System.Company.Read', 'System.ApplicationUser.Read', 'System.ApplicationRole.Read']
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      url: '/report',
      children: [
        { name: 'Báo cáo tổng quan', url: '/report/sale-dashboard-reports' },
        { name: 'Kết quả kinh doanh', url: '/report/financial-report', permissions: ['Report.Financial'] },
        { name: 'Báo cáo doanh thu', url: '/report/account-invoice-reports/revenue-time', permissions: ['Report.Revenue'] },
        { name: 'Báo cáo dịch vụ', url: '/report/sale-report/service-report', permissions: ['Report.Sale'] },
        { name: 'Báo cáo công nợ khách hàng', url: '/report/report-account-common/partner-debit-report', permissions: ['Report.AccountPartner'] },
        { name: 'Báo cáo tiếp nhận', url: '/report/customer-receipt-reports' },
        { name: 'Công nợ nhà cung cấp', url: '/report/report-account-common/partner', params: { result_selection: 'supplier' }, permissions: ['Report.AccountPartner'] },
        { name: 'Khách hàng lân cận phòng khám', url: '/report/partner-report-location', permissions: ['Report.PartnerLocation'] },
        { name: 'Thống kê nguồn khách hàng', url: '/report/report-partner-sources', permissions: ['Report.PartnerSource'] },
        { name: 'Quản lý điều trị', url: '/report/sale-orders/management', permissions: ['Basic.SaleOrder.Read'] },
        { name: 'Hoa hồng nhân viên', url: '/report/commission-settlements/report', permissions: ['Report.Commission'] },
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

  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  searchString: string = '';
  searchUpdate = new Subject<string>();
  expire = '';
  @ViewChild('searchAllSelect', {static: true}) searchAllSelect: NgSelectComponent;

  minimized = false;

  constructor(
    private modalService: NgbModal,
    public authService: AuthService,
    private router: Router,
    private userService: UserService,
    private webService: WebService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    if (localStorage.getItem('user_change_company_vm')) {
      this.userChangeCurrentCompany = JSON.parse(localStorage.getItem('user_change_company_vm'));
    }
    this.authService.currentUser.subscribe(result => {
      if (result) {
        this.loadChangeCurrentCompany();
        this.loadExpire();
      }
    });
  }

  toggleMinimize(e) {
    this.minimized = e;
  }

  loadExpire() {
    this.webService.getExpire().subscribe((res: any) => {
      if (res && res.expireText) {
        this.expire = res.expireText;
      }
    });
  }

  loadChangeCurrentCompany() {
    this.userService.getChangeCurrentCompany().subscribe(result => {
      this.userChangeCurrentCompany = result;
      localStorage.setItem('user_change_company_vm', JSON.stringify(result));
    });
  }

  switchCompany(companyId) {
    this.userService.switchCompany({ companyId: companyId }).subscribe(() => {
      this.authService.refresh().subscribe(() => {
        this.userService.getChangeCurrentCompany().subscribe(result => {
          localStorage.setItem('user_change_company_vm', JSON.stringify(result));
          var userInfo = JSON.parse(localStorage.getItem("user_info"));
          localStorage.removeItem('user_info');
          userInfo.companyId = result.currentCompany.id;
          localStorage.setItem('user_info', JSON.stringify(userInfo));
          window.location.reload();
        });
      });
    });
  }

  changePassword() {
    let modalRef = this.modalService.open(ChangePasswordDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Đổi mật khẩu';

    modalRef.result.then(() => {
    }, () => {
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  removeSampleData() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa dữ liệu mẫu';
    modalRef.componentInstance.body = 'Hệ thống sẽ xóa toàn bộ dữ liệu về trạng thái ban đầu, bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.webService.removeSampleData().subscribe(
        () => {
          this.notificationService.show({
            content: 'Xóa dữ liệu mẫu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          window.location.reload();
        }
      )
    }, () => { });

  }

  getAvatarImgSource(obj: string) {
    if (obj) {
      return obj;
    } else {
      return '/assets/images/user_avatar.png';
    }
  }

  editProfile(item) {
    let modalRef = this.modalService.open(UserProfileEditComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa thông tin';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.authService.getUserInfo(item.id).subscribe(
        rs => {
          var user_info = JSON.parse(localStorage.getItem('user_info'));
          user_info.name = rs.name;
          user_info.avatar = rs.avatar;
          localStorage.setItem('user_info', JSON.stringify(user_info));
        }
      )
    }, () => {
    });
  }
}
