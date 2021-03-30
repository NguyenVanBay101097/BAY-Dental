import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToothDiagnosisRoutingModule } from './tooth-diagnosis-routing.module';
import { ToothDiagnosisListComponent } from './tooth-diagnosis-list/tooth-diagnosis-list.component';
import { ToothDiagnosisCreateUpdateDialogComponent } from './tooth-diagnosis-create-update-dialog/tooth-diagnosis-create-update-dialog.component';

import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
@NgModule({
  declarations: [ToothDiagnosisListComponent, ToothDiagnosisCreateUpdateDialogComponent],
  imports: [
    CommonModule,
    ToothDiagnosisRoutingModule,
    MyCustomKendoModule
  ]
})
export class ToothDiagnosisModule { }
