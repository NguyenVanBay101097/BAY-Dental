import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UomRoutingModule } from './uom-routing.module';
import { UomCrUpComponent } from './uom-cr-up/uom-cr-up.component';
import { UomListComponent } from './uom-list/uom-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    UomCrUpComponent,
    UomListComponent],
  imports: [
    CommonModule,
    UomRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    ReactiveFormsModule
  ],
  entryComponents: [
    UomCrUpComponent
  ]

})
export class UomModule { }
