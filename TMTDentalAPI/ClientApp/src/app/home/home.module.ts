import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home/home.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { HomeService } from './home.service';
import { SaleReportComponent } from './sale-report/sale-report.component';

@NgModule({
  declarations: [HomeComponent, SaleReportComponent],
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
