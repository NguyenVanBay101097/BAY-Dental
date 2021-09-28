import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { CashBookDetailFilter, CashBookReportFilter, CashBookService, CashBookSummarySearch, SumaryCashBookFilter } from 'src/app/cash-book/cash-book.service';
import { DashboardReportService, SumaryRevenueReportFilter } from 'src/app/core/services/dashboard-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-cashbook',
  templateUrl: './day-dashboard-report-cashbook.component.html',
  styleUrls: ['./day-dashboard-report-cashbook.component.css']
})
export class DayDashboardReportCashbookComponent implements OnInit {
  @Input('dateFrom') dateFrom: any;
  @Input('dateTo') dateTo: any;
  @Input('company') company: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  stateOptions = [
    { text: 'Tiền mặt', value: 'cash' },
    { text: 'Ngân hàng', value: 'bank' },
  ]
  search: string;
  resultSelection = 'cash_bank';
  summarySearchResult: any;
  totalCash = 0;
  totalBank = 0;
  filterSumaryCashbookReport: any[] = [
    { value: 'cash_bank', text: 'TM/CK', code: '131', type: 'customer' },
    { value: 'debt', text: 'công nợ khách hàng', code: 'CNKH', type: 'customer' },
    { value: 'advance', text: 'khách hàng tạm ứng', code: 'KHTU', type: 'customer' },
    { value: 'cash_bank', text: 'Nhà cung cấp ', code: '331', type: 'supplier' },
    { value: 'payroll', text: 'Chi lương và tạm ứng lương nhân viên', code: '334', type: 'customer' },
    { value: 'commission', text: 'Hoa hồng', code: 'HHNGT', type: 'agent' },
    { value: 'all', text: 'Total' },
  ];
  dataCashBooks: any[];

  cashbookCashBank: any;
  cashbookCusDebt: any;
  cashbookCusAdvance: any;
  cashbookSupp: any;
  cashbookCusSalary: any;
  cashbookAgentCommission: any;
  totalCashbook: any;
  totalDataCashBook: any;

  constructor(
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataFromApi();
    this.loadGridData();
    this.loadCashBankTotal();
    this.loadDataCashbookApi();
    this.loadTotalDataFromApi();

  }

  loadCashBankTotal() {
    const companyId = this.company ? this.company.id : '';
    const dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    const dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    var summaryCashSearch = new CashBookSummarySearch();
    summaryCashSearch.resultSelection = 'cash';
    summaryCashSearch.companyId = companyId
    summaryCashSearch.dateFrom = dateFrom
    summaryCashSearch.dateTo = dateTo;

    var summaryBankSearch = new CashBookSummarySearch();
    summaryBankSearch.resultSelection = 'bank';
    summaryBankSearch.companyId = companyId
    summaryBankSearch.dateFrom = dateFrom
    summaryBankSearch.dateTo = dateTo;

    let cash = this.cashBookService.getSumary(summaryCashSearch);
    let bank = this.cashBookService.getSumary(summaryBankSearch);

    forkJoin([cash, bank]).subscribe(results => {
      this.totalCash = results[0].totalAmount;
      this.totalBank = results[1].totalAmount;
    });

  }

  loadDataFromApi() {
    this.loading = true;
    var summarySearch = new CashBookSummarySearch();
    summarySearch.resultSelection = this.resultSelection;
    summarySearch.companyId = this.company ? this.company.id : '';
    summarySearch.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    summarySearch.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    this.cashBookService.getSumary(summarySearch)
      .subscribe(
        (res) => {
          this.summarySearchResult = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  loadDataCashbookApi() {
    forkJoin(this.filterSumaryCashbookReport.map(x => {
      var filter = new SumaryCashBookFilter();
      filter.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
      filter.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
      filter.companyId = this.company ? this.company.id : '';
      filter.resultSelection = x.value;
      filter.accountCode = x.code || '';
      filter.partnerType = x.type || '';
      return this.dashboardReportService.getSumaryRevenueReport(filter).pipe(
        switchMap(total => of({ text: x.value, total: total }))
      );
    })).subscribe((result) => {
      this.dataCashBooks = result.map(x => x.total);
      this.loadDataCashbookSeries();
    });
  }

  loadTotalDataFromApi() {
    this.loading = true;
    var summarySearch = new CashBookSummarySearch();
    summarySearch.resultSelection = 'cash_bank';
    summarySearch.companyId = this.company ? this.company.id : '';
    summarySearch.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    summarySearch.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    this.cashBookService.getSumary(summarySearch)
      .subscribe(
        (res) => {
          this.totalDataCashBook = res;
          this.totalCashbook = this.totalDataCashBook;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  loadDataCashbookSeries() {
    this.cashbookCashBank = this.dataCashBooks[0];
    this.cashbookCusDebt = this.dataCashBooks[1];
    this.cashbookCusAdvance = this.dataCashBooks[2];
    this.cashbookSupp = this.dataCashBooks[3];
    this.cashbookCusSalary = this.dataCashBooks[4];
    this.cashbookAgentCommission = this.dataCashBooks[5];
  }

  get ortherThu() {
    if (this.totalCashbook && this.dataCashBooks) {
      return (this.totalCashbook.totalThu ? this.totalCashbook.totalThu : 0) - (this.dataCashBooks.reduce((total, val) => total += val.credit, 0));
    }
    return 0;
  }

  get ortherChi() {
    if (this.totalCashbook && this.dataCashBooks) {
      return (this.totalCashbook.totalChi ? this.totalCashbook.totalChi : 0) - (this.cashbookSupp.debit + this.cashbookCusAdvance.debit + this.cashbookCusSalary.debit + this.cashbookAgentCommission.debit);
    }
    return 0;
  }

  loadGridData() {
    this.loading = true;
    var gridPaged = new CashBookDetailFilter();
    gridPaged.companyId = this.company ? this.company.id : '';
    gridPaged.resultSelection = this.resultSelection;
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;
    gridPaged.offset = this.skip;
    gridPaged.limit = this.limit;
    gridPaged.search = this.search || '';
    this.cashBookService.getDetails(gridPaged)
      .pipe(
        map((response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          })
      ).subscribe(
        (res) => {
          // console.log(res);
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          this.loading = false;
        }
      );
  }
  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    this.loadGridData();
  }

  onSelectChange(e) {
    if (e) {
      this.resultSelection = e.value;
    } else {
      this.resultSelection = 'cash_bank';
    }
    this.skip = 0;
    this.loadGridData();
    this.loadDataFromApi();

    this.loadDataCashbookApi();
    this.loadTotalDataFromApi();
  }
}
