import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardOrdersRoutingModule } from './service-card-orders-routing.module';
import { ServiceCardOrderListComponent } from './service-card-order-list/service-card-order-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update/service-card-order-create-update.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [ServiceCardOrderListComponent, ServiceCardOrderCreateUpdateComponent],
  imports: [
    CommonModule,
    ServiceCardOrdersRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    NgbModule,
    SharedModule
  ]
})
export class ServiceCardOrdersModule { }
