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
import { AgentCommissionListComponent } from '../agents/agent-commission-list/agent-commission-list.component';
import { AgentCommissionFormComponent } from '../agents/agent-commission-form/agent-commission-form.component';
import { AgentCommmissionFormDetailComponent } from '../agents/agent-commmission-form-detail/agent-commmission-form-detail.component';
import { AgentCommmissionHistoryComponent } from '../agents/agent-commmission-history/agent-commmission-history.component';
import { AgentCommmissionFormDetailItemComponent } from '../agents/agent-commmission-form-detail-item/agent-commmission-form-detail-item.component';
import { AgentCommmissionPaymentDialogComponent } from '../agents/agent-commmission-payment-dialog/agent-commmission-payment-dialog.component';
import { CommissionSettlementAgentDetailComponent } from './commission-settlement-agent-detail/commission-settlement-agent-detail.component';
import { CommissionSettlementAgentProfileComponent } from './commission-settlement-agent-detail/commission-settlement-agent-profile/commission-settlement-agent-profile.component';
import { CommissionSettlementAgentCommissionComponent } from './commission-settlement-agent-detail/commission-settlement-agent-commission/commission-settlement-agent-commission.component';
import { CommissionSettlementAgentHistoryComponent } from './commission-settlement-agent-detail/commission-settlement-agent-history/commission-settlement-agent-history.component';

@NgModule({
  declarations: [CommissionSettlementReportListComponent, CommissionSettlementReportDetailComponent,
    CommissionSettlementReportComponent,
    AgentCommissionListComponent,
    AgentCommissionFormComponent,
    AgentCommmissionFormDetailComponent,
    AgentCommmissionHistoryComponent,
    AgentCommmissionFormDetailItemComponent,
    AgentCommmissionPaymentDialogComponent,
    CommissionSettlementAgentDetailComponent,
    CommissionSettlementAgentProfileComponent,
    CommissionSettlementAgentCommissionComponent,
    CommissionSettlementAgentHistoryComponent
  ],
  imports: [
    CommonModule,
    CommissionSettlementsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [CommissionSettlementsService],
  entryComponents: [AgentCommmissionPaymentDialogComponent]
})
export class CommissionSettlementsModule { }
