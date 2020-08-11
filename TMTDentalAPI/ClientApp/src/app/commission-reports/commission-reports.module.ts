import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionReportListComponent } from './commission-report-list/commission-report-list.component';
import { CommissionReportsRoutingModule } from './commission-reports-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '@progress/kendo-angular-dialog';
import { CommissionReportsService } from './commission-reports.service';


@NgModule({
  declarations: [CommissionReportListComponent],
  imports: [
    CommonModule,
    CommissionReportsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [CommissionReportsService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CommissionReportsModule { }
