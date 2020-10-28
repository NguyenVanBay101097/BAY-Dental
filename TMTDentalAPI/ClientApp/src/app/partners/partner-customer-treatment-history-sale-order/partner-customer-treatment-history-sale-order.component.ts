import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
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
  @Input() partnerId: string;
  @Output() newItemEvent = new EventEmitter<any>();

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
      this.newItemEvent.emit(this.listSaleOrder[0])
    }, err => {
      console.log(err);
    })
  }

  chossesSaleOrder(saleOrder) {
    this.newItemEvent.emit(saleOrder);
  }
}
