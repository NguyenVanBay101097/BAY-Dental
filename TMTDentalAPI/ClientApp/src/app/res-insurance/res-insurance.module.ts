import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { ResInsuranceListComponent } from './res-insurance-list/res-insurance-list.component';
import { ResInsuranceRoutingModule } from './res-insurance-routing.module';

@NgModule({
  declarations: [ResInsuranceListComponent],
  imports: [
    CommonModule,
    ResInsuranceRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ]
})
export class ResInsuranceModule { }
