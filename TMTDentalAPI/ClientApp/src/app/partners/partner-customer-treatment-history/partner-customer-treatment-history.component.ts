import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-partner-customer-treatment-history',
  templateUrl: './partner-customer-treatment-history.component.html',
  styleUrls: ['./partner-customer-treatment-history.component.css']
})
export class PartnerCustomerTreatmentHistoryComponent implements OnInit {
  partnerId: string;
  date: Date = new Date();
  saleOrderId: string;
  isReload: boolean ;
  constructor(
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
  }

  changeSaleOrder(event) {
    if (event) {
      this.saleOrderId = event;
    } else {
      this.saleOrderId = null;
    }
  }

  changeIsReload(event){
    debugger
    if (event) {
      this.isReload = event;
    } else {
      this.isReload = false;
    }
  }

}
