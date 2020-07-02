import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SamplePrescriptionsService } from './sample-prescriptions.service';
import { SamplePrescriptionListComponent } from './sample-prescription-list/sample-prescription-list.component';
import { SamplePrescriptionCreateUpdateDialogComponent } from './sample-prescription-create-update-dialog/sample-prescription-create-update-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CustomComponentModule } from '../common/common.module';
import { SharedModule, DialogsModule } from '@progress/kendo-angular-dialog';
import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { SamplePrescriptionsRoutingModule } from './sample-prescriptions-routing.module';
import { SamplePrescriptionLineCreateUpdateDialogComponent } from './sample-prescription-line-create-update-dialog/sample-prescription-line-create-update-dialog.component';

@NgModule({
  declarations: [SamplePrescriptionListComponent, SamplePrescriptionCreateUpdateDialogComponent, SamplePrescriptionLineCreateUpdateDialogComponent],
  imports: [
    SamplePrescriptionsRoutingModule,
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    CustomComponentModule,
    SharedModule,
    GridModule,
    ButtonsModule,
    DialogsModule,
    FormsModule
  ],  
  entryComponents: [
    SamplePrescriptionCreateUpdateDialogComponent,
    SamplePrescriptionLineCreateUpdateDialogComponent
  ],
  providers: [
    SamplePrescriptionsService
  ]
})
export class SamplePrescriptionsModule { }
