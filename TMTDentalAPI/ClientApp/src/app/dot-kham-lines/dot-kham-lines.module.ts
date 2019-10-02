import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DotKhamLinesRoutingModule } from './dot-kham-lines-routing.module';
import { DotKhamLineCuDialogComponent } from './dot-kham-line-cu-dialog/dot-kham-line-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { DotKhamLineChangeRoutingDialogComponent } from './dot-kham-line-change-routing-dialog/dot-kham-line-change-routing-dialog.component';

@NgModule({
  declarations: [DotKhamLineCuDialogComponent, DotKhamLineChangeRoutingDialogComponent],
  imports: [
    CommonModule,
    DotKhamLinesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  providers: [

  ],
  entryComponents: [
    DotKhamLineCuDialogComponent,
    DotKhamLineChangeRoutingDialogComponent
  ]
})
export class DotKhamLinesModule { }
