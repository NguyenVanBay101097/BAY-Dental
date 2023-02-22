import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleDashboardReportsRoutingModule } from './sale-dashboard-reports-routing.module';
import { SaleDashboardReportFormComponent } from './sale-dashboard-report-form/sale-dashboard-report-form.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { SaleDashboardInvoiceReportComponent } from './sale-dashboard-invoice-report/sale-dashboard-invoice-report.component';
import { SaleDashboardCashbookReportComponent } from './sale-dashboard-cashbook-report/sale-dashboard-cashbook-report.component';
import { SaleDashboardApCrChartComponent } from './sale-dashboard-ap-cr-chart/sale-dashboard-ap-cr-chart.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
@NgModule({
  declarations: [
    SaleDashboardReportFormComponent,
    SaleDashboardInvoiceReportComponent,
    SaleDashboardCashbookReportComponent,
    SaleDashboardApCrChartComponent, 
  ],
  imports: [
    CommonModule,
    SaleDashboardReportsRoutingModule,
    SharedModule,
    MyCustomKendoModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
  ]
})
export class SaleDashboardReportsModule { }
