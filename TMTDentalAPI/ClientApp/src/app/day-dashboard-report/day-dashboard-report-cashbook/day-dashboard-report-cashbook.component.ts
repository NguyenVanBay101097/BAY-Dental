import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import {
  CashBookDetailFilter, CashBookService, CashBookSummarySearch
} from 'src/app/cash-book/cash-book.service';
import {
  DashboardReportService
} from 'src/app/core/services/dashboard-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-cashbook',
  templateUrl: './day-dashboard-report-cashbook.component.html',
  styleUrls: ['./day-dashboard-report-cashbook.component.css']
})
export class DayDashboardReportCashbookComponent implements OnInit {
  cashBookDataReport: any;
  totalCashBook: any[] = [];
  gridDataCashBook: any[] = [];
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
  resultSelection = null;
  summaryResult: any;
  totalCash = 0;
  totalBank = 0;
  dataCashBooks: any[];

  cashbookCashBank: any;
  cashbookCusDebt: any;
  cashbookCusAdvance: any;
  cashbookSupp: any;
  cashbookCusSalary: any;
  cashbookAgentCommission: any;
  totalCashbook: any;
  totalDataCashBook: any;
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Input() companyId: string;

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

  constructor(
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loadCashBankTotal();
    this.loadCashbookReportApi();
    this.loadCashBookGridData();
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
      this.loadSumaryResult();
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
          this.gridDataCashBook = res.data;
          this.loadData();
        },
        (err) => {
        }
      );
  }


  loadDataCashbookReport() {
    this.cashbookCashBank = this.cashBookDataReport[0];
    this.cashbookCusDebt = this.cashBookDataReport[1];
    this.cashbookCusAdvance = this.cashBookDataReport[2];
    this.cashbookSupp = this.cashBookDataReport[3];
    this.cashbookCusSalary = this.cashBookDataReport[4];
    this.cashbookAgentCommission = this.cashBookDataReport[5];
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  loadData(): void {
    var res = this.resultSelection == null ? this.gridDataCashBook : this.gridDataCashBook.filter(x => x.journalType == this.resultSelection);
    this.gridData = {
      data: res.slice(this.skip, this.skip + this.limit),
      total: res.length
    };
  }

  loadSumaryResult() {
    if (this.totalCashBook.length > 0) {
      if (this.resultSelection == 'cash') {
        this.summaryResult = this.totalCashBook[1];
      }
      else if (this.resultSelection == 'bank') {
        this.summaryResult = this.totalCashBook[2];
      } else {
        this.summaryResult = this.totalCashBook[0];
      }
    }

  }

  get ortherThu() {
    if (this.totalCashbook && this.dataCashBooks) {
      return (this.totalCashbook.totalThu ? this.totalCashbook.totalThu : 0) - (this.dataCashBooks.reduce((total, val) => total += val.credit, 0));
    }

    return 0;
  }

  get ortherChi() {
    if (this.totalCashbook) {
      return (this.totalCashbook.totalChi ? this.totalCashbook.totalChi : 0) - (this.cashbookSupp.debit + this.cashbookCusAdvance.debit + this.cashbookCusSalary.debit + this.cashbookAgentCommission.debit);
    }

    return 0;

  }

  onSelectChange(e) {
    if (e) {
      this.resultSelection = e.value;
    } else {
      this.resultSelection = null;
    }

    this.loadData();
    this.loadSumaryResult();
  }
}