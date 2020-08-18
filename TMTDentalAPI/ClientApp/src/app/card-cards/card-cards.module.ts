import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CardCardsRoutingModule } from './card-cards-routing.module';
import { CardCardCuDialogComponent } from './card-card-cu-dialog/card-card-cu-dialog.component';
import { CardCardService } from './card-card.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { CardCardListComponent } from './card-card-list/card-card-list.component';
import { CardTypeService } from '../card-types/card-type.service';

@NgModule({
  declarations: [CardCardCuDialogComponent, CardCardListComponent],
  imports: [
    CommonModule,
    CardCardsRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule,
    SharedModule
  ],
  exports: [
  ],
  entryComponents: [
    CardCardCuDialogComponent,
  ],
  providers: [
    CardCardService,
    CardTypeService
  ]
})
export class CardCardsModule { }
