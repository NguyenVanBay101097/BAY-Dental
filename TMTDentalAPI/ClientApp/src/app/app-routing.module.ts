import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'histories', 
    loadChildren: () => import('./history/history.module').then(m => m.HistoryModule)
  },
  {
    path: 'partners', 
    loadChildren: () => import('./partners/partners.module').then(m => m.PartnersModule)
  },
  {
    path: 'sale-orders', 
    loadChildren: () => import('./sale-orders/sale-orders.module').then(m => m.SaleOrdersModule)
  },
  {
    path: 'appointments', 
    loadChildren: () => import('./appointment/appointment.module').then(m => m.AppointmentModule)
   },
  {
    path: 'financial-report', 
    loadChildren: () => import('./account-financial-report/account-financial-report.module').then(m => m.AccountFinancialReportModule)
  },
  {
    path: 'report-general-ledgers', 
    loadChildren: () => import('./account-report-general-ledgers/account-report-general-ledgers.module').then(m => m.AccountReportGeneralLedgersModule)
  },
  {
    path: 'revenue-report', 
    loadChildren: () => import('./revenue-report/revenue-report.module').then(m => m.RevenueReportModule)
  },
  {
    path: 'report-account-common', 
    loadChildren: () => import('./account-common-partner-reports/account-common-partner-reports.module').then(m => m.AccountCommonPartnerReportsModule)
  },
  {
    path: 'stock-report-xuat-nhap-ton', 
    loadChildren: () => import('./stock-reports/stock-reports.module').then(m => m.StockReportsModule)
  },
  {
    path: 'real-revenue-report', 
    loadChildren: () => import('./real-revenue-report/real-revenue-report.module').then(m => m.RealRevenueReportModule)
  },
  {
    path: 'sale-report', 
    loadChildren: () => import('./sale-report/sale-report.module').then(m => m.SaleReportModule)
  },
  {
    path: 'dot-kham-report', 
    loadChildren: () => import('./dot-kham-steps/dot-kham-steps.module').then(m => m.DotKhamStepsModule)
  },
  {
    path: 'partner-report-location', 
    loadChildren: () => import('./partner-report/partner-report.module').then(m => m.PartnerReportModule)
  },
  {
    path: 'report-partner-sources', 
    loadChildren: () => import('./report-partner-sources/report-partner-sources.module').then(m => m.ReportPartnerSourcesModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
