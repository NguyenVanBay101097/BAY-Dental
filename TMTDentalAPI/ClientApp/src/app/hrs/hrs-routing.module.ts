import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PayrollStructureListComponent } from './PayrollStructures/payroll-structure-list/payroll-structure-list.component';
import { PayrollStructureCreateUpdateComponent } from './PayrollStructures/payroll-structure-create-update/payroll-structure-create-update.component';

const routes: Routes = [
  {
    path: 'payroll-structures',
    component: PayrollStructureListComponent
  },
  {
    path: 'payroll-structures/create',
    component: PayrollStructureCreateUpdateComponent
  },
  {
    path: 'payroll-structures/edit/:id',
    component: PayrollStructureCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrsRoutingModule { }
