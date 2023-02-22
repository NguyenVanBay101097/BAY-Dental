import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UomRoutingModule } from './uom-routing.module';
import { UomCrUpComponent } from './uom-cr-up/uom-cr-up.component';
import { UomListComponent } from './uom-list/uom-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UomImportExcelDialogComponent } from './uom-import-excel-dialog/uom-import-excel-dialog.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    UomCrUpComponent,
    UomListComponent,
    UomImportExcelDialogComponent],
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
    UomCrUpComponent,
    UomImportExcelDialogComponent
  ]

})
export class UomModule { }
