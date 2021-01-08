import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoaiThuChiListComponent } from './loai-thu-chi-list/loai-thu-chi-list.component';
import { LoaiThuChiManagementComponent } from './loai-thu-chi-management/loai-thu-chi-management.component';

const routes: Routes = [
  {
    path: '',
    component: LoaiThuChiManagementComponent,
    children: [
      { path: '', redirectTo: 'loai-thu', pathMatch: 'full' },
      { path: 'loai-thu', data:{type:'thu'}, component:  LoaiThuChiListComponent},
      { path: 'loai-chi', data:{type:'chi'}, component:  LoaiThuChiListComponent},
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LoaiThuChiRoutingModule { }
