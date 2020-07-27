import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnerTitlesRoutingModule } from './partner-titles-routing.module';
import { PartnerTitleListComponent } from './partner-title-list/partner-title-list.component';
import { PartnerTitleCuDialogComponent } from './partner-title-cu-dialog/partner-title-cu-dialog.component';
import { PartnerService } from '../partners/partner.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    PartnerTitleListComponent, 
    PartnerTitleCuDialogComponent
  ],
  imports: [
    CommonModule,
    PartnerTitlesRoutingModule, 
    MyCustomKendoModule, 
    FormsModule, 
    ReactiveFormsModule
  ], 
  providers: [
    PartnerService
  ], 
  entryComponents: [
    PartnerTitleCuDialogComponent
  ]
})
export class PartnerTitlesModule { }
