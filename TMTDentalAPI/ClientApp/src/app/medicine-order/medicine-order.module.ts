import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MedicineOrderRoutingModule } from './medicine-order-routing.module';
import { MedicineOrderComponent } from './medicine-order/medicine-order.component';
import { MedicineOrderPrescriptionListComponent } from './medicine-order-prescription-list/medicine-order-prescription-list.component';
import { MedicineOrderPrescriptionPaymentListComponent } from './medicine-order-prescription-payment-list/medicine-order-prescription-payment-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { MedicineOrderService } from './medicine-order.service';

@NgModule({
  declarations: [MedicineOrderComponent, MedicineOrderPrescriptionListComponent, MedicineOrderPrescriptionPaymentListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule,
    NgbModule,
    MedicineOrderRoutingModule
  ],
  providers: [
    MedicineOrderService
  ]
})
export class MedicineOrderModule { }
