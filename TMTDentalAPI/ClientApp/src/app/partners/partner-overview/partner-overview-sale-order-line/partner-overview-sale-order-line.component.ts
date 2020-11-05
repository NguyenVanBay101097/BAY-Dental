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
  gridData: GridDataResult;
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


  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadFromApi() {
    this.saleOrderlineService.getDisplayBySaleOrder(this.saleOrderId).subscribe(
      result => {
        this.details = result;
        console.log(result);
        this.gridData = {
          data: this.details.slice(this.skip, this.skip + this.limit),
          total: this.details.length
        };
      }
    )
  }


  loadItems(): void {

  }

}
