import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
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
  @Input() cashBookDataReport: any[] = [];
  @Input() totalCashBook: any[] = [];
  @Input() gridDataCashBook: any[] = [];
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

  constructor(
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadSumaryResult();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
    this.loadDataCashbookReport();
    this.loadSumaryResult();
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
    debugger
    var res = this.resultSelection == null ? this.gridDataCashBook : this.gridDataCashBook.filter(x => x.journalType == this.resultSelection);
    this.gridData = {
      data: res.slice(this.skip, this.skip + this.limit),
      total: res.length
    };
  }

  loadSumaryResult() {
    if(this.totalCashBook.length > 0){
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
