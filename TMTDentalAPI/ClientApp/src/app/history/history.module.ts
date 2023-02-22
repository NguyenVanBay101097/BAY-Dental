import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HistoryRoutingModule } from './history-routing.module';
import { HistoriesListComponent } from './histories-list/histories-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { HistoryService } from './history.service';
import { HistoriesCreateUpdateComponent } from './histories-create-update/histories-create-update.component';
import { HistoryImportExcelDialogComponent } from './history-import-excel-dialog/history-import-excel-dialog.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [HistoriesListComponent, HistoriesCreateUpdateComponent, HistoryImportExcelDialogComponent],
  imports: [
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    HistoryRoutingModule,
    SharedModule
  ],
  providers: [HistoryService],
  entryComponents: [HistoriesCreateUpdateComponent,HistoryImportExcelDialogComponent]
})
export class HistoryModule { }
