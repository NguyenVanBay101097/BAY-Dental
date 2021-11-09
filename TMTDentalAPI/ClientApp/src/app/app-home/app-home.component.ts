import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { IrConfigParameterService } from '../core/services/ir-config-parameter.service';
import { SessionInfoStorageService } from '../core/services/session-info-storage.service';
import { WebSessionService } from '../core/services/web-session.service';
import { WebService } from '../core/services/web.service';
import { ChangePasswordDialogComponent } from '../shared/change-password-dialog/change-password-dialog.component';
import { CheckPermissionService } from '../shared/check-permission.service';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { ImportSampleDataComponent } from '../shared/import-sample-data/import-sample-data.component';
import { PermissionService } from '../shared/permission.service';
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
      children: [
        { name: 'Quản lý phiếu Labo', url: '/labo-orders/service', permissions: ['Labo.LaboOrder.Read', '.LaboOrder.Create', 'Labo.LaboOrder.Update', 'Labo.LaboOrder.Delete'] },
        { name: 'Đơn hàng Labo', url: '/labo-orders/order', permissions: ['Labo.OrderLabo.Read', 'Labo.OrderLabo.Update'] },
        { name: 'Xuất Labo cho khách', url: '/labo-orders/export', permissions: ['Labo.ExportLabo.Read', 'Labo.ExportLabo.Update'] },
        { name: 'Quản lý bảo hành', url: '/labo-orders/warranty', permissions: ['Labo.LaboWarranty.Read', '.LaboWarranty.Create', 'Labo.LaboWarranty.Update', 'Labo.LaboWarranty.Delete'] },
      ],
      permissions: ['Labo.LaboOrder.Read', 'Labo.OrderLabo.Read', 'Labo.ExportLabo.Read', 'Labo.LaboWarranty.Read']
    },
    {
      name: 'Khảo sát đánh giá',
      icon: 'fas fa-poll',
      url: '/surveys',
      children: [
        { name: 'Danh sách khảo sát', url: '/surveys/list', permissions: ['Survey.UserInput.Read'], groups: 'survey.group_user,survey.group_manager' },
        { name: 'Quản lý phân việc', url: '/surveys/manage', permissions: ['Survey.Assignment.Read'], groups: 'survey.group_manager' },
        { name: 'Câu hỏi khảo sát', url: '/surveys/config', permissions: ['Survey.Question.Read', 'Survey.Question.Create', 'Survey.Question.Update', 'Survey.Question.Delete'], groups: 'survey.group_manager' },
      ],
      groups: 'survey.group_survey',
      permissions: ['Survey.UserInput.Read', 'Survey.Assignment.Read', 'Survey.Question.Read']
    },
    {
      name: 'Mua hàng',
      icon: 'fas fa-shopping-cart',
      children: [
        { name: 'Mua hàng', url: '/purchase/order', permissions: ['Purchase.Order.Read', 'Purchase.Order.Create', 'Purchase.Order.Update', 'Purchase.Order.Delete'] },
        { name: 'Trả hàng', url: '/purchase/refund', permissions: ['Purchase.Order.Read'] },
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
      name: 'Marketing',
      icon: 'fas fa-gift',
      groups: 'sale.group_sale_coupon_promotion',
      children: [
        { name: 'Chương trình khuyến mãi', url: '/programs/promotion-programs', permissions: ['SaleCoupon.SaleCouponProgram.Read'] },
        { name: 'Quản lý thẻ', url: '/service-card', permissions: ['ServiceCard.Card.Read'] },
        { name: 'Loại thẻ', url: '/card-types', permissions: ['ServiceCard.Type.Read'] },
      ],
      permissions: ['SaleCoupon.SaleCouponProgram.Read', 'ServiceCard.Card.Read']
    },
    {
      name: 'SMS Brandname',
      icon: 'fas fa-sms',
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
      name: 'Hoa hồng',
      icon: 'fas fa-poll',
      children: [
        { name: 'Người giới thiệu', url: '/commission-settlements/agent' },
        { name: 'Nhân viên', url: '/commission-settlements/employee', permissions: ['Report.Commission'] },
      ],
      permissions: ['Report.Commission']
    },
    {
      name: 'Danh mục',
      icon: 'fas fa-list',
      children: [
        { name: 'Thông tin khách hàng', url: '/customer-management' },
        { name: 'Hạng thành viên', url: '/member-level/management' },
        { name: 'Nhà cung cấp', url: '/partners/suppliers', permissions: ['Basic.Partner.Read'] },
        { name: 'Dịch vụ - Vật tư - Thuốc', url: '/products', permissions: ['Catalog.Products.Read'] },
        { name: 'Đơn thuốc mẫu', url: '/sample-prescriptions', permissions: ['Catalog.SamplePrescription.Read'] },
        { name: 'Đơn vị tính', url: '/uoms', groups: 'product.group_uom', permissions: ['UoM.UoMs.Read'] },
        { name: 'Nhóm Đơn vị tính', url: '/uom-categories', groups: 'product.group_uom', permissions: ["UoM.UoMCategory.Read"] },
        { name: 'Bảng hoa hồng', url: '/commissions', permissions: ['Catalog.Commission.Read'] },
        { name: 'Nhân viên', url: '/employees', permissions: ['Catalog.Employee.Read'] },
        { name: 'Chức vụ nhân viên', url: '/hr/jobs' },
        { name: 'Thông số Labo', url: '/labo-management', permissions: ['Catalog.LaboFinishLine.Read', 'Catalog.LaboBridge.Read', 'Catalog.LaboBiteJoint.Read'] },
        { name: 'Loại thu chi', url: '/loai-thu-chi', permissions: ['Account.LoaiThuChi.Read'] },
        { name: 'Tiêu chí kiểm kho', url: '/stock/criterias', permissions: ['Stock.StockInventoryCriteria.Read'] },
        { name: 'Chẩn đoán răng', url: '/tooth-diagnosis', permissions: ['Catalog.ToothDiagnosis.Read'] },
        { name: 'Nhãn khảo sát', url: '/surveys/survey-tag', groups: 'survey.group_survey' },
        { name: 'Người giới thiệu', url: '/agents/list', Permissions: ['Catalog.Agent.Read'] },
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
      id: 'settingMenu',
      children: [
        { name: 'Chi nhánh', url: '/companies', permissions: ['System.Company.Read'] },
        { name: 'Nhóm quyền', url: '/roles', permissions: ['System.ApplicationRole.Read'] },
        { name: 'Cấu hình chung', url: '/config-settings' },
        // { name: 'Mẫu in', url: '/print-template-config' },
        // { name: 'Thiết lập kết nối API', url: '/setting-public-api' }
      ],
      permissions: ['System.Company.Read', 'System.ApplicationUser.Read', 'System.ApplicationRole.Read']
    },
    {
      name: 'Báo cáo',
      icon: 'far fa-chart-bar',
      id: 'reportMenu',
      children: [
        { name: 'Báo cáo tổng quan', url: '/sale-dashboard-reports' },
        { name: 'Báo cáo tổng quan ngày', url: '/day-dashboard-report' },
        { name: 'Kết quả kinh doanh', url: '/financial-report', permissions: ['Report.Financial'] },
        { name: 'Báo cáo doanh thu', url: '/account-invoice-reports/revenue-time', permissions: ['Report.Revenue'] },
        { name: 'Báo cáo dịch vụ', url: '/sale-report/service-report', permissions: ['Report.Sale'] },
        { name: 'Báo cáo khách hàng', url: '/report-account-common/partner-report-overview', permissions: ['Report.PartnerOldNew'] },
        { name: 'Báo cáo tiếp nhận', url: '/customer-receipt-reports' },
        { name: 'Công nợ nhà cung cấp', url: '/report-account-common/partner', linkProps: { queryParams: { result_selection: 'supplier' } }, permissions: ['Report.AccountPartner'] },
        { name: 'Thống kê nguồn khách hàng', url: '/report-partner-sources', permissions: ['Report.PartnerSource'] },
        { name: 'Quản lý điều trị', url: '/sale-orders/management', permissions: ['Basic.SaleOrder.Read'] },
      ],
      permissions: [
        'Report.Financial',
        // 'Report.CashBankAccount',
        'Report.Revenue',
        'Report.Sale',
        // 'Report.AccountPartner',
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

  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  searchString: string = '';
  searchUpdate = new Subject<string>();
  expire = '';
  @ViewChild('searchAllSelect', { static: true }) searchAllSelect: NgSelectComponent;

  minimized = false;
  irImportSampleData = null;
  sessionInfo: any;

  constructor(
    private modalService: NgbModal,
    public authService: AuthService,
    private router: Router,
    private userService: UserService,
    private webService: WebService,
    private notificationService: NotificationService,
    private permissionService: PermissionService,
    private irConfigParamService: IrConfigParameterService,
    private sessionInfoStorageService: SessionInfoStorageService,
    private webSessionService: WebSessionService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.sessionInfo = this.sessionInfoStorageService.getSessionInfo();
    this.menus = this.filterMenus();
    this.loadIrConfigParam();
  }

  showExpirationDate(dateStr) {
    const expireDate = new Date(dateStr);
    const date = new Date();
    var diff = expireDate.getTime() - date.getTime();

    var days = Math.floor(diff / (1000 * 60 * 60 * 24));
    diff -=  days * (1000 * 60 * 60 * 24);
    
    var hours = Math.floor(diff / (1000 * 60 * 60));
    diff -= hours * (1000 * 60 * 60);
    
    var mins = Math.floor(diff / (1000 * 60));
    diff -= mins * (1000 * 60);

    return `${days} ngày ${hours} giờ ${mins} phút`;
  }

  filterMenus() {
    var menuItems = this.navItems.filter(x => {
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
          var childArr: any[] = [];
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
    if (!menuItem.permissions) {
      return true;
    }

    return this.checkPermissionService.check(menuItem.permissions);
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
    this.userService.switchCompany({ companyId: companyId })
      .pipe(
        mergeMap(() => {
          return this.authService.refresh();
        }),
        mergeMap(() => {
          return this.webSessionService.getSessionInfo();
        }),
        mergeMap((sessionInfo) => {
          this.sessionInfoStorageService.saveSession(sessionInfo);
          return this.webSessionService.getCurrentUserInfo();
        }),
      )
      .subscribe(userInfo => {
        localStorage.setItem('user_info', JSON.stringify(userInfo));
        window.location.reload();
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

  loadIrConfigParam() {
    var key = "import_sample_data";
    this.irConfigParamService.getParam(key).subscribe(
      (result: any) => {
        this.irImportSampleData = result.value;
        if (!this.irImportSampleData) {
          this.openPopupImportSimpleData();
        }
      }
    )
  }

  openPopupImportSimpleData() {
    const modalRef = this.modalService.open(ImportSampleDataComponent, { scrollable: true, size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.value = this.irImportSampleData;
    modalRef.result.then(result => {
      if (result) {
        this.notificationService.show({
          content: 'Khởi tạo dữ liệu mẫu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        window.location.reload();
      }
    }, err => {
      console.log(err);
    });
  }
}
