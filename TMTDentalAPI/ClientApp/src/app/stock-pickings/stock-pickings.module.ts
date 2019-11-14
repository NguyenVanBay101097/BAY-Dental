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
import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list/stock-picking-outgoing-list.component';
import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update/stock-picking-outgoing-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { StockPickingIncomingListComponent } from './stock-picking-incoming-list/stock-picking-incoming-list.component';

@NgModule({
  declarations: [StockPickingListComponent, StockPickingCreateUpdateComponent, StockPickingMlDialogComponent, StockPickingOutgoingListComponent, StockPickingOutgoingCreateUpdateComponent, StockPickingIncomingListComponent],
  imports: [
    CommonModule,
    StockPickingsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    ProductsModule,
    SharedModule
  ],
  providers: [
    StockPickingService
  ],
  entryComponents: [
    StockPickingMlDialogComponent
  ]
})
export class StockPickingsModule { }
