import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PrintLayoutComponent } from './print-layout/print-layout.component';
import { CustomerInvoicePrintComponent } from './account-invoices/customer-invoice-print/customer-invoice-print.component';
import { PartnerListComponent } from './partners/partner-list/partner-list.component';
import { AppointmentListComponent } from './appointment/appointment-list/appointment-list.component';
import { FieldBinaryImageSimpleComponent } from './shared/field-binary-image-simple/field-binary-image-simple.component';

const routes: Routes = [
  {
    path: 'print',
    outlet: 'print',
    component: PrintLayoutComponent,
    children: [
      { path: 'invoice', component: CustomerInvoicePrintComponent }
    ]
  },
  {
    path: 'binary-image',
    component: FieldBinaryImageSimpleComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
