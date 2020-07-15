import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PhieuThuChiListComponent } from './phieu-thu-chi-list/phieu-thu-chi-list.component';

const routes: Routes = [
  {
    path: 'phieu-thu-chi',
    component: PhieuThuChiListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PhieuThuChiRoutingModule { }
