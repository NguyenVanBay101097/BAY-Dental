import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

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
    path: 'sale-quotations',
    loadChildren: () => import('./sale-quotations/sale-quotations.module').then(m => m.SaleQuotationsModule)
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
    path: 'phieu-thu-chi',
    loadChildren: () => import('./phieu-thu-chi/phieu-thu-chi.module').then(m => m.PhieuThuChiModule)
  },
  {
    path: 'loai-thu-chi',
    loadChildren: () => import('./loai-thu-chi/loai-thu-chi.module').then(m => m.LoaiThuChiModule)
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
  {
    path: 'card-cards', 
    loadChildren: () => import('./card-cards/card-cards.module').then(m => m.CardCardsModule)
  },
  {
    path: 'card-types', 
    loadChildren: () => import('./card-types/card-types.module').then(m => m.CardTypesModule)
  },
  {
    path: 'programs', 
    loadChildren: () => import('./sale-coupon-promotion/sale-coupon-promotion.module').then(m => m.SaleCouponPromotionModule)
  },
  {
    path: 'service-card-orders', 
    loadChildren: () => import('./service-card-orders/service-card-orders.module').then(m => m.ServiceCardOrdersModule)
  },
  {
    path: 'service-card-types', 
    loadChildren: () => import('./service-card-types/service-card-types.module').then(m => m.ServiceCardTypesModule)
  },
  {
    path: 'service-cards', 
    loadChildren: () => import('./service-card-cards/service-card-cards.module').then(m => m.ServiceCardCardsModule)
  },
  {
    path: 'uoms', 
    loadChildren: () => import('./uoms/uom.module').then(m => m.UomModule)
  },
  {
    path: 'uom-categories', 
    loadChildren: () => import('./uom-categories/uom-category.module').then(m => m.UomCategoryModule)
  },
  {
    path: 'auth', 
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: '',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      routes,
      {
        preloadingStrategy: PreloadAllModules
      }
    )
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
