import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PriceListListComponent } from './price-list-list/price-list-list.component';
import { PriceListCreateUpdateComponent } from './price-list-create-update/price-list-create-update.component';

const routes: Routes = [
  {
    path: 'pricelists',
    component: PriceListListComponent
  },
  {
    path: 'pricelists/create',
    component: PriceListCreateUpdateComponent
  },
  {
    path: 'pricelists/edit/:id',
    component: PriceListCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PriceListRoutingModule { }
