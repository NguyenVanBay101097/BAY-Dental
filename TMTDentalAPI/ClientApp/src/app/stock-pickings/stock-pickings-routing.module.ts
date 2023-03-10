import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list/stock-picking-outgoing-list.component';
import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update/stock-picking-outgoing-create-update.component';
import { StockPickingIncomingListComponent } from './stock-picking-incoming-list/stock-picking-incoming-list.component';
import { StockPickingIncomingCreateUpdateComponent } from './stock-picking-incoming-create-update/stock-picking-incoming-create-update.component';
import { StockPickingManagementComponent } from './stock-picking-management/stock-picking-management.component';
import { StockXuatNhapTonComponent } from './stock-xuat-nhap-ton/stock-xuat-nhap-ton.component';
import { StockInventoryListComponent } from '../stock-inventories/stock-inventory-list/stock-inventory-list.component';
import { StockInventoryFormComponent } from '../stock-inventories/stock-inventory-form/stock-inventory-form.component';
import { StockPickingRequestProductComponent } from './stock-picking-request-product/stock-picking-request-product.component';

const routes: Routes = [
  // {
  //   path: 'outgoing-pickings',
  //   component: StockPickingOutgoingListComponent
  // },
  {
    path: 'outgoing-pickings/create',
    component: StockPickingOutgoingCreateUpdateComponent
  },
  {
    path: 'outgoing-pickings/edit/:id',
    component: StockPickingOutgoingCreateUpdateComponent
  },
  // {
  //   path: 'incoming-pickings',
  //   component: StockPickingIncomingListComponent
  // },
  {
    path: 'incoming-pickings/create',
    component: StockPickingIncomingCreateUpdateComponent
  },
  {
    path: 'incoming-pickings/edit/:id',
    component: StockPickingIncomingCreateUpdateComponent
  },
  {
    path: '',
    component: StockPickingManagementComponent,
    children: [
      { path: '', redirectTo: 'stock-report-xuat-nhap-ton', pathMatch: 'full' },
      { path: 'stock-report-xuat-nhap-ton', component: StockXuatNhapTonComponent },
      { path: 'incoming-pickings', component: StockPickingIncomingListComponent },
      { path: 'outgoing-pickings', component: StockPickingOutgoingListComponent },
      { path: 'stock-inventories', component: StockInventoryListComponent },
      { path: 'request-products', component: StockPickingRequestProductComponent }
    ]
  },
  {
    path: 'stock-inventories/from',
    component: StockInventoryFormComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockPickingsRoutingModule { }
