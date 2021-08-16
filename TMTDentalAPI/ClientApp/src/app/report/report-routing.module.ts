import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'financial-report',
    loadChildren: () => import('../account-financial-report/account-financial-report.module').then(m => m.AccountFinancialReportModule),
  },
  {
    path: 'report-general-ledgers',
    loadChildren: () => import('../account-report-general-ledgers/account-report-general-ledgers.module').then(m => m.AccountReportGeneralLedgersModule),
  },
  {
    path: 'account-invoice-reports',
    loadChildren: () => import('../account-invoice-reports/account-invoice-reports.module').then(m => m.AccountInvoiceReportsModule),
  },
  {
    path: 'report-account-common',
    loadChildren: () => import('../account-common-partner-reports/account-common-partner-reports.module').then(m => m.AccountCommonPartnerReportsModule)
  },
  {
    path: 'sale-dashboard-reports',
    loadChildren: () => import('../sale-dashboard-reports/sale-dashboard-reports.module').then(m => m.SaleDashboardReportsModule),
  },
  {
    path: 'sale-report',
    loadChildren: () => import('../sale-report/sale-report.module').then(m => m.SaleReportModule)
  },
  {
    path: 'customer-receipt-reports',
    loadChildren: () => import('../customer-receipt-reports/customer-receipt-reports.module').then(m => m.CustomerReceiptReportsModule),
  },
  {
    path: 'partner-report-location',
    loadChildren: () => import('../partner-report/partner-report.module').then(m => m.PartnerReportModule),
  },
  {
    path: 'report-partner-sources',
    loadChildren: () => import('../report-partner-sources/report-partner-sources.module').then(m => m.ReportPartnerSourcesModule),
  },
  {
    path: 'sale-orders',
    loadChildren: () => import('../sale-orders/sale-orders.module').then(m => m.SaleOrdersModule),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
