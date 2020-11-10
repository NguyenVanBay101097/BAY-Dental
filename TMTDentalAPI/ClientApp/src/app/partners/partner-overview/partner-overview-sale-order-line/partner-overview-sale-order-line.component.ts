import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';

@Component({
  selector: 'app-partner-overview-sale-order-line',
  templateUrl: './partner-overview-sale-order-line.component.html',
  styleUrls: ['./partner-overview-sale-order-line.component.css']
})
export class PartnerOverviewSaleOrderLineComponent implements OnInit {
  @Input() public saleOrderId: string;
  skip = 0;
  limit = 10;
  gridData: any = [];
  details: SaleOrderLineDisplay[];
  loading = false;

  public total: any;
  constructor(
    private saleOrderlineService: SaleOrderLineService
  ) { }

  ngOnInit() {
    if (this.saleOrderId) {
      this.loadFromApi();
    }
  }

  loadFromApi() {
    this.loading = true;
    this.saleOrderlineService.getDisplayBySaleOrder(this.saleOrderId).subscribe(
      result => {
        this.gridData = result;
        this.loading = false;
      }, () => {
        this.loading = false;
      });
  }
}
