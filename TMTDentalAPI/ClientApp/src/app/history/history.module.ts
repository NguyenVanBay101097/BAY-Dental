import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HistoryRoutingModule } from './history-routing.module';
import { HistoriesListComponent } from './histories-list/histories-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { HistoryService } from './history.service';
import { HistoriesCreateUpdateComponent } from './histories-create-update/histories-create-update.component';

@NgModule({
  declarations: [HistoriesListComponent, HistoriesCreateUpdateComponent],
  imports: [
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    HistoryRoutingModule
  ],
  providers: [HistoryService],
  entryComponents: [HistoriesCreateUpdateComponent]
})
export class HistoryModule { }
