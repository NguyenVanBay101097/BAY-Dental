import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceReportService, RevenueServiceReportPar } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import * as moment from 'moment';

@Component({
  selector: 'app-day-dashboard-report-revenue',
  templateUrl: './day-dashboard-report-revenue.component.html',
  styleUrls: ['./day-dashboard-report-revenue.component.css']
})
export class DayDashboardReportRevenueComponent implements OnInit {
  @Input('dateFrom') dateFrom: any;
  @Input('dateTo') dateTo: any;
  @Input('company') company: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  allDataInvoice: any;
  filter = new RevenueServiceReportPar();

  constructor(
    private intlService: IntlService,
    private accInvService: AccountInvoiceReportService,

    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.initFilterData();
    this.loadAllData();
  }
  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.skip = 0;
  }

  loadAllData(){
    var val = Object.assign({}, this.filter) as RevenueServiceReportPar;
    val.companyId = val.companyId || '';
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.accInvService.getRevenueServiceReport(val).subscribe(res => {
      this.allDataInvoice = res;
      this.loading = false;
      this.loadReport();
    },
      err => {
        this.loading = false;
      });
  }

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataInvoice.length,
      data: this.allDataInvoice.slice(this.skip, this.skip + this.limit)
    };
    console.log(this.gridData);
    
  }

  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    // this.loadDataFromApi();
  }

}
