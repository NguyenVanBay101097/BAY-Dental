import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'loai-thu-chi',
    loadChildren: () => import('./loai-thu-chi/loai-thu-chi.module').then(m => m.LoaiThuChiModule)
  },
  {
    path: 'phieu-thu-chi',
    loadChildren: () => import('./phieu-thu-chi/phieu-thu-chi.module').then(m => m.PhieuThuChiModule)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
