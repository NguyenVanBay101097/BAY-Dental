import { Component, OnInit, Input } from '@angular/core';
import { PartnerDisplay, PartnerInfoViewModel } from '../partner-simple';

@Component({
  selector: 'app-partner-customer-info',
  templateUrl: './partner-customer-info.component.html',
  styleUrls: ['./partner-customer-info.component.css']
})
export class PartnerCustomerInfoComponent implements OnInit {
  @Input() partner: PartnerInfoViewModel;
  constructor() { }

  ngOnInit() {

  }

}
