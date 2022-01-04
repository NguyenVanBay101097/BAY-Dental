import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountInvoiceReportService, RevenueTimeReportPar } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { CashBookReportFilter, CashBookService } from 'src/app/cash-book/cash-book.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { DashboardReportService, ReportRevenueChartFilter } from 'src/app/core/services/dashboard-report.service';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from 'src/app/customer-receipt-reports/customer-receipt-report.service';
import { PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';

@Component({
  selector: 'app-sale-dashboard-report-form',
  templateUrl: './sale-dashboard-report-form.component.html',
  styleUrls: ['./sale-dashboard-report-form.component.css'],
  host: {
    'class': 'w-100 h-100'
  }
})
export class SaleDashboardReportFormComponent implements OnInit {
  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  dateFrom: Date;
  dateTo: Date;
  filteredCompanies: CompanyBasic[] = [];
  companyId: string;
  groupBy: string = 'groupby:day';
  dataNoTreatment: any[] = [];
  dataCustomer: any[] = [];
  saleRevenueCashBookData: any[] = [];
  cashBooks: any[] = [];
  partnerTypes = [
    { text: 'Khách mới', value: 'new' },
    { text: 'Khách quay lại', value: 'old' }
  ];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  monthFrom = new Date(new Date().getFullYear(), 0, 1);
  monthTo = new Date(new Date().getFullYear(), 11, 1);
  chartTimeUnit = 'day';
  revenueActualReportData: any;
  thuChiReportData: any;
  summaryReport: any;

  constructor(
    private revenueReportService: AccountInvoiceReportService,
    private companyService: CompanyService,
    private intlService: IntlService,
    private router: Router,
    private partnerOldNewRpService: PartnerOldNewReportService,
    private customerReceiptReportService: CustomerReceiptReportService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.companyCbx.loading = true),
      switchMap(val => this.searchCompany(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.filteredCompanies = rs.items;
        this.companyCbx.loading = false;
      });

    this.loadCompany();
    this.loadReportAmountTotal();
    this.loadDataRevenueChartApi();
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
  }

  changeCompany(e) {
    this.loadReportAmountTotal();
    this.loadDataRevenueChartApi();
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
  }

  loadCompany() {
    this.searchCompany().subscribe(
      result => {
        this.filteredCompanies = result.items;
      }
    )
  }

  searchCompany(search?: string) {
    var params = new CompanyPaged();
    params.limit = 20;
    params.offset = 0;
    params.search = search || '';
    params.active = true;
    return this.companyService.getPaged(params);
  }

  loadReportAmountTotal() {
    var val = {
      companyId: this.companyId ? this.companyId : null
    }

    this.dashboardReportService.getSummaryReport(val).subscribe(result => {
      this.summaryReport = result;
    });
  }

  loadDataRevenueChartApi() {
    var val = new ReportRevenueChartFilter();
    val.companyId = this.companyId || '';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.groupBy = this.groupBy;

    this.dashboardReportService.getRevenueReportChart(val).subscribe((result: any[]) => {
      this.saleRevenueCashBookData = result;
    })
  }

  loadDataRevenueApi() {
    var val = {
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null,
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null,
      companyId: this.companyId ? this.companyId : null
    };

    this.dashboardReportService.getRevenueActualReport(val).subscribe((result: any) => {
      this.revenueActualReportData = result;
    });
  }

  loadDataCashbookApi() {
    var val = {
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null,
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null,
      companyId: this.companyId ? this.companyId : null,
      groupBy: this.groupBy
    };

    this.cashBookService.getChartReport(val).subscribe(result => {
      this.cashBooks = result;
    })

    this.dashboardReportService.getThuChiReport(val).subscribe(result => {
      this.thuChiReportData = result;
    });
  }

  loadDataNotreatmentApi() {
    var val = new CustomerReceiptReportFilter();
    val.limit = 0;
    val.companyId = this.companyId || '';
    val.state = 'done';
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
      (res: any[]) => {
        this.dataNoTreatment = res;
      },
      (err) => {
        console.log(err);
      }
    );
  }

  loadDataCustomerApi() {
    var val = {
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null,
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null,
      companyId: this.companyId
    }
   
    return this.partnerOldNewRpService.reportByIsNew(val).subscribe((result: any) => {
      this.dataCustomer = result;
    });
  }

  onChangeType(value) {
    this.groupBy = value;
    if (this.groupBy == 'groupby:month') {
      let dateFrom = new Date(new Date().getFullYear(), 0, 1);
      let dateTo = new Date(new Date().getFullYear(), 11, 31);
      this.dateFrom = dateFrom;
      this.dateTo = dateTo;
      this.chartTimeUnit = 'month';
    }
    else {
      this.dateFrom = this.monthStart;
      this.dateTo = this.monthEnd;
      this.chartTimeUnit = 'day';
    }

    this.loadDataRevenueChartApi();
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
  }

  onSearchDateChange(e) {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
    this.loadDataRevenueChartApi();
    this.loadDataRevenueApi();
    this.loadDataCustomerApi();
    this.loadDataNotreatmentApi();
    this.loadDataCashbookApi();
  }

  redirectTo(value) {
    switch (value) {
      case 'cash-book':
        this.router.navigateByUrl("cash-book/tab-cabo");
        break;
      case 'cash-book-cash':
        this.router.navigateByUrl("cash-book/tab-cabo?result_selection=cash");
        break;
      case 'cash-book-bank':
        this.router.navigateByUrl("cash-book/tab-cabo?result_selection=bank");
        break;
      case 'account-invoice-reports':
        this.router.navigateByUrl("account-invoice-reports/revenue-expecting");
        break;
      case "ncc-debit-report":
        this.router.navigateByUrl("report-account-common/partner?result_selection=supplier");
        break;
      case "customer-debit-report":
        this.router.navigateByUrl("report-account-common/partner-debit");
        break;
      case "money-report":
        this.router.navigateByUrl("report-general-ledgers/cash-bank");
        break;
      case "new-old-customer-report":
        this.router.navigateByUrl("sale-report/partner");
        break;
      case "financial-report":
        this.router.navigateByUrl("financial-report");
        break;
      case "chi-report":
        this.router.navigateByUrl("phieu-thu-chi?type=chi");
        break;
      case "thu-report":
        this.router.navigateByUrl("phieu-thu-chi?type=thu");
        break;
      case "hoa-hong-report":
        this.router.navigateByUrl("commission-settlements/report");
        break;
        case "res-insurance-reports":
          this.router.navigateByUrl(value);
        break;
      default:
        break;
    }
  }
}
