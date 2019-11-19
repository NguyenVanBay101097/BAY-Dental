import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home/home.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { HomeService } from './home.service';
import { SaleReportComponent } from './sale-report/sale-report.component';
import { HomeBusinessSituationComponent } from './home-business-situation/home-business-situation.component';
import { HomeTodayAppointmentComponent } from './home-today-appointment/home-today-appointment.component';

@NgModule({
  declarations: [HomeComponent, SaleReportComponent, HomeBusinessSituationComponent, HomeTodayAppointmentComponent],
  imports: [
    CommonModule,
    HomeRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [HomeService]
})
export class HomeModule { }
