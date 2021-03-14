import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockInventoryCriteriaListComponent } from './stock-inventory-criteria-list/stock-inventory-criteria-list.component';
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
  },
  {
    path: 'criterias',
    component: StockInventoryCriteriaListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockInventoryRoutingModule { }
