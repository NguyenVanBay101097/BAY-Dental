

import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountInvoiceReportByTimeItem, AccountInvoiceReportByTimeDetail, AccountInvoiceReportService } from '../account-invoice-report.service';
import { process, GroupDescriptor, State, aggregateBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-account-invoice-report-by-time-detail',
  templateUrl: './account-invoice-report-by-time-detail.component.html',
  styleUrls: ['./account-invoice-report-by-time-detail.component.css']
})

export class AccountInvoiceReportByTimeDetailComponent implements OnInit {
  @Input() public item: AccountInvoiceReportByTimeItem;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: AccountInvoiceReportByTimeDetail[];
  loading = false;
  public total: any;
  public aggregates: any[] = [
    { field: 'amountTotal', aggregate: 'sum' },
    { field: 'residual', aggregate: 'sum' }
  ];

  constructor(private reportService: AccountInvoiceReportService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.reportService.getDetailByTime(this.item).subscribe(res => {
      this.details = res;
      this.total = aggregateBy(this.details, this.aggregates);
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
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }
}

