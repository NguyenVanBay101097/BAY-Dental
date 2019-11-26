import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResPartnerBanksRoutingModule } from './res-partner-banks-routing.module';
import { ResPartnerBankListComponent } from './res-partner-bank-list/res-partner-bank-list.component';
import { ResPartnerBankCreateUpdateComponent } from './res-partner-bank-create-update/res-partner-bank-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ResPartnerBankService } from './res-partner-bank.service';

@NgModule({
  declarations: [ResPartnerBankListComponent, ResPartnerBankCreateUpdateComponent],
  imports: [
    CommonModule,
    ResPartnerBanksRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ],
  providers: [ResPartnerBankService],
  entryComponents: [
    ResPartnerBankCreateUpdateComponent
  ]
})
export class ResPartnerBanksModule { }
