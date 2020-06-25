import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToaThuocsRoutingModule } from './toa-thuocs-routing.module';
import { ToaThuocCuDialogComponent } from './toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule } from '@angular/forms';
import { ToaThuocService } from './toa-thuoc.service';
import { ToaThuocLineDialogComponent } from './toa-thuoc-line-dialog/toa-thuoc-line-dialog.component';
import { ToaThuocLineCuFormComponent } from './toa-thuoc-line-cu-form/toa-thuoc-line-cu-form.component';
import { ToaThuocCuDialogSaveComponent } from './toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocLinesSaveCuFormComponent } from './toa-thuoc-lines-save-cu-form/toa-thuoc-lines-save-cu-form.component';

@NgModule({
  declarations: [ToaThuocCuDialogComponent, ToaThuocLineDialogComponent, ToaThuocLineCuFormComponent, ToaThuocCuDialogSaveComponent, ToaThuocLinesSaveCuFormComponent],
  imports: [
    CommonModule,
    ToaThuocsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule
  ],
  entryComponents: [
    ToaThuocCuDialogComponent,
    ToaThuocLineDialogComponent,
    ToaThuocCuDialogSaveComponent
  ],
  providers: [
    ToaThuocService
  ]
})
export class ToaThuocsModule { }
