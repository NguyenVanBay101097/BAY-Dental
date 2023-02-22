import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnerTitlesRoutingModule } from './partner-titles-routing.module';
import { PartnerTitleListComponent } from './partner-title-list/partner-title-list.component';
import { PartnerService } from '../partners/partner.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    PartnerTitleListComponent, 
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
  ]
})
export class PartnerTitlesModule { }
