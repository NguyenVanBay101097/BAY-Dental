import { Component, OnInit, ViewChild } from '@angular/core';
import { RealRevenueReportSearch, RealRevenueReportService, RealRevenueReportItem, RealRevenueReportResult } from '../real-revenue-report.service';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-real-revenue-report-overview',
  templateUrl: './real-revenue-report-overview.component.html',
  styleUrls: ['./real-revenue-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class RealRevenueReportOverviewComponent implements OnInit {
  reportResult: RealRevenueReportResult;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(private intlService: IntlService, private realRevenueReportService: RealRevenueReportService) {
  }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new RealRevenueReportSearch();
    this.loading = true;
    this.realRevenueReportService.getReport(val).subscribe(result => {
      this.loading = false;
      this.reportResult = result;
      this.loadItems();
    }, () => {
      this.loading = false;
    });
  }

  loadItems(): void {
    this.gridData = {
      data: this.reportResult.items.slice(this.skip, this.skip + this.limit),
      total: this.reportResult.items.length
    };
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }
}

