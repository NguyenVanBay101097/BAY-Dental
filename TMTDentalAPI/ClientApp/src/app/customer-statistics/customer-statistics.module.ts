import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomerStatisticsRoutingModule } from './customer-statistics-routing.module';
import { CustomerStatisticsComponent } from './customer-statistics/customer-statistics.component';
import { SharedModule } from '../shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [CustomerStatisticsComponent],
  imports: [
    CommonModule,
    CustomerStatisticsRoutingModule,
    SharedModule, 
    ReactiveFormsModule
  ]
})
export class CustomerStatisticsModule { }
