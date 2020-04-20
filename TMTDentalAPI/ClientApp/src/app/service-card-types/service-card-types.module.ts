import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ServiceCardTypesRoutingModule } from './service-card-types-routing.module';
import { ServiceCardTypeListComponent } from './service-card-type-list/service-card-type-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ServiceCardTypeCuDialogComponent } from './service-card-type-cu-dialog/service-card-type-cu-dialog.component';

@NgModule({
  declarations: [ServiceCardTypeListComponent, ServiceCardTypeCuDialogComponent],
  imports: [
    CommonModule,
    ServiceCardTypesRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ],
  entryComponents: [
    ServiceCardTypeCuDialogComponent
  ]
})
export class ServiceCardTypesModule { }
