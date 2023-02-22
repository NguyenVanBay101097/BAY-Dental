import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PriceListRoutingModule } from './price-list-routing.module';
import { PriceListListComponent } from './price-list-list/price-list-list.component';
import { PriceListCreateUpdateComponent } from './price-list-create-update/price-list-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [PriceListListComponent, PriceListCreateUpdateComponent],
  imports: [
    CommonModule,
    PriceListRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ]
})
export class PriceListModule { }
