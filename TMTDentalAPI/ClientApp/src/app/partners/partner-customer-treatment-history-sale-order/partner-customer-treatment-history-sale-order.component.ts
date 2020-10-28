import { Component, Input, OnInit } from '@angular/core';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-partner-customer-treatment-history-sale-order',
  templateUrl: './partner-customer-treatment-history-sale-order.component.html',
  styleUrls: ['./partner-customer-treatment-history-sale-order.component.css']
})
export class PartnerCustomerTreatmentHistorySaleOrderComponent implements OnInit {
  // @Input() partnerId: string;
  limit: number = 20;
  skip: number = 0;
  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  listSaleOrder: SaleOrderBasic[] = [];
  partnerId: string = '346fa7c1-73be-459d-d6c0-08d8757a3015';
  constructor(
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;
    val.isQuotation = false;

    this.saleOrderService.getPaged(val).subscribe(res => {
      this.listSaleOrder = res.items;
      console.log(this.listSaleOrder[0]);

    }, err => {
      console.log(err);
    })
  }

}
