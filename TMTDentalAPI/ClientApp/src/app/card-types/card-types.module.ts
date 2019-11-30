import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CardTypesRoutingModule } from './card-types-routing.module';
import { CardTypeListComponent } from './card-type-list/card-type-list.component';
import { CardTypeService } from './card-type.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CardTypeCreateUpdateComponent } from './card-type-create-update/card-type-create-update.component';

@NgModule({
  declarations: [CardTypeListComponent, CardTypeCreateUpdateComponent],
  imports: [
    CommonModule,
    CardTypesRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MyCustomKendoModule
  ],
  providers: [
    CardTypeService
  ]
})
export class CardTypesModule { }
