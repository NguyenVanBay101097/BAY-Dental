import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockPickingTypesRoutingModule } from './stock-picking-types-routing.module';
import { PickingTypeOverviewComponent } from './picking-type-overview/picking-type-overview.component';
import { StockPickingTypeService } from './stock-picking-type.service';

@NgModule({
  declarations: [PickingTypeOverviewComponent],
  imports: [
    CommonModule,
    StockPickingTypesRoutingModule
  ],
  providers: [
    StockPickingTypeService
  ]
})
export class StockPickingTypesModule { }
