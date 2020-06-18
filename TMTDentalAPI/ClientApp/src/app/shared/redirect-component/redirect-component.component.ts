import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { RedirectComponentDirective } from '../redirect-component.directive';
import { PartnerCustomerTreatmentPaymentDetailComponent } from 'src/app/partners/partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';

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

  loadComponent(adItem, id) {
    var viewContainerRef = this.sharedHost.viewContainerRef;
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(adItem.component);
    viewContainerRef.clear();
    var componentRef = viewContainerRef.createComponent(componentFactory);
    (<PartnerCustomerTreatmentPaymentDetailComponent>componentRef.instance).saleOrderId = id;
  }

  destroyComponent(): void {
    var viewContainerRef = this.sharedHost.viewContainerRef;
    viewContainerRef.clear();
  }
}
