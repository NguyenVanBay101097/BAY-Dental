import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockPickingsRoutingModule } from './stock-pickings-routing.module';
import { StockPickingListComponent } from './stock-picking-list/stock-picking-list.component';
import { StockPickingService } from './stock-picking.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { StockPickingCreateUpdateComponent } from './stock-picking-create-update/stock-picking-create-update.component';
import { StockPickingMlDialogComponent } from './stock-picking-ml-dialog/stock-picking-ml-dialog.component';
import { ProductsModule } from '../products/products.module';

@NgModule({
  declarations: [StockPickingListComponent, StockPickingCreateUpdateComponent, StockPickingMlDialogComponent],
  imports: [
    CommonModule,
    StockPickingsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    ProductsModule
  ],
  providers: [
    StockPickingService
  ],
  entryComponents: [
    StockPickingMlDialogComponent
  ]
})
export class StockPickingsModule { }
