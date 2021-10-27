import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaboManagementRoutingModule } from './labo-management-routing.module';
import { LaboBiteJointsModule } from '../labo-bite-joints/labo-bite-joints.module';
import { LaboBridgesModule } from '../labo-bridges/labo-bridges.module';
import { LaboFinishLinesModule } from '../labo-finish-lines/labo-finish-lines.module';
import { LaboManagementComponent } from '../catalog/labo-management/labo-management.component';
import { ProductLaboAttachCuDialogComponent } from '../products/product-labo-attach-cu-dialog/product-labo-attach-cu-dialog.component';
import { ProductLaboAttachListComponent } from '../products/product-labo-attach-list/product-labo-attach-list.component';
import { ProductLaboCuDialogComponent } from '../products/product-labo-cu-dialog/product-labo-cu-dialog.component';
import { ProductLaboListComponent } from '../products/product-labo-list/product-labo-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';


@NgModule({
  declarations: [
    LaboManagementComponent,
    ProductLaboAttachCuDialogComponent,
    ProductLaboAttachListComponent,
    ProductLaboCuDialogComponent,
    ProductLaboListComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    LaboManagementRoutingModule,
    LaboBiteJointsModule,
    LaboBridgesModule,
    LaboFinishLinesModule
  ],
})
export class LaboManagementModule { }
