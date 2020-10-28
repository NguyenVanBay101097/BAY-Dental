import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-customer-treatment-history-form',
  templateUrl: './partner-customer-treatment-history-form.component.html',
  styleUrls: ['./partner-customer-treatment-history-form.component.css']
})
export class PartnerCustomerTreatmentHistoryFormComponent implements OnInit {
  @Input() partnerId: string;
  @Input() saleOrderId: string;
  saleOrderLine: any;
  constructor() { }

  ngOnInit() {
  }

  chosseSaleOrderLine(event) {
    this.saleOrderLine = event;
  }

}
