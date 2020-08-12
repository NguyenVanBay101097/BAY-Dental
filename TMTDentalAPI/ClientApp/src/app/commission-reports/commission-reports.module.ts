import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { CommissionReportListComponent } from './commission-report-list/commission-report-list.component';
import { CommissionReportsRoutingModule } from './commission-reports-routing.module';
import { CommissionReportsService } from './commission-reports.service';
import { CommissionReportDetailComponent } from './commission-report-detail/commission-report-detail.component';


@NgModule({
  declarations: [CommissionReportListComponent, CommissionReportDetailComponent],
  imports: [
    CommonModule,
    CommissionReportsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [CommissionReportsService]
})
export class CommissionReportsModule { }
