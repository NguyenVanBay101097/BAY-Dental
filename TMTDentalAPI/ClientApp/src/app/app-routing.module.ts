import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'histories', 
    loadChildren: () => import('./history/history.module').then(m => m.HistoryModule)
  },
  {
    path: 'partners', 
    loadChildren: () => import('./partners/partners.module').then(m => m.PartnersModule)
  },
  {
    path: 'sale-orders', 
    loadChildren: () => import('./sale-orders/sale-orders.module').then(m => m.SaleOrdersModule)
  },
  {
    path: 'appointments', 
    loadChildren: () => import('./appointment/appointment.module').then(m => m.AppointmentModule)
  },
  {
    path: 'labo-orders', 
    loadChildren: () => import('./labo-orders/labo-orders.module').then(m => m.LaboOrdersModule)
  },
  {
    path: 'purchase', 
    loadChildren: () => import('./purchase-orders/purchase-orders.module').then(m => m.PurchaseOrdersModule)
  },
  {
    path: 'stock', 
    loadChildren: () => import('./stock-pickings/stock-pickings.module').then(m => m.StockPickingsModule)
  },
  {
    path: 'account', 
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
