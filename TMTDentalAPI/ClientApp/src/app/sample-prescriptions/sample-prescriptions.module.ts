import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SamplePrescriptionListComponent } from './sample-prescription-list/sample-prescription-list.component';
import { SamplePrescriptionCreateUpdateDialogComponent } from './sample-prescription-create-update-dialog/sample-prescription-create-update-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CustomComponentModule } from '../common/common.module';
import { SamplePrescriptionsRoutingModule } from './sample-prescriptions-routing.module';
import { SamplePrescriptionLineCreateUpdateDialogComponent } from './sample-prescription-line-create-update-dialog/sample-prescription-line-create-update-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [SamplePrescriptionListComponent, SamplePrescriptionCreateUpdateDialogComponent, SamplePrescriptionLineCreateUpdateDialogComponent],
  imports: [
    SamplePrescriptionsRoutingModule,
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    CustomComponentModule,
    FormsModule,
    NgbModule
  ],  
  entryComponents: [
    SamplePrescriptionCreateUpdateDialogComponent,
    SamplePrescriptionLineCreateUpdateDialogComponent,
  ],
  providers: []
})
export class SamplePrescriptionsModule { }
