import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResInsuranceReportsRoutingModule } from './res-insurance-reports-routing.module';
import { ResInsuranceReportsOverviewComponent } from './res-insurance-reports-overview/res-insurance-reports-overview.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ResInsuranceDetailComponent } from './res-insurance-detail/res-insurance-detail.component';
import { ResInsuranceDebitComponent } from './res-insurance-debit/res-insurance-debit.component';
import { ResInsuranceHistoriesComponent } from './res-insurance-histories/res-insurance-histories.component';


@NgModule({
  declarations: [ResInsuranceReportsOverviewComponent, ResInsuranceDetailComponent, ResInsuranceDebitComponent, ResInsuranceHistoriesComponent],
  imports: [
    CommonModule,
    ResInsuranceReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ]
})
export class ResInsuranceReportsModule { }
