import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockInventoryFormComponent } from './stock-inventory-form/stock-inventory-form.component';
import { StockInventoryListComponent } from './stock-inventory-list/stock-inventory-list.component';


const routes: Routes = [
  {
    path: '',
    component: StockInventoryListComponent
  },
  {
    path: 'form',
    component: StockInventoryFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockInventoryRoutingModule { }
