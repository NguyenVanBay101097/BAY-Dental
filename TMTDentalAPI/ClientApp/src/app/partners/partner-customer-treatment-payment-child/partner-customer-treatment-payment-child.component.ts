import { Component, OnInit, ViewChild, Input, Renderer2 } from '@angular/core';
import { RedirectComponentComponent } from 'src/app/shared/redirect-component/redirect-component.component';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-partner-customer-treatment-payment-child',
  templateUrl: './partner-customer-treatment-payment-child.component.html',
  styleUrls: ['./partner-customer-treatment-payment-child.component.css']
})
export class PartnerCustomerTreatmentPaymentChildComponent implements OnInit {

  @ViewChild(RedirectComponentComponent, { static: false }) redirecComponent: RedirectComponentComponent;
  @Input() saleOrder: SaleOrderBasic;
  show = false;

  constructor(
    private renderer2: Renderer2,
    private sharedService: SharedService
  ) { }

  ngOnInit() {
  }

  showTreatment(id) {
    var element = document.getElementById(id);
    document.getElementById(id + '_hide').classList.add('show')
    document.getElementById(id + '_show').classList.add('hide')
    document.getElementById(id + '_hide').classList.remove('hide')
    document.getElementById(id + '_show').classList.remove('show')
    this.renderer2.setStyle(element, 'height', 'auto');
    this.show = true;
    this.redirecComponent.loadComponent(this.sharedService.getComponentSearchUser(), id);
  }

  hideTreatment(id) {
    document.getElementById(id + '_hide').classList.add('hide')
    document.getElementById(id + '_show').classList.add('show')
    document.getElementById(id + '_hide').classList.remove('show')
    document.getElementById(id + '_show').classList.remove('hide')
    var element = document.getElementById(id);
    this.renderer2.setStyle(element, 'height', '75px');
    this.show = false;
    this.redirecComponent.destroyComponent();
  }

}
