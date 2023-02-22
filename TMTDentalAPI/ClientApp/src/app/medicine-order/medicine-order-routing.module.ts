import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MedicineOrderPrescriptionListComponent } from './medicine-order-prescription-list/medicine-order-prescription-list.component';
import { MedicineOrderPrescriptionPaymentListComponent } from './medicine-order-prescription-payment-list/medicine-order-prescription-payment-list.component';
import { MedicineOrderComponent } from './medicine-order/medicine-order.component';

const routes: Routes = [
  {
    path: '',
    component: MedicineOrderComponent,
    children: [
      { path: '', redirectTo: 'prescriptions', pathMatch: 'full' },
      { path: 'prescriptions', component: MedicineOrderPrescriptionListComponent },
      { path: 'prescription-payments', component: MedicineOrderPrescriptionPaymentListComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MedicineOrderRoutingModule { }
