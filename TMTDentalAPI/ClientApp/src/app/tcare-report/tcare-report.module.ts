import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TCareReportListComponent } from './tcare-report-list/tcare-report-list.component';
import { TCareReportDetailComponent } from './tcare-report-detail/tcare-report-detail.component';
import { TcareReportRoutingModule } from './tcare-report-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { TcareReportService } from './tcare-report.service';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [TCareReportListComponent, TCareReportDetailComponent],
  imports: [
    CommonModule,
    TcareReportRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  providers: [
    TcareReportService
  ]
})
export class TcareReportModule { }
