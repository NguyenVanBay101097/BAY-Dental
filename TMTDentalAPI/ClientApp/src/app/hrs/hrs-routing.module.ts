import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HrPayrollStructureListComponent } from './hr-payroll-structure-list/hr-payroll-structure-list.component';
import { HrPayrollStructureCreateUpdateComponent } from './hr-payroll-structure-create-update/hr-payroll-structure-create-update.component';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';
import { HrPayslipToPayListComponent } from './hr-payslip-to-pay-list/hr-payslip-to-pay-list.component';
import { HrPayslipToPayCreateUpdateComponent } from './hr-payslip-to-pay-create-update/hr-payslip-to-pay-create-update.component';

const routes: Routes = [
  {
    path: 'payroll-structures',
    component: HrPayrollStructureListComponent
  },
  {
    path: 'payroll-structures/create',
    component: HrPayrollStructureCreateUpdateComponent
  },
  {
    path: 'payroll-structures/edit/:id',
    component: HrPayrollStructureCreateUpdateComponent
  },
  {
    path: 'payroll-structure-types', component: HrPayrollStructureTypeListComponent
  },
  {
    path: 'payslip-to-pay', component: HrPayslipToPayListComponent
  },
  {
    path: 'payslip-to-pay/create', component: HrPayslipToPayCreateUpdateComponent
  },
  {
    path: 'payslip-to-pay/edit/:id', component: HrPayslipToPayCreateUpdateComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrsRoutingModule { }
