import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaboOrderLinesRoutingModule } from './labo-order-lines-routing.module';
import { LaboOrderLineCuDialogComponent } from './labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { LaboOrderLineListComponent } from './labo-order-line-list/labo-order-line-list.component';
import { LaboOrderLineService } from './labo-order-line.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LaboOrderLineAdvanceSearchComponent } from './labo-order-line-advance-search/labo-order-line-advance-search.component';

@NgModule({
  declarations: [LaboOrderLineCuDialogComponent, LaboOrderLineListComponent, LaboOrderLineAdvanceSearchComponent],
  imports: [
    CommonModule,
    LaboOrderLinesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    LaboOrderLineService
  ],
  entryComponents: [
    LaboOrderLineCuDialogComponent
  ]
})
export class LaboOrderLinesModule { }
