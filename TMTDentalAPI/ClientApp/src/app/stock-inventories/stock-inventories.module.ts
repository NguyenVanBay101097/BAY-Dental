import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StockInventoryRoutingModule } from './stock-inventories-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StockInventoryListComponent } from './stock-inventory-list/stock-inventory-list.component';
import { StockInventoryService } from './stock-inventory.service';
import { StockInventoryFormComponent } from './stock-inventory-form/stock-inventory-form.component';

@NgModule({
  declarations: [StockInventoryListComponent, StockInventoryFormComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule,
    NgbModule,
    StockInventoryRoutingModule
  ],
  providers: [
    StockInventoryService
  ],
  entryComponents: [
    StockInventoryFormComponent
  ]
})
export class StockInventoriesModule { }
