import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DotKhamStepsRoutingModule } from './dot-kham-steps-routing.module';
import { DotKhamStepReportComponent } from './dot-kham-step-report/dot-kham-step-report.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { NgbDropdown, NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DotKhamStepService } from '../dot-khams/dot-kham-step.service';

@NgModule({
  declarations: [DotKhamStepReportComponent],
  imports: [
    CommonModule,
    SharedModule,
    MyCustomKendoModule,
    DotKhamStepsRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule
  ],providers: [
    DotKhamStepService
  ],
  entryComponents: [
    DotKhamStepReportComponent,    
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DotKhamStepsModule { } 