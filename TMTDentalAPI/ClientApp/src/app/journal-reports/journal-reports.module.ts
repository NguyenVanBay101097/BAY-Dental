import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { JournalReportsRoutingModule } from './journal-reports-routing.module';
import { JournalReportsViewComponent } from './journal-reports-view/journal-reports-view.component';
import { JournalReportService } from './journal-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { JournalReportDetailComponent } from './journal-report-detail/journal-report-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [JournalReportsViewComponent, JournalReportDetailComponent],
  imports: [
    CommonModule,
    JournalReportsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ],
  providers: [JournalReportService]
})
export class JournalReportsModule { }
