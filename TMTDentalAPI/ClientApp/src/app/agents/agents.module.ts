import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentListComponent } from './agent-list/agent-list.component';
import { AgentRoutingModule } from './agent-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { AgentService } from './agent.service';
import { AgentCommmissionPaymentDialogComponent } from './agent-commmission-payment-dialog/agent-commmission-payment-dialog.component';
import { CommissionSettlementAgentPaymentDialogComponent } from '../commission-settlements/commission-settlement-agent-payment-dialog/commission-settlement-agent-payment-dialog.component';
import { CommissionSettlementsModule } from '../commission-settlements/commission-settlements.module';

@NgModule({
  declarations: [AgentListComponent, AgentCommmissionPaymentDialogComponent],
  imports: [
    CommonModule,
    AgentRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    CommissionSettlementsModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ], providers: [
    AgentService
  ], entryComponents: [
    CommissionSettlementAgentPaymentDialogComponent
  ]
})
export class AgentsModule { }
