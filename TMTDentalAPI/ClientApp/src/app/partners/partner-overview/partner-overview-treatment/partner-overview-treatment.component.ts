import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-partner-overview-treatment',
  templateUrl: './partner-overview-treatment.component.html',
  styleUrls: ['./partner-overview-treatment.component.css']
})
export class PartnerOverviewTreatmentComponent implements OnInit {

  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  @Input() listSaleOrder: SaleOrderBasic[] = [];
  constructor(
    private router: Router
  ) { }

  ngOnInit() {
  }

  chossesSaleOrder(id) {
    if (id) {
      this.router.navigateByUrl(`sale-orders/form?id=${id}`)
    }
  }
}