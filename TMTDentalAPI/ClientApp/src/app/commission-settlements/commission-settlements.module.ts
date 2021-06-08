import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommissionSettlementsRoutingModule } from './commission-settlements-routing.module';
import { CommissionSettlementReportDetailComponent } from './commission-settlement-report-detail/commission-settlement-report-detail.component';
import { CommissionSettlementReportListComponent } from './commission-settlement-report-list/commission-settlement-report-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { CommissionSettlementsService } from './commission-settlements.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommissionSettlementReportComponent } from './commission-settlement-report/commission-settlement-report.component';

@NgModule({
  declarations: [CommissionSettlementReportListComponent, CommissionSettlementReportDetailComponent, CommissionSettlementReportComponent],
  imports: [
    CommonModule,
    CommissionSettlementsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [CommissionSettlementsService]
})
export class CommissionSettlementsModule { }
