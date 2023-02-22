import { Injectable, Type } from '@angular/core';
import { PartnerCustomerTreatmentPaymentDetailComponent } from '../partners/partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';

export class Shared {
  constructor(public component: Type<any>) { }
}

@Injectable({
  providedIn: 'root'
})



export class SharedService {
  getComponentSearchUser() {
    return new Shared(PartnerCustomerTreatmentPaymentDetailComponent)
  }
}
