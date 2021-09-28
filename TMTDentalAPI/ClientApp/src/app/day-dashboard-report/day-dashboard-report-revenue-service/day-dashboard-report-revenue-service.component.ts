import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-revenue-service',
  templateUrl: './day-dashboard-report-revenue-service.component.html',
  styleUrls: ['./day-dashboard-report-revenue-service.component.css']
})
export class DayDashboardReportRevenueServiceComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  @Input() dataInvoices: any[] = [];

  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  ngOnInit() {
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
