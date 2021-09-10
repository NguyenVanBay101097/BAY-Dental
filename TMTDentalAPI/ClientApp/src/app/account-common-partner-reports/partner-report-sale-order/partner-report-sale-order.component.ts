import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { GetSaleOrderPagedReq, PartnerOldNewReportReq, PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-report-sale-order',
  templateUrl: './partner-report-sale-order.component.html',
  styleUrls: ['./partner-report-sale-order.component.css']
})
export class PartnerReportSaleOrderComponent implements OnInit {

  @Input() filter = new GetSaleOrderPagedReq();
  loading = false;
  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  pagerSettings: any;

  constructor(
    private partnerOldNewRpService: PartnerOldNewReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    var val = Object.assign({}, this.filter) as GetSaleOrderPagedReq;
    val.limit = this.limit;
    val.offset = this.offset;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getSaleOrderPaged(val).subscribe(res => {
      this.loading = false;
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
    },
      err => {
        this.loading = false;
      });
  }

  getStateDisplay(state) {
    switch (state) {
      case 'sale':
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }

  pageChange(e) {
    this.offset = e.skip;
    this.limit = e.take;
    this.loadData();
  }
}
