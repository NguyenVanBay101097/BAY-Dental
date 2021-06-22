import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { ToaThuocsRoutingModule } from './toa-thuocs-routing.module';
import { ToaThuocCuDialogComponent } from './toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ToaThuocService } from './toa-thuoc.service';
import { ToaThuocLineDialogComponent } from './toa-thuoc-line-dialog/toa-thuoc-line-dialog.component';
import { ToaThuocLineCuFormComponent } from './toa-thuoc-line-cu-form/toa-thuoc-line-cu-form.component';
import { ToaThuocLinesSaveCuFormComponent } from './toa-thuoc-lines-save-cu-form/toa-thuoc-lines-save-cu-form.component';
import { ProductsModule } from '../products/products.module';
import { ToaThuocLineUseatPopoverComponent } from './toa-thuoc-line-useat-popover/toa-thuoc-line-useat-popover.component';

@NgModule({
  declarations: [ToaThuocCuDialogComponent, ToaThuocLineDialogComponent, ToaThuocLineCuFormComponent, ToaThuocLinesSaveCuFormComponent, ToaThuocLineUseatPopoverComponent],
  imports: [
    CommonModule,
    ToaThuocsRoutingModule,
    ProductsModule,
    MyCustomKendoModule,
    ReactiveFormsModule, 
    FormsModule,
    NgbModule
  ],exports:[
    ToaThuocLineUseatPopoverComponent
  ],
  entryComponents: [
    ToaThuocCuDialogComponent,
    ToaThuocLineDialogComponent,
    ToaThuocLineUseatPopoverComponent,
  ],
  providers: [
    ToaThuocService
  ]
})
export class ToaThuocsModule { }
