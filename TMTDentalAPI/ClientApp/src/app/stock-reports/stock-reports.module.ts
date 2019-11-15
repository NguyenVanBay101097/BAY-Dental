import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockReportsRoutingModule } from './stock-reports-routing.module';
import { StockReportXuatNhapTonComponent } from './stock-report-xuat-nhap-ton/stock-report-xuat-nhap-ton.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { StockReportService } from './stock-report.service';
import { StockReportXuatNhapTonDetailComponent } from './stock-report-xuat-nhap-ton-detail/stock-report-xuat-nhap-ton-detail.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [StockReportXuatNhapTonComponent, StockReportXuatNhapTonDetailComponent],
  imports: [
    CommonModule,
    StockReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  providers: [
    StockReportService
  ]
})
export class StockReportsModule { }
