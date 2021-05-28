import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { AccountInvoiceReportDetailPaged, AccountInvoiceReportDisplay, AccountInvoiceReportPaged, AccountInvoiceReportService } from '../account-invoice-report.service';

@Component({
  selector: 'app-account-invoice-report-revenue-detail',
  templateUrl: './account-invoice-report-revenue-detail.component.html',
  styleUrls: ['./account-invoice-report-revenue-detail.component.css']
})
export class AccountInvoiceReportRevenueDetailComponent implements OnInit {

  filter = new AccountInvoiceReportDetailPaged();
  @Input() parent: AccountInvoiceReportDisplay;
  @Input() parentFilter: AccountInvoiceReportPaged;
  gridData: GridDataResult;
  loading = false;

  constructor(
    private accInvService: AccountInvoiceReportService
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
  }

  initFilterData() {
    Object.assign(this.filter, this.parentFilter);
    this.filter.limit = 20;
    this.filter.offset = 0;
    if (this.parent) {
      switch (this.parentFilter.groupBy) {
        case 'InvoiceDate':
          this.filter.date = moment(this.parent.invoiceDate).format('YYYY/MM/DD')
          break;
        case 'ProductId':
          this.filter.productId = this.parent.productId;
          break;
        case 'EmployeeId':
          this.filter.productId = this.parent.employeeId;
          break;
        case 'AssistantId':
          this.filter.productId = this.parent.assistantId;
          break;
        default:
          break;
      }
    }
  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.search = '';
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
