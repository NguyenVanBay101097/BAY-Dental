import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';
import { SaleOrderLineDisplay } from '../sale-order-line-display';

@Component({
  selector: 'app-sale-order-line-management',
  templateUrl: './sale-order-line-management.component.html',
  styleUrls: ['./sale-order-line-management.component.css']
})
export class SaleOrderLineManagementComponent implements OnInit {

  @Input() public saleOrderId: string;
  skip = 0;
  limit = 10;
  gridData: any = [];
  details: SaleOrderLineDisplay[];
  loading = false;

  public total: any;
  constructor(
    private saleOrderLineService: SaleOrderLineService
  ) { }

  ngOnInit() {
    this.loadDataFromOData();
  }

  loadDataFromOData() {
    this.loading = true;
    var val = {
      orderId: this.saleOrderId,
    }

    this.saleOrderLineService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

}
