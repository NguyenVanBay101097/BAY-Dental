import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigPrintManagementComponent } from './config-print-management/config-print-management.component';
import { ConfigPrintFormComponent } from './config-print-form/config-print-form.component';
import { PrintPaperSizeListComponent } from './print-paper-size-list/print-paper-size-list.component';
import { PrintPaperSizeCreateUpdateDialogComponent } from './print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ConfigPrintsRoutingModule } from './config-prints-routing.module';
import { ConfigPrintService } from './config-print.service';
import { PrintPaperSizeService } from './print-paper-size.service';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [ConfigPrintManagementComponent, ConfigPrintFormComponent, PrintPaperSizeListComponent, PrintPaperSizeCreateUpdateDialogComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule,
    NgbModule,
    ConfigPrintsRoutingModule
  ],
  providers: [
    ConfigPrintService,
    PrintPaperSizeService
  ],
  entryComponents: [
    PrintPaperSizeCreateUpdateDialogComponent
  ]
})
export class ConfigPrintsModule { }
