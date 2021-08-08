import { AppointmentBasic } from './../../../appointment/appointment';
import { DashboardReportService, ReportTodayRequest, RevenueTodayReponse } from './../../../core/services/dashboard-report.service';
import { AuthService } from 'src/app/auth/auth.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM, AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { CashBookService, CashBookSummarySearch } from 'src/app/cash-book/cash-book.service';
import { CheckPermissionService } from '../../check-permission.service';
import { CustomerReceiptBasic, CustomerReceiptPaged, CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { DashboardCustomerReceiptListTodayComponent } from '../dashboard-customer-receipt-list-today/dashboard-customer-receipt-list-today.component';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';

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

  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;
  @ViewChild('customerReceiptComp', { static: false }) customerReceiptComp: DashboardCustomerReceiptListTodayComponent;


  constructor(private checkPermissionService: CheckPermissionService, private dashboardReportService: DashboardReportService,
    private intlService: IntlService,
    private authService: AuthService,
    private appointmentService : AppointmentService,
    private employeeService: EmployeeService,
    private customerReceiptService: CustomerReceiptService,
    private cashBookService: CashBookService) {

  }

  ngOnInit() {
    this.loadTotalCash();
    this.loadCountSaleOrder();
    this.loadMedicalXamination();
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
    this.dashboardReportService.getCountSaleOrder(val).subscribe(rs => {
      this.countSaleOrder = rs;
    }, () => {

    });

  }

  loadMedicalXamination() {
    var val = new ReportTodayRequest();
    val.companyId = this.authService.userInfo.companyId;
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    this.dashboardReportService.getCountMedicalXamination(val).subscribe((rs: any) => {
      this.medicalXamination = rs;
    }, () => {

    });

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
    this.customerReceiptList.push(event);
    this.loadMedicalXamination();
  }

  onUpdateCR(event) {    
    var item = this.customerReceiptList.find(x => x.id === event.id);
    if (item) {
      Object.assign(item, event);
    }
    this.loadMedicalXamination();
  }

  onCreateAP(event) {
    this.appointmenttList.push(event);
    this.loadMedicalXamination();
  }

  onUpdateAP(event) {    
    var item = this.appointmenttList.find(x => x.id === event.id);
    if (item) {
      Object.assign(item, event);
    }
    this.loadMedicalXamination();
  }

  onDeleteAP(event) {    
    const index = this.appointmenttList.findIndex(el => el.id === event.id)
    if (index > -1) {
      this.appointmenttList.splice(index, 1);
    }
    this.loadMedicalXamination();
  }

}
