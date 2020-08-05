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
    path: 'labo-orders', 
    loadChildren: () => import('./labo-orders/labo-orders.module').then(m => m.LaboOrdersModule)
  },
  {
    path: 'purchase', 
    loadChildren: () => import('./purchase-orders/purchase-orders.module').then(m => m.PurchaseOrdersModule)
  },
  {
    path: 'stock', 
    loadChildren: () => import('./stock-pickings/stock-pickings.module').then(m => m.StockPickingsModule)
  },
  {
    path: 'account', 
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule)
  },
  {
    path: 'partner-categories', 
    loadChildren: () => import('./partner-categories/partner-categories.module').then(m => m.PartnerCategoriesModule)
  },
  {
    path: 'partner-sources', 
    loadChildren: () => import('./partner-sources/partner-sources.module').then(m => m.PartnerSourcesModule)
  },
  {
    path: 'products', 
    loadChildren: () => import('./products/products.module').then(m => m.ProductsModule)
  },
  {
    path: 'product-categories', 
    loadChildren: () => import('./product-categories/product-categories.module').then(m => m.ProductCategoriesModule)
  },
  {
    path: 'sample-prescriptions', 
    loadChildren: () => import('./sample-prescriptions/sample-prescriptions.module').then(m => m.SamplePrescriptionsModule)
  },
  {
    path: 'companies', 
    loadChildren: () => import('./companies/companies.module').then(m => m.CompaniesModule)
  },
  {
    path: 'users', 
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
  },
  {
    path: 'res-groups', 
    loadChildren: () => import('./res-groups/res-groups.module').then(m => m.ResGroupsModule)
  },
  {
    path: 'config-settings',
    loadChildren: () => import('./res-config-settings/res-config-settings.module').then(m => m.ResConfigSettingsModule)
  },
  {
    path: '',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  }
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
