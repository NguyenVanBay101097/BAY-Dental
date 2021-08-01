import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
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
    private saleOrderOdataService: SaleOrdersOdataService
  ) { }

  ngOnInit() {
    this.loadDataFromOData();
  }

  loadDataFromOData() {
    this.loading = true;
    var val = {
      id: this.saleOrderId,
      func: "GetSaleOrderLines",
      options: {
        params: new HttpParams().set('$count', 'true')
      }
    }
    this.saleOrderOdataService.getSaleOrderLines(val).subscribe(
      result => {
        this.gridData = {
          data: result && result['value'],
          
          total: result['@odata.count']
        };
        this.loading = false;
      }, () => {
        this.loading = false;
      });
  }

}
