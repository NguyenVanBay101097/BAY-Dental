import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';

import { PartnerCustomerTreatmentPaymentDetailComponent } from 'src/app/partners/partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';
import { RedirectComponentDirective } from '../redirect-component.directive';

@Component({
  selector: 'app-redirect-component',
  templateUrl: './redirect-component.component.html',
  styleUrls: ['./redirect-component.component.css']
})
export class RedirectComponentComponent implements OnInit {

  @ViewChild(RedirectComponentDirective, { static: true }) sharedHost: RedirectComponentDirective;
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver
  ) { }

  ngOnInit(

  ) {

  }

  loadComponent(adItem, id, partner) {
    var viewContainerRef = this.sharedHost.viewContainerRef;
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(adItem.component);
    viewContainerRef.clear();
    var componentRef = viewContainerRef.createComponent(componentFactory);
    (<PartnerCustomerTreatmentPaymentDetailComponent>componentRef.instance).saleOrderId = id;
    (<PartnerCustomerTreatmentPaymentDetailComponent>componentRef.instance).partner = partner;
  }

  destroyComponent(): void {
    var viewContainerRef = this.sharedHost.viewContainerRef;
    viewContainerRef.clear();
  }
}
