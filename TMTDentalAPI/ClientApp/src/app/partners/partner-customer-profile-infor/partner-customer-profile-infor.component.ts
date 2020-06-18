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

  getCustomerAddress(customerInfo: any) {
    var list = [];

    if (customerInfo.street) {
      list.push(customerInfo.street);
    }

    if (customerInfo.wardName) {
      list.push(customerInfo.wardName);
    }

    if (customerInfo.districtName) {
      list.push(customerInfo.districtName);
    }

    if (customerInfo.cityName) {
      list.push(customerInfo.cityName);
    }

    return list.join(', ');
  }

  getCustomerCategories(customerInfo: any) {
    if (!customerInfo.categories) {
      return '';
    }

    return customerInfo.categories.map(x => x.name).join(', ');
  }

  getCustomerHistories(customerInfo: any) {
    if (!customerInfo.histories) {
      return '';
    }

    return customerInfo.histories.map(x => x.name).join(', ');
  }

}


