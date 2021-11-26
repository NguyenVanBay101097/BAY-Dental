import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { CashBookService, DataInvoiceFilter } from 'src/app/cash-book/cash-book.service';
import { DashboardReportService } from 'src/app/core/services/dashboard-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-revenue-service',
  templateUrl: './day-dashboard-report-revenue-service.component.html',
  styleUrls: ['./day-dashboard-report-revenue-service.component.css']
})
export class DayDashboardReportRevenueServiceComponent implements OnInit {
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Input() companyId: string;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  dataInvoices: any[] = [];
  revenueActualReportData: any;

  constructor(
    private intlService: IntlService,
    private cashBookService: CashBookService,
    private dashboardReportService: DashboardReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataInvoiceApi();
    this.loadDataRevenueApi();
  }

  loadDataInvoiceApi() {
    var gridPaged = new DataInvoiceFilter();
    gridPaged.companyId = this.companyId;
    gridPaged.resultSelection = 'all';
    gridPaged.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : null;
    gridPaged.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : null;

    this.cashBookService.getDataInvoices(gridPaged).subscribe(
      (res: any) => {
        this.dataInvoices = res;
        this.loadData();
      },
      (err) => {
      }
    );
  }

  loadDataRevenueApi() {
    var val = {
      dateFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null,
      dateTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null,
      companyId: this.companyId ? this.companyId : null
    };

    this.dashboardReportService.getRevenueActualReport(val).subscribe(result => {
      this.revenueActualReportData = result;
    });
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  loadData(): void {
    this.gridData = {
      data: this.dataInvoices.slice(this.skip, this.skip + this.limit),
      total: this.dataInvoices.length
    };
  }

  sum(field): any {
    if (this.dataInvoices.length == 0) {
      return 0;
    } else {
      var res = aggregateBy(this.dataInvoices, [{ aggregate: "sum", field: field }]);
      return res[field].sum;
    }
  }

  sumType(field): any {
    if (this.dataInvoices.length == 0) {
      return 0;
    } else {
      var res = this.dataInvoices.filter(x => x.journalType == field);
      return res.map(s => s.amount).reduce((a, b) => a + b, 0);
    }
  }
}
