import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookService, CashBookSummarySearch } from 'src/app/cash-book/cash-book.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { CustomerReceiptBasic, CustomerReceiptPaged, CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { CheckPermissionService } from '../../check-permission.service';
import { DashboardCustomerReceiptListTodayComponent } from '../dashboard-customer-receipt-list-today/dashboard-customer-receipt-list-today.component';
import { AppointmentBasic } from './../../../appointment/appointment';
import { DashboardReportService, ReportTodayRequest, RevenueTodayReponse } from './../../../core/services/dashboard-report.service';

@Component({
  selector: 'app-reception-dashboard',
  templateUrl: './reception-dashboard.component.html',
  styleUrls: ['./reception-dashboard.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ReceptionDashboardComponent implements OnInit {
  loading = false;
  skip = 0;
  // permission
  canAppointmentLink = this.checkPermissionService.check(['Basic.Appointment.Read']);
  canCashBankReport = this.checkPermissionService.check(['Report.CashBankAccount']);
  canSaleReport = this.checkPermissionService.check(['Report.Sale']);
  canAppointment = this.checkPermissionService.check(['Report.Appointment']);
  public today: Date = new Date(new Date().toDateString());
  totalCash: number = 0;
  countSaleOrder: number = 0;
  medicalXamination: any;
  customerReceiptList: CustomerReceiptBasic[] = [];
  appointmenttList: AppointmentBasic[] = [];
  revenueReportToDay: RevenueTodayReponse;
  initialListEmployees: any[] = [];

  @ViewChild('employeeCbx') employeeCbx: ComboBoxComponent;
  @ViewChild('customerReceiptComp') customerReceiptComp: DashboardCustomerReceiptListTodayComponent;


  constructor(private checkPermissionService: CheckPermissionService, private dashboardReportService: DashboardReportService,
    private intlService: IntlService,
    private authService: AuthService,
    private saleOrderService: SaleOrderService,
    private appointmentService : AppointmentService,
    private employeeService: EmployeeService,
    private customerReceiptService: CustomerReceiptService,
    private cashBookService: CashBookService) {

  }

  ngOnInit() {
    this.loadTotalCash();
    this.loadCountSaleOrder();
    this.loadDataCustomerRecieptApi();
    this.loadDataAppointmentApi();
    this.loadDataRevenueApi();
    this.loadEmployees();
  }

  loadTotalCash() {
    var val = new CashBookSummarySearch();
    val.companyId = this.authService.userInfo.companyId;
    // val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    // val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.resultSelection = 'cash';
    this.cashBookService.getTotal(val).subscribe(rs => {

      this.totalCash = rs;
    }, () => {

    });

  }

  loadCountSaleOrder() {
    var val = new ReportTodayRequest();
    val.companyId = this.authService.userInfo.companyId;
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    this.saleOrderService.getCountSaleOrder(val).subscribe(rs => {
      this.countSaleOrder = rs;
    }, () => {

    });

  }

  getCountIsRepeatCustomer(value) {
    return this.customerReceiptList.filter(x => x.isRepeatCustomer == value).length;
  }

  loadEmployees() {
    var val = new EmployeePaged();
    val.limit = 0;
    val.offset = 0;
    val.active = true;
    val.isDoctor = true;

    this.employeeService
      .getEmployeePaged(val)
      .subscribe((result: any) => {
        this.initialListEmployees = result.items;
      });
  }

  loadDataCustomerRecieptApi() {
    this.loading = true;
    var val = new CustomerReceiptPaged();
    val.limit = 0;
    val.offset = this.skip;
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.companyId = this.authService.userInfo.companyId;
    this.customerReceiptService.getPaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.customerReceiptList = res.data;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataAppointmentApi() {
    this.loading = true;
    var val = new AppointmentPaged();
    val.limit = 0;
    val.offset = this.skip;
    val.dateTimeFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.companyId = this.authService.userInfo.companyId;
    this.appointmentService.loadAppointmentList(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.appointmenttList = res.data;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataRevenueApi() {
    this.loading = true;
    var val = new ReportTodayRequest();
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.companyId = this.authService.userInfo.companyId;
    this.dashboardReportService.getSumary(val).subscribe(result => {
      this.revenueReportToDay = result;
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  onCreateCR(event) {
    this.loadDataCustomerRecieptApi();
  }

  onSuccessReceiveAppointment(data) {
    this.loadDataAppointmentApi();
    this.loadDataCustomerRecieptApi();
  }

  onUpdateCR(event) {    
    this.loadDataCustomerRecieptApi();
  }

  onCreateAP(event) {
    this.loadDataAppointmentApi();
  }

  onUpdateAP(event) {    
    this.loadDataAppointmentApi();
  }

  onDeleteAP(event) {    
    this.loadDataAppointmentApi();
  }

}
