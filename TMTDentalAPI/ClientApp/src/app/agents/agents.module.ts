import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentListComponent } from './agent-list/agent-list.component';
import { AgentCreateUpdateDialogComponent } from './agent-create-update-dialog/agent-create-update-dialog.component';
import { AgentRoutingModule } from './agent-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { AgentService } from './agent.service';
import { AgentCommissionListComponent } from './agent-commission-list/agent-commission-list.component';
import { AgentCommissionFormComponent } from './agent-commission-form/agent-commission-form.component';
import { AgentCommmissionFormDetailComponent } from './agent-commmission-form-detail/agent-commmission-form-detail.component';
import { AgentCommmissionHistoryComponent } from './agent-commmission-history/agent-commmission-history.component';
import { AgentCommmissionPaymentDialogComponent } from './agent-commmission-payment-dialog/agent-commmission-payment-dialog.component';
import { AgentCommmissionFormDetailItemComponent } from './agent-commmission-form-detail-item/agent-commmission-form-detail-item.component';

@NgModule({
  declarations: [AgentListComponent, AgentCreateUpdateDialogComponent, AgentCommissionListComponent, AgentCommissionFormComponent, AgentCommmissionFormDetailComponent, AgentCommmissionHistoryComponent, AgentCommmissionPaymentDialogComponent, AgentCommmissionFormDetailItemComponent],
  imports: [
    CommonModule,
    AgentRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ], providers: [
    AgentService
  ], entryComponents:
  [
    AgentCreateUpdateDialogComponent,
    AgentCommmissionPaymentDialogComponent
  ]
})
export class AgentsModule { }
