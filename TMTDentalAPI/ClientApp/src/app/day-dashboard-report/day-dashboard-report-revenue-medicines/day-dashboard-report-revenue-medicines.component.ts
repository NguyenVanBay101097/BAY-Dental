import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { MedicineOrderService, PrecscriptionPaymentPaged } from 'src/app/medicine-order/medicine-order.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-day-dashboard-report-revenue-medicines',
  templateUrl: './day-dashboard-report-revenue-medicines.component.html',
  styleUrls: ['./day-dashboard-report-revenue-medicines.component.css']
})
export class DayDashboardReportRevenueMedicinesComponent implements OnInit {
  @Input('dateFrom') dateFrom: any;
  @Input('dateTo') dateTo: any;
  @Input('company') company: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  search: string;
  state: string = '';
  totalAmount: number = 0;
  totalAmountCash: number = 0;
  totalAmountBank: number = 0;
  constructor(
    private intlService: IntlService,
    private medicineOrderSerive: MedicineOrderService,

    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataFromApi();
  }
  
  ngOnInit() {
  }

  loadDataFromApi() {
    this.loading = true;
    var filter = new PrecscriptionPaymentPaged();
    filter.limit = this.limit;
    filter.offset = this.skip;
    filter.search = this.search ? this.search : '';
    filter.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    filter.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50");
    filter.state = this.state;
    this.medicineOrderSerive.getPaged(filter).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      const data = this.gridData.data;

      const aggregateData = aggregateBy(data, [
        { aggregate: "sum", field: "amount" },
      ]);

      this.totalAmount = aggregateData.amount ? aggregateData.amount.sum : 0;
      this.totalAmountCash = 0;
      this.totalAmountBank = 0;
      for (const val of data) {
        if (val.journal.type == 'cash') {
          this.totalAmountCash += val.amount
        } else {
          this.totalAmountBank += val.amount
        }
      }

      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(e) {
    this.skip = e.skip;
    this.limit = e.take;
    this.loadDataFromApi();
  }

}
