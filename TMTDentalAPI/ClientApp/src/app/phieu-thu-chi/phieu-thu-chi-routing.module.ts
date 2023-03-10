import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PhieuThuChiListComponent } from './phieu-thu-chi-list/phieu-thu-chi-list.component';
import { PhieuThuChiFormComponent } from './phieu-thu-chi-form/phieu-thu-chi-form.component';

const routes: Routes = [
  {
    path: '',
    component: PhieuThuChiListComponent
  },
  {
    path: 'form',
    component: PhieuThuChiFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PhieuThuChiRoutingModule { }
