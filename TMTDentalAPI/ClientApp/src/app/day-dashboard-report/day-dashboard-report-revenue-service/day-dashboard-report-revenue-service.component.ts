import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-revenue-service',
  templateUrl: './day-dashboard-report-revenue-service.component.html',
  styleUrls: ['./day-dashboard-report-revenue-service.component.css']
})
export class DayDashboardReportRevenueServiceComponent implements OnInit {
  @Input('dateFrom') dateFrom: any;
  @Input('dateTo') dateTo: any;
  @Input('company') company: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  
  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
  }

  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    // this.loadReport();
  }
}
