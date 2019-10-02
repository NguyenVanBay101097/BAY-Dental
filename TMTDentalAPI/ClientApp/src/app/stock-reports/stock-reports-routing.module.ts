import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockReportXuatNhapTonComponent } from './stock-report-xuat-nhap-ton/stock-report-xuat-nhap-ton.component';

const routes: Routes = [
  {
    path: 'stock-report-xuat-nhap-ton',
    component: StockReportXuatNhapTonComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockReportsRoutingModule { }
