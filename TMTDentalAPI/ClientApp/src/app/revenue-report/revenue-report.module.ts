import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RevenueReportRoutingModule } from './revenue-report-routing.module';
import { RevenueReportManagerComponent } from './revenue-report-manager/revenue-report-manager.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RevenueReportService } from './revenue-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [RevenueReportManagerComponent],
  imports: [
    CommonModule,
    RevenueReportRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [
    RevenueReportService
  ]
})
export class RevenueReportModule { }
