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
import { AgentCommissionFormDetailComponent } from './agent-commission-form-detail/agent-commission-form-detail.component';

@NgModule({
  declarations: [AgentListComponent, AgentCreateUpdateDialogComponent, AgentCommissionListComponent, AgentCommissionFormDetailComponent],
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
    AgentCreateUpdateDialogComponent
  ]
})
export class AgentsModule { }
