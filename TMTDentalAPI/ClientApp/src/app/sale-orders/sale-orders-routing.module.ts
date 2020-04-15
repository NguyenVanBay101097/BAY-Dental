import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleQuotationListComponent } from './sale-quotation-list/sale-quotation-list.component';
import { SaleQuotationCreateUpdateComponent } from './sale-quotation-create-update/sale-quotation-create-update.component';
import { SaleOrderInvoiceListComponent } from './sale-order-invoice-list/sale-order-invoice-list.component';

const routes: Routes = [
  {
    path: 'sale-orders',
    component: SaleOrderListComponent
  },
  {
    path: 'sale-orders/form',
    component: SaleOrderCreateUpdateComponent
  },
  {
    path: 'sale-quotations',
    component: SaleQuotationListComponent
  },
  {
    path: 'sale-quotations/form',
    component: SaleQuotationCreateUpdateComponent
  },
  {
    path: 'sale-orders/:id/invoices',
    component: SaleOrderInvoiceListComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleOrdersRoutingModule { }
