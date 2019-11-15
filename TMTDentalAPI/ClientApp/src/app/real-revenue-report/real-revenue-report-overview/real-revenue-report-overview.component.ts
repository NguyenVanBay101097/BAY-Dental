import { Component, OnInit, ViewChild } from '@angular/core';
import { RealRevenueReportSearch, RealRevenueReportService, RealRevenueReportItem } from '../real-revenue-report.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-real-revenue-report-overview',
  templateUrl: './real-revenue-report-overview.component.html',
  styleUrls: ['./real-revenue-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class RealRevenueReportOverviewComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: RealRevenueReportItem[] = [];
  dateFrom: Date;
  dateTo: Date;
  groupBy = "day";
  groups: {}[] = [
    { text: 'Ngày', value: 'day' },
    { text: 'Tuần', value: 'week' },
    { text: 'Tháng', value: 'month' },
    { text: 'Quý', value: 'quarter' },
  ];

  public total: any;
  public aggregates: any[] = [
    { field: 'debit', aggregate: 'sum' },
    { field: 'credit', aggregate: 'sum' },
    { field: 'balance', aggregate: 'sum' }
  ];


  constructor(private intlService: IntlService, private realRevenueReportService: RealRevenueReportService) {
  }

  ngOnInit() {
    // this.dateFrom = new Date(this.monthStart);
    // this.dateTo = new Date(this.monthEnd);

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new RealRevenueReportSearch();
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    }
    val.groupBy = this.groupBy;
    this.loading = true;
    this.realRevenueReportService.getReport(val).subscribe(result => {
      this.gridData = result;
      this.total = aggregateBy(result, this.aggregates);
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }

  setGroupBy(groupBy) {
    this.groupBy = groupBy;
    this.loadDataFromApi();
  }
}

