import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleQuotationListComponent } from './sale-quotation-list/sale-quotation-list.component';
import { SaleQuotationCreateUpdateComponent } from './sale-quotation-create-update/sale-quotation-create-update.component';

const routes: Routes = [
  {
    path: '',
    component: SaleQuotationListComponent
  },
  {
    path: 'form',
    component: SaleQuotationCreateUpdateComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleQuotationsRoutingModule { }
