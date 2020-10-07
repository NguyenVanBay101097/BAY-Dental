import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardOrdersRoutingModule } from './service-card-orders-routing.module';
import { ServiceCardOrderListComponent } from './service-card-order-list/service-card-order-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update/service-card-order-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { ServiceCardOrderLineDialogComponent } from './service-card-order-line-dialog/service-card-order-line-dialog.component';
import { ServiceCardOrderTypeListComponent } from './service-card-order-type-list/service-card-order-type-list.component';
import { ServiceCardOrderPosComponent } from './service-card-order-pos/service-card-order-pos.component';
import { ServiceCardOrderPaymentsDialogComponent } from './service-card-order-payments-dialog/service-card-order-payments-dialog.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';

@NgModule({
  declarations: [ServiceCardOrderListComponent, ServiceCardOrderCreateUpdateComponent, ServiceCardOrderLineDialogComponent, ServiceCardOrderTypeListComponent, ServiceCardOrderPosComponent, ServiceCardOrderPaymentsDialogComponent],
  imports: [
    CommonModule,
    ServiceCardOrdersRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    NgbModule,
    SharedModule,
    Ng2SearchPipeModule,
  ],
  entryComponents: [
    ServiceCardOrderLineDialogComponent,
    ServiceCardOrderCreateUpdateComponent,
    ServiceCardOrderPaymentsDialogComponent
  ]
})
export class ServiceCardOrdersModule { }
