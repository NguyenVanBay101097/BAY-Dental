import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookDetailFilter, CashBookService, CashBookSummarySearch, DataInvoiceFilter, SumaryCashBookFilter } from 'src/app/cash-book/cash-book.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { DashboardReportService } from 'src/app/core/services/dashboard-report.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderLinePaged } from 'src/app/partners/partner.service';
import { SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';
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
  dateFrom: Date;
  dateTo: Date;
  services: any[] = [];
  dataInvoices: any[] = [];
  companyId: string;
  totalCashBook: any[] = [];
  gridDataCashBook: any[] = [];
  cashBookDataReport: any;
  keyTab = 'registration_service';
  filterResultSelection: any[] = [
    { value: '', text: 'TM/CK' },
    { value: 'cash', text: 'Tiền mặt' },
    { value: 'bank', text: 'Ngân hàng' }
  ];

  filterSumaryCashbookReport: any[] = [
    { value: 'cash_bank', text: 'TM/CK', code: '131', type: 'customer' },
    { value: 'debt', text: 'công nợ khách hàng', code: 'CNKH', type: 'customer' },
    { value: 'advance', text: 'khách hàng tạm ứng', code: 'KHTU', type: 'customer' },
    { value: 'cash_bank', text: 'Nhà cung cấp ', code: '331', type: 'supplier' },
    { value: 'payroll', text: 'Chi lương và tạm ứng lương nhân viên', code: '334', type: 'customer' },
    { value: 'commission', text: 'Hoa hồng', code: 'HHNGT', type: 'agent' },
    { value: 'all', text: 'Total' },
  ];

  maxDate = new Date();
  constructor(
    private authService: AuthService,
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private companyService: CompanyService,
    private dayDashboardReportService: DayDashboardReportService,
    private saleReportService: SaleReportService,
    private dashboardReportService: DashboardReportService,
    private saleOrderLineService: SaleOrderLineService,
    private router: Router
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

    // this.loadAllData();
    this.onLoadTabData(this.keyTab)
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

  loadAllData() {
    this.loadDataServiceApi();
    this.loadDataInvoiceApi();
    this.loadCashBankTotal();
    this.loadCashbookReportApi();
    this.loadCashBookGridData();
  }

  loadDataServiceApi() {
    var val = new SaleOrderLinePaged();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.companyId = this.companyId || '';
    val.state = 'sale,done,cancel';
    this.saleOrderLineService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.services = res.data;
    }, err => {
      console.log(err);
    })
  }

  loadDataInvoiceApi() {
    var gridPaged = new DataInvoiceFilter();
    gridPaged.companyId = this.companyId || '';
    gridPaged.resultSelection = 'all';
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    this.cashBookService.getDataInvoices(gridPaged).subscribe(
      (res: any) => {
        this.dataInvoices = res;
      },
      (err) => {
      }
    );
  }

  loadCashBankTotal() {
    forkJoin(this.filterResultSelection.map(x => {
      var summaryFilter = new CashBookSummarySearch();
      summaryFilter.companyId = this.companyId || '';
      summaryFilter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
      summaryFilter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
      summaryFilter.resultSelection = x.value;
      return this.cashBookService.getSumary(summaryFilter).pipe(
        switchMap(total => of({ text: x.value, total: total }))
      );
    })).subscribe((result) => {
      this.totalCashBook = result.map(x => x.total);
    });
  }

  loadCashbookReportApi() {
    var val = {
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null,
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null,
      companyId: this.companyId ? this.companyId : null
    };

    this.dashboardReportService.getThuChiReport(val).subscribe(result => {
      this.cashBookDataReport = result;
    });
  }

  loadCashBookGridData() {
    var gridPaged = new CashBookDetailFilter();
    gridPaged.companyId = this.companyId || '';
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    gridPaged.offset = 0;
    gridPaged.limit = 0;

    this.cashBookService.getDetails(gridPaged)
      .pipe(
        map((response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          })
      ).subscribe(
        (res) => {
          console.log(res);
          this.gridDataCashBook = res.data;
        },
        (err) => {
        }
      );
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    // this.loadAllData();
    this.onLoadTabData(this.keyTab)
  }

  onChangeDate(value: any) {
    this.dateFrom = value;
    this.dateTo = value;
    // this.loadAllData();
    this.onLoadTabData(this.keyTab)
  }

  onLoadTabData(value) {
    this.keyTab = value;
    if (this.keyTab == 'registration_service') {
      this.loadDataServiceApi();
    }
    if (this.keyTab == 'revenue') {
      this.loadDataInvoiceApi();
    }
    if (this.keyTab == 'cashbook') {
      this.loadCashBankTotal();
      this.loadCashbookReportApi();
      this.loadCashBookGridData();
    }
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
