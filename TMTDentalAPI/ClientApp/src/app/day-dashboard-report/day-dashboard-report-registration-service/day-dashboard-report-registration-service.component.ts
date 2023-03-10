import { Component, Inject, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import * as _ from 'lodash';
import { map } from 'rxjs/operators';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderLinePaged } from 'src/app/partners/partner.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-registration-service',
  templateUrl: './day-dashboard-report-registration-service.component.html',
  styleUrls: ['./day-dashboard-report-registration-service.component.css']
})
export class DayDashboardReportRegistrationServiceComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  services: any[] = [];
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  @Input() companyId: string;
  
  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
  private router: Router,
  private intlService: IntlService,
  private saleOrderLineService: SaleOrderLineService
  ) { 
    this.pagerSettings = config.pagerSettings 
  }

  ngOnInit() {
    this.loadDataServiceApi();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  loadData2(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.companyId = data.company;
    this.loadDataServiceApi();
  }

  loadDataServiceApi() {
    var val = new SaleOrderLinePaged();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd") : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, "yyyy-MM-dd") : '';
    val.companyId = this.companyId || '';
    val.state = 'sale,done,cancel';
    this.saleOrderLineService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.services = res.data;
      this.loadData();
    }, err => {
      console.log(err);
    })
  }

  loadData(): void {
    this.gridData = {
      data: this.services.slice(this.skip, this.skip + this.limit),
      total: this.services.length
    };
  }

  getPartnerCount() {
    var group = _.groupBy(this.services, (item) => {
      return item.orderPartnerId;
    });
    return Object.keys(group).length;
  }

  redirectSaleOrder(item) {
    if (item) {
      this.router.navigate(['/sale-orders', item.id]);
    }
  }

  sum(field) : any{
    if(this.services.length == 0 ) 
    {
     return 0;
    } else {
      var res = aggregateBy(this.services, [ { aggregate: "sum", field: field }]);
      return res[field].sum;
    }
  }

  getState(state) {
    switch (state) {
      case 'sale':
        return '??ang ??i???u tr???';
      case 'done':
        return 'Ho??n th??nh';
      case 'cancel':
        return 'Ng???ng ??i???u tr???';
      default:
        return 'Nh??p';
    }
  }

}
