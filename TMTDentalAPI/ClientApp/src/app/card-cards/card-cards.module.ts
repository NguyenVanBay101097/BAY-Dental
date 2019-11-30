import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CardCardsRoutingModule } from './card-cards-routing.module';
import { CardCardCuDialogComponent } from './card-card-cu-dialog/card-card-cu-dialog.component';
import { CardCardService } from './card-card.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CardCardGridComponent } from './card-card-grid/card-card-grid.component';
import { SharedModule } from '../shared/shared.module';
import { CardCardListComponent } from './card-card-list/card-card-list.component';

@NgModule({
  declarations: [CardCardCuDialogComponent, CardCardGridComponent, CardCardListComponent],
  imports: [
    CommonModule,
    CardCardsRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    SharedModule
  ],
  exports: [
    CardCardGridComponent
  ],
  entryComponents: [
    CardCardCuDialogComponent
  ],
  providers: [
    CardCardService
  ]
})
export class CardCardsModule { }
