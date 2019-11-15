import { Component, OnInit, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { SaleReportItem, SaleReportService, SaleReportSearch } from '../sale-report.service';

@Component({
  selector: 'app-sale-report-overview',
  templateUrl: './sale-report-overview.component.html',
  styleUrls: ['./sale-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleReportOverviewComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: SaleReportItem[] = [];
  dateFrom: Date;
  dateTo: Date;
  groupBy = "date";
  groupBy2 = "month";
  search: string;
  groups: {}[] = [
    { text: 'Ngày điều trị', value: 'date' },
    { text: 'Khách hàng', value: 'customer' },
    { text: 'Nhân viên', value: 'user' },
    { text: 'Dịch vụ', value: 'product' },
  ];

  group2s: {}[] = [
    { text: 'Ngày', value: 'day' },
    { text: 'Tuần', value: 'week' },
    { text: 'Tháng', value: 'month' },
    { text: 'Quý', value: 'quarter' },
  ];

  public total: any;
  public aggregates: any[] = [
    { field: 'productUOMQty', aggregate: 'sum' },
    { field: 'priceTotal', aggregate: 'sum' },
  ];


  constructor(private intlService: IntlService, private saleReportService: SaleReportService) {
  }

  ngOnInit() {
    // this.dateFrom = new Date(this.monthStart);
    // this.dateTo = new Date(this.monthEnd);
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new SaleReportSearch();
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    }
    if (this.search) {
      val.search = this.search;
    }
    val.groupBy = this.groupBy;
    if (this.groupBy2 && this.groupBy == 'date') {
      val.groupBy = val.groupBy + ":" + this.groupBy2;
    }
    this.loading = true;
    this.saleReportService.getReport(val).subscribe(result => {
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

  setGroupBy2(groupBy) {
    this.groupBy2 = groupBy;
    this.loadDataFromApi();
  }
}

