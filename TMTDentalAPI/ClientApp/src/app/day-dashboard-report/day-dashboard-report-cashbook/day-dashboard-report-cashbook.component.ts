import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-cashbook',
  templateUrl: './day-dashboard-report-cashbook.component.html',
  styleUrls: ['./day-dashboard-report-cashbook.component.css']
})
export class DayDashboardReportCashbookComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  stateOptions = [
    { text: 'Tiền mặt', value: 'cash' },
    { text: 'Ngân hàng', value: 'bank' },
  ]

  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
  }

  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    // this.loadReport();
  }

  onSelectChange(e) {
    // console.log(e.value);

  }
}
