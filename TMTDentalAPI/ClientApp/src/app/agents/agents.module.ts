import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentListComponent } from './agent-list/agent-list.component';
import { AgentRoutingModule } from './agent-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { AgentService } from './agent.service';

@NgModule({
  declarations: [AgentListComponent],
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
  ], entryComponents: [
  ]
})
export class AgentsModule { }
