import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import * as _ from 'lodash';

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
  @Input() services: any[] = [];
  
  constructor(@Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
  private router: Router
  ) { this.pagerSettings = config.pagerSettings }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  ngOnInit() {
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  loadData(): void {
    console.log(this.services);
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
      this.router.navigate(['sale-order', item.id]);
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
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Ngừng điều trị';
      default:
        return 'Nháp';
    }
  }

}
