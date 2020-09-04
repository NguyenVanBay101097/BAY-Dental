import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ZaloOaConfigEstablishComponent } from './zalo-oa-config-establish/zalo-oa-config-establish.component';

const routes: Routes = [
  {
    path: '',
    component: ZaloOaConfigEstablishComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ZaloOaConfigRoutingModule { }
