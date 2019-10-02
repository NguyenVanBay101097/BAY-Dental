
import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceReportByTimeItem, AccountInvoiceReportService, AccountInvoiceReportByTimeSearch } from '../account-invoice-report.service';
import { aggregateBy } from '@progress/kendo-data-query';
import { debug } from 'util';

@Component({
  selector: 'app-account-invoice-report-by-time',
  templateUrl: './account-invoice-report-by-time.component.html',
  styleUrls: ['./account-invoice-report-by-time.component.css']
})
export class AccountInvoiceReportByTimeComponent implements OnInit {

  loading = false;
  items: AccountInvoiceReportByTimeItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  monthFrom: Date;
  monthTo: Date;
  yearFrom: Date;
  yearTo: Date;
  groupBy = 'day';
  view_type = 'list';

  public total: any;
  public aggregates: any[] = [
    { field: 'amountTotal', aggregate: 'sum' },
    { field: 'residual', aggregate: 'sum' }
  ];


  constructor(private reportService: AccountInvoiceReportService, private intlService: IntlService) { }

  ngOnInit() {
    var date = new Date();
    this.dateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
    this.dateTo = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    this.monthFrom = new Date(date.getFullYear(), 0, 1);
    this.monthTo = new Date(date.getFullYear(), date.getMonth(), 1);
    this.yearFrom = new Date(date.getFullYear(), 0, 1);
    this.yearTo = new Date(date.getFullYear(), 0, 1);


    this.loadDataFromApi();
  }

  onChangeDate(value: any) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  switchView(viewType) {
    this.view_type = viewType;
  }

  getDateStr(dateStr: string) {
    var date = this.intlService.parseDate(dateStr);
    if (this.groupBy == 'year') {
      return this.intlService.formatDate(date, 'yyyy');
    } else if (this.groupBy == 'month') {
      return this.intlService.formatDate(date, 'MM/yyyy');
    } else {
      return this.intlService.formatDate(date, 'd');
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountInvoiceReportByTimeSearch();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'd', 'en-US') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'd', 'en-US') : null;
    val.monthFrom = this.monthFrom ? this.intlService.formatDate(this.monthFrom, 'd', 'en-US') : null;
    val.monthTo = this.monthTo ? this.intlService.formatDate(this.monthTo, 'd', 'en-US') : null;
    val.yearFrom = this.yearFrom ? this.intlService.formatDate(this.yearFrom, 'd', 'en-US') : null;
    val.yearTo = this.yearTo ? this.intlService.formatDate(this.yearTo, 'd', 'en-US') : null;
    val.groupBy = this.groupBy;

    this.reportService.getSummaryByTime(val).subscribe(res => {
      console.log(res);
      this.items = res;
      this.total = aggregateBy(this.items, this.aggregates);
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }
}
