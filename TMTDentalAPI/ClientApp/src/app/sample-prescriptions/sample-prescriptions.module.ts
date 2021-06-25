import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SamplePrescriptionListComponent } from './sample-prescription-list/sample-prescription-list.component';
import { SamplePrescriptionCreateUpdateDialogComponent } from './sample-prescription-create-update-dialog/sample-prescription-create-update-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SamplePrescriptionsRoutingModule } from './sample-prescriptions-routing.module';
import { SamplePrescriptionLineCreateUpdateDialogComponent } from './sample-prescription-line-create-update-dialog/sample-prescription-line-create-update-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToaThuocsModule } from '../toa-thuocs/toa-thuocs.module';

@NgModule({
  declarations: [SamplePrescriptionListComponent, SamplePrescriptionCreateUpdateDialogComponent, SamplePrescriptionLineCreateUpdateDialogComponent],
  imports: [
    SamplePrescriptionsRoutingModule,
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    ToaThuocsModule,
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
