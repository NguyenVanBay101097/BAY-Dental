import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { AccountInvoiceReportService, RevenueReportDetailPaged } from '../account-invoice-report.service';

@Component({
  selector: 'app-account-invoice-report-revenue-detail',
  templateUrl: './account-invoice-report-revenue-detail.component.html',
  styleUrls: ['./account-invoice-report-revenue-detail.component.css']
})
export class AccountInvoiceReportRevenueDetailComponent implements OnInit {

  filter = new RevenueReportDetailPaged();
  @Input() parent: any;
  gridData: GridDataResult;
  loading = false;

  constructor(
    private accInvService: AccountInvoiceReportService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
  }

  initFilterData() {
    this.filter.dateFrom = this.parent.invoiceDate ? moment(this.parent.invoiceDate).format('YYYY/MM/DD') : ( this.parent.dateFrom ? moment(this.parent.dateFrom).format('YYYY/MM/DD') : '');
    this.filter.dateTo =   this.parent.invoiceDate ? moment(this.parent.invoiceDate).format('YYYY/MM/DD') : ( this.parent.dateTo ? moment(this.parent.dateTo).format('YYYY/MM/DD') : '');
    this.filter.companyId = this.parent.companyId || '';
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.productId = this.parent.productId  || '';
    this.filter.employeeId = this.parent.groupBy && this.parent.groupBy == 'employee'? this.parent.toDetailEmployeeId : '';
    this.filter.assistantId = this.parent.groupBy && this.parent.groupBy == 'assistant'? this.parent.toDetailEmployeeId : '';
    this.filter.partnerId = this.parent.partnerId  || '';

  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueReportDetailPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

}
