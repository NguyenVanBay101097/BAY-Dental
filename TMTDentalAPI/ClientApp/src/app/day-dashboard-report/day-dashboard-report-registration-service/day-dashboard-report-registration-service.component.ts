import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy, DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { SaleOrderLineService, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';
import { SaleReportService, ServiceReportDetailReq } from 'src/app/sale-report/sale-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-registration-service',
  templateUrl: './day-dashboard-report-registration-service.component.html',
  styleUrls: ['./day-dashboard-report-registration-service.component.css']
})
export class DayDashboardReportRegistrationServiceComponent implements OnInit {
  @Input('dateFrom') dateFrom: any;
  @Input('dateTo') dateTo: any;
  @Input('company') company: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  totalService: number;
  totalOrderPartner: number;
  totalAmount: number;
  totalPaid: number;

  constructor(
    private saleOrderlineService: SaleOrderLineService,
    private intlService: IntlService,
    private saleReportService: SaleReportService,

    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataFromApi();
  }

  ngOnInit() {

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ServiceReportDetailReq();
    val.limit = this.limit;
    val.offset = this.skip;
    val.companyId= this.company ? this.company.id : '';
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY/MM/DD') : '';
    val.state = 'sale,done,cancel';
    this.saleReportService.getServiceReportDetailPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      const data = this.gridData.data;
      const aggregateData = aggregateBy(data, [
        { aggregate: "sum", field: "productUOMQty" },
        { aggregate: "sum", field: "priceSubTotal" },
        { aggregate: "sum", field: "amountPaid" },
      ]);

      this.totalService = aggregateData.productUOMQty ? aggregateData.productUOMQty.sum : 0;
      this.totalAmount = aggregateData.priceSubTotal ? aggregateData.priceSubTotal.sum : 0;
      this.totalPaid = aggregateData.amountPaid ? aggregateData.amountPaid.sum : 0;
      let arr = []
      for (const val of data) {
        if(val.orderPartnerId){
          arr.push(val.orderPartnerId);
        }
      }
      this.totalOrderPartner = [...new Set(arr)].length;
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    this.loadDataFromApi();
  }

}
