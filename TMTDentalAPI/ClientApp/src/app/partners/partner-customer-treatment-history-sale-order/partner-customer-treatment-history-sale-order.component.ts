import { Component, Input, OnInit, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-partner-customer-treatment-history-sale-order',
  templateUrl: './partner-customer-treatment-history-sale-order.component.html',
  styleUrls: ['./partner-customer-treatment-history-sale-order.component.css']
})
export class PartnerCustomerTreatmentHistorySaleOrderComponent implements OnInit {
  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  @Input() listSaleOrder: SaleOrderBasic[] = [];
  @Output() newItemEvent = new EventEmitter<any>();
  id: string;
  constructor(
  ) { }

  ngOnInit() {
    setTimeout(() => {
      if (this.listSaleOrder && this.listSaleOrder[0]) {
        this.id = this.listSaleOrder[0].id
      }
    }, 200);
    // this.loadDataFromApi();
  }

  chossesSaleOrder(value) {
    if (value) {
      this.id = value
    }
    this.newItemEvent.emit(value);
  }
}
