import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoaiThuChiListComponent } from './loai-thu-chi-list/loai-thu-chi-list.component';

const routes: Routes = [
  {
    path: '',
    component: LoaiThuChiListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LoaiThuChiRoutingModule { }
