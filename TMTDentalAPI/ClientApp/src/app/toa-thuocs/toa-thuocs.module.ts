import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToaThuocsRoutingModule } from './toa-thuocs-routing.module';
import { ToaThuocCuDialogComponent } from './toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { ToaThuocService } from './toa-thuoc.service';
import { ToaThuocLineDialogComponent } from './toa-thuoc-line-dialog/toa-thuoc-line-dialog.component';
import { ToaThuocLineCuFormComponent } from './toa-thuoc-line-cu-form/toa-thuoc-line-cu-form.component';

@NgModule({
  declarations: [ToaThuocCuDialogComponent, ToaThuocLineDialogComponent, ToaThuocLineCuFormComponent],
  imports: [
    CommonModule,
    ToaThuocsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  entryComponents: [
    ToaThuocCuDialogComponent,
    ToaThuocLineDialogComponent
  ],
  providers: [
    ToaThuocService
  ]
})
export class ToaThuocsModule { }
