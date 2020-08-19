import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HrPayrollStructureTypeListComponent } from './hr-payroll-structure-type-list/hr-payroll-structure-type-list.component';

const routes: Routes = [
  { path: 'payroll-structure-types', component: HrPayrollStructureTypeListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrsRoutingModule { }
