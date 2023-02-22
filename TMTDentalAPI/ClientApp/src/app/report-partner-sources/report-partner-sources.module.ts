import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportPartnerSourceListComponent } from './report-partner-source-list/report-partner-source-list.component';
import { ReportPartnerSourcesRoutingModule } from './report-partner-sources-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { ReportPartnerSourcesService } from './report-partner-sources.service';

@NgModule({
  declarations: [ReportPartnerSourceListComponent],
  imports: [
    CommonModule,
    ReportPartnerSourcesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [ReportPartnerSourcesService],
})
export class ReportPartnerSourcesModule { }
