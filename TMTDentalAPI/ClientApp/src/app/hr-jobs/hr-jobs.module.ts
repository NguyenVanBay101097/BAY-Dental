import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HrJobsRoutingModule } from './hr-jobs-routing.module';
import { HrJobListComponent } from './hr-job-list/hr-job-list.component';
import { HrJobCuDialogComponent } from './hr-job-cu-dialog/hr-job-cu-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '@progress/kendo-angular-dropdowns';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HrJobService } from './hr-job.service';

@NgModule({
  declarations: [HrJobListComponent, HrJobCuDialogComponent],
  imports: [
    CommonModule,
    HrJobsRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ],
  providers: [HrJobService],
  entryComponents: [HrJobCuDialogComponent]
})
export class HrJobsModule { }
