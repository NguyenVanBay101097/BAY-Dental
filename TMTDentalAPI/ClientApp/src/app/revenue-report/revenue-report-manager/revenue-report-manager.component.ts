import { Component, OnInit, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { RevenueReportResult, RevenueReportService, RevenueReportSearch } from '../revenue-report.service';
import { CompanyBasic } from 'src/app/companies/company.service';

@Component({
  selector: 'app-revenue-report-manager',
  templateUrl: './revenue-report-manager.component.html',
  styleUrls: ['./revenue-report-manager.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class RevenueReportManagerComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  reportResult: RevenueReportResult;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  groupBy = "date:month";
  search: string;
  searchUpdate = new Subject<string>();
  viewType = 'list';
  listCompanies: CompanyBasic[] = [];

  groups: { text: string, value: string }[] = [
    { text: 'Ngày', value: 'date:day' },
    { text: 'Tuần', value: 'date:week' },
    { text: 'Tháng', value: 'date:month' },
    { text: 'Quý', value: 'date:quarter' },
    { text: 'Năm', value: 'date:year' },
    { text: 'Khách hàng', value: 'partner' },
    { text: 'Sản phẩm', value: 'product' },
    { text: 'Bác sĩ', value: 'salesman' }
  ];

  public total: any;
  public aggregates: any[] = [
    { field: 'balance', aggregate: 'sum' },
  ];

  constructor(private intlService: IntlService, 
    private revenueReportService: RevenueReportService) {
  }

  ngOnInit() {
    this.reportResult = new RevenueReportResult();
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  getTitle() {
    var item = _.find(this.groups, o => o.value == this.groupBy);
    return item.text;
  }

  loadDataFromApi() {
    var val = new RevenueReportSearch();

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
    this.loading = true;
    this.revenueReportService.getReport(val).subscribe(result => {
      this.reportResult = result;
      this.loadItems();
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  loadItems(): void {
    this.gridData = {
      data: this.reportResult.details.slice(this.skip, this.skip + this.limit),
      total: this.reportResult.details.length
    };
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
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

  setViewType(type) {
    this.viewType = type;
  }
}


