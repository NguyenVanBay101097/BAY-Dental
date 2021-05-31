import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DotKhamLinesRoutingModule } from './dot-kham-lines-routing.module';
import { DotKhamLineCuDialogComponent } from './dot-kham-line-cu-dialog/dot-kham-line-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DotKhamLineChangeRoutingDialogComponent } from './dot-kham-line-change-routing-dialog/dot-kham-line-change-routing-dialog.component';
import { DotKhamLineListComponent } from './dot-kham-line-list/dot-kham-line-list.component';
import { SharedModule } from '../shared/shared.module';
import { DotKhamLineService } from './dot-kham-line.service';

@NgModule({
  declarations: [DotKhamLineCuDialogComponent, DotKhamLineChangeRoutingDialogComponent, DotKhamLineListComponent],
  imports: [
    CommonModule,
    DotKhamLinesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule,
    FormsModule
  ],
  providers: [
    DotKhamLineService
  ],
  entryComponents: [
    DotKhamLineCuDialogComponent,
    DotKhamLineChangeRoutingDialogComponent
  ]
})
export class DotKhamLinesModule { }
