import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResInsuranceRoutingModule } from './res-insurance-routing.module';
import { ResInsuranceListComponent } from './res-insurance-list/res-insurance-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { ResInsuranceDetailComponent } from './res-insurance-detail/res-insurance-detail.component';
import { ResInsuranceDebitComponent } from './res-insurance-debit/res-insurance-debit.component';
import { ResInsuranceHistoriesComponent } from './res-insurance-histories/res-insurance-histories.component';


@NgModule({
  declarations: [ResInsuranceListComponent, ResInsuranceDetailComponent, ResInsuranceDebitComponent, ResInsuranceHistoriesComponent],
  imports: [
    CommonModule,
    ResInsuranceRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ]
})
export class ResInsuranceModule { }
