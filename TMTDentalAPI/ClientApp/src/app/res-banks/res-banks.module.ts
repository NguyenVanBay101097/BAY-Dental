import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResBanksRoutingModule } from './res-banks-routing.module';
import { ResBankListComponent } from './res-bank-list/res-bank-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ResBankService } from './res-bank.service';
import { ResBankCreateUpdateComponent } from './res-bank-create-update/res-bank-create-update.component';

@NgModule({
  declarations: [ResBankListComponent, ResBankCreateUpdateComponent],
  imports: [
    CommonModule,
    ResBanksRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ],
  providers: [ResBankService],
  entryComponents: [
    ResBankCreateUpdateComponent
  ]
})
export class ResBanksModule { }
