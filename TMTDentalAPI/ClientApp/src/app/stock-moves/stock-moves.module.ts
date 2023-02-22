import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockMovesRoutingModule } from './stock-moves-routing.module';
import { StockMoveService } from './stock-move.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    StockMovesRoutingModule
  ],
  providers: [
    StockMoveService
  ]
})
export class StockMovesModule { }
