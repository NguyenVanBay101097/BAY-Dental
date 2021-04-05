import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-customer-sale-order-quotations-lines',
  templateUrl: './partner-customer-sale-order-quotations-lines.component.html',
  styleUrls: ['./partner-customer-sale-order-quotations-lines.component.css']
})
export class PartnerCustomerSaleOrderQuotationsLinesComponent implements OnInit {

  @Input() public advisoryId: string;

  constructor() { }

  ngOnInit() {
  }

}
