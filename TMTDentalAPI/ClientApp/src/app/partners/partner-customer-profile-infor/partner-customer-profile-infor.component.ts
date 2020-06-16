import { Component, OnInit, Input } from '@angular/core';
import { PartnerDisplay } from '../partner-simple';

@Component({
  selector: 'app-partner-customer-profile-infor',
  templateUrl: './partner-customer-profile-infor.component.html',
  styleUrls: ['./partner-customer-profile-infor.component.css']
})
export class PartnerCustomerProfileInforComponent implements OnInit {

  @Input() customerInfo: PartnerDisplay;

  constructor() { }

  ngOnInit() {

  }

}
