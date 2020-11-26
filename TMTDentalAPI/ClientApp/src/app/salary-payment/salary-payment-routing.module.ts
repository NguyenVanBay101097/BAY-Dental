import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SalaryPaymentFormComponent } from './salary-payment-form/salary-payment-form.component';
import { SalaryPaymentListComponent } from './salary-payment-list/salary-payment-list.component';

const routes: Routes = [
  {
    path: '',
    component: SalaryPaymentListComponent
  },
  {
    path: 'form',
    component: SalaryPaymentFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SalaryPaymentRoutingModule { }
