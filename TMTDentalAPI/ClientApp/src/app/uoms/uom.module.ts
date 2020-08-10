import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UomRoutingModule } from './uom-routing.module';
import { UomCrUpComponent } from './uom-cr-up/uom-cr-up.component';
import { UomListComponent } from './uom-list/uom-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '@progress/kendo-angular-dialog';

@NgModule({
  declarations: [
    UomCrUpComponent,
    UomListComponent],
  imports: [
    CommonModule,
    UomRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    SharedModule
  ],
  entryComponents: [
    UomCrUpComponent
  ]

})
export class UomModule { }
