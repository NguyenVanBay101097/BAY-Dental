import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToothDiagnosisRoutingModule } from './tooth-diagnosis-routing.module';
import { ToothDiagnosisListComponent } from './tooth-diagnosis-list/tooth-diagnosis-list.component';
import { ToothDiagnosisCreateUpdateDialogComponent } from './tooth-diagnosis-create-update-dialog/tooth-diagnosis-create-update-dialog.component';

import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToothDiagnosisService } from './tooth-diagnosis.service';
@NgModule({
  declarations: [ToothDiagnosisListComponent, ToothDiagnosisCreateUpdateDialogComponent],
  imports: [
    CommonModule,
    FormsModule,
    NgbModule,
    ReactiveFormsModule,
    ToothDiagnosisRoutingModule,
    MyCustomKendoModule
  ],
  providers:[ToothDiagnosisService],
  entryComponents: [
    ToothDiagnosisCreateUpdateDialogComponent
  ]
})
export class ToothDiagnosisModule { }
