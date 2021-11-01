import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
// import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
// import { forkJoin, of } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import {
  CashBookService,
  // CashBookDetailFilter, CashBookSummarySearch, DataInvoiceFilter, SumaryCashBookFilter
} from 'src/app/cash-book/cash-book.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { DashboardReportService } from 'src/app/core/services/dashboard-report.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
// import { SaleOrderLinePaged } from 'src/app/partners/partner.service';
import {
  // SaleReportSearch,
  SaleReportService
} from 'src/app/sale-report/sale-report.service';
import { DayDashboardReportCashbookComponent } from '../day-dashboard-report-cashbook/day-dashboard-report-cashbook.component';
import { DayDashboardReportRegistrationServiceComponent } from '../day-dashboard-report-registration-service/day-dashboard-report-registration-service.component';
import { DayDashboardReportRevenueServiceComponent } from '../day-dashboard-report-revenue-service/day-dashboard-report-revenue-service.component';
import { DayDashboardReportService, ExportExcelDashBoardDayFilter } from '../day-dashboard-report.service';

@Component({
  selector: 'app-day-dashboard-report-management',
  templateUrl: './day-dashboard-report-management.component.html',
  styleUrls: ['./day-dashboard-report-management.component.css']
})
export class DayDashboardReportManagementComponent implements OnInit {
  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;
  listCompany: CompanySimple[] = [];
  date = new Date();
  dateFrom: Date = new Date();
  dateTo: Date = new Date();
  services: any[] = [];
  dataInvoices: any[] = [];
  companyId: string;
  totalCashBook: any[] = [];
  gridDataCashBook: any[] = [];
  cashBookDataReport: any;
  keyTab = 'registration_service';

  maxDate = new Date();

  active = 'services';
  @ViewChild('servicesComp', { static: false }) servicesComp: DayDashboardReportRegistrationServiceComponent;
  @ViewChild('revenueComp', { static: false }) revenueComp: DayDashboardReportRevenueServiceComponent;
  @ViewChild('cashbookComp', { static: false }) cashbookComp: DayDashboardReportCashbookComponent;
  constructor(
    private authService: AuthService,
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private companyService: CompanyService,
    private dayDashboardReportService: DayDashboardReportService,
    private saleReportService: SaleReportService,
    private dashboardReportService: DashboardReportService,
    private saleOrderLineService: SaleOrderLineService,
    private router: Router,
    public route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.dateFrom = this.date;
    this.dateTo = this.date;
    this.loadCompanies();
    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany$(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.listCompany = res.items;
    });
  }

  onSelectCompany(e) {
    this.onLoadTabData();
  }

  onChangeDate(value: any) {
    this.dateFrom = this.date;
    this.dateTo = this.date;
    this.onLoadTabData();
  }

  onLoadTabData() {
    setTimeout(() => {
      this.servicesComp?.loadDataServiceApi();
      this.revenueComp?.loadDataInvoiceApi();
      this.cashbookComp?.loadDataFromApi();
    });
  }

  exportExcelFile() {
    let val = new ExportExcelDashBoardDayFilter();
    val.dateFrom = this.companyId || '';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    this.dayDashboardReportService.exportReportDayExcel(val).subscribe((res: any) => {
      let filename = "BaoCaoTongQuanNgay";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

  public disabledDates = (date: Date): boolean => {
    return date >= this.maxDate;
  }
}
