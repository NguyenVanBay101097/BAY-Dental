import {  Component, ComponentFactoryResolver, ComponentRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { SaleOrderReportRevenuePaged } from 'src/app/core/services/sale-order.service';
import { AccountInvoiceReportRevenueComponent } from '../account-invoice-report-revenue/account-invoice-report-revenue.component';
import { AccountInvoiceReportPaged } from '../account-invoice-report.service';
import { SaleOrderReportRevenueComponent } from '../sale-order-report-revenue/sale-order-report-revenue.component';

@Component({
  selector: 'app-account-invoice-report-revenue',
  templateUrl: './account-invoice-report-revenue-manage.component.html',
  styleUrls: ['./account-invoice-report-revenue-manage.component.css']
})
export class AccountInvoiceReportRevenueManageComponent implements OnInit {

  groupBy= 'InvoiceDate';

  @ViewChild('container', { read: ViewContainerRef, static: true }) container: ViewContainerRef;
  componentRef: ComponentRef<any>;

  constructor(
 
  ) {

  }

  ngOnInit() {
    this.onChangeGroupBy();
  }

  renderComponent(AlertContentComponent) {
    const container = this.container;
    container.clear();
    const injector = container.injector;

    const cfr: ComponentFactoryResolver = injector.get(ComponentFactoryResolver);

    const componentFactory = cfr.resolveComponentFactory(AlertContentComponent);

    const componentRef = container.createComponent(componentFactory, 0, injector);
    (componentRef.instance as any).groupBy = this.groupBy;
    componentRef.changeDetectorRef.detectChanges();
    this.componentRef = componentRef;
  }

  onChangeGroupBy() {
    if(this.groupBy == 'expect') {
      this.renderComponent(SaleOrderReportRevenueComponent);
    } else {
      if(!(this.componentRef instanceof AccountInvoiceReportRevenueComponent))
     {
       this.renderComponent(AccountInvoiceReportRevenueComponent);
     } 
    }
  }

}
